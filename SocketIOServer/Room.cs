using GameModel;
using GameModel.Cards;

namespace Dominion.SocketIoServer;

internal class Room
{
    public string Name { get; set; }
    public int Size { get; set; } = 2;

    public Game? Game = null;

    public List<IPlayer> Players { get; set; } = new();

    public Room(string name, int size)
    {
        Name = name;
        Size = size;
    }

    public bool Join(IPlayer player)
    {
        if(Game != null || Players.Count >= Size)
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
                            CardEnum.Artisan, CardEnum.Cellar, CardEnum.Market, CardEnum.Merchant, CardEnum.Mine,
                            CardEnum.Moat, CardEnum.Moneylender, CardEnum.Poacher, CardEnum.Remodel, CardEnum.Witch
                            },
                            Players.Count
                        )
                    );
        await Game.StartGame();

        if (Players.Count == Size)
        {
            StartNewGame();
        }
    }
}
