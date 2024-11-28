using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using ThePalace.Core.Client.Core.Enums;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Client.Core.Protocols.Communications;
using ThePalace.Core.Client.Core.Protocols.Network;
using ThePalace.Core.Client.Core.Protocols.Server;
using ThePalace.Core.Client.Core.Protocols.Users;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Client.Core.Factories
{
    using IptAtomList = List<IptVariable>;

    public static class IptscraeEngine
    {
        private static readonly Type TYPE_STRING = typeof(string);
        private static readonly IReadOnlyDictionary<string, IptEventTypes> EventTypes =
            Enum.GetValues<IptEventTypes>()
                .ToDictionary(v => v.ToString().ToUpperInvariant(), v => v);

        internal const decimal iptVersion = (decimal)3.0;
        internal const int gNestedAtomlistMaxDepth = 256;
        internal const int gNestedArrayMaxDepth = 256;
        internal const int gWhileMaxIteration = 7500;
        internal const int gMaxPaintPenSize = 20;
        internal const int StackMaxSize = 1024;
        internal const string iptEngineUsername = "IptscraeEngine";
        internal const string clientType = "IptscraeEngine";

        internal static IptVariable popStack(IptTracking iptTracking)
        {
            if (iptTracking.Stack.Count < 1)
            {
                throw new Exception("Stack is empty...");
            }

            var register = iptTracking.Stack.Pop();
            register = getVariable(iptTracking, register);

            return register;
        }
        internal static IptVariable getVariable(IptTracking iptTracking, IptVariable variable)
        {
            if (variable.Type != IptVariableTypes.Variable) return variable;

            var key = variable.Value?.ToString();

            if (iptTracking.Variables.ContainsKey(key) &&
                iptTracking.Variables[key].Value.Type != IptVariableTypes.Shadow)
                return iptTracking.Variables[key].Value;
            else return new IptVariable
            {
                Type = IptVariableTypes.Integer,
                Value = 0,
            };
        }
        internal static void setVariable(IptTracking iptTracking, IptVariable destination, IptVariable value, int recursionDepth = 0, bool isGlobal = false)
        {
            var key = destination.Value?.ToString();

            if (iptTracking.Variables.ContainsKey(key))
            {
                if (iptTracking.Variables[key].IsReadOnly)
                {
                    throw new Exception($"{key} IsReadOnly...");
                }
                else if (iptTracking.Variables[key].IsSpecial &&
                    iptTracking.Variables[key].Value.Type != value.Type)
                {
                    throw new Exception($"Wrong datatype {iptTracking.Variables[key].Value.Type}, {value.Type}...");
                }

                iptTracking.Variables[key].Value = value;
            }
            else
            {
                iptTracking.Variables[key] = new IptMetaVariable
                {
                    Value = value,
                    Depth = recursionDepth,
                    IsGlobal = isGlobal,
                };
            }
        }

        internal static readonly ConcurrentDictionary<string, object> iptCommands = new();
        internal static readonly ConcurrentDictionary<string, IptOperator> iptOperators = new();

        static IptscraeEngine()
        {

            #region Iptscrae Version 1
            // Start Aliases
            iptCommands.TryAdd("ROOMGOTO", "GOTOROOM");
            iptCommands.TryAdd("CLEARPROPS", "NAKED");
            iptCommands.TryAdd("NETGOTO", "GOTOURL");
            iptCommands.TryAdd("USERID", "WHOME");
            iptCommands.TryAdd("CHAT", "SAY");
            iptCommands.TryAdd("ID", "ME");
            // End Aliases
            // Start Paint Commands
            iptCommands.TryAdd("LINE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);
                var register3 = popStack(iptTracking);
                var register4 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Integer:
                        if (register2.Type != IptVariableTypes.Integer || register3.Type != IptVariableTypes.Integer || register4.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register1.Type}, {register2.Type}, {register3.Type}, {register4.Type}...");
                        }

                        //$scope.model.Screen.paintPenPos = {
                        //    v: register3.Value,
                        //    h: register4.Value,
                        //};

                        //$scope.encodeDrawCmd({
                        //    v: register3.Value,
                        //    h: register4.Value,
                        //}, [{
                        //    v: register1.Value - register3.Value,
                        //    h: register2.Value - register4.Value,
                        //}]);

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("LINETO", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Integer:
                        if (register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        //$scope.encodeDrawCmd({
                        //    v: $scope.model.Screen.paintPenPos.v,
                        //    h: $scope.model.Screen.paintPenPos.h,
                        //}, [{
                        //    v: register1.Value - $scope.model.Screen.paintPenPos.v,
                        //    h: register2.Value - $scope.model.Screen.paintPenPos.h,
                        //}]);

                        //$scope.model.Screen.paintPenPos = {
                        //    v: register1.Value,
                        //    h: register2.Value,
                        //};

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("PENPOS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Integer:
                        if (register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        //$scope.model.Screen.paintPenPos = {
                        //    v: register1.Value,
                        //    h: register2.Value,
                        //};

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("PENTO", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Integer:
                        if (register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        //if (!$scope.model.Screen.paintPenPos) {
                        //    $scope.model.Screen.paintPenPos = {
                        //        v: 0,
                        //        h: 0,
                        //    };
                        //}

                        //var xCoord = $scope.model.Screen.paintPenPos.h + register2.Value;
                        //var yCoord = $scope.model.Screen.paintPenPos.v + register1.Value;

                        //$scope.model.Screen.paintPenPos = {
                        //    v: yCoord,
                        //    h: xCoord,
                        //};

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("PENCOLOR", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);
                var register3 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Integer:
                        if (register2.Type != IptVariableTypes.Integer || register3.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}, {register3.Type}...");
                        }

                        //$scope.model.Screen.paintPenColor = {
                        //    r: register3.Value % 256,
                        //    g: register2.Value % 256,
                        //    b: register1.Value % 256,
                        //};

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("PAINTCLEAR", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //$scope.serverSend(
                //    'MSG_DRAW',
                //    {
                //        Type = 'DC_Detonate',
                //        layer: false,
                //        data: null,
                //    });
            }));
            iptCommands.TryAdd("PAINTUNDO", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //$scope.serverSend(
                //    'MSG_DRAW',
                //    {
                //        Type = 'DC_Delete',
                //        layer: false,
                //        data: null,
                //    });
            }));
            iptCommands.TryAdd("PENBACK", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //$scope.model.Screen.paintLayer = false;
            }));
            iptCommands.TryAdd("PENFRONT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //$scope.model.Screen.paintLayer = true;
            }));
            iptCommands.TryAdd("PENSIZE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        register.Value = (int)register.Value % gMaxPaintPenSize;

                        if ((int)register.Value < 1)
                        {
                            register.Value = 1;
                        }

                        //$scope.model.Screen.paintPenSize = register.Value;

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            // End Paint Commands
            // Start Sound Commands
            iptCommands.TryAdd("SOUND", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        //if ($scope.model.Interface.soundsEnabled) {
                        //    var audioUrl = $scope.model.ServerInfo.mediaUrl + ($scope.model.ServerInfo.mediaUrl.substring($scope.model.ServerInfo.mediaUrl.length - 1, 1) == '/' ? '' : '/') + register.Value;
                        //
                        //    $scope.model.Application.soundPlayer.preload({
                        //        sourceUrl: audioUrl,
                        //        resolve: function (response) {
                        //            this.play();
                        //        },
                        //        reject: function (errors) {
                        //        },
                        //    });
                        //    $scope.model.Application.soundPlayer.load();
                        //}

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("SOUNDPAUSE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //$scope.model.Application.soundPlayer.pause();
            }));
            iptCommands.TryAdd("MIDIPLAY", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        //if ($scope.model.Interface.soundsEnabled) {
                        //    var audioUrl = $scope.model.ServerInfo.mediaUrl + ($scope.model.ServerInfo.mediaUrl.substring($scope.model.ServerInfo.mediaUrl.length - 1, 1) == '/' ? '' : '/') + register.Value;
                        //
                        //    $scope.model.Application.midiPlayer.play(audioUrl);
                        //}

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("MIDISTOP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //$scope.model.Application.midiPlayer.stop();
            }));
            // End Sound Commands
            // Start Math Commands
            iptCommands.TryAdd("SINE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = (int)Math.Sin((int)register.Value) * 1000,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("COSINE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = (int)Math.Cos((int)register.Value) * 1000,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("TANGENT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = (int)Math.Tan((int)register.Value) * 1000,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("SQUAREROOT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = (int)Math.Sqrt((int)register.Value) * 1000,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("RANDOM", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Bool:
                    case IptVariableTypes.Integer:
                        var value = (int)register.Value;

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = RndGenerator.Next(value),
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("MOD", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                Operator(iptTracking, "%", recursionDepth);
            }));
            // End Math Commands
            // Start Time Commands
            iptCommands.TryAdd("TICKS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = DateTime.UtcNow.ToTicks(),
                });
            }));
            iptCommands.TryAdd("DATETIME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = DateTime.UtcNow.ToTimestamp(),
                });
            }));
            // End Time Commands
            // Start Stack Commands
            iptCommands.TryAdd("OVER", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (iptTracking.Stack.Count < 2)
                {
                    throw new Exception("Not enough items on the stack...");
                }

                iptTracking.Stack.Push(iptTracking.Stack[iptTracking.Stack.Count - 2]);
            }));
            iptCommands.TryAdd("PICK", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        if (iptTracking.Stack.Count <= (int)register.Value)
                        {
                            throw new Exception("Not enough items on the stack...");
                        }

                        iptTracking.Stack.Push(iptTracking.Stack[iptTracking.Stack.Count - (int)register.Value - 1]);

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("DUP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = iptTracking.Stack.PeekL();
                iptTracking.Stack.Push(register);
            }));
            iptCommands.TryAdd("POP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                iptTracking.Stack.Pop();
            }));
            iptCommands.TryAdd("SWAP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = iptTracking.Stack.Pop();
                var register2 = iptTracking.Stack.Pop();

                iptTracking.Stack.Push(register1);
                iptTracking.Stack.Push(register2);
            }));
            iptCommands.TryAdd("STACKDEPTH", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = iptTracking.Stack.Count,
                });
            }));
            // End Stack Commands
            // Start Message Commands
            iptCommands.TryAdd("ROOMMSG", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        //$scope.serverSend(
                        //    'MSG_RMSG',
                        //    {
                        //        text: register.Value,
                        //    });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("SUSRMSG", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        //$scope.serverSend(
                        //    'MSG_SMSG',
                        //    {
                        //        text: register.Value,
                        //    });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("GLOBALMSG", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        //$scope.serverSend(
                        //    'MSG_GMSG',
                        //    {
                        //        text: register.Value,
                        //    });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("LOCALMSG", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        //$.connection.proxyHub.client.receive(
                        //    'MSG_TALK',
                        //    $scope.model.UserInfo.userId,
                        //    {
                        //        text: register.Value,
                        //        localmsg: true,
                        //    });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("STATUSMSG", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        //$scope.setStatusMsg(register.Value);

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("PRIVATEMSG", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Integer:
                        if (register2.Type != IptVariableTypes.String)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        //$scope.serverSend(
                        //    'MSG_XWHISPER',
                        //    {
                        //        target: register1.Value,
                        //        text: register2.Value,
                        //    });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("LOGMSG", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        //$scope.model.Interface.LogList.push({
                        //    userName: this.iptEngineUsername,
                        //    text: register.Value,
                        //    isWhisper: true,
                        //});

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("SAY", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) break;

                        var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as ISessionState;
                        if (sessionState == null) break;

                        ThreadManager.Current.Enqueue(ThreadQueues.Network, null, sessionState, NetworkCommandTypes.SEND, new Header
                        {
                            eventType = ThePalace.Core.Enums.EventTypes.MSG_XTALK,
                            protocolSend = new MSG_XTALK
                            {
                                text = register.Value.ToString(),
                            },
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("SAYAT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);
                var register3 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.String:
                        if (register2.Type != register3.Type && register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        //$scope.serverSend(
                        //    'MSG_XTALK',
                        //    {
                        //        text: ''.concat('@', register2.Value, ',', register1.Value, ' ', register3.Value),
                        //    });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            // End Message Commands
            // Start Prop Commands
            iptCommands.TryAdd("MACRO", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        // TODO:

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("USERPROP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        //for (var j = 0; j < $scope.model.RoomInfo.UserList.length; j++) {
                        //    if ($scope.model.RoomInfo.UserList[j].userID == $scope.model.UserInfo.userId) {
                        //        if ($scope.model.RoomInfo.UserList[j].propSpec && $scope.model.RoomInfo.UserList[j].propSpec.length > 0) {
                        //            if (register.Value < $scope.model.RoomInfo.UserList[j].propSpec.length) {
                        //                var propID = $scope.model.RoomInfo.UserList[j].propSpec[register.Value].id;
                        //
                        //                iptTracking.Stack.Push(new IptValue {
                        //                    Type = IptTypes.Integer,
                        //                    Value = propID,
                        //                });
                        //            }
                        //        }
                        //
                        //        break;
                        //    }
                        //}

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("DROPPROP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Integer:
                        if (register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        //for (var j = 0; j < $scope.model.RoomInfo.UserList.length; j++) {
                        //    if ($scope.model.RoomInfo.UserList[j].userID == $scope.model.UserInfo.userId) {
                        //        if ($scope.model.RoomInfo.UserList[j].propSpec && $scope.model.RoomInfo.UserList[j].propSpec.length > 0) {
                        //            var lastIndex = $scope.model.RoomInfo.UserList[j].propSpec.length - 1;
                        //            var propID = $scope.model.RoomInfo.UserList[j].propSpec[lastIndex].id;
                        //
                        //            $scope.serverSend(
                        //                'MSG_PROPNEW',
                        //
                        //                {
                        //                    propSpec: {
                        //                        id: propID,
                        //			            crc: 0,
                        //		            },
                        //		            loc: {
                        //                        h: register2.Value,
                        //			            v: register1.Value,
                        //		            },
                        //	            });
                        //
                        //            if (lastIndex == 0) {
                        //	            $scope.setProps(null);
                        //            }
                        //            else {
                        //	            $scope.model.RoomInfo.UserList[j].propSpec.splice(lastIndex, 1);
                        //
                        //	            $scope.setProps($scope.model.RoomInfo.UserList[j].propSpec);
                        //            }
                        //        }
                        //
                        //        break;
                        //    }
                        //}

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("DOFFPROP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //for (var j = 0; j < $scope.model.RoomInfo.UserList.length; j++) {
                //    if ($scope.model.RoomInfo.UserList[j].userID == $scope.model.UserInfo.userId) {
                //        if ($scope.model.RoomInfo.UserList[j].propSpec && $scope.model.RoomInfo.UserList[j].propSpec.length > 0) {
                //            $scope.model.RoomInfo.UserList[j].propSpec.splice($scope.model.RoomInfo.UserList[j].propSpec.length - 1, 1);
                //
                //            $scope.setProps($scope.model.RoomInfo.UserList[j].propSpec.length > 0 ? $scope.model.RoomInfo.UserList[j].propSpec : null);
                //        }
                //
                //        break;
                //    }
                //}
            }));
            iptCommands.TryAdd("TOPPROP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //for (var j = 0; j < $scope.model.RoomInfo.UserList.length; j++) {
                //    if ($scope.model.RoomInfo.UserList[j].userID == $scope.model.UserInfo.userId) {
                //        var propID = 0;
                //
                //        if ($scope.model.RoomInfo.UserList[j].propSpec && $scope.model.RoomInfo.UserList[j].propSpec.length > 0) {
                //            propID = $scope.model.RoomInfo.UserList[j].propSpec[$scope.model.RoomInfo.UserList[j].propSpec.length - 1].id;
                //        }
                //
                //        iptTracking.Stack.Push(new IptValue {
                //            Type = IptTypes.Integer,
                //            Value = propID,
                //        });
                //
                //        break;
                //    }
                //}
            }));
            iptCommands.TryAdd("HASPROP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                //for (var j = 0; j < $scope.model.RoomInfo.UserList.length; j++) {
                //    if ($scope.model.RoomInfo.UserList[j].userID == $scope.model.UserInfo.userId) {
                //        var propID = 0;
                //
                //        if ($scope.model.RoomInfo.UserList[j].propSpec && $scope.model.RoomInfo.UserList[j].propSpec.length > 0) {
                //            switch (register.Type) {
                //                case IptTypes.String:
                //		            for (var k in $scope.model.Screen.assetCache) {
                //                        if ($scope.model.Screen.assetCache[k].name == register.Value) {
                //                            propID = k;
                //
                //                            break;
                //                        }
                //                    }
                //
                //                    break;
                //                case IptTypes.Integer:
                //                    propID = register.Value;
                //
                //                    break;
                //                default:
                //                    throw new Exception($"Wrong datatype {register2.Type}...");
                //
                //                    break;
                //            }
                //        }
                //
                //        iptTracking.Stack.Push(new IptValue {
                //            Type = IptTypes.Bool,
                //            Value = propID != 0 ? 1 : 0,
                //        });
                //
                //        break;
                //    }
                //}
            }));
            iptCommands.TryAdd("CLEARLOOSEPROPS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //$scope.serverSend(
                //    'MSG_PROPDEL',
                //    {
                //        propNum: -1
                //    });
            }));
            iptCommands.TryAdd("ADDLOOSEPROP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);
                var register3 = popStack(iptTracking);

                if (register1.Type != register2.Type && register1.Type != IptVariableTypes.Integer)
                {
                    throw new Exception($"Wrong datatype {register1.Type}...");
                }

                switch (register3.Type)
                {
                    case IptVariableTypes.String:
                        //for (var k in $scope.model.Screen.assetCache) {
                        //    if ($scope.model.Screen.assetCache[k].name == register3.Value) {
                        //        $scope.serverSend(
                        //            'MSG_PROPNEW',
                        //
                        //            {
                        //                propSpec: {
                        //                    id: k,
                        //		            crc: 0,
                        //	            },
                        //	            loc: {
                        //                    h: register2.Value,
                        //		            v: register1.Value,
                        //	            },
                        //            });
                        //
                        //        break;
                        //    }
                        //}

                        break;
                    case IptVariableTypes.Integer:
                        //$scope.serverSend(
                        //    'MSG_PROPNEW',
                        //
                        //    {
                        //        propSpec: {
                        //            id: register3.Value,
                        //            crc: 0,
                        //        },
                        //        loc: {
                        //            h: register2.Value,
                        //            v: register1.Value,
                        //        },
                        //    });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register3.Type}...");
                }
            }));
            iptCommands.TryAdd("REMOVEPROP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                //for (var j = 0; j < $scope.model.RoomInfo.UserList.length; j++) {
                //    if ($scope.model.RoomInfo.UserList[j].userID == $scope.model.UserInfo.userId) {
                //        if ($scope.model.RoomInfo.UserList[j].propSpec && $scope.model.RoomInfo.UserList[j].propSpec.length > 0) {
                //
                //            var propID = 0;
                //
                //            switch (register.Type) {
                //                case IptTypes.String:
                //		            for (var k in $scope.model.Screen.assetCache) {
                //                        if ($scope.model.Screen.assetCache[k].name == register.Value) {
                //                            propID = k;
                //
                //                            break;
                //                        }
                //                    }
                //
                //                    break;
                //                case IptTypes.Integer:
                //                    propID = register.Value;
                //
                //                    break;
                //                default:
                //                    throw new Exception($"Wrong datatype {register.Type}...");
                //
                //                    break;
                //            }
                //
                //            if (propID != 0) {
                //                for (var k = 0; k < $scope.model.RoomInfo.UserList[j].propSpec.length; k++) {
                //                    if ($scope.model.RoomInfo.UserList[j].propSpec[k].id == propID) {
                //			            $scope.model.RoomInfo.UserList[j].propSpec.splice(k, 1);
                //
                //                        break;
                //                    }
                //                }
                //            }
                //        }
                //
                //        break;
                //    }
                //}

                //$scope.setProps(propSpec);
            }));
            iptCommands.TryAdd("DONPROP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);
                var propSpec = new List<AssetSpec>();

                //for (var j = 0; j < $scope.model.RoomInfo.UserList.length; j++) {
                //    if ($scope.model.RoomInfo.UserList[j].userID == $scope.model.UserInfo.userId) {
                //        if ($scope.model.RoomInfo.UserList[j].propSpec && $scope.model.RoomInfo.UserList[j].propSpec.length > 0) {
                //            propSpec = $scope.model.RoomInfo.UserList[j].propSpec;
                //        }
                //
                //        switch (register.Type) {
                //            case IptTypes.String:
                //	            for (var k in $scope.model.Screen.assetCache) {
                //                    if ($scope.model.Screen.assetCache[k].name == register.Value) {
                //                        propSpec.push({
                //                            id: k,
                //				            crc: 0,
                //			            });
                //
                //                        break;
                //                    }
                //                }
                //
                //                break;
                //            case IptTypes.Integer:
                //                propSpec.push({
                //                    id: register.Value,
                //		            crc: 0,
                //	            });
                //
                //                break;
                //            default:
                //                throw new Exception($"Wrong datatype {register.Type}...");
                //
                //                break;
                //        }
                //
                //        break;
                //    }
                //}

                //$scope.setProps(propSpec);
            }));
            iptCommands.TryAdd("SETPROPS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Array:
                        var propSpec = new List<AssetSpec>();

                        //for (var j = 0; j < register.Value.length; j++) {
                        //    switch (register.Value[j].Type) {
                        //        case IptTypes.String:
                        //            for (var k in $scope.model.Screen.assetCache) {
                        //                if ($scope.model.Screen.assetCache[k].name == register.Value[j].Value) {
                        //                    propSpec.push({
                        //                        id: k,
                        //			            crc: 0,
                        //		            });
                        //
                        //                    break;
                        //                }
                        //            }
                        //
                        //            break;
                        //        case IptTypes.Integer:
                        //            propSpec.push({
                        //                id: register.Value[j].Value,
                        //	            crc: 0,
                        //            });
                        //
                        //            break;
                        //        default:
                        //            throw new Exception($"Wrong datatype {register.Type}...");
                        //    }
                        //}

                        //$scope.setProps(propSpec.length ? propSpec : null);

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("SHOWLOOSEPROPS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //for (var j = 0; j < $scope.model.RoomInfo.LooseProps.length; j++) {
                //    var prop = $scope.model.RoomInfo.LooseProps[j];
                //
                //    $scope.model.Interface.LogList.push({
                //        userName: this.iptEngineUsername,
                //        isWhisper: true,
                //        text: ''.concat(prop.propSpec.id, ' ', prop.loc.h, ' ', prop.loc.v, ' ADDLOOSEPROP'),
                //    })
                //}
            }));
            iptCommands.TryAdd("NBRUSERPROPS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as ISessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = sessionState.UserInfo.assetSpec.Count,
                });
            }));
            // End Prop Commands
            // Start String Commands
            iptCommands.TryAdd("LOWERCASE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        var value = register.Value.ToString().ToLowerInvariant();

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.String,
                            Value = value,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("UPPERCASE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);
                register = getVariable(iptTracking, register);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        var value = register.Value.ToString().ToUpperInvariant();

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.String,
                            Value = value,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("STRINDEX", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register2.Type)
                {
                    case IptVariableTypes.String:
                        if (register1.Type != IptVariableTypes.String)
                        {
                            throw new Exception($"Wrong datatype {register1.Type}...");
                        }

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = register2.Value.ToString().IndexOf(register1.Value.ToString()),
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register2.Type}...");
                }
            }));
            iptCommands.TryAdd("STRLEN", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = register.Value.ToString().Length,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("SUBSTR", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register2.Type)
                {
                    case IptVariableTypes.String:
                        if (register1.Type != IptVariableTypes.String)
                        {
                            throw new Exception($"Wrong datatype {register1.Type}...");
                        }

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Bool,
                            Value = register2.Value.ToString().IndexOf(register1.Value.ToString()) != -1 ? 1 : 0,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register2.Type}...");
                }
            }));
            iptCommands.TryAdd("SUBSTRING", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);
                var register3 = popStack(iptTracking);

                switch (register3.Type)
                {
                    case IptVariableTypes.String:
                        if (register1.Type != IptVariableTypes.Integer && register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register1.Type}, {register2.Type}...");
                        }

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.String,
                            Value = register3.Value.ToString().Substring((int)register2.Value, (int)register1.Value),
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register3.Type}...");
                }
            }));
            iptCommands.TryAdd("GREPSTR", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register2.Type)
                {
                    case IptVariableTypes.String:
                        if (register1.Type != IptVariableTypes.String)
                        {
                            throw new Exception($"Wrong datatype {register1.Type}...");
                        }

                        var regExp = new Regex(register1.Value.ToString());

                        iptTracking.Grep = regExp.Matches(register2.Value.ToString())
                            .Cast<Match>()
                            .ToArray();

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Bool,
                            Value = iptTracking.Grep != null ? 1 : 0,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register2.Type}...");
                }
            }));
            iptCommands.TryAdd("GREPSUB", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        if (iptTracking.Grep != null)
                        {
                            register.Value = (register.Value?.ToString() ?? string.Empty).Trim();

                            for (var j = 1; j < 10; j++)
                            {
                                register.Value = register.Value?.ToString()?.Replace($"${j}", iptTracking.Grep[j].Value);
                            }

                            iptTracking.Stack.Push(new IptVariable
                            {
                                Type = IptVariableTypes.String,
                                Value = register.Value,
                            });
                        }

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("STRTOATOM", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Atomlist,
                            Value = Parser(iptTracking, register.Value.ToString(), false),
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            // End String Commands
            // Start Boolean Commands
            iptCommands.TryAdd("TRUE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Bool,
                    Value = 1,
                });
            }));
            iptCommands.TryAdd("FALSE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Bool,
                    Value = 0,
                });
            }));
            iptCommands.TryAdd("NOT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                Operator(iptTracking, "!", recursionDepth);
            }));
            iptCommands.TryAdd("OR", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                Operator(iptTracking, "||", recursionDepth);
            }));
            iptCommands.TryAdd("AND", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                Operator(iptTracking, "&&", recursionDepth);
            }));
            iptCommands.TryAdd("IF", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                if ((register1.Type != IptVariableTypes.Bool || register1.Type != IptVariableTypes.Integer) && register2.Type != IptVariableTypes.Atomlist)
                {
                    throw new Exception($"Wrong datatype {register1.Type}...");
                }

                switch (register1.Type)
                {
                    case IptVariableTypes.Bool:
                    case IptVariableTypes.Integer:
                        if ((int)register1.Value != 0)
                        {
                            Executor(register2.Value as IptAtomList, iptTracking, recursionDepth + 1);
                        }

                        break;
                }
            }));
            iptCommands.TryAdd("IFELSE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);
                var register3 = popStack(iptTracking);

                if ((register1.Type != IptVariableTypes.Bool || register1.Type != IptVariableTypes.Integer) && register2.Type != IptVariableTypes.Atomlist && register3.Type != IptVariableTypes.Atomlist)
                {
                    throw new Exception($"Wrong datatype {register1.Type}...");
                }

                switch (register1.Type)
                {
                    case IptVariableTypes.Bool:
                    case IptVariableTypes.Integer:
                        if ((int)register1.Value != 0)
                        {
                            Executor(register3.Value as IptAtomList, iptTracking, recursionDepth + 1);
                        }
                        else
                        {
                            Executor(register2.Value as IptAtomList, iptTracking, recursionDepth + 1);
                        }

                        break;
                }
            }));
            // End Boolean Commands
            // Start Array Commands
            iptCommands.TryAdd("ARRAY", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Array,
                            Value = new IptAtomList((int)register.Value),
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("DEF", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = iptTracking.Stack.Pop();
                var register2 = popStack(iptTracking);

                if (register1.Type != IptVariableTypes.Variable && register2.Type != IptVariableTypes.Atomlist && register2.Type != IptVariableTypes.Array)
                {
                    throw new Exception($"Wrong datatype {register1.Type}, {register2.Type}...");
                }

                setVariable(iptTracking, register1, register2, recursionDepth);
            }));
            iptCommands.TryAdd("GET", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register2.Type)
                {
                    case IptVariableTypes.Array:
                        var array = register2.Value as IptAtomList;

                        if (register1.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register1.Type}...");
                        }
                        else if ((int)register1.Value >= array.Count)
                        {
                            throw new Exception($"Index {register1.Value} out of bounds...");
                        }

                        iptTracking.Stack.Push(array[(int)register1.Value]);

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register2.Type}...");
                }
            }));
            iptCommands.TryAdd("PUT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);
                var register3 = popStack(iptTracking);

                switch (register2.Type)
                {
                    case IptVariableTypes.Array:
                        var array = register2.Value as IptAtomList;

                        if (register1.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register1.Type}...");
                        }
                        else if ((int)register1.Value >= array.Count)
                        {
                            throw new Exception($"Index {register1.Value} out of bounds...");
                        }

                        array[(int)register1.Value] = register3;

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register2.Type}...");
                }
            }));
            iptCommands.TryAdd("LENGTH", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Array:
                    case IptVariableTypes.String:
                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = (register.Value as IptAtomList).Count,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            // End Array Commands
            // Start Variable Commands
            iptCommands.TryAdd("ITOA", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Bool:
                    case IptVariableTypes.Integer:
                        var value = register.Value.ToString();

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.String,
                            Value = value,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("ATOI", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        var value = int.Parse(register.Value.ToString());

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = value,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("TOPTYPE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = iptTracking.Stack.PeekL();
                var typeID = 0;

                switch (register.Type)
                {
                    case IptVariableTypes.Bool:
                    case IptVariableTypes.Integer:
                        typeID = 1;

                        break;
                    case IptVariableTypes.Variable:
                        typeID = 2;

                        break;
                    case IptVariableTypes.Atomlist:
                        typeID = 3;

                        break;
                    case IptVariableTypes.String:
                        typeID = 4;

                        break;
                    case IptVariableTypes.Array:
                        typeID = 6;

                        break;
                }

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = typeID,
                });
            }));
            iptCommands.TryAdd("VARTYPE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = iptTracking.Stack.PeekL();
                register = getVariable(iptTracking, register);

                var typeID = 0;

                switch (register.Type)
                {
                    case IptVariableTypes.Bool:
                    case IptVariableTypes.Integer:
                        typeID = 1;

                        break;
                    case IptVariableTypes.Variable:
                        typeID = 2;

                        break;
                    case IptVariableTypes.Atomlist:
                        typeID = 3;

                        break;
                    case IptVariableTypes.String:
                        typeID = 4;

                        break;
                    case IptVariableTypes.Array:
                        typeID = 6;

                        break;
                }

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = typeID,
                });
            }));
            // End Variable Commands
            // Start Spot Commands
            iptCommands.TryAdd("INSPOT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        //for (var j = 0; j < $scope.model.RoomInfo.UserList.length; j++) {
                        //    if ($scope.model.RoomInfo.UserList[j].userID == $scope.model.UserInfo.userId) {
                        //        var xCoord = $scope.model.RoomInfo.UserList[j].roomPos.h;
                        //        var yCoord = $scope.model.RoomInfo.UserList[j].roomPos.v;
                        //        var inside = false;
                        //
                        //        for (var k = 0; k < !inside && $scope.model.RoomInfo.SpotList.length; k++) {
                        //            var spot = $scope.model.RoomInfo.SpotList[k];
                        //
                        //            if (spot.id == register.Value) {
                        //	            var polygon = [];
                        //
                        //	            for (var l = 0; l < spot.Vortexes.length; l++) {
                        //		            polygon.push({
                        //			            v: spot.loc.v + spot.Vortexes[l].v,
                        //			            h: spot.loc.h + spot.Vortexes[l].h,
                        //		            });
                        //	            }
                        //
                        //	            inside = utilService.pointInPolygon(polygon, {
                        //		            v: yCoord,
                        //		            h: xCoord,
                        //	            });
                        //
                        //	            break;
                        //            }
                        //        }
                        //
                        //        iptTracking.Stack.Push(new IptValue {
                        //            Type = IptTypes.Bool,
                        //            Value = inside ? 1 : 0,
                        //        });
                        //
                        //        break;
                        //    }
                        //}

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("LOCK", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        //$scope.serverSend(
                        //    'MSG_DOORLOCK',
                        //    {
                        //        roomID: $scope.model.RoomInfo.roomId,
                        //        spotID: register.Value,
                        //    });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("UNLOCK", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        //$scope.serverSend(
                        //    'MSG_DOORUNLOCK',
                        //    {
                        //        roomID: $scope.model.RoomInfo.roomId,
                        //        spotID: register.Value,
                        //    });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("ISLOCKED", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        //for (var j = 0; j < $scope.model.RoomInfo.SpotList.length; j++) {
                        //    var spot = $scope.model.RoomInfo.SpotList[j];
                        //    if (spot.id == register.Value) {
                        //        iptTracking.Stack.Push(new IptValue {
                        //            Type = IptTypes.Bool,
                        //            Value = spot.Type == HotSpotTypes.HT_Door && spot.state != 0 ? 1 : 0,
                        //        });
                        //
                        //        break;
                        //    }
                        //}

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("GETSPOTSTATE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        //for (var j = 0; j < $scope.model.RoomInfo.SpotList.length; j++) {
                        //    if ($scope.model.RoomInfo.SpotList[j].id == register.Value) {
                        //        iptTracking.Stack.Push(new IptValue {
                        //            Type = IptTypes.Integer,
                        //            Value = $scope.model.RoomInfo.SpotList[j].state,
                        //        });
                        //
                        //        break;
                        //    }
                        //}

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("SETSPOTSTATE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Integer:
                        if (register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        //for (var j = 0; j < $scope.model.RoomInfo.SpotList.length; j++) {
                        //    if ($scope.model.RoomInfo.SpotList[j].id == register1.Value) {
                        //        $scope.serverSend(
                        //            'MSG_SPOTSTATE',
                        //            {
                        //	            roomID: $scope.model.RoomInfo.roomId,
                        //	            spotID: register1.Value,
                        //	            state: register2.Value,
                        //            });
                        //
                        //        break;
                        //    }
                        //}

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("SETSPOTSTATELOCAL", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Integer:
                        if (register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        //for (var j = 0; j < $scope.model.RoomInfo.SpotList.length; j++) {
                        //    if ($scope.model.RoomInfo.SpotList[j].id == register1.Value) {
                        //        $scope.model.RoomInfo.SpotList[j].state == register2.Value;
                        //
                        //        $scope.model.Screen.spotLayerUpdate = true;
                        //
                        //        $scope.Screen_OnDraw();
                        //
                        //        break;
                        //    }
                        //}

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("SETLOC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);
                var register3 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Integer:
                        if (register2.Type != register3.Type || register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        //$scope.serverSend(
                        //    'MSG_SPOTMOVE',
                        //    {
                        //        roomID: $scope.model.RoomInfo.roomId,
                        //        spotID: register1.Value,
                        //        pos: {
                        //            h: register3.Value,
                        //            v: register2.Value,
                        //        }
                        //
                        //    });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("SETPICLOC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);
                var register3 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Integer:
                        if (register2.Type != register3.Type || register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        //$scope.serverSend(
                        //    'MSG_PICTMOVE',
                        //    {
                        //        roomID: $scope.model.RoomInfo.roomId,
                        //        spotID: register1.Value,
                        //        pos: {
                        //            h: register3.Value,
                        //            v: register2.Value,
                        //        }
                        //
                        //    });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("SELECT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        //$timeout(
                        //    $scope.Spot_OnEvent,
                        //    1,
                        //    false,
                        //    register.Value,
                        //    'SELECT'
                        //);

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("SETALARM", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Integer:
                        if (register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        //var futureTicks = Math.floor((register2.Value / 6) * 100);
                        //$timeout(
                        //    $scope.Spot_OnEvent,
                        //    futureTicks,
                        //    false,
                        //    register1.Value,
                        //    'ALARM'
                        //);

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("ME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var index = iptTracking.Variables.ContainsKey("ME") ? (int)iptTracking.Variables["ME"].Value.Value : 0;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = index,
                });
            }));
            iptCommands.TryAdd("DEST", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IUISessionState;
                if (sessionState == null) return;

                var index = iptTracking.Variables.ContainsKey("ME") ? (int)iptTracking.Variables["ME"].Value.Value : 0;
                if (index < 0) throw new Exception("Index out of bounds...");

                var spots = sessionState.RoomInfo.HotSpots.ToArray();
                if (index >= spots.Length) throw new Exception("Index out of bounds...");

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = spots[index].dest,
                });
            }));
            iptCommands.TryAdd("DOORIDX", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        //var doorList = [];

                        //for (var j = 0; j < $scope.model.RoomInfo.SpotList.length; j++) {
                        //    if ($scope.model.RoomInfo.SpotList[j].Type == HotSpotTypes.HT_Door) {
                        //        doorList.push($scope.model.RoomInfo.SpotList[j]);
                        //    }
                        //}

                        //if (register.Value < doorList.length) {
                        //    iptTracking.Stack.Push(new IptValue {
                        //        Type = IptTypes.Integer,
                        //        Value = doorList[register.Value].id,
                        //    });
                        //
                        //    break;
                        //}
                        //else {
                        //    throw 'Index out of bounds...';
                        //}

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("SPOTDEST", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                        var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IUISessionState;
                        if (sessionState == null) return;

                        var index = (Int32)register.Value;
                        if (index < 0) throw new Exception("Index out of bounds...");

                        var spots = sessionState.RoomInfo.HotSpots.ToArray();
                        if (index >= spots.Length) throw new Exception("Index out of bounds...");

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = spots[index].dest,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("NBRSPOTS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IUISessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = sessionState.RoomInfo.HotSpots.Count,
                });
            }));
            iptCommands.TryAdd("NBRDOORS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IUISessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = sessionState.RoomInfo.HotSpots
                        .Where(s => ((HotspotTypes)s.flags & HotspotTypes.HS_Door) == HotspotTypes.HS_Door)
                        .Count(),
                });
            }));
            iptCommands.TryAdd("SPOTNAME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                        var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IUISessionState;
                        if (sessionState == null) return;

                        var index = (Int32)register.Value;
                        if (index < 0) throw new Exception("Index out of bounds...");

                        var spots = sessionState.RoomInfo.HotSpots.ToArray();
                        if (index >= spots.Length) throw new Exception("Index out of bounds...");

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.String,
                            Value = spots[index].name,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            // End Spot Commands
            // Start Functional Commands
            iptCommands.TryAdd("BREAK", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                iptTracking.Break = true;
            }));
            iptCommands.TryAdd("RETURN", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                iptTracking.Return = true;
            }));
            iptCommands.TryAdd("EXIT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new Exception("Exiting Script");
            }));
            iptCommands.TryAdd("EXEC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Atomlist:
                        Executor(register.Value as IptAtomList, iptTracking, recursionDepth + 1);

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("ALARMEXEC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register2.Type)
                {
                    case IptVariableTypes.Atomlist:
                        if (register1.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register1.Type}...");
                        }

                        iptTracking.Alarms.Add(new IptAlarm(register2.Value as IptAtomList, (int)register1.Value));

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register2.Type}...");
                }
            }));
            iptCommands.TryAdd("WHILE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register2.Type)
                {
                    case IptVariableTypes.Atomlist:
                        if (register1.Type != IptVariableTypes.Atomlist)
                        {
                            throw new Exception($"Wrong datatype {register1.Type}...");
                        }

                        var limit = gWhileMaxIteration;

                        iptTracking.Break = false;

                        while (!iptTracking.Break && limit-- > 0)
                        {
                            Executor(register1.Value as IptAtomList, iptTracking, recursionDepth + 1);

                            var register3 = iptTracking.Stack.Pop();

                            if (register3.Type != IptVariableTypes.Bool && register3.Type != IptVariableTypes.Integer)
                            {
                                throw new Exception($"Wrong datatype {register3.Type}...");
                            }
                            else if ((int)register3.Value == 0)
                            {
                                break;
                            }

                            Executor(register2.Value as IptAtomList, iptTracking, recursionDepth + 1);
                        }

                        if (limit < 1)
                        {
                            throw new Exception("Endless loop, breaking...");
                        }

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register2.Type}...");
                }
            }));
            iptCommands.TryAdd("FOREACH", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register2.Type)
                {
                    case IptVariableTypes.Atomlist:
                        if (register1.Type != IptVariableTypes.Array)
                        {
                            throw new Exception($"Wrong datatype {register1.Type}...");
                        }

                        iptTracking.Break = false;

                        for (var j = 0; !iptTracking.Break && j < (register1.Value as IptAtomList).Count; j++)
                        {
                            iptTracking.Stack.Push((register1.Value as IptAtomList)[j]);

                            Executor(register2.Value as IptAtomList, iptTracking, recursionDepth + 1);
                        }

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register2.Type}...");
                }
            }));
            iptCommands.TryAdd("GLOBAL", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = iptTracking.Stack.Pop();

                switch (register.Type)
                {
                    case IptVariableTypes.Variable:
                        var key = register.Value?.ToString();

                        if (iptTracking.Variables.ContainsKey(key))
                        {
                            if (iptTracking.Variables[key].IsReadOnly ||
                                iptTracking.Variables[key].IsSpecial) break;

                            iptTracking.Variables[key].IsGlobal = true;
                        }
                        else
                        {
                            iptTracking.Variables[key] = new IptMetaVariable
                            {
                                Value = new IptVariable
                                {
                                    Type = IptVariableTypes.Integer,
                                    Value = 0,
                                },
                                Depth = recursionDepth,
                                IsGlobal = true,
                            };
                        }

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            // End Functional Commands
            // Start User Commands
            iptCommands.TryAdd("WHOME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IUISessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = sessionState.UserID,
                });
            }));
            iptCommands.TryAdd("MOVE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Bool:
                    case IptVariableTypes.Integer:
                        if (register2.Type != IptVariableTypes.Bool && register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) break;

                        var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as ISessionState;
                        if (sessionState == null) break;

                        ThreadManager.Current.Enqueue(ThreadQueues.Network, null, sessionState, NetworkCommandTypes.SEND, new Header
                        {
                            eventType = ThePalace.Core.Enums.EventTypes.MSG_USERMOVE,
                            protocolSend = new MSG_USERMOVE
                            {
                                pos = new Point
                                {
                                    h = (short)((int)register1.Value + sessionState.UserInfo.roomPos.h),
                                    v = (short)((int)register2.Value + sessionState.UserInfo.roomPos.v),
                                },
                            },
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("SETPOS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Bool:
                    case IptVariableTypes.Integer:
                        if (register2.Type != IptVariableTypes.Bool && register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) break;

                        var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as ISessionState;
                        if (sessionState == null) break;

                        ThreadManager.Current.Enqueue(ThreadQueues.Network, null, sessionState, NetworkCommandTypes.SEND, new Header
                        {
                            eventType = ThePalace.Core.Enums.EventTypes.MSG_USERMOVE,
                            protocolSend = new MSG_USERMOVE
                            {
                                pos = new Point
                                {
                                    h = (short)(int)register1.Value,
                                    v = (short)(int)register2.Value,
                                },
                            },
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("MOUSEPOS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //iptTracking.Stack.Push(new IptValue {
                // Type = IptTypes.Integer,
                // Value = $window.MousePositionX,
                //});

                //iptTracking.Stack.Push(new IptValue {
                // Type = IptTypes.Integer,
                // Value = $window.MousePositionY,
                //});
            }));
            iptCommands.TryAdd("POSX", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as ISessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = sessionState.UserInfo.roomPos.h,
                });
            }));
            iptCommands.TryAdd("POSY", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as ISessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = sessionState.UserInfo.roomPos.v,
                });
            }));
            iptCommands.TryAdd("SERVERNAME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.String,
                    Value = sessionState.ServerName,
                });
            }));
            iptCommands.TryAdd("ROOMNAME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.String,
                    Value = sessionState.RoomName,
                });
            }));
            iptCommands.TryAdd("ROOMID", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = sessionState.RoomID,
                });
            }));
            iptCommands.TryAdd("USERNAME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.String,
                    Value = sessionState.Name,
                });
            }));
            iptCommands.TryAdd("NBRROOMUSERS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = sessionState.RoomUsersInfo.Count,
                });
            }));
            iptCommands.TryAdd("ISWIZARD", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Bool,
                    Value = (sessionState.UserFlags & UserFlags.U_Moderator) != 0 ? 1 : 0,
                });
            }));
            iptCommands.TryAdd("ISGOD", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Bool,
                    Value = (sessionState.UserFlags & UserFlags.U_Administrator) != 0 ? 1 : 0,
                });
            }));
            iptCommands.TryAdd("ISGUEST", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                if (sessionState == null) return;

                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Bool,
                    Value = (sessionState.UserFlags & UserFlags.U_Guest) != 0 ? 1 : 0,
                });
            }));
            iptCommands.TryAdd("NAKED", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                if (sessionState == null) return;

                if (sessionState.UserInfo.assetSpec != null)
                    AssetsManager.Current.FreeAssets(
                        false,
                        sessionState.UserInfo.assetSpec
                            .Select(p => p.id)
                            .ToArray());
                sessionState.UserInfo.assetSpec.Clear();

                ThreadManager.Current.Enqueue(ThreadQueues.Network, null, sessionState, NetworkCommandTypes.SEND, new Header
                {
                    eventType = ThePalace.Core.Enums.EventTypes.MSG_USERPROP,
                    protocolSend = new MSG_USERPROP
                    {
                        assetSpec = new(),
                    },
                });
            }));
            iptCommands.TryAdd("WHOTARGET", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                //var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                //if (sessionState == null) return;

                //iptTracking.Stack.Push(new IptVariable
                //{
                //    Type = IptVariableTypes.Integer,
                //    Value = 0,
                //});

                throw new NotImplementedException("WHOTARGET");
            }));
            iptCommands.TryAdd("SETFACE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                        var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                        if (sessionState == null) return;

                        ThreadManager.Current.Enqueue(ThreadQueues.Network, null, sessionState, NetworkCommandTypes.SEND, new Header
                        {
                            eventType = ThePalace.Core.Enums.EventTypes.MSG_USERFACE,
                            protocolSend = new MSG_USERFACE
                            {
                                faceNbr = (short)(int)register.Value,
                            },
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("SETCOLOR", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                        var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                        if (sessionState == null) return;

                        ThreadManager.Current.Enqueue(ThreadQueues.Network, null, sessionState, NetworkCommandTypes.SEND, new Header
                        {
                            eventType = ThePalace.Core.Enums.EventTypes.MSG_USERCOLOR,
                            protocolSend = new MSG_USERCOLOR
                            {
                                colorNbr = (short)(int)register.Value,
                            },
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("WHONAME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //var register = popStack(iptTracking);

                //switch (register.Type)
                //{
                //    case IptVariableTypes.Integer:
                //        if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                //        var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                //        if (sessionState == null) return;

                //        var sourceID = (UInt32)register.Value;

                //        var user = null as UserRec;
                //        if (sourceID > 0)
                //              user = sessionState.RoomUsersInfo.GetValueLocked(sourceID);

                //        if (user == null) return;

                //        iptTracking.Stack.Push(new IptVariable
                //        {
                //            Type = IptVariableTypes.String,
                //            Value = user.name,
                //        });

                //        break;
                //    default:
                //        throw new Exception($"Wrong datatype {register.Type}...");
                //}

                throw new NotImplementedException("WHONAME");
            }));
            //iptCommands.TryAdd("WHOCHAT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            //{
            //}));
            iptCommands.TryAdd("WHOPOS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                if (sessionState == null) return;

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        {
                            var key = register.Value.ToString();
                            var user = sessionState.RoomUsersInfo.Values
                                .Where(u => u.name == key)
                                .FirstOrDefault();
                            if (user == null) return;

                            iptTracking.Stack.Push(new IptVariable
                            {
                                Type = IptVariableTypes.Integer,
                                Value = user.roomPos.h,
                            });

                            iptTracking.Stack.Push(new IptVariable
                            {
                                Type = IptVariableTypes.Integer,
                                Value = user.roomPos.v,
                            });
                        }

                        break;
                    case IptVariableTypes.Integer:
                        {
                            var sourceID = (UInt32)register.Value;

                            var user = null as UserRec;
                            if (sourceID > 0)
                                user = sessionState.RoomUsersInfo.GetValueLocked(sourceID);

                            if (user == null) return;

                            iptTracking.Stack.Push(new IptVariable
                            {
                                Type = IptVariableTypes.Integer,
                                Value = user.roomPos.h,
                            });

                            iptTracking.Stack.Push(new IptVariable
                            {
                                Type = IptVariableTypes.Integer,
                                Value = user.roomPos.v,
                            });
                        }

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("ROOMUSER", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                        var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IClientSessionState;
                        if (sessionState == null) return;

                        var index = (Int32)register.Value;
                        if (index < 0) throw new Exception("Index out of bounds...");

                        var users = sessionState.RoomUsersInfo.Values.ToArray();
                        if (index >= users.Length) throw new Exception("Index out of bounds...");

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = users[index].userID,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("GOTOROOM", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) break;

                        var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as ISessionState;
                        if (sessionState == null) break;

                        ThreadManager.Current.Enqueue(ThreadQueues.Network, null, sessionState, NetworkCommandTypes.SEND, new Header
                        {
                            eventType = ThePalace.Core.Enums.EventTypes.MSG_ROOMGOTO,
                            protocolSend = new MSG_ROOMGOTO
                            {
                                dest = (short)(int)register.Value,
                            },
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            // End User Commands
            // Start Misc Commands
            iptCommands.TryAdd("LAUNCHAPP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = iptTracking.Stack.Pop();

                // Effectively does nothing, just for legacy support
            }));
            iptCommands.TryAdd("DIMROOM", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) break;

                        var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IUISessionState;
                        if (sessionState == null) break;

                        var value = 1F - ((int)register.Value / 100F);

                        if (value > 1F) value = 1F;
                        else if (value < 0) value = 0F;

                        sessionState.LayerOpacity(value, ScreenLayers.DimRoom);
                        sessionState.RefreshScreen(ScreenLayers.DimRoom);

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("DELAY", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        Thread.Sleep(IptTracking.TicksToMilliseconds((int)register.Value));

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("BEEP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as IUISessionState;
                if (sessionState == null) return;

                ThreadManager.Current.Enqueue(ThreadQueues.Audio, null, sessionState, AudioCommandTypes.BEEP);
            }));
            iptCommands.TryAdd("CLIENTTYPE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.String,
                    Value = clientType,
                });
            }));
            iptCommands.TryAdd("GOTOURL", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        //$window.open(register.Value, '_blank');

                        throw new NotImplementedException("GOTOURL");
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("GOTOURLFRAME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.String:
                        if (register2.Type != IptVariableTypes.String)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        //$window.open(register2.Value, register1.Value);

                        throw new NotImplementedException("GOTOURLFRAME");
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("KILLUSER", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) break;

                        var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as ISessionState;
                        if (sessionState == null) break;

                        ThreadManager.Current.Enqueue(ThreadQueues.Network, null, sessionState, NetworkCommandTypes.SEND, new Header
                        {
                            eventType = ThePalace.Core.Enums.EventTypes.MSG_KILLUSER,
                            protocolSend = new MSG_KILLUSER
                            {
                                targetID = (UInt32)register.Value,
                            },
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("CLEARLOG", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                //$scope.model.Interface.LogList = [];

                throw new NotImplementedException("CLEARLOG");
            }));
            // End Misc Commands
            #endregion
            #region Iptscrae Version 2
            // Start Misc Commands
            iptCommands.TryAdd("ADDHEADER", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("ADDHEADER");
            }));
            iptCommands.TryAdd("ALERTBOX", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("ALERTBOX");
            }));
            iptCommands.TryAdd("BITAND", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("BITAND");
            }));
            iptCommands.TryAdd("BITOR", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("BITOR");
            }));
            iptCommands.TryAdd("BITSHIFTLEFT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("BITSHIFTLEFT");
            }));
            iptCommands.TryAdd("BITSHIFTRIGHT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("BITSHIFTRIGHT");
            }));
            iptCommands.TryAdd("BITXOR", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("BITXOR");
            }));
            iptCommands.TryAdd("CHARTONUM", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("CHARTONUM");
            }));
            iptCommands.TryAdd("CLEARTOOLTIP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("CLEARTOOLTIP");
            }));
            iptCommands.TryAdd("CLIENTID", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("CLIENTID");
            }));
            iptCommands.TryAdd("CONFIRMBOX", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("CONFIRMBOX");
            }));
            iptCommands.TryAdd("DECODEURL", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("DECODEURL");
            }));
            iptCommands.TryAdd("ENCODEURL", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("ENCODEURL");
            }));
            iptCommands.TryAdd("GETTIMEZONE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETTIMEZONE");
            }));
            iptCommands.TryAdd("HASHTOJSON", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("HASHTOJSON");
            }));
            iptCommands.TryAdd("HTTPGET", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("HTTPGET");
            }));
            iptCommands.TryAdd("HTTPPOST", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("HTTPPOST");
            }));
            iptCommands.TryAdd("HTTPCANCEL", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("HTTPCANCEL");
            }));
            iptCommands.TryAdd("JSONTOHASH", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("JSONTOHASH");
            }));
            iptCommands.TryAdd("LOADWEBSITE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("LOADWEBSITE");
            }));
            iptCommands.TryAdd("ISFUNCTION", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("ISFUNCTION");
            }));
            iptCommands.TryAdd("ISKEYDOWN", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("ISKEYDOWN");
            }));
            iptCommands.TryAdd("ISSOUNDPLAYING", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("ISSOUNDPLAYING");
            }));
            iptCommands.TryAdd("OPENPALACE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("OPENPALACE");
            }));
            iptCommands.TryAdd("PALACECHAT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("PALACECHAT");
            }));
            iptCommands.TryAdd("PROMPT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("PROMPT");
            }));
            iptCommands.TryAdd("NBRSERVERUSERS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("NBRSERVERUSERS");
            }));
            iptCommands.TryAdd("NEWHASH", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("NEWHASH");
            }));
            iptCommands.TryAdd("NUMTOCHAR", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("NUMTOCHAR");
            }));
            iptCommands.TryAdd("REGEXP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("REGEXP");
            }));
            iptCommands.TryAdd("REGEXPREPLACE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("REGEXPREPLACE");
            }));
            iptCommands.TryAdd("REMOVEHEADER", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("REMOVEHEADER");
            }));
            iptCommands.TryAdd("REPLACE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("REPLACE");
            }));
            iptCommands.TryAdd("REPLACEALL", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("REPLACEALL");
            }));
            iptCommands.TryAdd("RESETHEADERS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("RESETHEADERS");
            }));
            iptCommands.TryAdd("SELECTFILE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SELECTFILE");
            }));
            iptCommands.TryAdd("SETCURSOR", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETCURSOR");
            }));
            iptCommands.TryAdd("SETCURSORPIC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETCURSORPIC");
            }));
            iptCommands.TryAdd("SETTOOLTIP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETTOOLTIP");
            }));
            iptCommands.TryAdd("SETUSERNAME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETUSERNAME");
            }));
            iptCommands.TryAdd("SOUNDOPEN", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SOUNDOPEN");
            }));
            iptCommands.TryAdd("SOUNDPLAY", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SOUNDPLAY");
            }));
            //iptCommands.TryAdd("SOUNDPAUSE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            //{
            //    throw new NotImplementedException("CLEARLOG");
            //}));
            iptCommands.TryAdd("SOUNDSEEK", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SOUNDSEEK");
            }));
            iptCommands.TryAdd("SOUNDSTOP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SOUNDSTOP");
            }));
            iptCommands.TryAdd("SOUNDISPLAYING", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SOUNDISPLAYING");
            }));
            iptCommands.TryAdd("SOUNDGETPOSITION", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SOUNDGETPOSITION");
            }));
            iptCommands.TryAdd("SOUNDLENGTH", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SOUNDLENGTH");
            }));
            iptCommands.TryAdd("SOUNDPLAYFROM", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SOUNDPLAYFROM");
            }));
            iptCommands.TryAdd("STOPALARM", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("STOPALARM");
            }));
            iptCommands.TryAdd("STOPALARMS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("STOPALARMS");
            }));
            iptCommands.TryAdd("TEXTSPEECH", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("TEXTSPEECH");
            }));
            iptCommands.TryAdd("TIMEREXEC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("TIMEREXEC");
            }));
            iptCommands.TryAdd("UPDATELATER", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("UPDATELATER");
            }));
            iptCommands.TryAdd("UPDATENOW", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("UPDATENOW");
            }));
            iptCommands.TryAdd("WHOCOLOR", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("WHOCOLOR");
            }));
            iptCommands.TryAdd("WHOFACE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("WHOFACE");
            }));
            // End Misc Commands
            // Start Spot Commands
            iptCommands.TryAdd("ADDPIC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("ADDPIC");
            }));
            iptCommands.TryAdd("ADDPICNAME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("ADDPICNAME");
            }));
            iptCommands.TryAdd("ADDSPOT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("ADDSPOT");
            }));
            iptCommands.TryAdd("AUTOUSERLAYER", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("AUTOUSERLAYER");
            }));
            iptCommands.TryAdd("CACHESCRIPT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("CACHESCRIPT");
            }));
            iptCommands.TryAdd("FILEDATE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("FILEDATE");
            }));
            iptCommands.TryAdd("FILEDELETE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("FILEDELETE");
            }));
            iptCommands.TryAdd("FILEEXISTS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("FILEEXISTS");
            }));
            iptCommands.TryAdd("GETBUBBLESTYLE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETBUBBLESTYLE");
            }));
            iptCommands.TryAdd("GETPICANGLE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETPICANGLE");
            }));
            iptCommands.TryAdd("GETPICDIMENSIONS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETPICDIMENSIONS");
            }));
            iptCommands.TryAdd("GETPICLOC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETPICLOC");
            }));
            iptCommands.TryAdd("GETSPOTLOC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETSPOTLOC");
            }));
            iptCommands.TryAdd("GETSPOTTEXTSIZE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETSPOTTEXTSIZE");
            }));
            iptCommands.TryAdd("GETSPOTPOINTS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETSPOTPOINTS");
            }));
            iptCommands.TryAdd("GETSPOTOPTIONS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETSPOTOPTIONS");
            }));
            iptCommands.TryAdd("GETROOMOPTIONS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETROOMOPTIONS");
            }));
            iptCommands.TryAdd("GETPICBRIGHTNESS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETPICBRIGHTNESS");
            }));
            iptCommands.TryAdd("GETPICOPACITY", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETPICOPACITY");
            }));
            iptCommands.TryAdd("GETPICSATURATION", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETPICSATURATION");
            }));
            iptCommands.TryAdd("GETPICNAME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETPICNAME");
            }));
            iptCommands.TryAdd("GETPICPIXEL", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("GETPICPIXEL");
            }));
            iptCommands.TryAdd("HIDEAVATARS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("HIDEAVATARS");
            }));
            iptCommands.TryAdd("IMAGETOMEDIA", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("IMAGETOMEDIA");
            }));
            iptCommands.TryAdd("ISRIGHTCLICK", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("ISRIGHTCLICK");
            }));
            iptCommands.TryAdd("INSERTPIC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("INSERTPIC");
            }));
            iptCommands.TryAdd("LOCINSPOT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("LOCINSPOT");
            }));
            iptCommands.TryAdd("MEDIAADDRESS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("MEDIAADDRESS");
            }));
            iptCommands.TryAdd("NBRPICFRAMES", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("NBRPICFRAMES");
            }));
            iptCommands.TryAdd("PAUSEPIC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("PAUSEPIC");
            }));
            iptCommands.TryAdd("REMOVEPIC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("REMOVEPIC");
            }));
            iptCommands.TryAdd("REMOVESPOT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("REMOVESPOT");
            }));
            iptCommands.TryAdd("RESUMEPIC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("RESUMEPIC");
            }));
            iptCommands.TryAdd("ROOMPICNAME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("ROOMPICNAME");
            }));
            iptCommands.TryAdd("ROOMWIDTH", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("ROOMWIDTH");
            }));
            iptCommands.TryAdd("ROOMHEIGHT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("ROOMHEIGHT");
            }));
            iptCommands.TryAdd("SETPICANGLE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETPICANGLE");
            }));
            iptCommands.TryAdd("SETPICFRAME", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETPICFRAME");
            }));
            iptCommands.TryAdd("SETLOCLOCAL", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETLOCLOCAL");
            }));
            iptCommands.TryAdd("SETPICLOCLOCAL", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETPICLOCLOCAL");
            }));
            iptCommands.TryAdd("SETPICBRIGHTNESS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETPICBRIGHTNESS");
            }));
            iptCommands.TryAdd("SETPICBLUR", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETPICBLUR");
            }));
            iptCommands.TryAdd("SETPICCONTRAST", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETPICCONTRAST");
            }));
            iptCommands.TryAdd("SETPICHUE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETPICHUE");
            }));
            iptCommands.TryAdd("SETPICOPACITY", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETPICOPACITY");
            }));
            iptCommands.TryAdd("SETPICSATURATION", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETPICSATURATION");
            }));
            iptCommands.TryAdd("SETSPOTCLIP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETSPOTCLIP");
            }));
            iptCommands.TryAdd("SETSPOTCURVE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETSPOTCURVE");
            }));
            iptCommands.TryAdd("SETSPOTFONT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETSPOTFONT");
            }));
            iptCommands.TryAdd("SETSPOTNAMELOCAL", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETSPOTNAMELOCAL");
            }));
            iptCommands.TryAdd("SETSPOTOPTIONS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETSPOTOPTIONS");
            }));
            iptCommands.TryAdd("SETSPOTPICMODE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETSPOTPICMODE");
            }));
            iptCommands.TryAdd("SETSPOTPOINTS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETSPOTPOINTS");
            }));
            iptCommands.TryAdd("SETSPOTSCRIPT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETSPOTSCRIPT");
            }));
            iptCommands.TryAdd("SETSPOTSTYLE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SETSPOTSTYLE");
            }));
            iptCommands.TryAdd("SHOWAVATARS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SHOWAVATARS");
            }));
            iptCommands.TryAdd("WEBEMBED", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("WEBEMBED");
            }));
            iptCommands.TryAdd("WEBLOCATION", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("WEBLOCATION");
            }));
            iptCommands.TryAdd("WEBTITLE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("WEBTITLE");
            }));
            iptCommands.TryAdd("WEBSCRIPT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("WEBSCRIPT");
            }));
            // End Spot Commands
            // Start Prop Commands
            iptCommands.TryAdd("IMAGETOPROP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("IMAGETOPROP");
            }));
            iptCommands.TryAdd("LOADPROPS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("LOADPROPS");
            }));
            iptCommands.TryAdd("LOOSEPROP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("LOOSEPROP");
            }));
            iptCommands.TryAdd("LOOSEPROPIDX", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("LOOSEPROPIDX");
            }));
            iptCommands.TryAdd("LOOSEPROPPOS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("LOOSEPROPPOS");
            }));
            iptCommands.TryAdd("MOVELOOSEPROP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("MOVELOOSEPROP");
            }));
            iptCommands.TryAdd("NBRLOOSEPROPS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("NBRLOOSEPROPS");
            }));
            iptCommands.TryAdd("PROPDIMENSIONS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("PROPDIMENSIONS");
            }));
            iptCommands.TryAdd("PROPOFFSETS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("PROPOFFSETS");
            }));
            iptCommands.TryAdd("REMOVELOOSEPROP", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("REMOVELOOSEPROP");
            }));
            // End Prop Commands
            // Start Paint Commands
            iptCommands.TryAdd("OVAL", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("OVAL");
            }));
            iptCommands.TryAdd("PENOPACITY", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("PENOPACITY");
            }));
            iptCommands.TryAdd("PENFILLCOLOR", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("PENFILLCOLOR");
            }));
            iptCommands.TryAdd("PENFILLOPACITY", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("PENFILLOPACITY");
            }));
            iptCommands.TryAdd("POLYGON", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("POLYGON");
            }));
            iptCommands.TryAdd("DRAWTEXT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("DRAWTEXT");
            }));
            iptCommands.TryAdd("PENFONT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("PENFONT");
            }));
            iptCommands.TryAdd("PENBOLD", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("PENBOLD");
            }));
            iptCommands.TryAdd("PENUNDERLINE", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("PENUNDERLINE");
            }));
            iptCommands.TryAdd("PENITALIC", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("PENITALIC");
            }));
            iptCommands.TryAdd("SHOWPAINT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                throw new NotImplementedException("SHOWPAINT");
            }));
            // End Paint Commands
            #endregion
            #region Iptscrae Version 3
            // Start Message Commands
