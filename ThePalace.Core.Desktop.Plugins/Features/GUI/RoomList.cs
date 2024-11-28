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
    public sealed class RoomList : Disposable, IProvider
    {
        private IUISessionState _sessionState = null;

        public string Name => nameof(RoomList);
        public string Description => string.Empty;

        public DeviceTypes[] Devices => new DeviceTypes[] { DeviceTypes.NONE };
        public FeatureTypes[] Features => new FeatureTypes[] { FeatureTypes.GUI };
        public SubFeatureTypes[] SubFeatures => new SubFeatureTypes[] { SubFeatureTypes.NONE };
        public PurposeTypes Purpose => PurposeTypes.PROVIDER;

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            ScriptEvents.Current.UnregisterEvent(IptEventTypes.SignOn, this.ListOfAllRooms);
            ScriptEvents.Current.UnregisterEvent(IptEventTypes.MsgListOfAllRooms, this.ListOfAllRooms);
        }

        public void Initialize(params object[] args) { }

        public object Provide(params object[] args)
        {
            if (this.IsDisposed) return null;

            this._sessionState = args.FirstOrDefault() as IUISessionState;
            if (this._sessionState == null) return null;

            ScriptEvents.Current.RegisterEvent(IptEventTypes.SignOn, this.ListOfAllRooms);
            ScriptEvents.Current.RegisterEvent(IptEventTypes.MsgListOfAllRooms, this.ListOfAllRooms);

            ApiManager.Current.RegisterApi(nameof(this.ShowRoomListForm), this.ShowRoomListForm);
            //HotKeyManager.Current.RegisterHotKey(Keys.Control | Keys.G, binding);

            //ThreadManager.Current.Enqueue(ThreadQueues.GUI, args =>
            //{
            //    ShowRoomListForm();

            //    return null;
            //});

            return null;
        }

        private void ShowRoomListForm(object sender = null, EventArgs e = null)
        {
            if (this.IsDisposed) return;

            var roomList = this._sessionState.GetForm<Forms.RoomList>(nameof(Forms.RoomList));
            if (roomList == null)
            {
                roomList = FormsManager.Current.CreateForm<Forms.RoomList>(
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
                if (roomList == null) return;

                var sessionState = sender as IUISessionState;
                if (sessionState == null) return;

                sessionState.RegisterForm(nameof(Forms.RoomList), roomList);

                roomList.SessionState = sessionState;
                roomList.FormClosed += new FormClosedEventHandler((sender, e) =>
                {
                    this._sessionState.UnregisterForm(nameof(Forms.RoomList), sender as FormBase);

                    SettingsManager.SystemSettings.Ribbon[nameof(RoomsList)].Checked = false;
                    this._sessionState.RefreshRibbon();
                });
                var resize = new EventHandler((sender, e) =>
                {
                    var roomList = this._sessionState.GetForm<Forms.RoomList>(nameof(Forms.RoomList));
                    if (roomList == null) return;

                    var gridRoomList = roomList.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "gridRoomList")
                        .FirstOrDefault() as DataGridView;
                    if (gridRoomList != null)
                    {
                        gridRoomList.Size = new Size(roomList.Width - 16, roomList.Height - 80);
                    }

                    var buttonRefresh = roomList.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "buttonRefresh")
                        .FirstOrDefault() as Button;
                    if (buttonRefresh != null)
                    {
                        buttonRefresh.Location = new Point(buttonRefresh.Location.X, roomList.Height - 70);
                    }

                    var buttonClose = roomList.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "buttonClose")
                        .FirstOrDefault() as Button;
                    if (buttonClose != null)
                    {
                        buttonClose.Location = new Point(buttonClose.Location.X, roomList.Height - 70);
                    }

                    var buttonDelete = roomList.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "buttonDelete")
                        .FirstOrDefault() as Button;
                    if (buttonDelete != null)
                    {
                        buttonDelete.Location = new Point(buttonDelete.Location.X, roomList.Height - 70);
                    }
                });
                roomList.Resize += resize;
                resize(null, null);

                var gridRoomList = roomList.Controls
                    .Cast<Control>()
                    .Where(c => c.Name == "gridRoomList")
                    .FirstOrDefault() as DataGridView;
                if (gridRoomList != null)
                {
                    gridRoomList.DoubleClick += new EventHandler((sender, e) =>
                    {
                        var gridRoomList = sender as DataGridView;
                        if (gridRoomList != null &&
                            gridRoomList.SelectedCells.Count > 0)
                        {
                            var tag = gridRoomList.SelectedCells[0].Tag as string;

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

                var buttonRefresh = roomList.Controls
                    .Cast<Control>()
                    .Where(c => c.Name == "buttonRefresh")
                    .FirstOrDefault() as Button;
                if (buttonRefresh != null)
                {
                    buttonRefresh.Click += new EventHandler((sender, e) =>
                    {
                        ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.SEND, new Header
                        {
                            eventType = EventTypes.MSG_LISTOFALLROOMS,
                            protocolSend = new MSG_LISTOFALLROOMS(),
                        });
                    });
                }

                var buttonClose = roomList.Controls
                    .Cast<Control>()
                    .Where(c => c.Name == "buttonClose")
                    .FirstOrDefault() as Button;
                if (buttonClose != null)
                {
                    buttonClose.Click += new EventHandler((sender, e) =>
                    {
                        this._sessionState.GetForm(nameof(Forms.RoomList))?.Close();
                    });
                }
            }

            if (roomList != null)
            {
                if (roomList.Visible)
                {
                    roomList.Hide();

                    SettingsManager.SystemSettings.Ribbon[nameof(RoomsList)].Checked = false;
                }
                else
                {
                    roomList.TopMost = true;

                    roomList.Show();
                    roomList.Focus();

                    SettingsManager.SystemSettings.Ribbon[nameof(RoomsList)].Checked = true;

                    if ((this._sessionState.ConnectionState?.IsConnected ?? false) == true)
                    {
                        var buttonRefresh = roomList.Controls
                            .Cast<Control>()
                            .Where(c => c.Name == "buttonRefresh")
                            .FirstOrDefault() as Button;
                        buttonRefresh?.PerformClick();
                    }
                }

                this._sessionState.RefreshRibbon();
            }
        }

        private void ListOfAllRooms(object sender, EventArgs e)
        {
            var sessionState = sender as IUISessionState;
            if (sessionState == null) return;

            var roomList = sessionState.GetForm(nameof(Forms.RoomList));
            if (roomList == null) return;

            var scriptEvent = e as ScriptEvent;
            if (scriptEvent == null) return;

            if (scriptEvent.EventType == IptEventTypes.MsgListOfAllRooms)
                ThreadManager.Current.Enqueue(ThreadQueues.GUI, args =>
                {
                    var sessionState = args.FirstOrDefault() as IUISessionState;
                    if (sessionState == null) return null;

                    var roomList = sessionState.GetForm(nameof(Forms.RoomList));
                    if (roomList == null) return null;

                    var gridRoomList = roomList.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "gridRoomList")
                        .FirstOrDefault() as DataGridView;
                    if (gridRoomList != null)
                    {
                        gridRoomList.Rows.Clear();

                        if ((sessionState.ServerRooms?.Count ?? 0) > 0)
                            foreach (var room in sessionState.ServerRooms)
                            {
                                var row = gridRoomList.RowTemplate.Clone() as DataGridViewRow;
                                row.CreateCells(gridRoomList, room.name, room.refNum);
                                row.Cells[0].Tag = room.primaryID.ToString();
                                row.Cells[1].Tag = room.primaryID.ToString();
                                gridRoomList.Rows.Add(row);
                            }
                    }

                    return null;
                }, sessionState);
        }
    }
}
