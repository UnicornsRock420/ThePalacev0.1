using Microsoft.ClearScript.V8;
using ThePalace.Core;
using ThePalace.Core.Client.Core;
using ThePalace.Core.Client.Core.Factories;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Client.Core.Protocols.Assets;
using ThePalace.Core.Console.Bots.JavaScript.Models;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

var sessionState = SessionManager.Current.CreateSession(typeof(HeadlessSessionState)) as HeadlessSessionState;
if (sessionState == null) return;

var scriptState = sessionState.ScriptState as ScriptState;
if (scriptState == null) return;

Cipher.InitializeTable();

NetworkManager.Current.ProtocolsType = typeof(ThreadManager);
NetworkManager.Current.SessionStateType = typeof(HeadlessSessionState);
NetworkManager.Current.ConnectionStateType = typeof(PalaceConnectionState);

ThreadManager.Current.Initialize();

var engine = scriptState.ScriptEngine = new V8ScriptEngine();
//engine.DocumentSettings.SearchPath = Environment.CurrentDirectory;
//engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;

engine.AddHostType(nameof(Math), typeof(Math));
engine.AddHostType(nameof(Console), typeof(Console));
engine.AddHostObject(nameof(Random), new Random());
engine.AddHostType(nameof(File), typeof(File));
engine.AddHostType(nameof(Path), typeof(Path));
engine.AddHostType(nameof(Thread), typeof(Thread));
engine.AddHostType(nameof(Directory), typeof(Directory));
engine.AddHostType(nameof(PatStream), typeof(PatStream));
engine.AddHostType(nameof(PropPRPStream), typeof(PropPRPStream));
engine.AddHostType(nameof(AssetsManager), typeof(AssetsManager));
//engine.AddHostObject("lib", new HostTypeCollection("mscorlib", "System.Core"));

//var propIdLines = File.ReadAllLines("pids.txt");
//var propIdList = new List<Int32>();
//foreach (var propIdStr in propIdLines)
//{
//    var propId = 0;
//    try { propId = Convert.ToInt32(propIdStr.Trim()); } catch { continue; }
//    if (propId != 0 &&
//        !propIdList.Contains(propId))
//        propIdList.Add(propId);
//}
//engine.Script.propIds = propIdList.ToArray();

//engine.Execute("Console.WriteLine('{0} is an interesting number.', Math.PI)");
//engine.Execute("Console.WriteLine(Random.NextDouble())");
//engine.Execute("Console.WriteLine(lib.System.DateTime.Now)");

engine.AddHostObject("proxy", ProxyObject.CreateProxy(sessionState));

var packetNewUser = new EventTypes[] { EventTypes.MSG_USERNEW, EventTypes.MSG_USERENTER };
var packetTalk = new EventTypes[] { EventTypes.MSG_TALK, EventTypes.MSG_XTALK, EventTypes.MSG_WHISPER, EventTypes.MSG_XWHISPER };
var packetOtherComms = new EventTypes[] { EventTypes.MSG_GMSG, EventTypes.MSG_RMSG, EventTypes.MSG_SMSG, EventTypes.MSG_WMSG };

var populateVariables = (Action<ScriptEvent>)(e =>
{
    var t = e.ScriptState as ScriptState;
    var p = e.Packet as Header;

    engine.Script.ROOMID = sessionState.RoomInfo?.roomID ?? 0;
    engine.Script.ADDRESS = $"{sessionState.ConnectionState?.Host ?? string.Empty}:{sessionState.ConnectionState?.Port ?? 9998}";

    if (p != null)
    {
        if (p.eventType == EventTypes.MSG_ASSETSEND)
        {
            var _p = p.protocolReceive as MSG_ASSETSEND;
            if (_p != null)
            {
                engine.Script.WHATPROPID = _p.assetSpec.id;
            }
        }

        if (packetNewUser.Contains(p.eventType))
            engine.Script.WHOENTER = p.refNum;

        if (packetTalk.Contains(p.eventType))
            engine.Script.WHOCHAT = p.refNum;

        if (packetOtherComms.Contains(p.eventType) ||
            packetTalk.Contains(p.eventType))
        {
            var _p = p.protocolReceive as IProtocolCommunications;
            if (_p != null)
                engine.Script.CHATSTR = _p.text;
        }
    }
});

var cyborgTxt = null as string;
var reloadCyborg = () =>
{
    cyborgTxt =
        File.ReadAllLines(
            Path.Combine(
                Environment.CurrentDirectory,
                "Cyborg.js")).Join('\n');
    engine.Execute(cyborgTxt);
};
reloadCyborg();

foreach (var type in Enum.GetValues<IptEventTypes>())
{
    var eventTypeName = $"{type.ToString().ToUpperInvariant()}()";
    if (!cyborgTxt.Contains(eventTypeName)) continue;

    ScriptEvents.Current.RegisterEvent(type, new EventHandler((sender, e) =>
    {
        var sessionState = sender as HeadlessSessionState;
        if (sessionState == null) return;

        var scriptEvent = e as ScriptEvent;
        if (scriptEvent == null) return;

        populateVariables(scriptEvent);

        try
        {
            engine.Execute($"{scriptEvent.EventType.ToString().ToUpperInvariant()}()");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }));
}

ScriptEvents.Current.Invoke(IptEventTypes.Main, sessionState, null, sessionState.ScriptState);

var clientRunning = true;
while (clientRunning)
{
    var line = Console.ReadLine()?.Trim() ?? string.Empty;

    if (!string.IsNullOrWhiteSpace(line))
    {
        if (line.ToUpperInvariant() == "EXIT")
        {
            clientRunning = false;
            break;
        }
        else if (line.ToUpperInvariant() == "RELOAD")
        {
            reloadCyborg();
            Console.WriteLine("Cyborg Reloaded!");
        }
        else
            try
            {
                engine.Execute(line);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
    }

    Thread.Sleep(100);
}
