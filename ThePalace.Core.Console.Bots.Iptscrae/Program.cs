using System.Timers;
using ThePalace.Core;
using ThePalace.Core.Client.Core;
using ThePalace.Core.Client.Core.Enums;
using ThePalace.Core.Client.Core.Factories;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Client.Core.Protocols.Assets;
using ThePalace.Core.Console.Bots.Iptscrae.Models;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

var sessionState = SessionManager.Current.CreateSession(typeof(HeadlessSessionState)) as HeadlessSessionState;
if (sessionState == null) return;

var iptTracking = sessionState.ScriptState as ScriptState;
if (iptTracking == null) return;

var reloadCyborg = () =>
{
    IptscraeEngine.Parser(
        iptTracking,
        File.ReadAllLines(
            Path.Combine(
                Environment.CurrentDirectory,
                "Cyborg.ipt")).Join('\n'),
        true);
};
reloadCyborg();

if (!iptTracking.Events.ContainsKey(IptEventTypes.Main)) return;

iptTracking.Timer.Elapsed += new ElapsedEventHandler((sender, e) =>
{
    var now = DateTime.Now;

    var alarms = iptTracking.Alarms
        .Where(a => now > a.Created.AddMilliseconds(
            IptTracking.TicksToMilliseconds(a.Delay)))
        .ToList();
    if (alarms.Count > 0)
        foreach (var alarm in alarms)
        {
            ThreadManager.Current.Enqueue(ThreadQueues.ScriptEngine, args =>
            {
                var iptTracking = args[0] as IptTracking;
                if (iptTracking == null) return null;

                var alarm = args[1] as IptAlarm;
                if (alarm == null) return null;

                try
                {
                    IptscraeEngine.Executor(alarm.Value, iptTracking);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    iptTracking.Alarms.Remove(alarm);
                }

                return null;
            }, iptTracking, alarm);
        }
});
iptTracking.Timer.AutoReset = true;
iptTracking.Timer.Start();

var packetNewUser = new EventTypes[] { EventTypes.MSG_USERNEW, EventTypes.MSG_USERENTER };
var packetTalk = new EventTypes[] { EventTypes.MSG_TALK, EventTypes.MSG_XTALK, EventTypes.MSG_WHISPER, EventTypes.MSG_XWHISPER };
var packetOtherComms = new EventTypes[] { EventTypes.MSG_GMSG, EventTypes.MSG_RMSG, EventTypes.MSG_SMSG, EventTypes.MSG_WMSG };

