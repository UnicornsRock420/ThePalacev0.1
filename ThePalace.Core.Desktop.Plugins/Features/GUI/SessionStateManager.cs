using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using ThePalace.Core.Client.Core;
using ThePalace.Core.Client.Core.Constants;
using ThePalace.Core.Client.Core.Enums;
using ThePalace.Core.Client.Core.Factories;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Client.Core.Models.Ribbon;
using ThePalace.Core.Client.Core.Protocols.Assets;
using ThePalace.Core.Client.Core.Protocols.Communications;
using ThePalace.Core.Client.Core.Protocols.Network;
using ThePalace.Core.Client.Core.Protocols.Rooms;
using ThePalace.Core.Client.Core.Protocols.Server;
using ThePalace.Core.Client.Core.Protocols.Users;
using ThePalace.Core.Constants;
using ThePalace.Core.Desktop.Core;
using ThePalace.Core.Desktop.Core.Models;
using ThePalace.Core.Desktop.Plugins.Models;
using ThePalace.Core.Desktop.Plugins.Options.GUI;
using ThePalace.Core.Enums;
using ThePalace.Core.ExtensionMethods;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Desktop.Plugins.Features.GUI
{
    public sealed class SessionStateManager : Disposable, IProvider
    {
        private ContextMenuStrip _contextMenu = new();
        private IUISessionState _sessionState = null;

        private static readonly IptEventTypes[] _eventTypes = new IptEventTypes[]
        {
            IptEventTypes.ColorChange,
            IptEventTypes.FaceChange,
            IptEventTypes.InChat,
            IptEventTypes.Lock,
            IptEventTypes.LoosePropAdded,
            IptEventTypes.LoosePropDeleted,
            IptEventTypes.LoosePropMoved,
            IptEventTypes.MsgAssetSend,
            IptEventTypes.MsgDraw,
            IptEventTypes.MsgHttpServer,
            IptEventTypes.MsgPictDel,
            IptEventTypes.MsgPictMove,
            IptEventTypes.MsgPictNew,
            IptEventTypes.MsgServerInfo,
            IptEventTypes.MsgSpotDel,
            IptEventTypes.MsgSpotMove,
            IptEventTypes.MsgSpotNew,
            IptEventTypes.MsgUserDesc,
            IptEventTypes.MsgUserList,
            IptEventTypes.MsgUserLog,
            IptEventTypes.MsgUserProp,
            IptEventTypes.MsgUserStatus,
            IptEventTypes.NameChange,
            IptEventTypes.RoomLoad,
            IptEventTypes.SignOn,
            IptEventTypes.StateChange,
            IptEventTypes.UnLock,
            IptEventTypes.UserEnter,
            IptEventTypes.UserLeave,
            IptEventTypes.UserMove,
        };

        public string Name => nameof(SessionStateManager);
        public string Description => string.Empty;

        public DeviceTypes[] Devices => new DeviceTypes[] { DeviceTypes.NONE };
        public FeatureTypes[] Features => new FeatureTypes[] { FeatureTypes.GUI };
        public SubFeatureTypes[] SubFeatures => new SubFeatureTypes[] { SubFeatureTypes.MANAGER, SubFeatureTypes.SYSTRAYICON };
        public PurposeTypes Purpose => PurposeTypes.PROVIDER;

        public SessionStateManager()
        {
            this._managedResources.Add(_contextMenu);
        }
        ~SessionStateManager() =>
            this.Dispose(false);

        public override void Dispose()
        {
            if (this.IsDisposed) return;

            base.Dispose();

            foreach (var type in _eventTypes)
                ScriptEvents.Current.UnregisterEvent(type, this.RefreshScreen);

            var trayIcon = this._sessionState.UIControls.GetValue(nameof(NotifyIcon)) as NotifyIcon;
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                try { trayIcon.Dispose(); } catch { }
            }
        }

        public void Initialize(params object[] args) { }

        public object Provide(params object[] args)
        {
            if (this.IsDisposed) return null;

            this._sessionState = args.FirstOrDefault() as IUISessionState;
            if (this._sessionState == null) return null;

            foreach (var type in _eventTypes)
                ScriptEvents.Current.RegisterEvent(type, this.RefreshScreen);

            ApiManager.Current.RegisterApi(nameof(this.ShowConnectionForm), this.ShowConnectionForm);
            ApiManager.Current.RegisterApi(nameof(this.toolStripDropdownlist_Click), this.toolStripDropdownlist_Click);
            ApiManager.Current.RegisterApi(nameof(this.toolStripMenuItem_Click), this.toolStripMenuItem_Click);
            ApiManager.Current.RegisterApi(nameof(this.contextMenuItem_Click), this.contextMenuItem_Click);

#if WINDOWS10_0_17763_0_OR_GREATER
            if (Toast.Current.Value)
                ThreadManager.Current.DispatchToast(new ToastCfg
                {
                    ExpirationTime = DateTime.Now.AddMinutes(1),
                    Args = (IReadOnlyDictionary<string, object>)new Dictionary<string, object>
                    {
                        { "action", "whisperMsg" },
                        { "connectionId", 123 },
                        { "conversationId", 456 },
                    },
                    Text = (IReadOnlyList<string>)new List<string>
                    {
                        "Beat it like it owes you money!",
                    },
                });
#endif

            ThreadManager.Current.Enqueue(ThreadQueues.GUI, args =>
            {
                var sessionState = args.FirstOrDefault() as IUISessionState;
                if (sessionState == null) return null;

                ShowAppForm();

                if (SysTrayIcon.Current.Value)
                {
                    var form = sessionState.GetForm(nameof(SessionStateManager));
                    if (form == null) return null;

                    var trayIcon = sessionState.UIControls.GetValue(nameof(NotifyIcon)) as NotifyIcon;
                    if (trayIcon == null)
                    {
                        trayIcon = new NotifyIcon
                        {
                            ContextMenuStrip = new ContextMenuStrip(),
                            Icon = form.Icon,
                            Visible = true,
                        };
                        sessionState.RegisterControl(nameof(NotifyIcon), trayIcon);

                        trayIcon.ContextMenuStrip.Items.Add("Exit", null, new EventHandler((sender, e) => ThreadManager.Current.Dispose()));
                    }
                }

                return null;
            }, this._sessionState);

            return null;
        }

        internal void RefreshScreen(object sender, EventArgs e)
        {
            var sessionState = sender as IUISessionState;
            if (sessionState == null) return;

            var scriptEvent = e as ScriptEvent;
            if (scriptEvent == null) return;

            var uiRefresh = false;

            switch (scriptEvent.EventType)
            {
                case IptEventTypes.MsgServerInfo:
                case IptEventTypes.MsgUserLog:
                case IptEventTypes.MsgUserList:
                case IptEventTypes.MsgUserStatus:
                case IptEventTypes.RoomLoad:
                case IptEventTypes.SignOn:
                case IptEventTypes.UserEnter:
                case IptEventTypes.UserLeave:
                    uiRefresh = true;
                    break;
            }

            var screenLayers = null as ScreenLayers[];
            switch (scriptEvent.EventType)
            {
                case IptEventTypes.MsgHttpServer:
                case IptEventTypes.RoomLoad:
                    screenLayers = new[] { ScreenLayers.Base };
                    break;
                case IptEventTypes.InChat:
                    screenLayers = new[] { ScreenLayers.Messages };
                    break;
                case IptEventTypes.NameChange:
                    screenLayers = new[] { ScreenLayers.UserNametag };
                    break;
                case IptEventTypes.FaceChange:
                case IptEventTypes.MsgUserProp:
                    screenLayers = new[] { ScreenLayers.UserProp };
                    break;
                case IptEventTypes.LoosePropAdded:
                case IptEventTypes.LoosePropDeleted:
                case IptEventTypes.LoosePropMoved:
                    screenLayers = new[] { ScreenLayers.LooseProp };
                    break;
                case IptEventTypes.Lock:
                case IptEventTypes.MsgPictDel:
                case IptEventTypes.MsgPictMove:
                case IptEventTypes.MsgPictNew:
                case IptEventTypes.StateChange:
                case IptEventTypes.UnLock:
                    screenLayers = new[] { ScreenLayers.SpotImage };
                    break;
                case IptEventTypes.ColorChange:
                case IptEventTypes.MsgUserDesc:
                case IptEventTypes.MsgUserList:
                case IptEventTypes.MsgUserLog:
                case IptEventTypes.UserEnter:
                    screenLayers = new[] {
                        ScreenLayers.UserProp,
                        ScreenLayers.UserNametag, };
                    break;
                case IptEventTypes.MsgAssetSend:
                    screenLayers = new[] {
                        ScreenLayers.UserProp,
                        ScreenLayers.LooseProp, };
                    break;
                case IptEventTypes.SignOn:
                case IptEventTypes.UserLeave:
                case IptEventTypes.UserMove:
                    screenLayers = new[] {
                        ScreenLayers.UserProp,
                        ScreenLayers.UserNametag,
                        ScreenLayers.Messages, };
                    break;
                case IptEventTypes.MsgDraw:
                    screenLayers = new[] {
                        ScreenLayers.BottomPaint,
                        ScreenLayers.TopPaint, };
                    break;
                case IptEventTypes.MsgSpotDel:
                case IptEventTypes.MsgSpotMove:
                case IptEventTypes.MsgSpotNew:
                    screenLayers = new[] {
                        ScreenLayers.SpotBorder,
                        ScreenLayers.SpotNametag,
                        ScreenLayers.SpotImage, };
                    break;
            }

            ThreadManager.Current.Enqueue(ThreadQueues.GUI, args =>
            {
                var sessionState = args[0] as IUISessionState;
                if (sessionState == null) return null;

                var scriptEvent = args[1] as ScriptEvent;
                if (scriptEvent == null) return null;

                var screenLayers = args[2] as ScreenLayers[];
                if (screenLayers == null ||
                    screenLayers.Length < 1) return null;

                var uiRefresh = (bool)args[3];

                if (screenLayers.Contains(ScreenLayers.Base))
                    sessionState.RefreshScreen(ScreenLayers.Base);

                if (uiRefresh)
                {
                    sessionState.RefreshUI();
                    sessionState.RefreshRibbon();
                }

                if (!screenLayers.Contains(ScreenLayers.Base))
                    sessionState.RefreshScreen(screenLayers);
                else
                    sessionState.RefreshScreen(new[] {
                        ScreenLayers.LooseProp,
                        ScreenLayers.SpotImage,
                        ScreenLayers.BottomPaint,
                        ScreenLayers.SpotNametag,
                        ScreenLayers.UserProp,
                        ScreenLayers.UserNametag,
                        ScreenLayers.ScriptedImage,
                        ScreenLayers.ScriptedText,
                        ScreenLayers.SpotBorder,
                        ScreenLayers.TopPaint,
                        ScreenLayers.Messages, });

                switch (scriptEvent.EventType)
                {
                    case IptEventTypes.RoomLoad:
                        sessionState.History.RegisterHistory(
                            $"{sessionState.ServerName} - {sessionState.RoomInfo.roomName}",
                            $"palace://{sessionState.ConnectionState.Host}:{sessionState.ConnectionState.Port}/{sessionState.RoomInfo.roomID}");

                        sessionState.RefreshRibbon();

                        ScriptEvents.Current.Invoke(IptEventTypes.RoomReady, sessionState, null, sessionState.ScriptState);
                        ScriptEvents.Current.Invoke(IptEventTypes.Enter, sessionState, null, sessionState.ScriptState);

                        break;
                }

                return null;
            }, sessionState, scriptEvent, screenLayers, uiRefresh);
        }

        private void ShowAppForm()
        {
            if (this.IsDisposed) return;

            var form = FormsManager.Current.CreateForm<FormDialog>(new FormCfg
            {
                Load = new EventHandler((sender, e) => ThreadManager.Current.Run(ThreadQueues.GUI)),
                WindowState = FormWindowState.Minimized,
                AutoScaleMode = AutoScaleMode.Font,
                AutoScaleDimensions = new SizeF(7F, 15F),
                Margin = new Padding(0, 0, 0, 0),
                Visible = false,
            });
            if (form == null) return;

            this._sessionState.RegisterForm(nameof(SessionStateManager), form);

            form.SessionState = this._sessionState;
            form.FormClosed += new FormClosedEventHandler((sender, e) =>
                this._sessionState.UnregisterForm(nameof(SessionStateManager), sender as FormBase));

            form.MouseMove += new MouseEventHandler((sender, e) =>
            {
                this._sessionState.LastActivity = DateTime.UtcNow;

                ScriptEvents.Current.Invoke(IptEventTypes.MouseMove, this._sessionState, null, this._sessionState.ScriptState);
            });
            form.MouseUp += new MouseEventHandler((sender, e) =>
            {
                this._sessionState.LastActivity = DateTime.UtcNow;

                ScriptEvents.Current.Invoke(IptEventTypes.MouseUp, this._sessionState, null, this._sessionState.ScriptState);
            });
            form.MouseDown += new MouseEventHandler((sender, e) =>
            {
                this._sessionState.LastActivity = DateTime.UtcNow;

                ScriptEvents.Current.Invoke(IptEventTypes.MouseDown, this._sessionState, null, this._sessionState.ScriptState);
            });
            form.DragEnter += new DragEventHandler((sender, e) =>
            {
                this._sessionState.LastActivity = DateTime.UtcNow;

                ScriptEvents.Current.Invoke(IptEventTypes.MouseDrag, this._sessionState, null, this._sessionState.ScriptState);
            });
            form.DragLeave += new EventHandler((sender, e) =>
            {
                this._sessionState.LastActivity = DateTime.UtcNow;

                ScriptEvents.Current.Invoke(IptEventTypes.MouseDrag, this._sessionState, null, this._sessionState.ScriptState);
            });
            form.DragOver += new DragEventHandler((sender, e) =>
            {
                this._sessionState.LastActivity = DateTime.UtcNow;

                ScriptEvents.Current.Invoke(IptEventTypes.MouseDrag, this._sessionState, null, this._sessionState.ScriptState);
            });
            form.Resize += new EventHandler((sender, e) =>
            {
                this._sessionState.LastActivity = DateTime.UtcNow;

                var form = sender as FormBase;
                if (form == null ||
                    form.WindowState != FormWindowState.Normal) return;

                var screenWidth = (Screen.PrimaryScreen?.Bounds.Width ?? 0);
                var screenHeight = (Screen.PrimaryScreen?.Bounds.Height ?? 0);

                if (form.Location.X < 0 ||
                    form.Location.Y < 0 ||
                    form.Location.X > screenWidth ||
                    form.Location.Y > screenHeight)
                {
                    form.ClientSize = new Size(screenWidth - 16, screenHeight - 16);
                    form.Location = new Point(0, 0);
                }

                form.Location = new Point(
                    (screenWidth / 2) - (form.Width / 2),
                    (screenHeight / 2) - (form.Height / 2));

                var toolStrip = this._sessionState.GetControl("toolStrip") as ToolStrip;
                if (toolStrip != null)
                {
                    toolStrip.Size = new Size(form.Width, form.Height);
                }
            });

            FormsManager.UpdateForm(form, new FormCfg
            {
                Size = new Size(
                    UIConstants.AspectRatio.WidescreenDef.Default.Width,
                    UIConstants.AspectRatio.WidescreenDef.Default.Height),
                WindowState = FormWindowState.Normal,
                Visible = true,
                Focus = true,
            });

            var tabIndex = 0;

            var toolStrip = this._sessionState.GetControl("toolStrip") as ToolStrip;
            if (toolStrip == null)
            {
                toolStrip = FormsManager.Current.CreateControl<FormBase, ToolStrip>(form, true, new ControlCfg
                {
                    Visible = true,
                    TabIndex = 0, //tabIndex++,
                    Text = string.Empty,
                    //Size = new Size(800, 25),
                    Margin = new Padding(0, 0, 0, 0),
                })?.FirstOrDefault();

                if (toolStrip != null)
                {
                    this._sessionState.RegisterControl(nameof(toolStrip), toolStrip);

                    toolStrip.Stretch = true;
                    toolStrip.GripMargin = new Padding(0);
                    toolStrip.ImageScalingSize = new Size(38, 38);
                    toolStrip.LayoutStyle = ToolStripLayoutStyle.Flow;
                    toolStrip.RenderMode = ToolStripRenderMode.Professional;
                    toolStrip.Renderer = new CustomToolStripRenderer();
                    toolStrip.ItemClicked += new ToolStripItemClickedEventHandler(this.toolStrip_ItemClicked);
                }
            }

            var imgScreen = this._sessionState.GetControl("imgScreen") as PictureBox;
            if (imgScreen == null)
            {
                imgScreen = FormsManager.Current.CreateControl<FormBase, PictureBox>(form, true, new ControlCfg
                {
                    Visible = true,
                    TabIndex = 0, //tabIndex++,
                    Margin = new Padding(0, 0, 0, 0),
                    BorderStyle = BorderStyle.FixedSingle,
                })?.FirstOrDefault();

                if (imgScreen != null)
                {
                    imgScreen.MouseClick += new MouseEventHandler((sender, e) =>
                    {
                        if ((this._sessionState.ConnectionState?.IsConnected ?? false) == false)
                            ShowConnectionForm();
                        else
                        {
                            var point = new ThePalace.Core.Models.Palace.Point((short)e.X, (short)e.Y);

                            switch (e.Button)
                            {
                                case MouseButtons.Left:
                                    this._sessionState.UserInfo.roomPos = point;

                                    var user = null as UserRec;
                                    user = this._sessionState.RoomUsersInfo.GetValueLocked(this._sessionState.UserID);
                                    if (user != null)
                                    {
                                        user.roomPos = point;
                                        user.Extended["CurrentMessage"] = null;

                                        var queue = user.Extended["MessageQueue"] as DisposableQueue<MsgBubble>;
                                        if (queue != null) queue.Clear();

                                        this._sessionState.RefreshScreen(
                                            ScreenLayers.UserProp,
                                            ScreenLayers.UserNametag,
                                            ScreenLayers.Messages);

                                        ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.SEND, new Header
                                        {
                                            eventType = EventTypes.MSG_USERMOVE,
                                            protocolSend = new MSG_USERMOVE
                                            {
                                                pos = point,
                                            },
                                        });
                                    }

                                    break;
                                case MouseButtons.Right:
                                    _contextMenu.Items.Clear();

                                    var toolStripItem = _contextMenu.Items.Add($"Move here: {point.h}, {point.v}");
                                    if (toolStripItem != null)
                                    {
                                        toolStripItem.Tag = new object[]
                                        {
                                            ContextMenuCommandTypes.MSG_USERMOVE,
                                            point,
                                        };
                                        toolStripItem.Click += contextMenuItem_Click;
                                    }

                                    if ((this._sessionState.RoomUsersInfo?.Count ?? 0) > 0)
                                        foreach (var roomUser in this._sessionState.RoomUsersInfo.Values)
                                            if (roomUser.userID == 0 ||
                                                roomUser.roomPos == null) continue;
                                            else if (roomUser.userID != this._sessionState.UserID &&
                                                point.IsPointInPolygon(
                                                    roomUser.roomPos.GetBoundingBox(
                                                        new Size(
                                                            AssetConstants.DefaultPropWidth,
                                                            AssetConstants.DefaultPropHeight),
                                                        true)))
                                            {
                                                toolStripItem = _contextMenu.Items.Add($"Select User: {roomUser.name}");
                                                if (toolStripItem != null)
                                                {
                                                    toolStripItem.Tag = new object[]
                                                    {
                                                        ContextMenuCommandTypes.UI_USERSELECT,
                                                        roomUser.userID,
                                                    };
                                                    toolStripItem.Click += contextMenuItem_Click;
                                                }

                                                if (this._sessionState.UserInfo.IsModerator ||
                                                    this._sessionState.UserInfo.IsAdministrator)
                                                {
                                                    toolStripItem = _contextMenu.Items.Add($"Pin User: {roomUser.name}");
                                                    if (toolStripItem != null)
                                                    {
                                                        toolStripItem.Tag = new object[]
                                                        {
                                                            ContextMenuCommandTypes.CMD_PIN,
                                                            roomUser.userID,
                                                        };
                                                        toolStripItem.Click += contextMenuItem_Click;
                                                    }

                                                    toolStripItem = _contextMenu.Items.Add($"Unpin User: {roomUser.name}");
                                                    if (toolStripItem != null)
                                                    {
                                                        toolStripItem.Tag = new object[]
                                                        {
                                                            ContextMenuCommandTypes.CMD_UNPIN,
                                                            roomUser.userID,
                                                        };
                                                        toolStripItem.Click += contextMenuItem_Click;
                                                    }

                                                    toolStripItem = _contextMenu.Items.Add($"Gag User: {roomUser.name}");
                                                    if (toolStripItem != null)
                                                    {
                                                        toolStripItem.Tag = new object[]
                                                        {
                                                            ContextMenuCommandTypes.CMD_GAG,
                                                            roomUser.userID,
                                                        };
                                                        toolStripItem.Click += contextMenuItem_Click;
                                                    }

                                                    toolStripItem = _contextMenu.Items.Add($"Ungag User: {roomUser.name}");
                                                    if (toolStripItem != null)
                                                    {
                                                        toolStripItem.Tag = new object[]
                                                        {
                                                            ContextMenuCommandTypes.CMD_UNGAG,
                                                            roomUser.userID,
                                                        };
                                                        toolStripItem.Click += contextMenuItem_Click;
                                                    }

                                                    toolStripItem = _contextMenu.Items.Add($"Propgag User: {roomUser.name}");
                                                    if (toolStripItem != null)
                                                    {
                                                        toolStripItem.Tag = new object[]
                                                        {
                                                            ContextMenuCommandTypes.CMD_PROPGAG,
                                                            roomUser.userID,
                                                        };
                                                        toolStripItem.Click += contextMenuItem_Click;
                                                    }

                                                    toolStripItem = _contextMenu.Items.Add($"Unpropgag User: {roomUser.name}");
                                                    if (toolStripItem != null)
                                                    {
                                                        toolStripItem.Tag = new object[]
                                                        {
                                                            ContextMenuCommandTypes.CMD_UNPROPGAG,
                                                            roomUser.userID,
                                                        };
                                                        toolStripItem.Click += contextMenuItem_Click;
                                                    }

                                                    toolStripItem = _contextMenu.Items.Add($"Kill User: {roomUser.name}");
                                                    if (toolStripItem != null)
                                                    {
                                                        toolStripItem.Tag = new object[]
                                                        {
                                                            ContextMenuCommandTypes.MSG_KILLUSER,
                                                            roomUser.userID,
                                                        };
                                                        toolStripItem.Click += contextMenuItem_Click;
                                                    }
                                                }
                                            }

                                    if ((this._sessionState.RoomInfo?.LooseProps?.Count ?? 0) > 0)
                                    {
                                        toolStripItem = _contextMenu.Items.Add("Delete All Props");
                                        if (toolStripItem != null)
                                        {
                                            toolStripItem.Tag = new object[]
                                            {
                                                ContextMenuCommandTypes.MSG_PROPDEL,
                                                -1,
                                            };
                                            toolStripItem.Click += contextMenuItem_Click;
                                        }

                                        var j = 0;
                                        foreach (var looseProp in this._sessionState.RoomInfo.LooseProps)
                                        {
                                            if (looseProp.loc == null) continue;

                                            var prop = AssetsManager.Current.Assets.GetValueLocked(looseProp.assetSpec.id);
                                            if (prop == null) continue;

                                            if (point.IsPointInPolygon(
                                                looseProp.loc.GetBoundingBox(
                                                    new Size(
                                                        prop.Width,
                                                        prop.Height),
                                                    true)))
                                            {
                                                toolStripItem = _contextMenu.Items.Add($"Select Prop: {looseProp.assetSpec.id}");
                                                if (toolStripItem != null)
                                                {
                                                    toolStripItem.Tag = new object[]
                                                    {
                                                        ContextMenuCommandTypes.UI_PROPSELECT,
                                                        j,
                                                    };
                                                    toolStripItem.Click += contextMenuItem_Click;
                                                }

                                                toolStripItem = _contextMenu.Items.Add($"Delete Prop: {looseProp.assetSpec.id}");
                                                if (toolStripItem != null)
                                                {
                                                    toolStripItem.Tag = new object[]
                                                    {
                                                        ContextMenuCommandTypes.MSG_PROPDEL,
                                                        j,
                                                    };
                                                    toolStripItem.Click += contextMenuItem_Click;
                                                }
                                            }

                                            j++;
                                        }
                                    }

                                    if ((this._sessionState.RoomInfo?.HotSpots?.Count ?? 0) > 0)
                                        foreach (var hotSpot in this._sessionState.RoomInfo.HotSpots)
                                            if (point.IsPointInPolygon(hotSpot.Vortexes.ToArray()))
                                            {
                                                toolStripItem = _contextMenu.Items.Add($"Select Spot: {hotSpot.id}");
                                                if (toolStripItem != null)
                                                {
                                                    toolStripItem.Tag = new object[]
                                                    {
                                                        ContextMenuCommandTypes.UI_SPOTSELECT,
                                                        hotSpot.id,
                                                    };
                                                    toolStripItem.Click += contextMenuItem_Click;
                                                }

                                                if (this._sessionState.UserInfo.IsModerator ||
                                                    this._sessionState.UserInfo.IsAdministrator)
                                                {
                                                    toolStripItem = _contextMenu.Items.Add($"Delete Spot: {hotSpot.id}");
                                                    if (toolStripItem != null)
                                                    {
                                                        toolStripItem.Tag = new object[]
                                                        {
                                                            ContextMenuCommandTypes.MSG_SPOTDEL,
                                                            hotSpot.id,
                                                        };
                                                        toolStripItem.Click += contextMenuItem_Click;
                                                    }
                                                }
                                            }

                                    _contextMenu.Show(Cursor.Position);

                                    break;
                            }
                        }
                    });
                    imgScreen.MouseMove += new MouseEventHandler((sender, e) =>
                    {
                        imgScreen.Cursor = Cursors.Default;

                        if ((this._sessionState.ConnectionState?.IsConnected ?? false) == true)
                        {
                            var point = new ThePalace.Core.Models.Palace.Point((short)e.X, (short)e.Y);

                            if ((this._sessionState.RoomUsersInfo?.Count ?? 0) > 0)
                                foreach (var roomUser in this._sessionState.RoomUsersInfo.Values)
                                    if (roomUser.userID == 0 ||
                                        roomUser.roomPos == null) continue;
                                    else if (point.IsPointInPolygon(
                                        roomUser.roomPos.GetBoundingBox(
                                            new Size(
                                                AssetConstants.DefaultPropWidth,
                                                AssetConstants.DefaultPropHeight),
                                            true)))
                                    {
                                        imgScreen.Cursor = Cursors.Hand;
                                        break;
                                    }

                            if ((this._sessionState.RoomInfo?.LooseProps?.Count ?? 0) > 0)
                                foreach (var looseProp in this._sessionState.RoomInfo.LooseProps)
                                {
                                    if (looseProp.loc == null) continue;

                                    var prop = AssetsManager.Current.Assets.GetValueLocked(looseProp.assetSpec.id);
                                    if (prop == null) continue;

                                    if (point.IsPointInPolygon(
                                        looseProp.loc.GetBoundingBox(
                                            new Size(
                                                prop.Width,
                                                prop.Height),
                                            true)))
                                    {
                                        imgScreen.Cursor = Cursors.Hand;
                                        break;
                                    }
                                }

                            if ((this._sessionState.RoomInfo?.HotSpots?.Count ?? 0) > 0)
                                foreach (var hotSpot in this._sessionState.RoomInfo.HotSpots)
                                    if (point.IsPointInPolygon(hotSpot.Vortexes.ToArray()))
                                    {
                                        imgScreen.Cursor = Cursors.Hand;
                                        break;
                                    }
                        }
                    });

                    this._sessionState.RegisterControl(nameof(imgScreen), imgScreen);
                }
            }

            var labelInfo = this._sessionState.GetControl("labelInfo") as Label;
            if (labelInfo == null)
            {
                labelInfo = FormsManager.Current.CreateControl<FormBase, Label>(form, true, new ControlCfg
                {
                    Visible = true,
                    TabIndex = 0, //tabIndex++,
                    Text = string.Empty,
                    Margin = new Padding(0, 0, 0, 0),
                })?.FirstOrDefault();

                if (labelInfo != null)
                    this._sessionState.RegisterControl(nameof(labelInfo), labelInfo);
            }

            var txtInput = this._sessionState.GetControl("txtInput") as TextBox;
            if (txtInput == null)
            {
                txtInput = FormsManager.Current.CreateControl<FormBase, TextBox>(form, true, new ControlCfg
                {
                    Visible = true,
                    TabIndex = tabIndex++,
                    Text = string.Empty,
                    Margin = new Padding(0, 0, 0, 0),
                    BackColor = Color.FromKnownColor(KnownColor.DimGray),
                    Multiline = true,
                    MaxLength = 255,
                })?.FirstOrDefault();

                if (txtInput != null)
                {
                    txtInput.LostFocus += new EventHandler((sender, e) =>
                    {
                        txtInput.BackColor = Color.FromKnownColor(KnownColor.LightGray);
                    });
                    txtInput.GotFocus += new EventHandler((sender, e) =>
                    {
                        txtInput.BackColor = Color.FromKnownColor(KnownColor.White);
                    });
                    txtInput.KeyUp += new KeyEventHandler((sender, e) =>
                    {
                        this._sessionState.LastActivity = DateTime.UtcNow;

                        if (e.KeyCode == Keys.Tab)
                        {
                            e.Handled = true;

                            txtInput.Text = string.Empty;
                        }

                        if ((this._sessionState?.ConnectionState?.IsConnected ?? false) == false)
                        {
                            this.ShowConnectionForm();

                            return;
                        }

                        ScriptEvents.Current.Invoke(IptEventTypes.KeyUp, this._sessionState, null, this._sessionState.ScriptState);

                        if (e.KeyCode == Keys.Enter)
                        {
                            e.Handled = true;

                            var text = txtInput.Text?.Trim();
                            txtInput.Text = string.Empty;

                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                if (text[0] == '/')
                                {
                                    ThreadManager.Current.Enqueue(ThreadQueues.ScriptEngine, args =>
                                    {
                                        var sessionState = args[0] as SessionState;
                                        if (sessionState == null) return null;

                                        var text = args[1] as string;
                                        if (text == null) return null;

                                        try
                                        {
                                            var atomlist = IptscraeEngine.Parser(
                                                sessionState.ScriptState as IptTracking,
                                                text,
                                                false);
                                            IptscraeEngine.Executor(atomlist, sessionState.ScriptState as IptTracking);
                                        }
                                        catch (Exception ex)
                                        {
#if DEBUG
                                            Debug.WriteLine(ex.Message);
#endif
                                        }

                                        return null;
                                    }, this._sessionState, string.Concat(text.Skip(1)));
                                }
                                else
                                {
                                    var xTalk = new MSG_XTALK
                                    {
                                        text = text,
                                    };
                                    var outboundPacket = new Header
                                    {
                                        eventType = EventTypes.MSG_XTALK,
                                        protocolSend = xTalk,
                                    };

                                    ScriptEvents.Current.Invoke(IptEventTypes.Chat, this._sessionState, outboundPacket, this._sessionState.ScriptState);
                                    ScriptEvents.Current.Invoke(IptEventTypes.OutChat, this._sessionState, outboundPacket, this._sessionState.ScriptState);

                                    var iptTracking = this._sessionState.ScriptState as IptTracking;
                                    if (iptTracking != null)
                                    {
                                        if (iptTracking.Variables?.ContainsKey("CHATSTR") == true)
                                            xTalk.text = iptTracking.Variables["CHATSTR"].Value.Value.ToString();

                                        if (!string.IsNullOrWhiteSpace(xTalk.text))
                                            ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.SEND, outboundPacket);
                                    }
                                }
                            }
                        }
                    });
                    txtInput.KeyDown += new KeyEventHandler((sender, e) =>
                    {
                        this._sessionState.LastActivity = DateTime.UtcNow;

                        if (e.KeyCode == Keys.Tab)
                        {
                            e.Handled = true;

                            txtInput.Text = string.Empty;
                        }

                        if ((this._sessionState?.ConnectionState?.IsConnected ?? false) == false) return;

                        ScriptEvents.Current.Invoke(IptEventTypes.KeyDown, this._sessionState, null, this._sessionState.ScriptState);
                    });

                    this._sessionState.RegisterControl(nameof(txtInput), txtInput);
                }
            }

            this._sessionState.RefreshScreen(ScreenLayers.Base);

            ShowConnectionForm();
        }
        private void ShowConnectionForm(object sender = null, EventArgs e = null)
        {
            if (this.IsDisposed) return;

            var connectionForm = this._sessionState.GetForm<Forms.Connection>(nameof(Forms.Connection));
            if (connectionForm == null)
            {
                connectionForm = FormsManager.Current.CreateForm<Forms.Connection>(
                    new FormCfg
                    {
                        AutoScaleDimensions = new SizeF(7F, 15F),
                        AutoScaleMode = AutoScaleMode.Font,
                        WindowState = FormWindowState.Normal,
                        StartPosition = FormStartPosition.CenterScreen,
                        Margin = new Padding(0, 0, 0, 0),
                        Size = new Size(303, 182),
                        Visible = true,
                    });
                if (connectionForm == null) return;

                connectionForm.SessionState = this._sessionState;
                connectionForm.FormClosed += new FormClosedEventHandler((sender, e) =>
                {
                    this._sessionState.UnregisterForm(nameof(Forms.Connection), sender as FormBase);
                });

                if (connectionForm != null)
                {
                    this._sessionState.RegisterForm(nameof(Forms.Connection), connectionForm);

                    var buttonDisconnect = connectionForm.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "buttonDisconnect")
                        .FirstOrDefault() as Button;
                    if (buttonDisconnect != null)
                    {
                        buttonDisconnect.Click += new EventHandler((sender, e) =>
                        {
                            if ((this._sessionState.ConnectionState?.IsConnected ?? false) == true)
                                NetworkManager.Current.Disconnect(this._sessionState);

                            var connectionForm = this._sessionState.GetForm(nameof(Forms.Connection));
                            connectionForm?.Close();
                        });
                        buttonDisconnect.Visible = this._sessionState.ConnectionState?.IsConnected ?? false;
                    }

                    var buttonConnect = connectionForm.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "buttonConnect")
                        .FirstOrDefault() as Button;
                    if (buttonConnect != null)
                    {
                        buttonConnect.Click += new EventHandler((sender, e) =>
                        {
                            var connectionForm = this._sessionState.GetForm(nameof(Forms.Connection));
                            if (connectionForm != null)
                            {
                                //var checkBoxNewTab = connectionForm.Controls
                                //    .Cast<Control>()
                                //    .Where(c => c.Name == "checkBoxNewTab")
                                //    .FirstOrDefault() as CheckBox;
                                //if (checkBoxNewTab?.Checked == true)
                                //{
                                //}

                                var comboBoxUsernames = connectionForm.Controls
                                    .Cast<Control>()
                                    .Where(c => c.Name == "comboBoxUsernames")
                                    .FirstOrDefault() as ComboBox;
                                if (comboBoxUsernames != null)
                                {
                                    this._sessionState.UserInfo.name = this._sessionState.RegInfo.userName = comboBoxUsernames.Text;
                                }

                                var textBoxRoomID = connectionForm.Controls
                                    .Cast<Control>()
                                    .Where(c => c.Name == "textBoxRoomID")
                                    .FirstOrDefault() as TextBox;
                                if (textBoxRoomID != null)
                                {
                                    var roomID = (short)0;

                                    if (!string.IsNullOrEmpty(textBoxRoomID.Text))
                                        roomID = Convert.ToInt16(textBoxRoomID.Text);

                                    this._sessionState.RegInfo.desiredRoom = roomID;
                                }

                                var comboBoxAddresses = connectionForm.Controls
                                    .Cast<Control>()
                                    .Where(c => c.Name == "comboBoxAddresses")
                                    .FirstOrDefault() as ComboBox;
                                if (comboBoxAddresses != null &&
                                    !string.IsNullOrWhiteSpace(comboBoxAddresses.Text))
                                    ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.CONNECT, $"palace://{comboBoxAddresses.Text}");

                                connectionForm.Close();
                            }
                        });
                    }

                    var buttonCancel = connectionForm.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "buttonCancel")
                        .FirstOrDefault() as Button;
                    if (buttonCancel != null)
                    {
                        buttonCancel.Click += new EventHandler((sender, e) =>
                        {
                            var connectionForm = this._sessionState.GetForm(nameof(Forms.Connection));
                            connectionForm?.Close();
                        });
                    }

                    var comboBoxUsernames = connectionForm.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "comboBoxUsernames")
                        .FirstOrDefault() as ComboBox;
                    if (comboBoxUsernames != null)
                    {
                        var usernamesList = SettingsManager.Current.Settings[@"\GUI\Connection\Usernames"] as ISettingList;
                        if (usernamesList != null)
                        {
                            comboBoxUsernames.Items.AddRange(usernamesList.Text
                                .Select(v => new ComboboxItem
                                {
                                    Text = v,
                                    Value = v,
                                })
                                .ToArray());

                            comboBoxUsernames.Text = usernamesList.Text?.FirstOrDefault();
                        }
                    }

                    var comboBoxAddresses = connectionForm.Controls
                        .Cast<Control>()
                        .Where(c => c.Name == "comboBoxAddresses")
                        .FirstOrDefault() as ComboBox;
                    if (comboBoxAddresses != null)
                    {
                        var addressesList = SettingsManager.Current.Settings[@"\GUI\Connection\Addresses"] as ISettingList;
                        if (addressesList != null)
                        {
                            comboBoxAddresses.Items.AddRange(addressesList.Text
                                .Select(v => new ComboboxItem
                                {
                                    Text = v,
                                    Value = v,
                                })
                                .ToArray());

                            comboBoxAddresses.Text = addressesList.Text?.FirstOrDefault();
                        }
                    }
                }
            }

            if (connectionForm != null)
            {
                connectionForm.TopMost = true;

                connectionForm.Show();
                connectionForm.Focus();

                this._sessionState.RefreshScreen(ScreenLayers.Base);
                this._sessionState.RefreshUI();
                this._sessionState.RefreshScreen();
                this._sessionState.RefreshRibbon();
            }
        }

        private void toolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var name = e?.ClickedItem?.Name;

            if (!string.IsNullOrWhiteSpace(name))
                switch (name)
                {
                    case nameof(GoBack):
                    case nameof(GoForward):
                        if ((this._sessionState.ConnectionState?.IsConnected ?? false) == true &&
                            (this._sessionState.History.History.Count > 0))
                        {
                            var url = null as string;

                            switch (name)
                            {
                                case nameof(GoBack):
                                    if ((!this._sessionState.History.Position.HasValue ||
                                        this._sessionState.History.History.Keys.Min() != this._sessionState.History.Position.Value))
                                        url = this._sessionState.History.Back();
                                    break;
                                case nameof(GoForward):
                                    if (this._sessionState.History.Position.HasValue &&
                                        this._sessionState.History.History.Keys.Max() != this._sessionState.History.Position.Value)
                                        url = this._sessionState.History.Forward();
                                    break;
                            }

                            if (url != null &&
                                ClientConstants.REGEX_PALACEURL.IsMatch(url))
                            {
                                var match = ClientConstants.REGEX_PALACEURL.Match(url);
                                if (match.Groups.Count < 2) break;

                                var host = match.Groups[1].Value;
                                var port = match.Groups.Count > 2 &&
                                    !string.IsNullOrWhiteSpace(match.Groups[2].Value)
                                        ? Convert.ToUInt16(match.Groups[2].Value) : (ushort)0;
                                var roomID = match.Groups.Count > 3 &&
                                    !string.IsNullOrWhiteSpace(match.Groups[3].Value)
                                        ? Convert.ToInt16(match.Groups[3].Value) : (short)0;

                                if ((this._sessionState.ConnectionState?.IsConnected ?? false) == true &&
                                    this._sessionState.ConnectionState.Host == host &&
                                    this._sessionState.ConnectionState.Port == port &&
                                    roomID != 0)
                                    ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.SEND, new Header
                                    {
                                        eventType = EventTypes.MSG_ROOMGOTO,
                                        protocolSend = new MSG_ROOMGOTO
                                        {
                                            dest = roomID,
                                        },
                                    });
                                else ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.CONNECT, url);
                            }
                        }

                        break;
                    case nameof(Connection):
                        ApiManager.Current.ApiBindings.GetValue("ShowConnectionForm")?.Binding(this._sessionState, null);
                        break;
                    case nameof(Chatlog):
                        ApiManager.Current.ApiBindings.GetValue("ShowLogForm")?.Binding(this._sessionState, null);
                        break;
                    case nameof(UsersList):
                        ApiManager.Current.ApiBindings.GetValue("ShowUserListForm")?.Binding(this._sessionState, null);
                        break;
                    case nameof(RoomsList):
                        ApiManager.Current.ApiBindings.GetValue("ShowRoomListForm")?.Binding(this._sessionState, null);
                        break;
                    case nameof(Bookmarks):
                        break;
                    case nameof(LiveDirectory):
                        break;
                    case nameof(DoorOutlines):
                        break;
                    case nameof(UserNametags):
                        break;
                    case nameof(Tabs):
                        break;
                    case nameof(Terminal):
                        break;
                    case nameof(SuperUser):
                        break;
                    case nameof(Draw):
                        break;
                    case nameof(Sounds):
                        break;
                }
        }
        private void toolStripDropdownlist_Click(object sender = null, EventArgs e = null)
        {
        }
        private void toolStripMenuItem_Click(object sender = null, EventArgs e = null)
        {
        }
        private void contextMenuItem_Click(object sender = null, EventArgs e = null)
        {
            var contextMenuItem = sender as ToolStripMenuItem;
            if (contextMenuItem == null) return;

            var values = contextMenuItem.Tag as object[];
            if (values == null) return;

            var cmd = (ContextMenuCommandTypes)values[0];

            if (this._sessionState.UserInfo.IsModerator ||
                this._sessionState.UserInfo.IsAdministrator)
                switch (cmd)
                {
                    case ContextMenuCommandTypes.CMD_PIN:
                    case ContextMenuCommandTypes.CMD_UNPIN:
                    case ContextMenuCommandTypes.CMD_GAG:
                    case ContextMenuCommandTypes.CMD_UNGAG:
                    case ContextMenuCommandTypes.CMD_PROPGAG:
                    case ContextMenuCommandTypes.CMD_UNPROPGAG:
                        {
                            var value = (UInt32)values[1];

                            ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.SEND, new Header
                            {
                                eventType = EventTypes.MSG_WHISPER,
                                protocolSend = new MSG_WHISPER
                                {
                                    targetID = value,
                                    text = $"`{cmd.GetDescription()}",
                                },
                            });
                        }

                        break;
                    case ContextMenuCommandTypes.MSG_KILLUSER:
                        {
                            var value = (UInt32)values[1];

                            ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.SEND, new Header
                            {
                                eventType = EventTypes.MSG_KILLUSER,
                                protocolSend = new MSG_KILLUSER
                                {
                                    targetID = value,
                                },
                            });
                        }

                        break;
                    case ContextMenuCommandTypes.MSG_SPOTDEL:
                        {
                            var value = (short)values[1];

                            ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.SEND, new Header
                            {
                                eventType = EventTypes.MSG_SPOTDEL,
                                protocolSend = new MSG_SPOTDEL
                                {
                                    spotID = value,
                                },
                            });
                        }

                        break;
                }

            switch (cmd)
            {
                case ContextMenuCommandTypes.UI_SPOTSELECT:
                    {
                        var value = (Int32)values[1];

                        this._sessionState.SelectedHotSpot = this._sessionState.RoomInfo?.HotSpots
                            ?.Where(s => s.id == value)
                            ?.FirstOrDefault();
                    }

                    break;
                case ContextMenuCommandTypes.UI_PROPSELECT:
                    {
                        var value = (Int32)values[1];

                        this._sessionState.SelectedProp = this._sessionState.RoomInfo?.LooseProps
                            ?.Where(s => s.assetSpec.id == value)
                            ?.Select(s => s.assetSpec)
                            ?.FirstOrDefault();
                    }

                    break;
                case ContextMenuCommandTypes.UI_USERSELECT:
                    {
                        var value = (UInt32)values[1];

                        this._sessionState.SelectedUser = this._sessionState.RoomUsersInfo.GetValueLocked(value);
                    }

                    break;
                case ContextMenuCommandTypes.MSG_PROPDEL:
                    {
                        var value = (Int32)values[1];

                        ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.SEND, new Header
                        {
                            eventType = EventTypes.MSG_PROPDEL,
                            protocolSend = new MSG_PROPDEL
                            {
                                propNum = value,
                            },
                        });
                    }

                    break;
                case ContextMenuCommandTypes.MSG_USERMOVE:
                    {
                        var value = values[1] as ThePalace.Core.Models.Palace.Point;

                        this._sessionState.UserInfo.roomPos = value;

                        var user = null as UserRec;
                        user = this._sessionState.RoomUsersInfo.GetValueLocked(this._sessionState.UserID);
                        if (user != null)
                        {
                            user.roomPos = value;
                            user.Extended["CurrentMessage"] = null;

                            var queue = user.Extended["MessageQueue"] as DisposableQueue<MsgBubble>;
                            if (queue != null) queue.Clear();

                            this._sessionState.RefreshScreen(
                                ScreenLayers.UserProp,
                                ScreenLayers.UserNametag,
                                ScreenLayers.Messages);

                            ThreadManager.Current.Enqueue(ThreadQueues.Network, null, this._sessionState, NetworkCommandTypes.SEND, new Header
                            {
                                eventType = EventTypes.MSG_USERMOVE,
                                protocolSend = new MSG_USERMOVE
                                {
                                    pos = value,
                                },
                            });
                        }
                    }

                    break;
            }
        }
    }
}
