using EvoClient.Utils;
using GameModel;
using GameModel.Cards;
using System.Numerics;

namespace EvoClient.Evo;

internal class GenomePlayer : IPlayer
{
    public string Id { get; set; }

    public string Name { get; set; }

    public StrategyGenome Genome { get; set; }

    public IGameState Game { get; set; }
    public PlayerState State { get; set; }

    public int CurrentGenomePhaseIndex { get; set; }
    public List<CardEnum> CardsBoughtInThisPhase { get; set; } = new();

    public GenomePlayer(string id, string name, StrategyGenome genome = null)
    {
        Id = id;
        Name = name;
        State = new PlayerState();
        Genome = genome ?? StrategyGenome.GenerateRandom();
    }

    public async Task PlayTurnAsync(IGameState game)
    {
        Game = game;
        if (game.Turn == 1 || game.Turn == 2)
        {
            var phase = Genome.PurchasePhases[0];

            Buy(game, phase.Take(4));

            return;
        }
        else
        {
            var purchasePhaseIndex = (game.Turn - 1) / StrategyGenome.PurchasePhaseLength + 1;

            if (purchasePhaseIndex >= Genome.PurchasePhases.Length)
            {
                purchasePhaseIndex = Genome.PurchasePhases.Length - 1;
                CardsBoughtInThisPhase.Clear();
            }
            if (purchasePhaseIndex != CurrentGenomePhaseIndex)
            {
                CurrentGenomePhaseIndex = purchasePhaseIndex;
                CardsBoughtInThisPhase.Clear();
            }

            var playPhaseIndex = game.Turn / StrategyGenome.PlayPhaseLength;

            if (playPhaseIndex >= Genome.PlayPhases.Length)
            {
                playPhaseIndex = Genome.PlayPhases.Length - 1;
            }
            CardEnum[] purchasePhase = Genome.PurchasePhases[purchasePhaseIndex];
            var playPhase = Genome.PlayPhases[playPhaseIndex];

            await Play(game, playPhase);

            Buy(game, purchasePhase);

            return;
        }
    }

    private async Task Play(IGameState game, GenomePlayPhase phase)
    {
        var playOrder = phase.PlayOrder.ToList();
        var actionsCardsInHand = State.Hand.Where(c => c.Types.Contains(CardType.Action));

        while (State.ActionsCount > 0 && actionsCardsInHand.Any())
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

            if (State.CanPlayCard(game, new PlayCardMessage(
                    playCard.Value,
                    phase.CardsArguments[playCard.Value]), this))
            {
                try
                {
                    await State.PlayCard(game, this, new PlayCardMessage(
                        playCard.Value,
                        phase.CardsArguments[playCard.Value]));
                }
                catch
                {
                    playOrder.Remove(playCard.Value);
                }
            }
            else
            {
                playOrder.Remove(playCard.Value);
            }

            actionsCardsInHand = State.Hand.Where(c => c.Types.Contains(CardType.Action));
        }
    }

    private void Buy(IGameState game, IEnumerable<CardEnum> purchaseList)
    {
        var cardsToBuy = purchaseList.ToList();
        foreach (var boughtCard in CardsBoughtInThisPhase)
        {
            cardsToBuy.Remove(boughtCard);
        }

        var canBuyCards = cardsToBuy
                .Select(c => CardEnumDict.GetCard(c))
                .Where(c => !game.Kingdom.IsPileEmpty(c.CardTypeId) 
                    && c.Cost <= State.TotalMoney);

        if (canBuyCards.Count() == 0)
        {
            State.BuyCards(game, new BuyMessage(), this);
            return;
        }

        var buyCard = canBuyCards.MaxBy(c => c.Cost);

        State.BuyCards(game, new BuyMessage(buyCard.CardTypeId), this);
        CardsBoughtInThisPhase.Add(buyCard.CardTypeId);
    }

    public async Task<ClarificationResponseMessage> ClarifyPlay(ClarificationRequestMessage request)
    {
        var playPhaseIndex = Game.Turn / StrategyGenome.PlayPhaseLength;

        if (playPhaseIndex >= Genome.PlayPhases.Length)
        {
            playPhaseIndex = Genome.PlayPhases.Length - 1;
        }
        var playPhase = Genome.PlayPhases[playPhaseIndex];

        return new ClarificationResponseMessage(playPhase.CardsArguments[request.PlayedCard]);
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


