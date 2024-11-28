using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using ThePalace.Core.Client.Core.Models.Ribbon;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Client.Core
{
    public sealed class SettingsManager : Disposable
    {
        public static class UserSettings
        {
            internal const string Filename = "UserSettings.xml";
            internal const string RootNode = "Config";
            internal const string OptionsNode = "Options";
            internal const string SettingsNode = "Settings";
            internal const string HotKeysNode = "HotKeys";
            internal const string HotKeyNode = "HotKey";
            internal const string RibbonNode = "Ribbon";
            internal const string MacrosNode = "Macros";
            internal const string MacroNode = "Macro";

            public static List<ItemBase> Ribbon { get; internal set; } = new();
        }
        public static class SystemSettings
        {
            internal const string Filename = "SystemSettings.xml";
            internal const string RootNode = "System";
            internal const string RibbonNode = "Ribbon";
            internal const string UserFlagsNode = "UserFlags";
            internal const string RoomFlagsNode = "RoomFlags";

            public static ConcurrentDictionary<string, ItemBase> Ribbon { get; internal set; } = new();
        }
        public static class Localization
        {
            internal static string Filename
            {
                get
                {
                    var filePath = $@"Localization\Localization.{CultureInfo.CurrentCulture.Name}.xml";
                    if (File.Exists(filePath)) return filePath;
                    else return @"Localization\Localization.en-US.xml";
                }
            }
            internal const string RootNode = "Localization";
            internal const string RibbonNode = "Ribbon";
            internal const string UserFlagsNode = "UserFlags";
            internal const string RoomFlagsNode = "RoomFlags";
        }

        private static readonly Keys[] _keyModifiers = new Keys[]
        {
            Keys.Control,
            Keys.Shift,
            Keys.Alt,
            Keys.LWin,
            Keys.RWin,
        };

        //private static readonly Keys[] _keyCharacters = new Keys[]
        //{
        //    Keys.D0,
        //    Keys.D1,
        //    Keys.D2,
        //    Keys.D3,
        //    Keys.D4,
        //    Keys.D5,
        //    Keys.D6,
        //    Keys.D7,
        //    Keys.D8,
        //    Keys.D9,
        //    Keys.NumPad0,
        //    Keys.NumPad1,
        //    Keys.NumPad2,
        //    Keys.NumPad3,
        //    Keys.NumPad4,
        //    Keys.NumPad5,
        //    Keys.NumPad6,
        //    Keys.NumPad7,
        //    Keys.NumPad8,
        //    Keys.NumPad9,
        //    Keys.A,
        //    Keys.B,
        //    Keys.C,
        //    Keys.D,
        //    Keys.E,
        //    Keys.F,
        //    Keys.G,
        //    Keys.H,
        //    Keys.I,
        //    Keys.J,
        //    Keys.K,
        //    Keys.L,
        //    Keys.M,
        //    Keys.N,
        //    Keys.O,
        //    Keys.P,
        //    Keys.Q,
        //    Keys.R,
        //    Keys.S,
        //    Keys.T,
        //    Keys.U,
        //    Keys.V,
        //    Keys.W,
        //    Keys.X,
        //    Keys.Y,
        //    Keys.Z,
        //};

        private static readonly Lazy<SettingsManager> _current = new();
        public static SettingsManager Current => _current.Value;

        public ConcurrentDictionary<string, ISettingBase> Settings { get; private set; } = new();

        public SettingsManager() { }
        ~SettingsManager() =>
            Dispose(false);

        public new void Dispose()
        {
            if (IsDisposed) return;

            base.Dispose();

            Settings?.Clear();
            Settings = null;

            UserSettings.Ribbon?.Clear();
            UserSettings.Ribbon = null;

            foreach (var ribbonNode in SystemSettings.Ribbon.Values)
                try { ribbonNode?.Unload(); } catch { }

            SystemSettings.Ribbon?.Clear();
            SystemSettings.Ribbon = null;
        }

        public SettingsManager Initialize()
        {
            try
            {
                var options = PluginManager.Current.GetTypes()
                    .Where(t =>
                        t?.FullName?.Contains(UserSettings.OptionsNode) == true ||
                        t?.FullName?.Contains(UserSettings.SettingsNode) == true)
                    .ToList();
                foreach (var option in options)
                {
                    try
                    {
                        var instance = option.StaticProperty<ISettingBase>("Current");
                        if (instance == null) continue;

                        var path = Path.Combine(instance.Category, instance.Name);
                        Settings.TryAdd(path, instance);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine(ex.Message);
#endif
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
            }

            return this;
        }

        internal string Read(string path) =>
            File.ReadAllLines(path).Join('\n');

        public void Load(Assembly assembly)
        {
            var load = (Func<string, XmlDocument>)(f =>
            {
                try
                {
                    var settingsPath = Path.Combine(Environment.CurrentDirectory, f);
                    if (File.Exists(settingsPath))
                    {
                        var xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(Read(settingsPath));

                        return xmlDocument;
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }

                return null;
            });

            var systemSettings = load(SystemSettings.Filename);
            var localizations = load(Localization.Filename);

            if (systemSettings != null &&
                localizations != null)
            {
                var item = null as ItemBase;

                var xNode = systemSettings[SystemSettings.RootNode][SystemSettings.RibbonNode] as XmlNode;
                foreach (var ribbonNode in xNode.ChildNodes.Cast<XmlNode>())
                {
                    try
                    {
                        var nodeName = ribbonNode.Name?.Trim();
                        if (string.IsNullOrWhiteSpace(nodeName) ||
                            SystemSettings.Ribbon.ContainsKey(nodeName)) continue;

                        item = ItemBase.Instance(nodeName);
                        if (item == null) continue;

                        item.HoverIcon = ribbonNode.ChildNodes
                            .Cast<XmlNode>()
                            .Where(n => n.Name == "Hover")
                            .SelectMany(n => n.Attributes
                                .Cast<XmlNode>()
                                .Where(a => a.Name == "path")
                                .Select(a => a.Value?.Trim()))
                            .FirstOrDefault();

                        if (item is StandardItem _standardItem)
                        {
                            _standardItem.Icon = ribbonNode.ChildNodes
                                .Cast<XmlNode>()
                                .Where(n => n.Name == "Icon")
                                .SelectMany(n => n.Attributes
                                    .Cast<XmlNode>()
                                    .Where(a => a.Name == "path")
                                    .Select(a => a.Value?.Trim()))
                                .FirstOrDefault();
                        }
                        else if (item is BooleanItem _booleanItem)
                        {
                            _booleanItem.OnIcon = ribbonNode.ChildNodes
                                .Cast<XmlNode>()
                                .Where(n => n.Name == "IconOn")
                                .SelectMany(n => n.Attributes
                                    .Cast<XmlNode>()
                                    .Where(a => a.Name == "path")
                                    .Select(a => a.Value?.Trim()))
                                .FirstOrDefault();
                            _booleanItem.OffIcon = ribbonNode.ChildNodes
                                .Cast<XmlNode>()
                                .Where(n => n.Name == "IconOff")
                                .SelectMany(n => n.Attributes
                                    .Cast<XmlNode>()
                                    .Where(a => a.Name == "path")
                                    .Select(a => a.Value?.Trim()))
                                .FirstOrDefault();
                        }

                        SystemSettings.Ribbon.TryAdd(nodeName, item);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine(ex.Message);
#endif
                    }
                }

                xNode = localizations[Localization.RootNode][Localization.RibbonNode];
                foreach (var ribbonNode in xNode.ChildNodes.Cast<XmlNode>())
                {
                    try
                    {
                        var nodeName = ribbonNode.Name?.Trim();
                        if (string.IsNullOrWhiteSpace(nodeName) ||
                            !SystemSettings.Ribbon.ContainsKey(nodeName)) continue;

                        var buttonTitle = ribbonNode.Attributes
                            .Cast<XmlNode>()
                            .Where(a => a.Name == "title")
                            .Select(a => a.Value?.Trim())
                            .FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(buttonTitle)) continue;

                        SystemSettings.Ribbon[nodeName].Title = buttonTitle;
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine(ex.Message);
#endif
                    }
                }
            }

            foreach (var ribbonItem in SystemSettings.Ribbon.Values)
                ribbonItem.Load(assembly, "ThePalace.Core.Desktop.Core.Resources.icons");

            var userSettings = load(UserSettings.Filename);
            if (userSettings != null)
            {
                foreach (var option in Settings.Values)
                {
                    var path = Path
                        .Combine(
                            option.Category,
                            option.Name)
                        .Split(
                            new char[] { '\\' },
                            StringSplitOptions.RemoveEmptyEntries);

                    var _option = option as IOption;
                    if (_option != null)
                        try
                        {
                            var xNode = userSettings[UserSettings.RootNode][UserSettings.OptionsNode] as XmlNode;
                            foreach (var key in path)
                            {
                                if (xNode == null ||
                                    xNode[key] == null) break;

                                xNode = xNode[key];
                            }

                            if (_option is IOption<bool>)
                                _option.Load(xNode.Attributes
                                    .Cast<XmlNode>()
                                    .Where(a => a.Name == "enabled")
                                    .Select(a => a.Value)
                                    .FirstOrDefault());
                            else
                                _option.Load(xNode.Attributes
                                    .Cast<XmlNode>()
                                    .Where(a => a.Name == "value")
                                    .Select(a => a.Value)
                                    .FirstOrDefault());
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Debug.WriteLine(ex.Message);
#endif
                        }

                    var _setting = option as ISetting;
                    if (_setting != null)
                        try
                        {
                            var xNode = userSettings[UserSettings.RootNode][UserSettings.SettingsNode] as XmlNode;
                            foreach (var key in path)
                            {
                                if (xNode == null ||
                                    xNode[key] == null) break;

                                xNode = xNode[key];
                            }

                            if (_setting is ISetting<string>)
                                _setting.Load(xNode.ChildNodes
                                    .Cast<XmlNode>()
                                    .Select(a => a.Value)
                                    .Distinct()
                                    .ToArray());

                            // TODO: else
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Debug.WriteLine(ex.Message);
#endif
                        }

                    var _settingList = option as ISettingList;
                    if (_settingList != null)
                        try
                        {
                            var xNode = userSettings[UserSettings.RootNode][UserSettings.SettingsNode] as XmlNode;
                            foreach (var key in path)
                            {
                                if (xNode?[key] == null) break;

                                xNode = xNode[key];
                            }

                            if (_settingList is ISettingList<string>)
                                _settingList.Load(xNode.ChildNodes
                                    .Cast<XmlNode>()
                                    .SelectMany(n => n.ChildNodes
                                        .Cast<XmlNode>()
                                        .Select(s => s.Value))
                                    .Distinct()
                                    .ToArray());

                            // TODO: else
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Debug.WriteLine(ex.Message);
#endif
                        }
                }

                try
                {
                    var xNode = userSettings[UserSettings.RootNode][UserSettings.HotKeysNode] as XmlNode;
                    foreach (var hotKey in xNode.ChildNodes.Cast<XmlNode>())
                    {
                        if (hotKey.Name != UserSettings.HotKeyNode) continue;

                        var bindingName = hotKey.Attributes
                            .Cast<XmlNode>()
                            .Where(a => a.Name == "binding")
                            .Select(a => a.Value?.Trim())
                            .FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(bindingName)) continue;
                        else if (!ApiManager.Current.ApiBindings.ContainsKey(bindingName)) continue;

                        var keysArray = hotKey.Attributes
                            .Cast<XmlNode>()
                            .Where(a => a.Name == "keys")
                            .Select(a => a.Value?.Trim()?.Split('|'))
                            .FirstOrDefault();
                        var keys = Keys.None;
                        try
                        {
                            foreach (var key in keysArray)
                                keys |= Enum.Parse<Keys>(key);
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Debug.WriteLine(ex.Message);
#endif
                        }
                        if (keys == Keys.None) continue;
                        else if (
                            !((keys & Keys.Control) == Keys.Control) &&
                            !((keys & Keys.Shift) == Keys.Shift) &&
                            !((keys & Keys.Alt) == Keys.Alt) &&
                            !((keys & Keys.LWin) == Keys.LWin) &&
                            !((keys & Keys.RWin) == Keys.RWin)) continue;

                        var values = hotKey.Attributes
                            .Cast<XmlNode>()
                            .Where(a => a.Name == "values")
                            .Select(a => a.Value?.Trim()?.Split('|'))
                            .FirstOrDefault();

                        var apiBinding = ApiManager.Current.ApiBindings.GetValue(bindingName);
                        HotKeyManager.Current.RegisterHotKey(keys, apiBinding, values);
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }

                try
                {
                    var xNode = userSettings[UserSettings.RootNode][UserSettings.MacrosNode] as XmlNode;
                    foreach (var macroNode in xNode.ChildNodes.Cast<XmlNode>())
                    {
                        if (macroNode.Name != UserSettings.MacroNode) continue;

                        var values = macroNode.Attributes
                            .Cast<XmlNode>()
                            .Where(a => a.Name == "values")
                            .SelectMany(a => a.Value?.Trim()?.Split('|'))
                            .Select(id => Convert.ToInt32(id))
                            .Where(id => id != 0)
                            .Select(id => new AssetSpec
                            {
                                id = id,
                            })
                            .ToArray();
                        if (values.Length < 1) continue;

                        AssetsManager.Current.Macros.Add(values);
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }

                try
                {
                    var item = null as ItemBase;

                    var xNode = userSettings[UserSettings.RootNode][UserSettings.RibbonNode] as XmlNode;
                    foreach (var ribbonNode in xNode.ChildNodes.Cast<XmlNode>())
                    {
                        var nodeName = ribbonNode.Name?.Trim();
                        if (string.IsNullOrWhiteSpace(nodeName)) continue;

                        var buttonType = ribbonNode.Attributes
                            .Cast<XmlNode>()
                            .Where(a => a.Name == "type")
                            .Select(a => a.Value?.Trim())
                            .FirstOrDefault();

                        item = ItemBase.Clone(SystemSettings.Ribbon[nodeName], buttonType);
                        if (item == null) continue;

                        else UserSettings.Ribbon.Add(item);
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine(ex.Message);
#endif
                }
            }
        }

        public void Save()
        {
            try
            {
                var settingsPath = Path.Combine(Environment.CurrentDirectory, UserSettings.Filename);
                var xmlDoc = new XmlDocument();

                var xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.InsertBefore(xmldecl, xmlDoc.DocumentElement);

                var rootNode = xmlDoc.CreateNode(XmlNodeType.Element, UserSettings.RootNode, null);
                xmlDoc.AppendChild(rootNode);

                var optionsNode = xmlDoc.CreateNode(XmlNodeType.Element, UserSettings.OptionsNode, null);
                rootNode.AppendChild(optionsNode);

                var settingsNode = xmlDoc.CreateNode(XmlNodeType.Element, UserSettings.SettingsNode, null);
                rootNode.AppendChild(settingsNode);

                var hotKeysNode = xmlDoc.CreateNode(XmlNodeType.Element, UserSettings.HotKeysNode, null);
                rootNode.AppendChild(hotKeysNode);

                var ribbonNode = xmlDoc.CreateNode(XmlNodeType.Element, UserSettings.RibbonNode, null);
                rootNode.AppendChild(ribbonNode);

                var macrosNode = xmlDoc.CreateNode(XmlNodeType.Element, UserSettings.MacrosNode, null);
                rootNode.AppendChild(macrosNode);

                foreach (var option in Settings.Values)
                {
                    var path = Path
                        .Combine(
                            option.Category,
                            option.Name)
                        .Split(
                            new char[] { '\\' },
                            StringSplitOptions.RemoveEmptyEntries);

                    var _option = option as IOption;
                    if (_option != null)
                        try
                        {
                            var xNode = xmlDoc[UserSettings.RootNode][UserSettings.OptionsNode] as XmlNode;
                            foreach (var key in path)
                            {
                                if (xNode[key] == null)
                                    xNode.AppendChild(
                                        xmlDoc.CreateNode(
                                            XmlNodeType.Element,
                                            key,
                                            null));

                                xNode = xNode[key];
                            }

                            var valueAttrib = null as XmlAttribute;
                            if (_option is IOption<bool>)
                            {
                                valueAttrib = xmlDoc.CreateAttribute("enabled");
                                valueAttrib.Value = _option.Text.ToLowerInvariant();
                            }
                            else
                            {
                                valueAttrib = xmlDoc.CreateAttribute("value");
                                valueAttrib.Value = _option.Text;
                            }
                            xNode.Attributes.Append(valueAttrib);
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Debug.WriteLine(ex.Message);
#endif
                        }

                    var _setting = option as ISetting;
                    if (_setting != null)
                        try
                        {
                            var xNode = xmlDoc[UserSettings.RootNode][UserSettings.SettingsNode] as XmlNode;
                            foreach (var key in path)
                            {
                                if (xNode[key] == null)
                                    xNode.AppendChild(
                                        xmlDoc.CreateNode(
                                            XmlNodeType.Element,
                                            key,
                                            null));

                                xNode = xNode[key];
                            }

                            var newNode = xmlDoc.CreateNode(
                                XmlNodeType.Element,
                                _setting.Name,
                                null);
                            newNode.Value = _setting.Text;
                            xNode.AppendChild(newNode);
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Debug.WriteLine(ex.Message);
#endif
                        }

                    var _settingList = option as ISettingList;
                    if (_settingList != null)
                        try
                        {
                            var xNode = xmlDoc[UserSettings.RootNode][UserSettings.SettingsNode] as XmlNode;
                            foreach (var key in path)
                            {
                                if (xNode[key] == null)
                                    xNode.AppendChild(
                                        xmlDoc.CreateNode(
                                            XmlNodeType.Element,
                                            key,
                                            null));

                                xNode = xNode[key];
                            }

                            foreach (var value in _settingList.Text)
                            {
                                var newNode = xmlDoc.CreateNode(
                                    XmlNodeType.Element,
                                    _settingList.Name,
                                    null);
                                xNode.AppendChild(newNode);

                                var newText = xmlDoc.CreateNode(
                                    XmlNodeType.Text,
                                    _settingList.Name,
                                    null);
                                newText.Value = value;
                                newNode.AppendChild(newText);
                            }
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Debug.WriteLine(ex.Message);
#endif
                        }
                }

                foreach (var hotKey in HotKeyManager.Current.KeyBindings)
                {
                    try
                    {
                        var apiBindingName = ApiManager.Current.ApiBindings
                            .Where(a => a.Value == hotKey.Value.Binding)
                            .Select(a => a.Key)
                            .FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(apiBindingName)) continue;

                        var hotKeys = new List<string>();
                        var _hotKey = hotKey.Key;

                        foreach (var keyModifier in _keyModifiers)
                            if ((hotKey.Key & keyModifier) == keyModifier)
                            {
                                hotKeys.Add(keyModifier.ToString());
                                _hotKey &= ~keyModifier;
                            }

                        //foreach (var keyCharacter in _keyCharacters)
                        //    if ((hotKey.Key & keyCharacter) == keyCharacter)
                        //    {
                        //        hotKeys.Add(keyCharacter.ToString());
                        //        _hotKey &= ~keyCharacter;
                        //    }

                        if (hotKeys.Count < 1 ||
                            _hotKey == Keys.None) continue;

                        hotKeys.Add(_hotKey.ToString());

                        var hotKeyNode = xmlDoc.CreateNode(XmlNodeType.Element, UserSettings.HotKeyNode, null);
                        hotKeysNode.AppendChild(hotKeyNode);

                        var keysAttrib = xmlDoc.CreateAttribute("keys");
                        keysAttrib.Value = hotKeys.ToArray().Join('|');
                        hotKeyNode.Attributes.Append(keysAttrib);

                        var bindingAttrib = xmlDoc.CreateAttribute("binding");
                        bindingAttrib.Value = apiBindingName;
                        hotKeyNode.Attributes.Append(bindingAttrib);

                        if ((hotKey.Value?.Values?.Length ?? 0) > 0)
                        {
                            var valuesAttrib = xmlDoc.CreateAttribute("values");
                            valuesAttrib.Value = hotKey.Value.Values
                                .Select(v => v.ToString())
                                .ToArray()
                                .Join('|');
                            hotKeyNode.Attributes.Append(valuesAttrib);
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine(ex.Message);
#endif
                    }
                }

                foreach (var ribbonItem in UserSettings.Ribbon)
                {
                    try
                    {
                        var nodeType = ribbonItem.GetType().Name;
                        if (!SystemSettings.Ribbon.Keys.Contains(nodeType)) continue;

                        var ribbonItemNode = xmlDoc.CreateNode(XmlNodeType.Element, nodeType, null);
                        ribbonNode.AppendChild(ribbonItemNode);

                        if (ribbonItem.Type != "btn")
                            switch (nodeType)
                            {
                                case nameof(GoBack):
                                case nameof(GoForward):
                                case nameof(UsersList):
                                case nameof(RoomsList):
                                case nameof(Sounds):
                                    var typeAttrib = xmlDoc.CreateAttribute("type");
                                    typeAttrib.Value = ribbonItem.Type;
                                    ribbonItemNode.Attributes.Append(typeAttrib);
                                    break;
                            }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine(ex.Message);
#endif
                    }
                }

                foreach (var macro in AssetsManager.Current.Macros)
                {
                    try
                    {
                        var macroNode = xmlDoc.CreateNode(XmlNodeType.Element, UserSettings.MacroNode, null);
                        macrosNode.AppendChild(macroNode);

                        var valuesAttrib = xmlDoc.CreateAttribute("values");
                        valuesAttrib.Value = macro
                            .Select(m => m.id.ToString())
                            .ToArray()
                            .Join('|');
                        macroNode.Attributes.Append(valuesAttrib);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine(ex.Message);
#endif
                    }
                }

                xmlDoc.Save(settingsPath);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(ex.Message);
#endif
            }
        }

        public T GetOption<T>(string settingName)
        {
            if (Settings.GetValue(settingName) is IOption<T> option)
                return option.Value;
            else return default;
        }
    }
}
