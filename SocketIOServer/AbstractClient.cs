using Dominion.SocketIoServer;
using Dominion.SocketIoServer.Dtos;
using EngineIOSharp.Common.Enum;
using GameModel;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using SocketIOSharp.Common.Packet;

namespace DominionClient;

internal abstract class AbstractClient
{
    protected SocketIOClient _client;

    public bool IsConnected = false;

    protected void Init(string host)
    {
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

        _client.On("exception", (data) => HandleException(data[0]!.ToString()));

        _client.On("playTurn", HandlePlayTurn);
        _client.On("clarificatePlay", HandleClarificatePlay);
    }

    public abstract Task PlayTurnAsync(GameStateDto game);

    public abstract ClarificationResponseMessage ClarifyPlay(ClarificationRequestMessage message);

    public abstract void HandleException(string exceptionMessage);

    public abstract void HandleDisconnect(JToken[] data);

    public virtual void JoinRoom(JoinRoomMessage message)
    {
        while (IsConnected == false)
        {
            Thread.Sleep(1000);
        }
        _client.Emit("joinRoom", message);
    }

    protected async Task<GameStateDto?> PlayCardAsync(PlayCardMessage message)
    {
        var gameStateData = await _client.AskAsync("playCard", message);

        if (gameStateData.Length != 0)
        {
            return Deserialise<GameStateDto>(gameStateData);
        }

        return null;
    }

    protected void BuyCards(BuyMessage message)
    {
        _client.Emit("buyCards", message);
    }

    protected void HandlePlayTurn(JToken[] data)
    {
        try
        {
            var game = Deserialise<GameStateDto>(data);

            PlayTurnAsync(game);
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

            var response = ClarifyPlay(clarificationRequest);

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


