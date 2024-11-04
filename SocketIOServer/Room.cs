using GameModel;
using GameModel.Cards;

namespace Dominion.SocketIoServer;

internal class Room
{
    public string Name { get; set; }
    public int Size { get; set; } = 2;

    public Game? Game = null;

    public List<IPlayer> Players { get; set; } = new();
    public List<IPlayer> Spectators { get; set; } = new();

    public Room(string name, int size)
    {
        Name = name;
        Size = size;
    }

    public bool Join(IPlayer player, bool isSpectator = false)
    {
        if (isSpectator)
        {
            Spectators.Add(player);

            if(Game != null)
            {
                Game.AddSpectator(player);
            }
            return true;
        }

        if (Game != null || Players.Count >= Size)
        {
            return false;
        }

        Players.Add(player);

        if(Players.Count == Size)
        {
            StartNewGame();
        }

        return true;
    }

    public bool Disconnect(string id)
    {
        Players.RemoveAll(player => player.Id == id);
        if (Game != null)
        {
            Game.Players.RemoveAll(player => player.Id == id);
            Game.Dispose();
            Game = null;
        }

        return true;
    }

    public async Task StartNewGame()
    {
        Game = new Game(
                        Players,
                        new Kingdom(
                            new List<CardEnum> {
                                CardEnum.Cellar, CardEnum.Artisan, CardEnum.Merchant, CardEnum.Market, CardEnum.Mine,
                                CardEnum.Moat, CardEnum.Remodel, CardEnum.Poacher, CardEnum.Moneylender, CardEnum.Witch
                            },
                            Players.Count
                        ),
                        Spectators
                    );
        await Game.StartGame();

        if (Players.Count == Size)
        {
            StartNewGame();
        }
    }
}
