using System.Collections.Generic;

namespace ThePalace.Core.Server.Interfaces
{
    public interface ICommandAttribute
    {
        bool OnBeforeCommandExecute(Dictionary<string, object> contextValues);
    }
}
