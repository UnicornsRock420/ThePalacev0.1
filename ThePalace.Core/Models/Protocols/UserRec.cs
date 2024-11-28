using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ThePalace.Core.Constants;
using ThePalace.Core.Enums;
using ThePalace.Core.Factories;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Palace;

namespace ThePalace.Core.Models.Protocols
{
    public class UserRec : IProtocolRec
    {
        public uint userID;
        public Point roomPos;
        public List<AssetSpec> assetSpec;
        public short roomID;
        public short faceNbr;
        public short colorNbr;
        public short awayFlag;
        public short openToMsgs;
        public short nbrProps;
        public string name;

        public UserFlags userFlags;
        public ConcurrentDictionary<string, object> Extended = new();

        public bool IsGagged => (userFlags & UserFlags.U_Gag) == UserFlags.U_Gag;
        public bool IsPinned => (userFlags & UserFlags.U_Pin) == UserFlags.U_Pin;
        public bool IsRejectWhisper => (userFlags & UserFlags.U_RejectWhisper) == UserFlags.U_RejectWhisper;
        public bool IsRejectEsp => (userFlags & UserFlags.U_RejectEsp) == UserFlags.U_RejectEsp;
        public bool IsPropGagged => (userFlags & UserFlags.U_PropGag) == UserFlags.U_PropGag;
        public bool IsNameGagged => (userFlags & UserFlags.U_NameGag) == UserFlags.U_NameGag;
        public bool IsModerator => (userFlags & UserFlags.U_Moderator) == UserFlags.U_Moderator;
        public bool IsAdministrator => (userFlags & UserFlags.U_Administrator) == UserFlags.U_Administrator;

        public UserRec() { }
        public UserRec(Packet packet) =>
            Deserialize(packet);

        public void Dispose()
        {
            foreach (var ext in Extended.Values)
            {
                if (ext is IDisposable _disposable)
                    try { _disposable?.Dispose(); } catch { }
            }

            assetSpec?.Clear();
            assetSpec = null;

            Extended?.Clear();
            Extended = null;
        }

        public void Deserialize(Packet packet, params object[] values)
        {
            userID = packet.ReadUInt32();
            roomPos = new Point(packet);

            assetSpec = new();
            for (var i = 0; i < AssetConstants.MaxNbrProps; i++)
            {
                var asset = new AssetSpec(packet);
                if (asset.id != 0)
                    assetSpec.Add(asset);
            }

            roomID = packet.ReadSInt16();
            faceNbr = packet.ReadSInt16();
            colorNbr = packet.ReadSInt16();
            awayFlag = packet.ReadSInt16();
            openToMsgs = packet.ReadSInt16();
            nbrProps = packet.ReadSInt16();
            name = packet.ReadPString(32, 1);

            if (nbrProps < 1)
                assetSpec.Clear();
            else if (assetSpec.Count > nbrProps)
                while (assetSpec.Count > nbrProps)
                    assetSpec.Pop();
        }

        public byte[] Serialize(params object[] values)
        {
            nbrProps = (short)(assetSpec?.Count ?? 0);

            using (var packet = new Packet())
            {
                packet.WriteInt32(userID);
                packet.WriteBytes(roomPos.Serialize());

                for (int i = 0; i < AssetConstants.MaxNbrProps; i++)
                    if (i < nbrProps)
                        packet.WriteBytes(assetSpec[i].Serialize());
                    else
                    {
                        packet.WriteInt32(0);
                        packet.WriteInt32(0);
                    }

                packet.WriteInt16(roomID);
                packet.WriteInt16(faceNbr);
                packet.WriteInt16(colorNbr);
                packet.WriteInt16(awayFlag);
                packet.WriteInt16(openToMsgs);
                packet.WriteInt16((short)(assetSpec?.Count ?? 0));

                packet.WritePString(name, 32, 1);

                return packet.GetData();
            }
        }
    }
}
