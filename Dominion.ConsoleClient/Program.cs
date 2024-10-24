using Dominion.SocketIoServer;
using Dominion.SocketIoServer.Dtos;
using EngineIOSharp.Common.Enum;
using GameModel;
using GameModel.Cards;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;

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
    client.Emit("joinRoom", name, roomName, roomSize);
});

client.On("disconnect", (JToken[] data) =>
{
    Console.WriteLine("Connected" + " " + data);
});

client.On("error", (JToken[] data) =>
{
    Console.WriteLine("Connected" + " " + data);
});

client.On("playTurn", PlayTurn);


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
    try
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
                var gameStateData = await client.Ask("playCard", message);
                game = Deserialise<GameStateDto>(gameStateData);

                DisplayGameState(game);
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
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}

void DisplayGameState(GameStateDto game)
{
    DisplayKingdomState(game.Kingdom);
    DisplayPlayerState(game.PlayerState);
}

void DisplayKingdomState(Kingdom kingdom)
{
    Console.WriteLine($"Kingdom:\n");
    foreach (var pile in kingdom.Piles)
    {
        Console.WriteLine($"{pile.Key}-{(int)pile.Key}   Count: {pile.Value.Count}");
    }
    Console.WriteLine($"\n");
}

void DisplayPlayerState(PlayerState player)
{
    Console.WriteLine($"Hand:\n");
    Console.WriteLine(string.Join(", ", player.Hand.Select(c => $"{c.Name}-{(int)c.CardTypeId}")));
    Console.WriteLine($"\n");
}

T Deserialise<T>(JToken[] data)
{
    if (data.Length < 1 || !data[0].TryDeserializeObject<T>(out var result))
    {
        throw new ArgumentException("BadRequest");
    }

    return result;
}