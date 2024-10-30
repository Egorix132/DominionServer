using GameModel;
using System.Numerics;

namespace Dominion.SocketIoServer
{
    internal static class RoomService
    {
        public static Dictionary<string, Room> RoomList { get; set; } = new();

        static RoomService() { }

        public static bool JoinRoom(string name, IPlayer player, int? size = 2, bool isSpectator = false)
        {
            if (!RoomList.TryGetValue(name, out var room))
            {
                room = new Room(name, size ?? 2);
                RoomList.Add(name, room);
            }

            return room.Join(player, isSpectator);
        }

        public static bool DisconnectFromRooms(string id)
        {
            var roomsForDisconnect = RoomList.Values.Where(r => r.Players.Any(p => p.Id == id));
            if (!roomsForDisconnect.Any())
            {
                return false;
            }

            foreach (var room in roomsForDisconnect)
            {
                room.Disconnect(id);
            }

            return true;
        }
    }
}
