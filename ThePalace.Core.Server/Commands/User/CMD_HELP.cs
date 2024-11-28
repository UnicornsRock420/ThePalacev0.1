using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Server.Attributes;
using ThePalace.Core.Server.Interfaces;
using ThePalace.Core.Server.Models;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Server.Commands
{
    public class CMD_HELP : ICommand
    {
        public const string Help = @"";

        public bool Command(ISessionState sessionState, ISessionState targetState, params string[] args)
        {
            var _sessionState = sessionState as SessionState;
            if (_sessionState != null) return false;

            args = args
                .Select(a => (a ?? string.Empty).ToLower())
                .ToArray();

            var isAdmin = (_sessionState.UserID == 0) || _sessionState.Authorized;
            var xTalk = new Business.MSG_XTALK();
            var protocol = new Protocols.MSG_XTALK();
            var type = typeof(ICommand);
            var types = new List<Type>();

            types.AddRange(Assembly.GetExecutingAssembly().GetTypes());
            types.AddRange(PluginManager.Current.GetTypes());

            var list = types
                .Where(t => t.Namespace == "ThePalace.Server.Commands" || t.Namespace == "ThePalace.Server.Plugins.Commands")
                .Where(t => isAdmin || !t.GetCustomAttributes(typeof(AdminOnlyCommandAttribute)).Any())
                .Where(t => Regex.IsMatch(t.Name, @"^CMD_[\w\d_]+$", RegexOptions.IgnoreCase))
                .Where(t => type.IsAssignableFrom(t))
                .Where(t => t.IsClass)
                .Select(t =>
                {
                    var result = Regex.Replace(t.Name.ToLower(), @"^CMD_", string.Empty, RegexOptions.IgnoreCase);

                    if (args.Length > 0 && !args.Contains(result))
                    {
                        return null;
                    }

                    var helpString = t
                       .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                       .Where(fi => fi.FieldType == typeof(string))
                       .Where(fi => fi.Name == "Help")
                       .Where(fi => !fi.IsInitOnly)
                       .Where(fi => fi.IsLiteral)
                       .Select(fi => (string)fi.GetRawConstantValue())
                       .FirstOrDefault();

                    return $"; `{result}: {helpString}";
                })
                .Where(h => !string.IsNullOrWhiteSpace(h))
                .OrderBy(h => h)
                .ToList();

            if (_sessionState.UserID == 0)
            {
                foreach (var item in list)
                {
                    Logger.ConsoleLog(item);
                }
            }
            else
            {
                foreach (var item in list)
                {
                    protocol.text = item;

                    xTalk.SendToUserID(sessionState, protocol);
                }
            }

            return true;
        }
    }
}