#if DEBUG
            iptCommands.TryAdd("DEBUGMSG", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        Debug.WriteLine(register.Value.ToString());

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("CONSOLEMSG", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        Console.WriteLine(register.Value.ToString());

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("CONNECT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.String:
                        if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) break;

                        var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as ISessionState;
                        if (sessionState == null) break;

                        ThreadManager.Current.Enqueue(ThreadQueues.Network, null, sessionState, NetworkCommandTypes.CONNECT, register.Value.ToString());

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("DISCONNECT", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                if (!iptTracking.Variables.ContainsKey("SESSIONSTATE")) return;

                var sessionState = iptTracking.Variables["SESSIONSTATE"].Value.Value as ISessionState;
                if (sessionState == null) return;

                NetworkManager.Current.Disconnect(sessionState);
            }));
#endif
            // End Message Commands
            iptCommands.TryAdd("TRYCATCH", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                if (register1.Type != IptVariableTypes.Atomlist && register2.Type != IptVariableTypes.Atomlist)
                {
                    throw new Exception($"Wrong datatype {register1.Type}, {register2.Type}...");
                }

                try
                {
                    Executor(register1.Value as IptAtomList, iptTracking, recursionDepth + 1);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif

                    iptTracking.Variables["ERRORMSG"] = new IptMetaVariable
                    {
                        Value = new IptVariable
                        {
                            Type = IptVariableTypes.String,
                            Value = ex.Message,
                        },
                    };
                    iptTracking.Variables["ERRORMSG"].IsReadOnly = true;

                    Executor(register2.Value as IptAtomList, iptTracking, recursionDepth + 1);
                }
            }));
            iptCommands.TryAdd("READONLY", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = iptTracking.Stack.Pop();

                switch (register.Type)
                {
                    case IptVariableTypes.Variable:
                        var key = register.Value?.ToString();

                        if (iptTracking.Variables.ContainsKey(key))
                        {
                            if (iptTracking.Variables[key].IsSpecial) break;

                            iptTracking.Variables[key].IsReadOnly = true;
                        }

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            // Start Math Commands
            iptCommands.TryAdd("PI", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = (int)(Math.PI * 1000000),
                });
            }));
            iptCommands.TryAdd("ABS", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Integer:
                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = Math.Abs((int)register.Value),
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("AVG", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Array:
                        var values = (register.Value as IptAtomList)
                        .Select(v =>
                        {
                            if (v.Type != IptVariableTypes.Integer)
                            {
                                throw new Exception($"Wrong datatype {v.Type}...");
                            }

                            return (decimal)v.Value;
                        })
                        .ToList();
                        var avg = values.Sum() / values.Count;

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = (int)avg,
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("POW", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register1 = popStack(iptTracking);
                var register2 = popStack(iptTracking);

                switch (register1.Type)
                {
                    case IptVariableTypes.Integer:
                        if (register2.Type != IptVariableTypes.Integer)
                        {
                            throw new Exception($"Wrong datatype {register2.Type}...");
                        }

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = Math.Pow((int)register2.Value, (int)register1.Value),
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register1.Type}...");
                }
            }));
            iptCommands.TryAdd("SUM", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Array:
                        var values = (register.Value as IptAtomList)
                        .Select(v =>
                        {
                            if (v.Type != IptVariableTypes.Integer)
                            {
                                throw new Exception($"Wrong datatype {v.Type}...");
                            }

                            return (decimal)v.Value;
                        })
                        .ToList();

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = (int)values.Sum(),
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("MIN", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Array:
                        var values = (register.Value as IptAtomList)
                        .Select(v =>
                        {
                            if (v.Type != IptVariableTypes.Integer)
                            {
                                throw new Exception($"Wrong datatype {v.Type}...");
                            }

                            return (decimal)v.Value;
                        })
                        .ToList();

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = (int)values.Min(),
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            iptCommands.TryAdd("MAX", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                var register = popStack(iptTracking);

                switch (register.Type)
                {
                    case IptVariableTypes.Array:
                        var values = (register.Value as IptAtomList)
                        .Select(v =>
                        {
                            if (v.Type != IptVariableTypes.Integer)
                            {
                                throw new Exception($"Wrong datatype {v.Type}...");
                            }

                            return (decimal)v.Value;
                        })
                        .ToList();

                        iptTracking.Stack.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = (int)values.Max(),
                        });

                        break;
                    default:
                        throw new Exception($"Wrong datatype {register.Type}...");
                }
            }));
            // End Math Commands
            #endregion

            iptCommands.TryAdd("IPTVERSION", (IptCommandFnc)((iptTracking, recursionDepth) =>
            {
                iptTracking.Stack.Push(new IptVariable
                {
                    Type = IptVariableTypes.Integer,
                    Value = (int)iptVersion,
                });
            }));

            iptOperators.TryAdd("~", new IptOperator
            {
                Flags = IptOperatorFlags.Unary | IptOperatorFlags.Push | IptOperatorFlags.NOT,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = ~(int)register1.Value,
                    },
            });
            iptOperators.TryAdd("!", new IptOperator
            {
                Flags = IptOperatorFlags.Unary | IptOperatorFlags.Boolean | IptOperatorFlags.Push | IptOperatorFlags.NOT,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Bool,
                        Value = (int)register1.Value == 0 ? 1 : 0,
                    },
            });
            iptOperators.TryAdd("++", new IptOperator
            {
                Flags = IptOperatorFlags.Unary | IptOperatorFlags.Assigning | IptOperatorFlags.Math | IptOperatorFlags.Add,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = (int)register1.Value + 1,
                    },
            });
            iptOperators.TryAdd("--", new IptOperator
            {
                Flags = IptOperatorFlags.Unary | IptOperatorFlags.Assigning | IptOperatorFlags.Math | IptOperatorFlags.Subtract,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = (int)register1.Value - 1,
                    },
            });
            iptOperators.TryAdd("-", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Math | IptOperatorFlags.Subtract,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = (int)register1.Value - (int)register2.Value,
                    },
            });
            iptOperators.TryAdd("*", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Math | IptOperatorFlags.Multiply,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = (int)register1.Value * (int)register2.Value,
                    },
            });
            iptOperators.TryAdd("/", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Math | IptOperatorFlags.Divide,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = (int)register1.Value / (int)register2.Value,
                    },
            });
            iptOperators.TryAdd("%", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Math | IptOperatorFlags.Mod,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = (int)register1.Value % (int)register2.Value,
                    },
            });
            iptOperators.TryAdd("&", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Concate,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.String,
                        Value = $"{register1.Value}{register2.Value}",
                    },
            });
            iptOperators.TryAdd("&&", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Boolean | IptOperatorFlags.AND,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Bool,
                        Value = (int)register1.Value != 0 && (int)register2.Value != 0 ? 1 : 0,
                    },
            });
            iptOperators.TryAdd("||", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Boolean | IptOperatorFlags.OR,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Bool,
                        Value = (int)register1.Value != 0 || (int)register2.Value != 0 ? 1 : 0,
                    },
            });
            iptOperators.TryAdd("=", new IptOperator
            {
                Flags = IptOperatorFlags.Assigning,
                OpFnc = (register1, register2) => register2,
            });
            iptOperators.TryAdd("~=", new IptOperator
            {
                Flags = IptOperatorFlags.Unary | IptOperatorFlags.Assigning | IptOperatorFlags.NOT,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = ~(int)register1.Value,
                    },
            });
            iptOperators.TryAdd("|=", new IptOperator
            {
                Flags = IptOperatorFlags.Assigning | IptOperatorFlags.Boolean | IptOperatorFlags.OR,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Bool,
                        Value = (int)register1.Value != 0 || (int)register2.Value != 0 ? 1 : 0,
                    },
            });
            iptOperators.TryAdd("-=", new IptOperator
            {
                Flags = IptOperatorFlags.Assigning | IptOperatorFlags.Math | IptOperatorFlags.Subtract,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = (int)register1.Value - (int)register2.Value,
                    },
            });
            iptOperators.TryAdd("+=", new IptOperator
            {
                Flags = IptOperatorFlags.Assigning | IptOperatorFlags.Math | IptOperatorFlags.Add,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = (int)register1.Value + (int)register2.Value,
                    },
            });
            iptOperators.TryAdd("*=", new IptOperator
            {
                Flags = IptOperatorFlags.Assigning | IptOperatorFlags.Math | IptOperatorFlags.Multiply,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = (int)register1.Value * (int)register2.Value,
                    },
            });
            iptOperators.TryAdd("/=", new IptOperator
            {
                Flags = IptOperatorFlags.Assigning | IptOperatorFlags.Math | IptOperatorFlags.Divide,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = (int)register1.Value / (int)register2.Value,
                    },
            });
            iptOperators.TryAdd("%=", new IptOperator
            {
                Flags = IptOperatorFlags.Assigning | IptOperatorFlags.Math | IptOperatorFlags.Mod,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Integer,
                        Value = (int)register1.Value % (int)register2.Value,
                    },
            });
            iptOperators.TryAdd(">", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Comparator | IptOperatorFlags.GreaterThan,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Bool,
                        Value = (int)register1.Value > (int)register2.Value ? 1 : 0,
                    },
            });
            iptOperators.TryAdd(">=", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Comparator | IptOperatorFlags.GreaterThan | IptOperatorFlags.EqualTo,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Bool,
                        Value = (int)register1.Value >= (int)register2.Value ? 1 : 0,
                    },
            });
            iptOperators.TryAdd("<", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Comparator | IptOperatorFlags.LessThan,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Bool,
                        Value = (int)register1.Value < (int)register2.Value ? 1 : 0,
                    },
            });
            iptOperators.TryAdd("<=", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Comparator | IptOperatorFlags.LessThan | IptOperatorFlags.EqualTo,
                OpFnc = (register1, register2) =>
                    new IptVariable
                    {
                        Type = IptVariableTypes.Bool,
                        Value = (int)register1.Value <= (int)register2.Value ? 1 : 0,
                    },
            });
            iptOperators.TryAdd("!=", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Comparator | IptOperatorFlags.NotEqualTo,
                OpFnc = (register1, register2) =>
                {
                    switch (register1.Type)
                    {
                        case IptVariableTypes.String:
                            return new IptVariable
                            {
                                Type = IptVariableTypes.Bool,
                                Value = register1.Value.ToString() != register2.Value.ToString() ? 1 : 0,
                            };
                        case IptVariableTypes.Bool:
                        case IptVariableTypes.Integer:
                            return new IptVariable
                            {
                                Type = IptVariableTypes.Bool,
                                Value = (int)register1.Value != (int)register2.Value ? 1 : 0,
                            };
                        default:
                            throw new Exception($"Wrong datatype {register1.Type}...");
                    }
                },
            });
            iptOperators.TryAdd("<>", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Comparator | IptOperatorFlags.NotEqualTo,
                OpFnc = (register1, register2) =>
                {
                    switch (register1.Type)
                    {
                        case IptVariableTypes.String:
                            return new IptVariable
                            {
                                Type = IptVariableTypes.Bool,
                                Value = register1.Value.ToString() != register2.Value.ToString() ? 1 : 0,
                            };
                        case IptVariableTypes.Bool:
                        case IptVariableTypes.Integer:
                            return new IptVariable
                            {
                                Type = IptVariableTypes.Bool,
                                Value = (int)register1.Value != (int)register2.Value ? 1 : 0,
                            };
                        default:
                            throw new Exception($"Wrong datatype {register1.Type}...");
                    }
                },
            });
            iptOperators.TryAdd("==", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Comparator | IptOperatorFlags.EqualTo,
                OpFnc = (register1, register2) =>
                {
                    switch (register1.Type)
                    {
                        case IptVariableTypes.String:
                            return new IptVariable
                            {
                                Type = IptVariableTypes.Bool,
                                Value = register1.Value.ToString() == register2.Value.ToString() ? 1 : 0,
                            };
                        case IptVariableTypes.Bool:
                        case IptVariableTypes.Integer:
                            return new IptVariable
                            {
                                Type = IptVariableTypes.Bool,
                                Value = (int)register1.Value == (int)register2.Value ? 1 : 0,
                            };
                        default:
                            throw new Exception($"Wrong datatype {register1.Type}...");
                    }
                },
            });
            iptOperators.TryAdd("+", new IptOperator
            {
                Flags = IptOperatorFlags.Push | IptOperatorFlags.Math | IptOperatorFlags.Add | IptOperatorFlags.Concate,
                OpFnc = (register1, register2) =>
                {
                    switch (register1.Type)
                    {
                        case IptVariableTypes.String:
                            return new IptVariable
                            {
                                Type = IptVariableTypes.String,
                                Value = $"{register1.Value}{register2.Value}",
                            };
                        case IptVariableTypes.Bool:
                        case IptVariableTypes.Integer:
                            return new IptVariable
                            {
                                Type = IptVariableTypes.Integer,
                                Value = (int)register1.Value + (int)register2.Value,
                            };
                        default:
                            throw new Exception($"Wrong datatype {register1.Type}...");
                    }
                },
            });
            iptOperators.TryAdd("&=", new IptOperator
            {
                Flags = IptOperatorFlags.Assigning | IptOperatorFlags.Boolean | IptOperatorFlags.AND | IptOperatorFlags.Concate,
                OpFnc = (register1, register2) =>
                {
                    switch (register1.Type)
                    {
                        case IptVariableTypes.String:
                            return new IptVariable
                            {
                                Type = IptVariableTypes.String,
                                Value = $"{register1.Value}{register2.Value}",
                            };
                        case IptVariableTypes.Bool:
                        case IptVariableTypes.Integer:
                            return new IptVariable
                            {
                                Type = IptVariableTypes.Bool,
                                Value = (int)register1.Value != 0 && (int)register2.Value != 0 ? 1 : 0,
                            };
                        default:
                            throw new Exception($"Wrong datatype {register1.Type}...");
                    }
                },
            });
        }

        internal static void Operator(IptTracking iptTracking, string opKey, int recursionDepth = 0)
        {
            if (iptOperators.ContainsKey(opKey))
            {
                var flags = iptOperators[opKey].Flags;

                var register1 = iptTracking.Stack.Pop();
                if ((flags & IptOperatorFlags.Assigning) == IptOperatorFlags.Assigning &&
                    register1.Type != IptVariableTypes.Variable)
                {
                    throw new Exception($"Cannot assign {register1.Value}...");
                }

                var originalRegister1 = register1;
                var errorDataType1 = true;

                var register2 = null as IptVariable;
                var errorDataType2 = true;

                register1 = getVariable(iptTracking, register1);

                if ((flags & (IptOperatorFlags.Boolean | IptOperatorFlags.Math)) != 0 &&
                    (register1.Type == IptVariableTypes.Bool ||
                    register1.Type == IptVariableTypes.Integer))
                    errorDataType1 = false;
                else if ((flags & IptOperatorFlags.Concate) == IptOperatorFlags.Concate &&
                    register1.Type == IptVariableTypes.String)
                    errorDataType1 = false;

                // Exclude Unary Operators
                if ((flags & IptOperatorFlags.Unary) == 0)
                {
                    register2 = iptTracking.Stack.Pop();
                    register2 = getVariable(iptTracking, register2);

                    if ((flags & (IptOperatorFlags.Boolean | IptOperatorFlags.Math)) != 0 &&
                        (register2.Type == IptVariableTypes.Bool ||
                        register2.Type == IptVariableTypes.Integer))
                        errorDataType2 = false;
                    else if ((flags & IptOperatorFlags.Concate) == IptOperatorFlags.Concate &&
                        register2.Type == IptVariableTypes.String)
                        errorDataType2 = false;
                    else if ((flags & IptOperatorFlags.Comparator) == IptOperatorFlags.Comparator &&
                        register1.Type == register2.Type)
                        switch (register1.Type)
                        {
                            case IptVariableTypes.Bool:
                            case IptVariableTypes.Integer:
                            case IptVariableTypes.String:
                                errorDataType1 = false;
                                errorDataType2 = false;
                                break;
                        }
                }
                else
                    errorDataType2 = false;

                if ((flags & (IptOperatorFlags.Boolean | IptOperatorFlags.Math | IptOperatorFlags.Concate | IptOperatorFlags.Comparator)) == 0)
                {
                    errorDataType1 = false;
                    errorDataType2 = false;
                }

                if (errorDataType1 &&
                    errorDataType2)
                {
                    throw new Exception($"Wrong datatypes {register1.Type} {opKey} {register2.Type}...");
                }
                else if (errorDataType1)
                {
                    throw new Exception($"Wrong datatype {register1.Type}...");
                }
                else if (errorDataType2)
                {
                    throw new Exception($"Wrong datatype {register2.Type}...");
                }

                var result = null as IptVariable;

                if (iptOperators[opKey].OpFnc != null)
                {
                    result = iptOperators[opKey].OpFnc(register1, register2);
                }

                if (result == null)
                {
                    throw new Exception($"Unexpected result {register1.Value} {opKey} {register2.Value}...");
                }
                else if ((flags & IptOperatorFlags.Push) == IptOperatorFlags.Push)
                {
                    iptTracking.Stack.Push(result);
                }
                else if ((flags & IptOperatorFlags.Assigning) == IptOperatorFlags.Assigning)
                {
                    setVariable(iptTracking, originalRegister1, result, recursionDepth);
                }
            }
        }
        public static void Executor(IptAtomList AtomList, IptTracking iptTracking, int recursionDepth = 0)
        {
            for (var j = 0; j < AtomList.Count; j++)
            {
                var key = AtomList[j].Value?.ToString();

                if (key == null) continue;
                else switch (AtomList[j].Type)
                    {
                        case IptVariableTypes.Command:
                            iptTracking.Return = false;

                            ((IptCommandFnc)iptCommands[key])(iptTracking, recursionDepth);

                            if (iptTracking.Return) return;

                            break;
                        case IptVariableTypes.Operator:
                            Operator(iptTracking, key, recursionDepth);

                            break;
                        default:
                            iptTracking.Stack.Push(AtomList[j]);

                            break;
                    }
            }

            // Garbage Collection:
            if (recursionDepth == 0)
            {
                foreach (var key in iptTracking.Variables.Keys)
                    if (!iptTracking.Variables[key].IsGlobal &&
                        !iptTracking.Variables[key].IsSpecial)
                        iptTracking.Variables.Remove(key);
            }
            else
            {
                foreach (var key in iptTracking.Variables.Keys)
                    if (!iptTracking.Variables[key].IsGlobal &&
                        !iptTracking.Variables[key].IsSpecial &&
                        iptTracking.Variables[key].Depth >= recursionDepth)
                        iptTracking.Variables.Remove(key);
            }

            if (iptTracking.Stack.Count > 0)
            {
                if (iptTracking.Stack.Count > StackMaxSize)
                {
                    iptTracking.Stack.RemoveRange(StackMaxSize, iptTracking.Stack.Count);

#if DEBUG
                    Debug.WriteLine("Stack space exceeded...");
#endif
                }

                if (recursionDepth < 1)
                {
#if DEBUG
                    Debug.WriteLine("Stack wasn't empty upon exit...");
#endif
                }
            }
        }
        public static IptAtomList Parser(IptTracking iptTracking, string str, bool hasEvents = true)
        {
#if DEBUG
            Debug.WriteLine($"PARSING: {str}");
#endif
            var chars = str?.ToCharArray() ?? Array.Empty<char>();
            var result = new IptAtomList();

            for (var j = 0; j < chars.Length;)
            {
                if (char.IsWhiteSpace(chars[j]))
                {
                    j++;
                    continue;
                }
                else if (chars[j] == ';')
                {
                    for (j++; j < chars.Length; j++)
                    {
                        if (chars[j] == '\r' || chars[j] == '\n')
                        {
                            break;
                        }
                    }
                }
                else if (hasEvents && chars[j] == 'O' && chars[j + 1] == 'N' && char.IsWhiteSpace(chars[j + 2]))
                {
                    var tokenStart = j += 3;
                    var bracketDepth = 0;
                    var arrayDepth = 0;

                    for (; j < chars.Length; j++)
                    {
                        if (chars[j] == '{')
                        {
                            break;
                        }
                    }

                    var eventName = str.Substr(tokenStart, j)
                        .Trim()
                        .ToUpperInvariant();

                    if (!EventTypes.ContainsKey(eventName))
                    {
                        throw new Exception($"Unexpected event {eventName}...");
                    }

                    var eventType = EventTypes[eventName];

                    tokenStart = j;

                    for (; j < chars.Length; j++)
                    {
                        if (chars[j] == '"')
                        {
                            for (; j < chars.Length; j++)
                            {
                                if (chars[j] == '"')
                                {
                                    break;
                                }
                                else if (chars[j] == '\\' && chars[j + 1] == '"')
                                {
                                    j++;
                                }
                            }
                        }
                        else if (chars[j] == '{')
                        {
                            bracketDepth++;

                            if (bracketDepth > gNestedAtomlistMaxDepth)
                            {
                                throw new Exception("Exceeded max atomlist depth...");
                            }
                        }
                        else if (chars[j] == '}')
                        {
                            if (bracketDepth > 0)
                            {
                                bracketDepth--;

                                if (bracketDepth == 0)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                throw new Exception("Unexpected closing bracket \"}\"");
                            }
                        }
                        else if (chars[j] == '[')
                        {
                            arrayDepth++;

                            if (arrayDepth > gNestedArrayMaxDepth)
                            {
                                throw new Exception("Exceeded max array depth...");
                            }
                        }
                        else if (chars[j] == ']')
                        {
                            if (arrayDepth > 0)
                            {
                                arrayDepth--;
                            }
                            else
                            {
                                throw new Exception("Unexpected closing bracket \"]\"");
                            }
                        }
                    }

                    var atomlistStr = str.Substr(tokenStart + 1, j - 1);

                    iptTracking.Events[eventType] = Parser(iptTracking, atomlistStr, false);

                    j++;
                    continue;
                }
                else if (chars[j] == '"')
                {
                    var tokenStart = j;

                    for (j++; j < chars.Length; j++)
                    {
                        if (chars[j] == '"')
                        {
                            break;
                        }
                        else if (chars[j] == '\\' && chars[j + 1] == '"')
                        {
                            j++;
                        }
                    }

                    var substr = str.Substr(tokenStart + 1, j);

                    result.Push(new IptVariable
                    {
                        Type = IptVariableTypes.String,
                        Value = substr,
                    });

                    j++;
                    continue;
                }
                else if (chars[j] == '{')
                {
                    var bracketDepth = 0;
                    var arrayDepth = 0;
                    var tokenStart = j;

                    for (; j < chars.Length; j++)
                    {
                        if (chars[j] == '"')
                        {
                            for (; j < chars.Length; j++)
                            {
                                if (chars[j] == '"')
                                {
                                    break;
                                }
                                else if (chars[j] == '\\' && chars[j + 1] == '"')
                                {
                                    j++;
                                }
                            }
                        }
                        else if (chars[j] == '{')
                        {
                            bracketDepth++;

                            if (bracketDepth > gNestedAtomlistMaxDepth)
                            {
                                throw new Exception("Exceeded max atomlist depth...");
                            }
                        }
                        else if (chars[j] == '}')
                        {
                            if (bracketDepth > 0)
                            {
                                bracketDepth--;

                                if (bracketDepth == 0)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                throw new Exception("Unexpected closing bracket \"}\"");
                            }
                        }
                        else if (chars[j] == '[')
                        {
                            arrayDepth++;

                            if (arrayDepth > gNestedArrayMaxDepth)
                            {
                                throw new Exception("Exceeded max array depth...");
                            }
                        }
                        else if (chars[j] == ']')
                        {
                            if (arrayDepth > 0)
                            {
                                arrayDepth--;
                            }
                            else
                            {
                                throw new Exception("Unexpected closing bracket \"]\"");
                            }
                        }
                    }

                    var atomlistContents = str.Substr(tokenStart + 1, j);

                    result.Push(new IptVariable
                    {
                        Type = IptVariableTypes.Atomlist,
                        Value = Parser(iptTracking, atomlistContents, false),
                    });

                    j++;
                    continue;
                }
                else if (chars[j] == '[')
                {
                    var bracketDepth = 0;
                    var arrayDepth = 0;
                    var tokenStart = j;

                    for (; j < chars.Length; j++)
                    {
                        if (chars[j] == '"')
                        {
                            for (; j < chars.Length; j++)
                            {
                                if (chars[j] == '"')
                                {
                                    break;
                                }
                                else if (chars[j] == '\\' && chars[j + 1] == '"')
                                {
                                    j++;
                                }
                            }
                        }
                        else if (chars[j] == '{')
                        {
                            bracketDepth++;

                            if (bracketDepth > gNestedAtomlistMaxDepth)
                            {
                                throw new Exception("Exceeded max atomlist depth...");
                            }
                        }
                        else if (chars[j] == '}')
                        {
                            if (bracketDepth > 0)
                            {
                                bracketDepth--;
                            }
                            else
                            {
                                throw new Exception("Unexpected closing bracket \"}\"");
                            }
                        }
                        else if (chars[j] == '[')
                        {
                            arrayDepth++;

                            if (arrayDepth > gNestedArrayMaxDepth)
                            {
                                throw new Exception("Exceeded max array depth...");
                            }
                        }
                        else if (chars[j] == ']')
                        {
                            if (arrayDepth > 0)
                            {
                                arrayDepth--;

                                if (arrayDepth == 0)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                throw new Exception("Unexpected closing bracket \"]\"");
                            }
                        }
                    }

                    var arrayContents = str.Substr(tokenStart + 1, j);

                    result.Push(new IptVariable
                    {
                        Type = IptVariableTypes.Array,
                        Value = Parser(iptTracking, arrayContents, false),
                    });

                    j++;
                    continue;
                }
                else if (char.IsDigit(chars[j]) ||
                    chars[j] == '-' && char.IsDigit(chars[j + 1]))
                {
                    var tokenStart = j;

                    for (j++; j < chars.Length; j++)
                    {
                        if (!char.IsDigit(chars[j]) ||
                            char.IsWhiteSpace(chars[j]))
                        {
                            break;
                        }
                    }

                    var token = str.Substr(tokenStart, j);

                    try
                    {
                        var value = int.Parse(token);

                        result.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Integer,
                            Value = value,
                        });

                        //j++;
                        continue;
                    }
                    catch
                    {
                        throw new Exception($"Unexpected {token}...");
                    }
                }
                else if (new char[] { '~', '!', '=', '+', '-', '*', '/', '%', '<', '>', '&', '|' }.Contains(chars[j]))
                {
                    if (j + 1 < chars.Length && chars[j + 1] == '=')
                    {
                        var key = $"{chars[j]}{chars[j + 1]}";

                        if (iptOperators.ContainsKey(key))
                        {
                            result.Push(new IptVariable
                            {
                                Type = IptVariableTypes.Operator,
                                Value = key,
                            });
                        }

                        j += 2;
                        continue;
                    }
                    else if (
                        j + 1 < chars.Length &&
                        (chars[j] == '&' && chars[j + 1] == '&' ||
                        chars[j] == '|' && chars[j + 1] == '|' ||
                        chars[j] == '<' && chars[j + 1] == '>' ||
                        chars[j] == '+' && chars[j + 1] == '+' ||
                        chars[j] == '-' && chars[j + 1] == '-'))
                    {
                        var key = $"{chars[j]}{chars[j + 1]}";

                        if (iptOperators.ContainsKey(key))
                        {
                            result.Push(new IptVariable
                            {
                                Type = IptVariableTypes.Operator,
                                Value = key,
                            });
                        }

                        j += 2;
                        continue;
                    }
                    else
                    {
                        var key = $"{chars[j]}";

                        if (iptOperators.ContainsKey(key))
                        {
                            result.Push(new IptVariable
                            {
                                Type = IptVariableTypes.Operator,
                                Value = key,
                            });
                        }

                        j++;
                        continue;
                    }
                }
                else
                {
                    var tokenStart = j;

                    for (j++; j < chars.Length; j++)
                    {
                        if (!char.IsLetterOrDigit(chars[j]) ||
                            char.IsWhiteSpace(chars[j]))
                        {
                            break;
                        }
                    }

                    var token = str.Substr(tokenStart, j);
                    var commmand = token.ToUpperInvariant();

                    if (iptCommands.ContainsKey(commmand))
                    {
                        while (iptCommands.ContainsKey(commmand) &&
                            iptCommands[commmand].GetType() == TYPE_STRING)
                        {
                            commmand = iptCommands[commmand].ToString().ToUpperInvariant();
                        }

                        if (iptCommands.ContainsKey(commmand))
                        {
                            result.Push(new IptVariable
                            {
                                Type = IptVariableTypes.Command,
                                Value = commmand,
                            });

                            j++;
                            continue;
                        }
                    }
                    else
                    {
                        result.Push(new IptVariable
                        {
                            Type = IptVariableTypes.Variable,
                            Value = token,
                        });

                        //j++;
                        continue;
                    }
                }
            }

            return result;
        }
    }
}