var populateVariables = (Action<ScriptEvent>)(e =>
{
    var t = e.ScriptState as ScriptState;
    var p = e.Packet as Header;

    var iptVar = null as IptMetaVariable;

    if (t?.Variables?.ContainsKey("SESSIONSTATE") == false)
    {
        iptVar = new IptMetaVariable
        {
            IsSpecial = true,
            IsGlobal = true,
        };
        t?.Variables?.TryAdd("SESSIONSTATE", iptVar);

        if (iptVar != null)
        {
            iptVar.IsReadOnly = false;
            iptVar.Value = new IptVariable
            {
                Type = IptVariableTypes.Shadow,
                Value = sessionState,
            };
            iptVar.IsReadOnly = true;
        }
    }

    if (t?.Variables?.ContainsKey("ROOMID") == false)
    {
        iptVar = new IptMetaVariable
        {
            IsSpecial = true,
        };
        t?.Variables?.TryAdd("ROOMID", iptVar);
    }
    else iptVar = t?.Variables["ROOMID"];

    if (iptVar != null)
    {
        iptVar.IsReadOnly = false;
        iptVar.Value = new IptVariable
        {
            Type = IptVariableTypes.Integer,
            Value = sessionState.RoomInfo?.roomID ?? 0,
        };
        iptVar.IsReadOnly = true;
    }

    if (t?.Variables?.ContainsKey("ADDRESS") == false)
    {
        iptVar = new IptMetaVariable
        {
            IsSpecial = true,
        };
        t?.Variables?.TryAdd("ADDRESS", iptVar);
    }
    else iptVar = t?.Variables["ADDRESS"];

    if (iptVar != null)
    {
        iptVar.IsReadOnly = false;
        iptVar.Value = new IptVariable
        {
            Type = IptVariableTypes.String,
            Value = $"{sessionState.ConnectionState?.Host ?? string.Empty}:{sessionState.ConnectionState?.Port ?? 9998}",
        };
        iptVar.IsReadOnly = true;
    }

    if (p != null)
    {
        if (p.eventType == EventTypes.MSG_ASSETSEND)
        {
            if (t?.Variables?.ContainsKey("WHATPROPID") == false)
            {
                iptVar = new IptMetaVariable
                {
                    IsSpecial = true,
                };
                t?.Variables?.TryAdd("WHATPROPID", iptVar);
            }
            else iptVar = t?.Variables["WHATPROPID"];

            if (iptVar != null)
            {
                iptVar.IsReadOnly = false;
                var _p = p.protocolReceive as MSG_ASSETSEND;
                if (_p != null)
                    iptVar.Value = new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = _p.assetSpec.id,
                    };
                iptVar.IsReadOnly = true;
            }
        }

        if (packetNewUser.Contains(p.eventType))
        {
            if (t?.Variables?.ContainsKey("WHOENTER") == false)
            {
                iptVar = new IptMetaVariable
                {
                    IsSpecial = true,
                };
                t?.Variables?.TryAdd("WHOENTER", iptVar);
            }
            else iptVar = t?.Variables["WHOENTER"];

            if (iptVar != null)
            {
                iptVar.IsReadOnly = false;
                iptVar.Value = new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = p.refNum,
                };
                iptVar.IsReadOnly = true;
            }
        }

        if (packetTalk.Contains(p.eventType))
        {
            if (t?.Variables?.ContainsKey("WHOCHAT") == false)
            {
                iptVar = new IptMetaVariable
                {
                    IsSpecial = true,
                };
                t?.Variables?.TryAdd("WHOCHAT", iptVar);
            }
            else iptVar = t?.Variables["WHOCHAT"];

            if (iptVar != null)
            {
                iptVar.IsReadOnly = false;
                iptVar.Value = new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = p.refNum,
                };
                iptVar.IsReadOnly = true;
            }
        }

        if (packetOtherComms.Contains(p.eventType) ||
            packetTalk.Contains(p.eventType))
        {
            if (t?.Variables?.ContainsKey("CHATSTR") == false)
            {
                iptVar = new IptMetaVariable
                {
                    IsSpecial = true,
                };
                t?.Variables?.TryAdd("CHATSTR", iptVar);
            }
            else iptVar = t?.Variables["CHATSTR"];

            if (iptVar != null &&
                p.protocolReceive is IProtocolCommunications msgComms)
            {
                iptVar.Value = new IptVariable
                {
                    Type = IptVariableTypes.String,
                    Value = msgComms.text,
                };
            }
        }
    }
});

foreach (var @event in iptTracking.Events)
{
    ScriptEvents.Current.RegisterEvent(@event.Key, new EventHandler((sender, e) =>
    {
        var sessionState = sender as HeadlessSessionState;
        if (sessionState == null) return;

        var scriptEvent = e as ScriptEvent;
        if (scriptEvent == null) return;

        var iptTracking = scriptEvent.ScriptState as ScriptState;
        if (iptTracking == null) return;

        populateVariables(scriptEvent);

        ThreadManager.Current.Enqueue(ThreadQueues.ScriptEngine, args =>
        {
            var iptTracking = args.FirstOrDefault() as IptTracking;
            if (iptTracking == null) return null;

            try
            {
                IptscraeEngine.Executor(iptTracking.Events[scriptEvent.EventType], iptTracking);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }, iptTracking);
    }));
}

Cipher.InitializeTable();

NetworkManager.Current.ProtocolsType = typeof(ThreadManager);
NetworkManager.Current.SessionStateType = typeof(HeadlessSessionState);
NetworkManager.Current.ConnectionStateType = typeof(PalaceConnectionState);

ThreadManager.Current.Initialize();

ScriptEvents.Current.Invoke(IptEventTypes.Main, sessionState, null, iptTracking);

var clientRunning = true;
while (clientRunning)
{
    var atomlist = null as List<IptVariable>;

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
        {
            ThreadManager.Current.Enqueue(ThreadQueues.ScriptEngine, args =>
            {
                var iptTracking = args.FirstOrDefault() as IptTracking;
                if (iptTracking == null) return null;

                try
                {
                    atomlist = IptscraeEngine.Parser(iptTracking, line, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (atomlist != null)
                    try
                    {
                        IptscraeEngine.Executor(atomlist, iptTracking);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                return null;
            }, iptTracking);
        }
    }

    Thread.Sleep(100);
}
