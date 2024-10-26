namespace Dominion.SocketIoServer.Dtos;

public class JoinRoomMessage
{
    public JoinRoomMessage() { }

    public JoinRoomMessage(string playerName, string roomName, int? roomSize)
    {
        PlayerName = playerName;
        RoomName = roomName;
        RoomSize = roomSize;
    }

    public string PlayerName { get; set; }
    public string RoomName { get; set; }
    public int? RoomSize { get; set; }
}
