using System.Collections.Generic;

namespace ThePalace.Core.Server.Interfaces
{
    public interface IProtocolAttribute
    {
        bool OnBeforeProtocolExecute(Dictionary<string, object> contextValues);
    }
}
