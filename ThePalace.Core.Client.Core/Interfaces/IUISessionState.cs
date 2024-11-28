using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Forms;
using ThePalace.Core.Client.Core;
using ThePalace.Core.Client.Core.Enums;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Interfaces
{
    public interface IUISessionState : IClientSessionState, IDisposable
    {
        IReadOnlyDictionary<string, IDisposable> UIControls { get; }
        IReadOnlyDictionary<ScreenLayers, ScreenLayer> UILayers { get; }

        bool Visible { get; set; }
        DateTime? LastActivity { get; set; }
        HistoryManager History { get; }
        TabPage TabPage { get; set; }
        int ScreenWidth { get; set; }
        int ScreenHeight { get; set; }

        AssetSpec SelectedProp { get; set; }
        UserRec SelectedUser { get; set; }
        HotspotRec SelectedHotSpot { get; set; }

        ConcurrentDictionary<string, object> Extended { get; }

        void RefreshRibbon();
        void RefreshUI();
        void RefreshScreen(params ScreenLayers[] layers);
        void LayerVisibility(bool visible, params ScreenLayers[] layers);
        void LayerOpacity(float opacity, params ScreenLayers[] layers);

        FormBase GetForm(string friendlyName);
        T GetForm<T>(string friendlyName)
            where T : FormBase;
        void RegisterForm(string friendlyName, FormBase form);
        void UnregisterForm(string friendlyName, FormBase form);
        Control GetControl(string friendlyName);
        void RegisterControl(string friendlyName, Control control);
        void RegisterControl(string friendlyName, IDisposable control);
        void UnregisterForm(string friendlyName, Control control);
        void UnregisterForm(string friendlyName, IDisposable control);
    }
}
