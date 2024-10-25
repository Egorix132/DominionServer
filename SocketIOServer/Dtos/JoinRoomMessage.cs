namespace Dominion.SocketIoServer.Dtos
{
    public class JoinRoomMessage
    {
        public string PlayerName { get; set; }
        public string RoomName { get; set; }
        public int? RoomSize { get; set; }
    }
}
