using System.Collections.Concurrent;
using System.Drawing;
using System.Windows.Forms;
using ThePalace.Core.Client.Core;
using ThePalace.Core.Client.Core.Constants;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Client.Core.Models.Ribbon;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Desktop.Core.Models
{
    public sealed partial class SessionState : Disposable, IUISessionState
    {
        private DisposableDictionary<string, IDisposable> _uiControls = new();
        public IReadOnlyDictionary<string, IDisposable> UIControls => this._uiControls;

        // UI Info
        public bool Visible { get; set; } = true;
        public DateTime? LastActivity { get; set; } = null;
        public HistoryManager History { get; private set; } = new();
        public TabPage TabPage { get; set; } = null;
        public int ScreenWidth { get; set; } = UIConstants.AspectRatio.WidescreenDef.Default.Width;
        public int ScreenHeight { get; set; } = UIConstants.AspectRatio.WidescreenDef.Default.Height;

        public AssetSpec SelectedProp { get; set; } = null;
        public UserRec SelectedUser { get; set; } = null;
        public HotspotRec SelectedHotSpot { get; set; } = null;

        public void RefreshUI()
        {
            var isConnected = this.ConnectionState?.IsConnected ?? false;

            var form = GetForm("SessionStateManager");
            if (form == null) return;

            var toolStrip = GetControl("toolStrip") as ToolStrip;
            if (toolStrip == null) return;

            var labelInfo = GetControl("labelInfo") as Label;
            if (labelInfo == null) return;

            if (!isConnected)
            {
                form.Text = labelInfo.Text = @"Disconnected";
            }
            else
            {
                form.Text = $"Connected: {this.ServerName} - {this.RoomName}";
                labelInfo.Text = $"Users ({this.RoomUsersInfo.Count(u => u.Key != 0)}/{this.ServerPopulation})";
            }

            var imgScreen = GetControl("imgScreen") as PictureBox;
            if (imgScreen != null)
            {
                toolStrip.Size = new Size(form.Width, form.Height);
                toolStrip.Location = new System.Drawing.Point(0, 0);

                var width = this.ScreenWidth;
                var height = this.ScreenHeight;

                if (width < 1) width = UIConstants.AspectRatio.WidescreenDef.Default.Width;
                if (height < 1) height = UIConstants.AspectRatio.WidescreenDef.Default.Height;

                imgScreen.Size = new Size(width, height);
                if (toolStrip.Visible &&
                    toolStrip.Dock == DockStyle.Top)
                    imgScreen.Location = new System.Drawing.Point(0, toolStrip.Location.Y + toolStrip.Height);
                else
                    imgScreen.Location = new System.Drawing.Point(0, 0);

                labelInfo.Size = new Size(width, 20);
                labelInfo.Location = new System.Drawing.Point(0, imgScreen.Location.Y + imgScreen.Height);

                var txtInput = GetControl("txtInput") as TextBox;
                if (txtInput != null)
                {
                    txtInput.Size = new Size(width, 50);
                    txtInput.Location = new System.Drawing.Point(0, labelInfo.Location.Y + labelInfo.Height);
                }
            }
        }
        public void RefreshRibbon()
        {
            var isConnected = this.ConnectionState?.IsConnected ?? false;

            var toolStrip = GetControl("toolStrip") as ToolStrip;
            if (toolStrip == null) return;

            toolStrip.Items.Clear();

            foreach (var ribbonItem in SettingsManager.UserSettings.Ribbon)
            {
                var nodeType = ribbonItem.GetType().Name;
                switch (nodeType)
                {
                    case nameof(Separator):
                        toolStrip.Items.Add(new ToolStripSeparator
                        {
                            AutoSize = false,
                            Height = toolStrip.Height,
                        });
                        break;
                    default:
                        var item = new ToolStripButton();

                        //var binding = ApiManager.Current.ApiBindings.GetValue("toolStripDropdownlist_Click");
                        //if (binding != null)
                        //    item.Click += binding.Binding;

                        if (ribbonItem.Checkable)
                        {
                            var ribbonItem2 = SettingsManager.SystemSettings.Ribbon.GetValue(nodeType);
                            if (ribbonItem2 != null)
                                item.Checked = ribbonItem2.Checked;
                        }

                        var condition = true;
                        switch (nodeType)
                        {
                            //case nameof(Bookmarks):
                            //case nameof(Connection):
                            //case nameof(LiveDirectory):
                            //case nameof(Chatlog):
                            //case nameof(Tabs):
                            //    condition = true;
                            //    break;
                            case nameof(DoorOutlines):
                            case nameof(UserNametags):
                            case nameof(Terminal):
                            case nameof(SuperUser):
                            case nameof(Draw):
                            case nameof(UsersList):
                            case nameof(RoomsList):
                            case nameof(Sounds):
                                condition = isConnected;
                                break;
                            case nameof(GoBack):
                                condition =
                                    (History.History.Count > 0) &&
                                    (!History.Position.HasValue || History.History.Keys.Min() != History.Position.Value);
                                break;
                            case nameof(GoForward):
                                condition =
                                    (History.History.Count > 0) &&
                                    History.Position.HasValue &&
                                    History.History.Keys.Max() != History.Position.Value;
                                break;
                        }

                        if (ribbonItem is StandardItem _standardItem)
                            item.Image = _standardItem.Image;
                        else if (ribbonItem is BooleanItem _booleanItem)
                            item.Image = _booleanItem.State ? _booleanItem.OnImage : _booleanItem.OffImage;

                        if (item.Image != null)
                        {
                            item.Name = nodeType;
                            item.Enabled = condition;
                            item.ToolTipText = ribbonItem.Title;

                            toolStrip.Items.Add(item);
                        }

                        break;
                }
            }
        }

        public FormBase GetForm(string friendlyName)
        {
            if (!string.IsNullOrWhiteSpace(friendlyName))
                return this._uiControls.GetValue(friendlyName) as FormBase;

            return null;
        }
        public T GetForm<T>(string friendlyName)
            where T : FormBase
        {
            if (!string.IsNullOrWhiteSpace(friendlyName))
                return this._uiControls.GetValue(friendlyName) as T;

            return default;
        }

        public void RegisterForm(string friendlyName, FormBase form)
        {
            if (!string.IsNullOrWhiteSpace(friendlyName) &&
                form != null)
                this._uiControls?.TryAdd(friendlyName, form);
        }

        public void UnregisterForm(string friendlyName, FormBase form)
        {
            if (!string.IsNullOrWhiteSpace(friendlyName) &&
                form != null)
                this._uiControls?.TryRemove(friendlyName, out var _);
        }

        public Control GetControl(string friendlyName)
        {
            if (!string.IsNullOrWhiteSpace(friendlyName))
                return this._uiControls.GetValue(friendlyName) as Control;

            return null;
        }

        public void RegisterControl(string friendlyName, Control control)
        {
            if (!string.IsNullOrWhiteSpace(friendlyName) &&
                control != null)
                this._uiControls?.TryAdd(friendlyName, control);
        }

        public void RegisterControl(string friendlyName, IDisposable control)
        {
            if (!string.IsNullOrWhiteSpace(friendlyName) &&
                control != null)
                this._uiControls?.TryAdd(friendlyName, control);
        }

        public void UnregisterForm(string friendlyName, Control control)
        {
            if (!string.IsNullOrWhiteSpace(friendlyName) &&
                control != null)
                this._uiControls?.TryRemove(friendlyName, out var _);
        }

        public void UnregisterForm(string friendlyName, IDisposable control)
        {
            if (!string.IsNullOrWhiteSpace(friendlyName) &&
                control != null)
                this._uiControls?.TryRemove(friendlyName, out var _);
        }
    }
}
