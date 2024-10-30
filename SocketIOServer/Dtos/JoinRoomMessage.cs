namespace Dominion.SocketIoServer.Dtos;

public class JoinRoomMessage
{
    public JoinRoomMessage() { }

    public JoinRoomMessage(string playerName, string roomName, int? roomSize, bool? isSpectator = false)
    {
        PlayerName = playerName;
        RoomName = roomName;
        RoomSize = roomSize;
        IsSpectator = isSpectator;
    }

    public string PlayerName { get; set; }
    public string RoomName { get; set; }
    public int? RoomSize { get; set; }
    public bool? IsSpectator { get; set; }
}
