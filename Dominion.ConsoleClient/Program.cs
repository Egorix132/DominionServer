using Dominion.SocketIoServer;
using Dominion.SocketIoServer.Dtos;
using EngineIOSharp.Common.Enum;
using GameModel;
using GameModel.Cards;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using SocketIOSharp.Common.Packet;

Console.WriteLine("Input your name!");
var name = Console.ReadLine();

Console.WriteLine("Input room name!");
var roomName = Console.ReadLine();

Console.WriteLine("Input room size!");
var roomSizeString = Console.ReadLine();

if (!int.TryParse(roomSizeString, out var roomSize))
{
    roomSize = 2;
}

var client = new SocketIOClient(new SocketIOClientOption(EngineIOScheme.http, "127.0.0.1", 3000));

client.Connect();

client.On("connection", (JToken[] data) =>
{
    Console.WriteLine("Connected");
    client.Emit("joinRoom", new JoinRoomMessage() { PlayerName = name, RoomName = roomName, RoomSize = roomSize});
});

client.On("disconnect", (JToken[] data) =>
{
    Console.WriteLine("Connected" + " " + data);
});

client.On("error", (JToken[] data) =>
{
    Console.WriteLine("Connected" + " " + data);
});

client.On("exception", (data) => Console.WriteLine(data[0]["Message"]));

client.On("playTurn", PlayTurn);
client.On("clarificatePlay", ClarificatePlay);


while (true)
{
    Thread.Sleep(1000);
}

void PlayTurn(JToken[] data)
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

async Task PlayTurnAsync(GameStateDto game)
{
    DisplayGameState(game);

    bool turnEnd = false;
    while (!turnEnd)
    {
        Console.WriteLine("Input command (play -> p [args], buy -> b [args]):");
        using var inputReader = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding);

        var input = await inputReader.ReadLineAsync();

        var args = input.Split(" ");

        var command = args[0];

        var argsList = args.Skip(1).Select(a => Enum.Parse<CardEnum>(a)).ToList();

        if (command == "p")
        {
            var message = new PlayCardMessage()
            {
                PlayedCard = argsList[0],
                Args = argsList.Skip(1).ToArray()
            };
            var gameStateData = await client.AskAsync("playCard", message);

            if(gameStateData.Length != 0)
            {
                game = Deserialise<GameStateDto>(gameStateData);

                DisplayGameState(game);
            }
        }

        if (command == "b")
        {
            var message = new BuyMessage()
            {
                Args = args.Skip(1).Select(a => Enum.Parse<CardEnum>(a)).ToArray()
            };
            client.Emit("buyCards", message, () => { });
            turnEnd = true;
        }
    }
}

void ClarificatePlay(SocketIOAckEvent askEvent)
{
    try
    {
        var clarificationRequest = Deserialise<ClarificationRequestMessage>(askEvent.Data);

        ClarificatePlayAsync(clarificationRequest, askEvent.Callback);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}

async Task ClarificatePlayAsync(ClarificationRequestMessage clarificationRequest, Action<JToken[]> callback)
{
    Console.WriteLine($"clarificate play card: {clarificationRequest.PlayedCard}");
    Console.WriteLine($"Args: {string.Join(", ", clarificationRequest.Args.Select(c => $"{c}-{(int)c}"))}");
    using var inputReader = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding);

    var input = await inputReader.ReadLineAsync();

    var args = input!.Split(" ");

    var argsList = args.Select(a => Enum.Parse<CardEnum>(a)).ToArray();

    var response = new ClarificationResponseMessage() { Args = argsList };

    callback.Invoke(new JToken[] { JToken.FromObject(response) });
}

void DisplayGameState(GameStateDto game)
{
    DisplayKingdomState(game.Kingdom);

    Console.WriteLine($"Turn: {game.Turn}\n");

    DisplayPlayerState(game.PlayerState);

}

void DisplayKingdomState(Kingdom kingdom)
{
    Console.WriteLine($"Kingdom:\n");
    foreach (var pile in kingdom.Piles)
    {
        Console.WriteLine($"{pile.Key}-{(int)pile.Key}   Count: {pile.Value.Count}  Cost: {pile.Value.Cost}");
    }
    Console.WriteLine($"\n");
}

void DisplayPlayerState(CurrentPlayerStateDto player)
{
    Console.WriteLine($"Hand:\n");
    Console.WriteLine(string.Join(", ", player.Hand.Select(c => $"{c.Name}-{(int)c.CardTypeId}")));
    Console.WriteLine();
    Console.WriteLine($"Money: {player.TotalMoney}\n");
}

T Deserialise<T>(JToken[] data)
{
    if (data.Length < 1 || !data[0].TryDeserializeObject<T>(out var result))
    {
        throw new ArgumentException("BadRequest");
    }

    return result;
}