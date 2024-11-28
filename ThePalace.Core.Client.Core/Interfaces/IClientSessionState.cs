using System.Collections.Concurrent;
using System.Collections.Generic;
using ThePalace.Core.Interfaces;
using ThePalace.Core.Models.Protocols;

namespace ThePalace.Core.Client.Core.Interfaces
{
    public interface IClientSessionState : ISessionState
    {
        string RoomName { get; set; }
        RoomRec RoomInfo { get; set; }
        DisposableDictionary<uint, UserRec> RoomUsersInfo { get; }

        int ServerPermissions { get; set; }
        int ServerPopulation { get; set; }
        string MediaUrl { get; set; }
        string ServerName { get; set; }
        List<ListRec> ServerUsers { get; set; }
        List<ListRec> ServerRooms { get; set; }
    }
}
