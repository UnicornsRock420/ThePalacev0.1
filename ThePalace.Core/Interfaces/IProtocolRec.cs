using System;
using ThePalace.Core.Factories;

namespace ThePalace.Core.Interfaces
{
    public interface IProtocolRec : IDisposable
    {
        void Deserialize(Packet packet, params object[] values);

        byte[] Serialize(params object[] values);
    }
}
