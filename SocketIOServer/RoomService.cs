using GameModel;

namespace Dominion.SocketIoServer
{
    internal static class RoomService
    {
        public static Dictionary<string, Room> RoomList { get; set; } = new();

        static RoomService() { }

        public static bool JoinRoom(string name, IPlayer player, int? size = 2)
        {
            if (!RoomList.TryGetValue(name, out var room))
            {
                room = new Room(name, size ?? 2);
                RoomList.Add(name, room);
            }

            return room.Join(player);
        }
    }
}
