﻿using Dominion.SocketIoServer.Dtos;
using GameModel;
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
        using var server = new SocketIOServer(new SocketIOServerOption(3000, PingInterval: 1000000, PingTimeout: 100000));
        
        server.OnConnection(socket =>
        {
            var client = new SocketIoClient(socket);
            client.ListenToMessage("joinRoom", (data) =>
            {
                try
                {
                    if (data.Length < 1 || !data[0].TryDeserializeObject<JoinRoomMessage>(out var joinRoomMessage))
                    {
                        throw new ArgumentException("BadRequest");
                    }

                    IPlayer player = (joinRoomMessage.IsSpectator ?? false)
                        ? new SpectatorPlayer(client, joinRoomMessage.PlayerName) 
                        : new WebSocketPlayer(client, joinRoomMessage.PlayerName);

                    RoomService.JoinRoom(
                        joinRoomMessage!.RoomName,
                        player,
                        joinRoomMessage.RoomSize,
                        joinRoomMessage.IsSpectator ?? false);     
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });
            /*client.ListenToMessage("botTest", (data) =>
            {
                try
                {
                    if (data.Length < 1 || !data[0].TryDeserializeObject<JoinRoomMessage>(out var joinRoomMessage))
                    {
                        throw new ArgumentException("BadRequest");
                    }

                    RoomService.JoinRoom(
                        joinRoomMessage!.RoomName,
                        new MoneyBot("1"),
                        joinRoomMessage.RoomSize);

                    RoomService.JoinRoom(
                        joinRoomMessage!.RoomName,
                        new MoneyBot("2"),
                        joinRoomMessage.RoomSize);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });*/
            socket.On(SocketIOEvent.DISCONNECT, () =>
            {
                RoomService.DisconnectFromRooms(client.Id.ToString());
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