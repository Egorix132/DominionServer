/*using Dominion.SocketIoServer;
using Dominion.SocketIoServer.Dtos;
using EngineIOSharp.Common.Enum;
using GameModel;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using SocketIOSharp.Common.Packet;

namespace DominionClient;

internal class SocketClient : IGameSenderClient
{
    protected SocketIOClient _client;

    protected ISocketClientStrategy _strategy;

    public bool IsConnected = false;

    public SocketClient(string host, ISocketClientStrategy strategy)
    {
        _strategy = strategy;
        _client = new SocketIOClient(new SocketIOClientOption(EngineIOScheme.http, host, 3000));

        _client.Connect();

        _client.On("connection", (JToken[] data) =>
        {
            IsConnected = true;
            Console.WriteLine("Connected");
        });

        _client.On("disconnect", (JToken[] data) =>
        {
            IsConnected = false;
            Console.WriteLine("Disconnected" + " " + data);
        });

        _client.On("error", (JToken[] data) =>
        {
            Console.WriteLine("Error" + " " + data);
        });

        _client.On("exception", (data) => _strategy.HandleException(data[0]!.ToString()));

        _client.On("playTurn", HandlePlayTurn);
        _client.On("clarificatePlay", HandleClarificatePlay);
    }

    public virtual void JoinRoom(JoinRoomMessage message)
    {
        while (IsConnected == false)
        {
            Thread.Sleep(1000);
        }
        _client.Emit("joinRoom", message);
    }

    public async Task<GameStateDto?> PlayCardAsync(PlayCardMessage message)
    {
        var gameStateData = await _client.AskAsync("playCard", message);

        if (gameStateData.Length != 0)
        {
            return Deserialise<GameStateDto>(gameStateData);
        }

        return null;
    }

    public void BuyCards(BuyMessage message)
    {
        _client.Emit("buyCards", message);
    }

    protected void HandlePlayTurn(JToken[] data)
    {
        try
        {
            var game = Deserialise<GameStateDto>(data);

            _strategy.PlayTurnAsync(game);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    protected void HandleClarificatePlay(SocketIOAckEvent askEvent)
    {
        try
        {
            var clarificationRequest = Deserialise<ClarificationRequestMessage>(askEvent.Data);

            var response = _strategy.ClarifyPlay(clarificationRequest);

            askEvent.Callback.Invoke(new JToken[] { JToken.FromObject(response) });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }


    protected T Deserialise<T>(JToken[] data)
    {
        if (data.Length < 1 || !data[0].TryDeserializeObject<T>(out var result))
        {
            throw new ArgumentException("BadRequest");
        }

        return result;
    }
}


*/