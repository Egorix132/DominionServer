using GameModel;
using GameModel.Cards;
using SocketIOSharp.Common;
using SocketIOSharp.Server;

namespace Dominion.SocketIoServer;

public class Server
{

    public Server() 
    {
    }

    public void Start()
    {
        using var server = new SocketIOServer(new SocketIOServerOption((ushort) 3000, PingInterval: 1000000, PingTimeout: 100000));
        
        server.OnConnection(socket =>
        {
            var client = new SocketIoClient(socket);
            client.ListenToMessage("joinRoom", (data) =>
            {
                try
                {
                    string playerName = data[0].ToString();
                    string roomName = data[1].ToString();
                    int? roomSize = null;

                    if(data.Length > 2)
                    {
                        if (int.TryParse(data[2].ToString(), out var roomSizeParsed))
                        {
                            roomSize = roomSizeParsed;
                        }
                    }

                    RoomService.JoinRoom(
                        roomName, 
                        new WebSocketPlayer(client) { Name = playerName, State = new PlayerState() },
                        roomSize);     
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });
            socket.On(SocketIOEvent.DISCONNECT, () =>
            {
                client.Dispose();
            });
                
            socket.On(SocketIOEvent.ERROR, data =>
            {
                Console.WriteLine("Error from client: {name} ", data[0].ToString());
            });
        });

        server.Start();

        Thread.Sleep(Timeout.Infinite);
    }
}