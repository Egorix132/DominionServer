using GameModel;
using GameModel.Cards;

namespace Dominion.SocketIoServer;

internal class Room
{
    public string Name { get; set; }
    public int Size { get; set; } = 2;

    public bool isGameStarted = false;

    public List<IPlayer> Players { get; set; } = new();

    public Room(string name, int size)
    {
        Name = name;
        Size = size;
    }

    public bool Join(IPlayer player)
    {
        if(isGameStarted || Players.Count >= Size)
        {
            return false;
        }

        Players.Add(player);

        if(Players.Count == Size)
        {
            isGameStarted = true;
            StartGame();
        }

        return true;
    }

    public void StartGame()
    {
        var newGame = new Game(
                        Players,
                        new Kingdom(
                            new List<CardEnum> {
                            CardEnum.Artisan, CardEnum.Cellar, CardEnum.Market, CardEnum.Merchant, CardEnum.Mine,
                            CardEnum.Moat, CardEnum.Moneylender, CardEnum.Poacher, CardEnum.Remodel, CardEnum.Witch
                            },
                            Players.Count
                        )
                    );
        newGame.StartGame();
    }
}
