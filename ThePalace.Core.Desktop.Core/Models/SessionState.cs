using System.Collections.Concurrent;
using System.Timers;
using System.Windows.Forms;
using ThePalace.Core.Client.Core;
using ThePalace.Core.Client.Core.Constants;
using ThePalace.Core.Client.Core.Enums;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Desktop.Core.Models
{
    public sealed partial class SessionState : Disposable, IUISessionState
    {
        // Session Info
        public Guid SessionID { get; } = Guid.NewGuid();
        public object ScriptState { get; set; }

        // Network Info
        public IConnectionState ConnectionState { get; set; } = null;

        // User Info
        public uint UserID
        {
            get => this.UserInfo.userID;
            set => this.UserInfo.userID = value;
        }
        public string Name
        {
            get => this.UserInfo.name;
            set => this.UserInfo.name = value;
        }
        public UserFlags UserFlags
        {
            get => this.UserInfo.userFlags;
            set => this.UserInfo.userFlags = value;
        }
        public short RoomID
        {
            get => this.RoomInfo.roomID;
            set => this.RoomInfo.roomID = value;
        }

        public UserRec UserInfo { get; private set; } = new();
        public RegistrationRec RegInfo { get; private set; } = new();
        public ConcurrentDictionary<string, object> Extended => this.UserInfo.Extended;

        // Room Info
        public string RoomName
        {
            get => RoomInfo.roomName;
            set => RoomInfo.roomName = value;
        }
        public RoomRec RoomInfo { get; set; } = new();
        public DisposableDictionary<uint, UserRec> RoomUsersInfo { get; private set; } = new();

        // Server Info
        public string ServerName { get; set; } = null;
        public int ServerPermissions { get; set; } = 0;
        public string MediaUrl { get; set; } = null;
        public int ServerPopulation { get; set; } = 0;
        public List<ListRec> ServerRooms { get; set; } = new();
        public List<ListRec> ServerUsers { get; set; } = new();

        public SessionState()
        {
            this._managedResources.AddRange(
                new IDisposable[]
                {
                    this.RoomUsersInfo,
                    this._uiControls,
                    this._uiLayers,
                    this._layerMessagesTimer,
                });

            FormsManager.Current.FormClosed += _FormClosed;
            NetworkManager.Current.ConnectionEstablished += _ConnectionEstablished;
            NetworkManager.Current.ConnectionDisconnected += _ConnectionDisconnected;

            this._layerMessagesTimer.Elapsed += new ElapsedEventHandler((sender, e) =>
            {
                ThreadManager.Current.Enqueue(ThreadQueues.GUI, args =>
                {
                    var sessionState = args.FirstOrDefault() as IUISessionState;
                    if (sessionState == null) return null;

                    sessionState.RefreshScreen(ScreenLayers.Messages);

                    return null;
                }, this);
            });
            this._layerMessagesTimer.AutoReset = true;

            foreach (var layer in _layerTypes)
                this._uiLayers.TryAdd(layer, new ScreenLayer(layer)
                {
                    ResourceType = typeof(FormsManager),
                });

            var iptTracking = new IptTracking();
            this.ScriptState = iptTracking;
            var iptVar = new IptMetaVariable
            {
                IsSpecial = true,
                IsGlobal = true,
                Value = new IptVariable
                {
                    Type = IptVariableTypes.Shadow,
                    Value = this,
                },
            };
            iptVar.IsReadOnly = true;
            iptTracking.Variables.TryAdd("SESSIONSTATE", iptVar);

            this.UserInfo.Extended.TryAdd(@"MessageQueue", new DisposableQueue<MsgBubble>());
            this.UserInfo.Extended.TryAdd(@"CurrentMessage", null);

            var seed = (uint)Cipher.WizKeytoSeed(ClientConstants.RegCodeSeed);
            this.RegInfo.crc = Cipher.ComputeLicenseCrc(seed);
            this.RegInfo.counter = (uint)Cipher.GetSeedFromReg(seed, this.RegInfo.crc);
            this.RegInfo.puidCRC = this.RegInfo.crc;
            this.RegInfo.puidCtr = this.RegInfo.counter;

            this.RegInfo.reserved = ClientConstants.ClientAgent;
            this.RegInfo.ulUploadCaps = 0x41;
            this.RegInfo.ulDownloadCaps = 0x0151;
            this.RegInfo.ul2DEngineCaps = 0x01;
            this.RegInfo.ul2DGraphicsCaps = 0x01;
        }
        ~SessionState() =>
            this.Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            FormsManager.Current.FormClosed -= _FormClosed;
            NetworkManager.Current.ConnectionEstablished -= _ConnectionEstablished;
            NetworkManager.Current.ConnectionDisconnected -= _ConnectionDisconnected;

            this._layerMessagesTimer = null;
            this._uiLayers = null;
            this._uiControls = null;
            this.RoomUsersInfo = null;
            this.ServerRooms.Clear();
            this.ServerRooms = null;
            this.ServerUsers.Clear();
            this.ServerUsers = null;
        }

        private void _FormClosed(object sender, EventArgs e)
        {
            if (this.IsDisposed) return;

            var form = sender as Form;
            if (form == null) return;

            var key = this._uiControls
                .Where(c => c.Value == form)
                .Select(c => c.Key)
                .FirstOrDefault();
            if (key != null)
                this._uiControls.TryRemove(key, out var _);
        }
        private void _ConnectionEstablished(object sender, EventArgs e)
        {
            if (this.IsDisposed) return;

            this._layerMessagesTimer?.Start();

            if (this == sender)
            {
                var sessionState = sender as IUISessionState;
                if (sessionState == null) return;

                ThreadManager.Current.Enqueue(ThreadQueues.GUI, args =>
                {
                    var sessionState = args.FirstOrDefault() as IUISessionState;
                    if (sessionState == null) return null;

                    sessionState.RefreshRibbon();

                    return null;
                }, sessionState);
            }
        }
        private void _ConnectionDisconnected(object sender, EventArgs e)
        {
            if (this.IsDisposed) return;

            this._layerMessagesTimer?.Stop();

            if (this == sender)
            {
                var sessionState = sender as IUISessionState;
                if (sessionState == null) return;

                ThreadManager.Current.Enqueue(ThreadQueues.GUI, args =>
                {
                    var sessionState = args.FirstOrDefault() as IUISessionState;
                    if (sessionState == null) return null;

                    sessionState.RefreshScreen();
                    sessionState.RefreshUI();
                    sessionState.RefreshRibbon();

                    return null;
                }, sessionState);
            }
        }
    }
}
