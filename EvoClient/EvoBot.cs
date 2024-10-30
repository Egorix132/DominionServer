/*using Dominion.SocketIoServer.Dtos;
using EvoClient;
using EvoClient.Evo;
using EvoClient.Utils;
using GameModel;
using GameModel.Cards;
using Newtonsoft.Json.Linq;

namespace DominionClient;

internal class EvoBot : ISocketClientStrategy
{
    public StrategyGenome Genome { get; set; }

    public int CurrentGenomePhaseIndex { get; set; }
    public List<CardEnum> CardsBoughtInThisPhase { get; set; } = new();

    private IGameSenderClient _client;

    public EvoBot(string host, StrategyGenome strategy)
    {
        _client = new SocketClient(host, this);
        Genome = strategy ?? StrategyGenome.GenerateRandom();
    }

    public EvoBot(string id, string name, StrategyGenome genome = null)
    {
        _client = new InCodePlayerClient(id, name, this);

        Genome = genome ?? StrategyGenome.GenerateRandom();
    }

    private async Task Play(IGameState game, GenomePhase phase)
    {
        var actionsCardsInHand = game.CurrentPlayer.State.Hand.Where(c => c.Types.Contains(CardType.Action));
        var playOrder = phase.PlayOrder.ToList();

        while (game.CurrentPlayer.State.ActionsCount > 0 && actionsCardsInHand.Any())
        {
            CardEnum? playCard = null;
            for (int i = 0; i < playOrder.Count; i++)
            {
                var type = playOrder[i];
                if (actionsCardsInHand.Any(c => c.CardTypeId == type))
                {
                    playCard = type;
                    break;
                }
            }

            if (playCard == null)
            {
                break;
            }

            var gameStateDto = (await _client.PlayCardAsync(new PlayCardMessage(
                playCard.Value,
                phase.CardsArguments[playCard.Value])))!;

            if (gameStateDto.ExceptionType != null)
            {
                playOrder.Remove(playCard.Value);
            }

            actionsCardsInHand = gameStateDto.PlayerState.Hand.Where(c => CardEnumDict.GetCard(c).Types.Contains(CardType.Action));
        }
    }

    private void Buy(IGameState game, GenomePhase phase)
    {
        var cardsToBuy = phase.Purchases.ToList();
        foreach (var boughtCard in CardsBoughtInThisPhase)
        {
            cardsToBuy.Remove(boughtCard);
        }

        var canBuyCards = cardsToBuy
                .Where(c => CardEnumDict.GetCard(c).Cost <= game.CurrentPlayer.State.TotalMoney)
                .Where(c => !game.Kingdom.Piles[c].IsEmpty());

        if (!canBuyCards.Any())
        {
            _client.BuyCards(new BuyMessage());
            return;
        }

        var buyCard = canBuyCards
                .MaxBy(c => CardEnumDict.GetCard(c).Cost);

        _client.BuyCards(new BuyMessage(buyCard));
        CardsBoughtInThisPhase.Add(buyCard);
    }
    public ClarificationResponseMessage ClarifyPlay(ClarificationRequestMessage message)
    {
        return new ClarificationResponseMessage();
    }

    public void HandleException(string exceptionMessage)
    {
        Console.WriteLine(exceptionMessage);
    }

    public void HandleDisconnect(JToken[] data)
    {
        Console.WriteLine("Disconnected" + " " + data);
    }


    #region IPlayer
    public async Task PlayTurnAsync(IGameState game)
    {
        if (game.Turn == 1 || game.Turn == 2)
        {
            var phase = Genome.GenomePhases[game.Turn - 1];

            Buy(game, phase);

            return;
        }
        else
        {
            var phaseIndex = (game.Turn - 2) / GenomePhase.PhaseTurnDuration + 2;

            if (phaseIndex >= Genome.GenomePhases.Length)
            {
                phaseIndex = Genome.GenomePhases.Length - 1;
                CardsBoughtInThisPhase.Clear();
            }
            if (phaseIndex != CurrentGenomePhaseIndex)
            {
                CurrentGenomePhaseIndex = phaseIndex;
                CardsBoughtInThisPhase.Clear();
            }
            var phase = Genome.GenomePhases[phaseIndex];

            await Play(game, phase);

            Buy(game, phase);

            return;
        }
    }

    async Task<ClarificationResponseMessage> IPlayer.ClarifyPlay(ClarificationRequestMessage request)
    {
        return ClarifyPlay(request);
    }

    public void GameEnded(GameEndDto gameEndDto)
    {
        return;
    }

    public void SendException(Exception e)
    {
        return;
    }

    #endregion
}


*/