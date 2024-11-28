using System.Collections.Concurrent;
using ThePalace.Core.Client.Core.Constants;
using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Client.Core.Models;
using ThePalace.Core.Enums;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;
using ThePalace.Core.Utility;

namespace ThePalace.Core.Console.Bots.Python.Models
{
    public sealed class HeadlessSessionState : IClientSessionState, ISessionState, IDisposable
    {
        // Session Info
        public Guid SessionID { get; } = Guid.NewGuid();
        public object ScriptState { get; set; }

        // Network Info
        public IConnectionState ConnectionState { get; set; } = null;

        // User Info
        public uint UserID
        {
            get => UserInfo.userID;
            set => UserInfo.userID = value;
        }
        public short RoomID
        {
            get => UserInfo.roomID;
            set => UserInfo.roomID = value;
        }
        public string Name
        {
            get => UserInfo.name;
            set => UserInfo.name = value;
        }
        public UserFlags UserFlags
        {
            get => UserInfo.userFlags;
            set => UserInfo.userFlags = value;
        }

        public UserRec UserInfo { get; private set; } = new();
        public RegistrationRec RegInfo { get; private set; } = new();
        public ConcurrentDictionary<string, object> Extended => UserInfo.Extended;

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

        public HeadlessSessionState()
        {
            ScriptState = new ScriptState();

            Extended.TryAdd(@"MessageQueue", new DisposableQueue<MsgBubble>());
            Extended.TryAdd(@"CurrentMessage", null);

            UserInfo.name = RegInfo.userName = "Janus (Bot)";

            RegInfo.reserved = ClientConstants.ClientAgent;
            RegInfo.ulUploadCaps = 0x41;
            RegInfo.ulDownloadCaps = 0x0151;
            RegInfo.ul2DEngineCaps = 0x01;
            RegInfo.ul2DGraphicsCaps = 0x01;

            var seed = (uint)Cipher.WizKeytoSeed(ClientConstants.RegCodeSeed);
            RegInfo.crc = Cipher.ComputeLicenseCrc(seed);
            RegInfo.counter = (uint)Cipher.GetSeedFromReg(seed, RegInfo.crc);
            RegInfo.puidCRC = RegInfo.crc;
            RegInfo.puidCtr = RegInfo.counter;
        }

        public void Dispose()
        {
            RoomUsersInfo?.Dispose();
            RoomUsersInfo = null;
            ServerRooms?.Clear();
            ServerRooms = null;
            ServerUsers?.Clear();
            ServerUsers = null;
        }
    }
}
