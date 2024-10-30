using GameModel;
using GameModel.Cards;

namespace EvoClient;

internal class MoneyBot : IPlayer
{
    public string Id { get; set; }

    public string Name { get; set; }

    public PlayerState State { get; set; }

    public MoneyBot(string host)
    {
        Id = host;
        Name = host;
        State = new PlayerState();
    }

    public async Task PlayTurnAsync(IGameState game)
    {
        if (State.TotalMoney >= 8)
        {
            State.BuyCards(game, new BuyMessage(CardEnum.Province), this);
        }
        else if (State.TotalMoney >= 6)
        {
            State.BuyCards(game, new BuyMessage(CardEnum.Gold), this);
        }
        else if (State.TotalMoney >= 3)
        {
            State.BuyCards(game, new BuyMessage(CardEnum.Silver), this);
        }
        else
        {
            return;
        }
    }

    async Task<ClarificationResponseMessage> IPlayer.ClarifyPlay(ClarificationRequestMessage request)
    {
        return new ClarificationResponseMessage();
    }

    public void GameEnded(GameEndDto gameEndDto)
    {
        return;
    }

    public void SendException(Exception e)
    {
        return;
    }
}


