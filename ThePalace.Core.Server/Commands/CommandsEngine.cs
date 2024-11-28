using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Business;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Commands
{
    public static class CommandsEngine
    {
        public static bool Eval(ISessionState sessionState, ISessionState targetState, string input)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            input = (input ?? string.Empty).Trim();

            if (input.Length > 0 && (_sessionState.UserID == 0 || input[0] == '`' || input[0] == '\''))
            {
                if (input[0] == '`' || input[0] == '\'')
                {
                    input = input.Remove(0, 1).Trim();
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    return true;
                }

                input = Regex.Replace(input, @"[^\w\d\s-]+", string.Empty);

                var args = Regex.Split(input, @"\s+").ToList();
                var cmd = args.FirstOrDefault().ToUpper();

                args = args.Skip(1).ToList();

                var type = $"ThePalace.Server.Plugins.Commands.CMD_{cmd}".GetType();

                if (type == null)
                {
                    type = Type.GetType($"ThePalace.Server.Commands.CMD_{cmd}");
                }

                if (type != null)
                {
                    var value = type.AttributeWrapper(typeof(AdminOnlyCommandAttribute), "OnBeforeCommandExecute", new object[] {
                        new Dictionary<string, object> {
                            { "UserID", _sessionState.UserID },
                        } });

                    if (!value)
                    {
                        return true;
                    }

                    var command = (ICommand)Activator.CreateInstance(type);

                    try
                    {
                        if (command != null && command.Command(sessionState, targetState, args.ToArray()))
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                if (_sessionState.UserID == 0)
                {
                    Logger.ConsoleLog("Invalid command... Command may not be adapted for console use or try help for a list of commands!");
                }
                else
                {
                    new MSG_XTALK().SendToUserID(sessionState, new Protocols.MSG_XTALK
                    {
                        text = "Invalid command... try again or try help for a list of commands!",
                    });
                }

                return true;
            }

            return false;
        }
    }
}
