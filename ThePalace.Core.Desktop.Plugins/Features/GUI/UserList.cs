using ThePalace.Core.Client.Core;
using ThePalace.Core.Client.Core.Enums;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Client.Core.Models.Ribbon;
using ThePalace.Core.Client.Core.Protocols.Network;
using ThePalace.Core.Client.Core.Protocols.Server;
using ThePalace.Core.Desktop.Core;
using ThePalace.Core.Desktop.Core.Models;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Desktop.Plugins.Features.GUI
{
    public sealed class UserList : Disposable, IProvider
    {
        private IUISessionState _sessionState = null;

        public string Name => nameof(UserList);
        public string Description => string.Empty;

        public DeviceTypes[] Devices => new DeviceTypes[] { DeviceTypes.NONE };
        public FeatureTypes[] Features => new FeatureTypes[] { FeatureTypes.GUI };
        public SubFeatureTypes[] SubFeatures => new SubFeatureTypes[] { SubFeatureTypes.NONE };
        public PurposeTypes Purpose => PurposeTypes.PROVIDER;

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            ScriptEvents.Current.UnregisterEvent(IptEventTypes.SignOn, this.ListOfAllUsers);
            ScriptEvents.Current.UnregisterEvent(IptEventTypes.MsgListOfAllUsers, this.ListOfAllUsers);
        }

        public void Initialize(params object[] args) { }

        public object Provide(params object[] args)
        {
            if (this.IsDisposed) return null;

            this._sessionState = args.FirstOrDefault() as IUISessionState;
            if (this._sessionState == null) return null;

            ScriptEvents.Current.RegisterEvent(IptEventTypes.SignOn, this.ListOfAllUsers);
            ScriptEvents.Current.RegisterEvent(IptEventTypes.MsgListOfAllUsers, this.ListOfAllUsers);

            ApiManager.Current.RegisterApi(nameof(this.ShowUserListForm), this.ShowUserListForm);
            //HotKeyManager.Current.RegisterHotKey(Keys.Control | Keys.F, binding);

            //ThreadManager.Current.Enqueue(ThreadQueues.GUI, args =>
            //{
            //    ShowUserListForm();

            //    return null;
            //});

            return null;
        }

        private void ShowUserListForm(object sender = null, EventArgs e = null)
        {
            if (this.IsDisposed) return;

            var userList = this._sessionState.GetForm<Forms.UserList>(nameof(Forms.UserList));
            if (userList == null)
            {
                userList = FormsManager.Current.CreateForm<Forms.UserList>(
                    new FormCfg
                    {
                        AutoScaleDimensions = new SizeF(7F, 15F),
                        AutoScaleMode = AutoScaleMode.Font,
                        WindowState = FormWindowState.Normal,
                        StartPosition = FormStartPosition.CenterScreen,
                        Margin = new Padding(0, 0, 0, 0),
                        Size = new Size(260, 266),
                        Visible = false,
                    });
                if (userList == null) return;

                var sessionState = sender as IUISessionState;
                if (sessionState == null) return;

                sessionState.RegisterForm(nameof(Forms.UserList), userList);

                userList.SessionState = sessionState;
                userList.FormClosed += new FormClosedEventHandler((sender, e) =>
                {
                    this._sessionState.UnregisterForm(nameof(Forms.UserList), sender as FormBase);

                    SettingsManager.SystemSettings.Ribbon[nameof(UsersList)].Checked = false;
                    this._sessionState.RefreshRibbon();
                });
                var resize = new EventHandler((sender, e) =>
                {
                    var userList = this._sessionState.GetForm<Forms.UserList>(nameof(Forms.UserList));
                    if (userList == null) return;

                    var gridUserList = userList.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "gridUserList")
                        .FirstOrDefault() as DataGridView;
                    if (gridUserList != null)
                    {
                        gridUserList.Size = new Size(userList.Width - 16, userList.Height - 80);
                    }

                    var buttonRefresh = userList.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "buttonRefresh")
                        .FirstOrDefault() as Button;
                    if (buttonRefresh != null)
                    {
                        buttonRefresh.Location = new Point(buttonRefresh.Location.X, userList.Height - 70);
                    }

                    var buttonClose = userList.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "buttonClose")
                        .FirstOrDefault() as Button;
                    if (buttonClose != null)
                    {
                        buttonClose.Location = new Point(buttonClose.Location.X, userList.Height - 70);
                    }

                    var buttonKill = userList.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "buttonKill")
                        .FirstOrDefault() as Button;
                    if (buttonKill != null)
                    {
                        buttonKill.Location = new Point(buttonKill.Location.X, userList.Height - 70);
                    }
                });
                userList.Resize += resize;
                resize(null, null);

                var gridUserList = userList.Controls
                    .Cast<Control>()
                    .Where(c => c.Name == "gridUserList")
                    .FirstOrDefault() as DataGridView;
                if (gridUserList != null)
                {
                    gridUserList.DoubleClick += new EventHandler((sender, e) =>
                    {
                        var gridUserList = sender as DataGridView;
                        if (gridUserList != null &&
                            gridUserList.SelectedCells.Count > 0)
                        {
                            var tag = gridUserList.SelectedCells[0].Tag as string;

                            if (!string.IsNullOrWhiteSpace(tag))
                                ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.SEND, new Header
                                {
                                    eventType = EventTypes.MSG_ROOMGOTO,
                                    protocolSend = new MSG_ROOMGOTO
                                    {
                                        dest = short.Parse(tag),
                                    },
                                });
                        }
                    });
                }

                var buttonRefresh = userList.Controls
                    .Cast<Control>()
                    .Where(c => c.Name == "buttonRefresh")
                    .FirstOrDefault() as Button;
                if (buttonRefresh != null)
                {
                    buttonRefresh.Click += new EventHandler((sender, e) =>
                    {
                        if ((this._sessionState.ServerRooms?.Count ?? 0) == 0)
                            ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.SEND, new Header
                            {
                                eventType = EventTypes.MSG_LISTOFALLROOMS,
                                protocolSend = new MSG_LISTOFALLROOMS(),
                            });

                        ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.SEND, new Header
                        {
                            eventType = EventTypes.MSG_LISTOFALLUSERS,
                            protocolSend = new MSG_LISTOFALLUSERS(),
                        });
                    });
                }

                var buttonClose = userList.Controls
                    .Cast<Control>()
                    .Where(c => c.Name == "buttonClose")
                    .FirstOrDefault() as Button;
                if (buttonClose != null)
                {
                    buttonClose.Click += new EventHandler((sender, e) =>
                    {
                        this._sessionState.GetForm(nameof(Forms.UserList))?.Close();
                    });
                }
            }

            if (userList != null)
            {
                if (userList.Visible)
                {
                    userList.Hide();

                    SettingsManager.SystemSettings.Ribbon[nameof(UsersList)].Checked = false;
                }
                else
                {
                    userList.TopMost = true;

                    userList.Show();
                    userList.Focus();

                    SettingsManager.SystemSettings.Ribbon[nameof(UsersList)].Checked = true;

                    if ((this._sessionState.ConnectionState?.IsConnected ?? false) == true)
                    {
                        var buttonRefresh = userList.Controls
                            .Cast<Control>()
                            .Where(c => c.Name == "buttonRefresh")
                            .FirstOrDefault() as Button;
                        buttonRefresh?.PerformClick();
                    }
                }

                this._sessionState.RefreshRibbon();
            }
        }

        private void ListOfAllUsers(object sender, EventArgs e)
        {
            var sessionState = sender as IUISessionState;
            if (sessionState == null) return;

            var userList = sessionState.GetForm(nameof(Forms.UserList));
            if (userList == null) return;

            var scriptEvent = e as ScriptEvent;
            if (scriptEvent == null) return;

            if (scriptEvent.EventType == IptEventTypes.MsgListOfAllUsers)
                ThreadManager.Current.Enqueue(ThreadQueues.GUI, args =>
                {
                    var sessionState = args.FirstOrDefault() as IUISessionState;
                    if (sessionState == null) return null;

                    var userList = sessionState.GetForm(nameof(Forms.UserList));
                    if (userList == null) return null;

                    var gridUserList = userList.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "gridUserList")
                        .FirstOrDefault() as DataGridView;
                    if (gridUserList != null)
                    {
                        gridUserList.Rows.Clear();

                        if ((sessionState.ServerUsers?.Count ?? 0) > 0)
                        {
                            var roomList = sessionState.ServerRooms?.ToList() ?? new();

                            foreach (var user in sessionState.ServerUsers)
                            {
                                var row = gridUserList.RowTemplate.Clone() as DataGridViewRow;
                                var roomName = roomList
                                    .Where(r => r.primaryID == user.refNum)
                                    .Select(r => r.name)
                                    .FirstOrDefault() ?? string.Empty;
                                row.CreateCells(gridUserList, user.name, roomName);
                                row.Cells[0].Tag = user.refNum.ToString();
                                row.Cells[1].Tag = user.refNum.ToString();
                                gridUserList.Rows.Add(row);
                            }
                        }
                    }

                    return null;
                }, sessionState);
        }
    }
}
