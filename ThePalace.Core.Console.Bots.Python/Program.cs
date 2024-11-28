using IronPython.Hosting;
using Microsoft.Scripting;
using ThePalace.Core;
using ThePalace.Core.Client.Core;
using ThePalace.Core.Client.Core.Factories;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Client.Core.Protocols.Assets;
using ThePalace.Core.Console.Bots.Python.Models;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

var sessionState = SessionManager.Current.CreateSession(typeof(HeadlessSessionState)) as HeadlessSessionState;
if (sessionState == null) return;

Cipher.InitializeTable();

NetworkManager.Current.ProtocolsType = typeof(ThreadManager);
NetworkManager.Current.SessionStateType = typeof(HeadlessSessionState);
NetworkManager.Current.ConnectionStateType = typeof(PalaceConnectionState);

ThreadManager.Current.Initialize();

var scriptState = sessionState.ScriptState as ScriptState;
if (scriptState == null) return;

scriptState.ScriptEngine = Python.CreateEngine();
dynamic scriptScore = scriptState.ScriptScope = scriptState.ScriptEngine.CreateScope();
scriptScore.proxy = ProxyObject.CreateProxy(sessionState);

var packetNewUser = new EventTypes[] { EventTypes.MSG_USERNEW, EventTypes.MSG_USERENTER };
var packetTalk = new EventTypes[] { EventTypes.MSG_TALK, EventTypes.MSG_XTALK, EventTypes.MSG_WHISPER, EventTypes.MSG_XWHISPER };
var packetOtherComms = new EventTypes[] { EventTypes.MSG_GMSG, EventTypes.MSG_RMSG, EventTypes.MSG_SMSG, EventTypes.MSG_WMSG };

var populateVariables = (Action<ScriptEvent>)(e =>
{
    var t = e.ScriptState as ScriptState;
    var p = e.Packet as Header;

    scriptScore.ROOMID = sessionState.RoomInfo?.roomID ?? 0;
    scriptScore.ADDRESS = $"{sessionState.ConnectionState?.Host ?? string.Empty}:{sessionState.ConnectionState?.Port ?? 9998}";

    if (p != null)
    {
        if (p.eventType == EventTypes.MSG_ASSETSEND)
        {
            var _p = p.protocolReceive as MSG_ASSETSEND;
            if (_p != null)
            {
                scriptScore.WHATPROPID = _p.assetSpec.id;
            }
        }

        if (packetNewUser.Contains(p.eventType))
            scriptScore.WHOENTER = p.refNum;

        if (packetTalk.Contains(p.eventType))
            scriptScore.WHOCHAT = p.refNum;

        if (packetOtherComms.Contains(p.eventType) ||
            packetTalk.Contains(p.eventType))
        {
            var _p = p.protocolReceive as IProtocolCommunications;
            if (_p != null)
                scriptScore.CHATSTR = _p.text;
        }
    }
});


var cyborgTxt = null as string;
var reloadCyborg = () =>
{
    cyborgTxt = File.ReadAllLines(
       Path.Combine(
           Environment.CurrentDirectory,
           "Cyborg.py")).Join("\r\n");
    scriptState.ScriptEngine
        .CreateScriptSourceFromString(cyborgTxt, SourceCodeKind.File)
        .Execute(scriptState.ScriptScope);
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
            scriptState.ScriptEngine
                .CreateScriptSourceFromString($"{scriptEvent.EventType.ToString().ToUpperInvariant()}()", SourceCodeKind.SingleStatement)
                .Execute(scriptState.ScriptScope);
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
                scriptState.ScriptEngine
                    .CreateScriptSourceFromString(line, SourceCodeKind.SingleStatement)
                    .Execute(scriptState.ScriptScope);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
    }

    Thread.Sleep(100);
}
