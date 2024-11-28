using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Timers;
using ThePalace.Core.Client.Core.Enums;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Client.Core.Protocols.Auth;
using ThePalace.Core.Client.Core.Protocols.Communications;
using ThePalace.Core.Client.Core.Protocols.Network;
using ThePalace.Core.Client.Core.Protocols.Server;
using ThePalace.Core.Client.Core.Protocols.Users;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Factories
{
    public static class ProxyObject
    {
        public static object CreateProxy(ISessionState sessionState)
        {
            var proxy = new ExpandoObject() as dynamic;
            proxy.SessionState = sessionState as IClientSessionState;

            proxy.alarms = new List<DateTime>();
            proxy.timer = new Timer(10);

            proxy.timer.Elapsed += new ElapsedEventHandler((sender, e) =>
            {
                if (proxy.alarms.Count < 1)
                {
                    proxy.timer.Stop();
                    return;
                }

                var alarm = proxy.alarms[0];
                if (DateTime.Now > alarm)
                {
                    proxy.alarms.RemoveAt(0);

                    ScriptEvents.Current.Invoke(IptEventTypes.Alarm, proxy.SessionState, null, proxy.SessionState.ScriptState);
                }
            });
            proxy.timer.AutoReset = true;

            // let's expose all methods we want to access from a script
            proxy.AlarmExec = new Action<int>(ticks =>
            {
                proxy.alarms.Add(
                    DateTime.Now.AddMilliseconds(
                        IptTracking.TicksToMilliseconds(
                            ticks)));

                proxy.timer.Start();
            });
            proxy.ConsoleMsg = new Action<string>(message =>
            {
                Console.WriteLine(message);
            });
            proxy.DebugMsg = new Action<string>(message =>
            {
                Debug.WriteLine(message);
            });
            proxy.IsConnected = new Func<bool>(() =>
            {
                return proxy.SessionState.ConnectionState?.IsConnected ?? false;
            });
            proxy.GetAssets = new Func<AssetRec[]>(() =>
            {
                return AssetsManager.Current.Assets.Values.ToArray();
            });
            proxy.Connect = new Action<string>(url =>
            {
                ThreadManager.Current.Enqueue(ThreadQueues.Network, null, proxy.SessionState, NetworkCommandTypes.CONNECT, url);
            });
            proxy.Disconnect = new Action(() =>
            {
                ThreadManager.Current.Enqueue(ThreadQueues.Network, null, proxy.SessionState, NetworkCommandTypes.DISCONNECT);
            });
            proxy.GotoRoom = new Action<short>(dest =>
            {
                if ((proxy.SessionState.ConnectionState?.IsConnected ?? false) == true)
                    ThreadManager.Current.Enqueue(ThreadQueues.Network, null, proxy.SessionState, NetworkCommandTypes.SEND, new Header
                    {
                        eventType = EventTypes.MSG_ROOMGOTO,
                        protocolSend = new MSG_ROOMGOTO
                        {
                            dest = dest,
                        },
                    });
            });
            proxy.SuperUser = new Action<string>(password =>
            {
                if ((proxy.SessionState.ConnectionState?.IsConnected ?? false) == true)
                    ThreadManager.Current.Enqueue(ThreadQueues.Network, null, proxy.SessionState, NetworkCommandTypes.SEND, new Header
                    {
                        eventType = EventTypes.MSG_SUPERUSER,
                        protocolSend = new MSG_SUPERUSER
                        {
                            password = password,
                        },
                    });
            });
            proxy.Say = new Action<string>(text =>
            {
                if ((proxy.SessionState.ConnectionState?.IsConnected ?? false) == true)
                    ThreadManager.Current.Enqueue(ThreadQueues.Network, null, proxy.SessionState, NetworkCommandTypes.SEND, new Header
                    {
                        eventType = EventTypes.MSG_XTALK,
                        protocolSend = new MSG_XTALK
                        {
                            text = text,
                        },
                    });
            });
            proxy.SetFace = new Action<short>(faceNbr =>
            {
                if ((proxy.SessionState.ConnectionState?.IsConnected ?? false) == true)
                    ThreadManager.Current.Enqueue(ThreadQueues.Network, null, proxy.SessionState, NetworkCommandTypes.SEND, new Header
                    {
                        eventType = EventTypes.MSG_USERFACE,
                        protocolSend = new MSG_USERFACE
                        {
                            faceNbr = faceNbr,
                        },
                    });
            });
            proxy.SetColour = new Action<short>(colorNbr =>
            {
                if ((proxy.SessionState.ConnectionState?.IsConnected ?? false) == true)
                    ThreadManager.Current.Enqueue(ThreadQueues.Network, null, proxy.SessionState, NetworkCommandTypes.SEND, new Header
                    {
                        eventType = EventTypes.MSG_USERCOLOR,
                        protocolSend = new MSG_USERCOLOR
                        {
                            colorNbr = colorNbr,
                        },
                    });
            });
            proxy.SetName = new Action<string>(name =>
            {
                proxy.SessionState.UserInfo.name = proxy.SessionState.RegInfo.userName = name;

                if ((proxy.SessionState.ConnectionState?.IsConnected ?? false) == true)
                    ThreadManager.Current.Enqueue(ThreadQueues.Network, null, proxy.SessionState, NetworkCommandTypes.SEND, new Header
                    {
                        eventType = EventTypes.MSG_USERNAME,
                        protocolSend = new MSG_USERNAME
                        {
                            name = name,
                        },
                    });
            });
            proxy.KillUser = new Action<uint>(targetId =>
            {
                if ((proxy.SessionState.ConnectionState?.IsConnected ?? false) == true)
                    ThreadManager.Current.Enqueue(ThreadQueues.Network, null, proxy.SessionState, NetworkCommandTypes.SEND, new Header
                    {
                        eventType = EventTypes.MSG_KILLUSER,
                        protocolSend = new MSG_KILLUSER
                        {
                            targetID = targetId,
                        },
                    });
            });
            proxy.Move = new Action<short, short>((x, y) =>
            {
                if ((proxy.SessionState.ConnectionState?.IsConnected ?? false) == true)
                    ThreadManager.Current.Enqueue(ThreadQueues.Network, null, proxy.SessionState, NetworkCommandTypes.SEND, new Header
                    {
                        eventType = EventTypes.MSG_USERMOVE,
                        protocolSend = new MSG_USERMOVE
                        {
                            pos = new Point(
                            (short)(proxy.SessionState.UserInfo.roomPos.h + x),
                            (short)(proxy.SessionState.UserInfo.roomPos.v + y)),
                        },
                    });
            });
            proxy.SetPos = new Action<short, short>((x, y) =>
            {
                if ((proxy.SessionState.ConnectionState?.IsConnected ?? false) == true)
                    ThreadManager.Current.Enqueue(ThreadQueues.Network, null, proxy.SessionState, NetworkCommandTypes.SEND, new Header
                    {
                        eventType = EventTypes.MSG_USERMOVE,
                        protocolSend = new MSG_USERMOVE
                        {
                            pos = new Point(x, y),
                        },
                    });
            });
            proxy.ListOfAllUsers = new Action(() =>
            {
                if ((proxy.SessionState.ConnectionState?.IsConnected ?? false) == true)
                    ThreadManager.Current.Enqueue(ThreadQueues.Network, null, proxy.SessionState, NetworkCommandTypes.SEND, new Header
                    {
                        eventType = EventTypes.MSG_LISTOFALLUSERS,
                    });
            });
            proxy.ListOfAllRooms = new Action(() =>
            {
                if ((proxy.SessionState.ConnectionState?.IsConnected ?? false) == true)
                    ThreadManager.Current.Enqueue(ThreadQueues.Network, null, proxy.SessionState, NetworkCommandTypes.SEND, new Header
                    {
                        eventType = EventTypes.MSG_LISTOFALLROOMS,
                    });
            });
            proxy.AssetQuery = new Action<Int32>(propId =>
            {
                if ((proxy.SessionState.ConnectionState?.IsConnected ?? false) == true)
                    ThreadManager.Current.DownloadAsset(proxy.SessionState, new AssetSpec { id = propId });
            });
            proxy.RoomUsers = new Func<IReadOnlyDictionary<UInt32, UserRec>>(() =>
            {
                return proxy.SessionState.RoomUsersInfo as IReadOnlyDictionary<UInt32, UserRec>;
            });
            proxy.ServerUsers = new Func<IReadOnlyList<ListRec>>(() =>
            {
                if ((proxy.SessionState.ServerUsers?.Count ?? 0) < 1)
                {
                    proxy.ListOfAllUsers();

                    return new List<ListRec>();
                }
                else return proxy.SessionState.ServerUsers as IReadOnlyList<ListRec>;
            });
            proxy.ServerRooms = new Func<IReadOnlyList<ListRec>>(() =>
            {
                if ((proxy.SessionState.ServerRooms?.Count ?? 0) < 1)
                {
                    proxy.ListOfAllRooms();

                    return new List<ListRec>();
                }
                else return proxy.SessionState.ServerRooms as IReadOnlyList<ListRec>;
            });
            proxy.RoomInfo = new Func<RoomRec>(() =>
            {
                return proxy.SessionState.RoomInfo;
            });
            proxy.UserInfo = new Func<UserRec>(() =>
            {
                return proxy.SessionState.UserInfo;
            });
            proxy.RegInfo = new Func<RegistrationRec>(() =>
            {
                return proxy.SessionState.RegInfo;
            });
            return proxy;
        }
    }
}
