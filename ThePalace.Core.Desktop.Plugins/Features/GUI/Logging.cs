using System.Collections.Concurrent;
using System.Text;
using ThePalace.Core.Client.Core;
using ThePalace.Core.Client.Core.Enums;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Client.Core.Models.Ribbon;
using ThePalace.Core.Desktop.Core;
using ThePalace.Core.Desktop.Core.Models;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Desktop.Plugins.Features.GUI
{
    public sealed class Logging : Disposable, IProvider
    {
        private IUISessionState _sessionState = null;

        public string Name => nameof(Logging);
        public string Description => string.Empty;

        public DeviceTypes[] Devices => new DeviceTypes[] { DeviceTypes.NONE };
        public FeatureTypes[] Features => new FeatureTypes[] { FeatureTypes.GUI };
        public SubFeatureTypes[] SubFeatures => new SubFeatureTypes[] { SubFeatureTypes.LOGGING };
        public PurposeTypes Purpose => PurposeTypes.PROVIDER;

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            ScriptEvents.Current.UnregisterEvent(IptEventTypes.InChat, this.LogCommunications);
            ScriptEvents.Current.UnregisterEvent(IptEventTypes.ServerMsg, this.LogCommunications);
        }

        public void Initialize(params object[] args) { }

        public object Provide(params object[] args)
        {
            if (this.IsDisposed) return null;

            this._sessionState = args.FirstOrDefault() as IUISessionState;
            if (this._sessionState == null) return null;

            ScriptEvents.Current.RegisterEvent(IptEventTypes.InChat, this.LogCommunications);
            ScriptEvents.Current.RegisterEvent(IptEventTypes.ServerMsg, this.LogCommunications);

            ApiManager.Current.RegisterApi(nameof(this.ShowLogForm), this.ShowLogForm);
            //HotKeyManager.Current.RegisterHotKey(Keys.Control | Keys.L, binding);

            ApiManager.Current.RegisterApi(nameof(this.ClearLogForm), this.ClearLogForm);
            //HotKeyManager.Current.RegisterHotKey(Keys.Control | Keys.R, binding);

            ThreadManager.Current.Enqueue(ThreadQueues.GUI, args =>
            {
                var sessionState = args.FirstOrDefault() as IUISessionState;
                if (sessionState == null) return null;

                ShowLogForm(sessionState);

                return null;
            }, this._sessionState);

            return null;
        }

        private void ShowLogForm(object sender = null, EventArgs e = null)
        {
            if (this.IsDisposed) return;

            var logging = this._sessionState.GetForm<Forms.Logging>(nameof(Forms.Logging));
            if (logging == null)
            {
                logging = FormsManager.Current.CreateForm<Forms.Logging>(
                    new FormCfg
                    {
                        AutoScaleDimensions = new SizeF(7F, 15F),
                        AutoScaleMode = AutoScaleMode.Font,
                        WindowState = FormWindowState.Normal,
                        StartPosition = FormStartPosition.CenterScreen,
                        Margin = new Padding(0, 0, 0, 0),
                        Size = new Size(260, 266),
                        Visible = false,
                    }, false);
                if (logging == null) return;

                var sessionState = sender as IUISessionState;
                if (sessionState == null) return;

                sessionState.RegisterForm(nameof(Forms.Logging), logging);

                logging.SessionState = sessionState;
                logging.FormClosing += new FormClosingEventHandler((sender, e) =>
                {
                    var logging = sessionState.GetForm<Forms.Logging>(nameof(Forms.Logging));
                    if (logging != null)
                    {
                        e.Cancel = true;

                        logging.Hide();

                        SettingsManager.SystemSettings.Ribbon[nameof(Chatlog)].Checked = false;
                        this._sessionState.RefreshRibbon();
                    }
                });
                var resize = new EventHandler((sender, e) =>
                {
                    var logging = sessionState.GetForm<Forms.Logging>(nameof(Forms.Logging));
                    if (logging == null) return;

                    var logWindow = logging.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "logWindow")
                        .FirstOrDefault() as RichTextBox;
                    if (logWindow != null)
                    {
                        logWindow.Width = logging.Width - 12;
                        logWindow.Height = logging.Height - 36;
                    }
                });
                logging.Resize += resize;
                resize(null, null);

                var logWindow = logging.Controls
                    .Cast<Control>()
                    .Where(c => c.Name == "logWindow")
                    .FirstOrDefault() as RichTextBox;
                if (logWindow != null)
                {
                    logWindow.Text = string.Empty;
                }

                logging.Hide();

                return;
            }

            if (logging != null)
            {
                if (logging.Visible)
                {
                    logging.Hide();

                    SettingsManager.SystemSettings.Ribbon[nameof(Chatlog)].Checked = false;
                }
                else
                {
                    logging.TopMost = true;
                    logging.Show();
                    logging.Focus();

                    SettingsManager.SystemSettings.Ribbon[nameof(Chatlog)].Checked = true;
                }

                this._sessionState.RefreshRibbon();
            }
        }

        private void ClearLogForm(object sender = null, EventArgs e = null)
        {
            if (this.IsDisposed) return;

            var logging = this._sessionState.GetForm<Forms.Logging>(nameof(Forms.Logging));
            if (logging != null)
            {
                var logWindow = logging.Controls
                    .Cast<Control>()
                    .Where(c => c.Name == "logWindow")
                    .FirstOrDefault() as RichTextBox;
                if (logWindow != null)
                {
                    logWindow.Text = string.Empty;
                }
            }
        }

        private void LogCommunications(object sender, EventArgs e)
        {
            var sessionState = sender as IUISessionState;
            if (sessionState == null) return;

            var scriptEvent = e as ScriptEvent;
            if (scriptEvent == null) return;

            switch (scriptEvent.EventType)
            {
                case IptEventTypes.InChat:
                case IptEventTypes.ServerMsg:
                    ThreadManager.Current.Enqueue(ThreadQueues.GUI, args =>
                    {
                        var sessionState = args[0] as IUISessionState;
                        if (sessionState == null) return null;

                        var scriptEvent = args[1] as ScriptEvent;
                        if (scriptEvent == null) return null;

                        var packetHeader = scriptEvent.Packet as Header;
                        if (packetHeader == null) return null;

                        var packetComms = packetHeader.protocolReceive as IProtocolCommunications;
                        if (packetComms == null) return null;

                        var logging = sessionState.GetForm(nameof(Forms.Logging));
                        if (logging == null) return null;

                        var logWindow = logging.Controls
                            .Cast<Control>()
                            .Where(c => c.Name == "logWindow")
                            .FirstOrDefault() as RichTextBox;
                        if (logWindow != null)
                        {
                            var whoSpoke = sessionState.RoomUsersInfo.GetValueLocked((UInt32)packetHeader.refNum);
                            if (whoSpoke == null) return null;

                            var sb = new StringBuilder(logWindow.Text);
                            sb.AppendLine($"{whoSpoke.name}: {packetComms.text}");

                            logWindow.Text = sb.ToString();
                        }

                        return null;
                    }, sessionState, scriptEvent);

                    break;
            }
        }
    }
}
