using EvoClient.Utils;
using GameModel;
using GameModel.Cards;
using System.Xml.Linq;

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

    public GenomePlayer(string id, string name, StrategyGenome? genome = null, int? seed = null)
    {
        Id = id;
        Name = name;
        State = new PlayerState(seed);
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
            var purchasePhaseIndex = (game.Turn - 2) / (StrategyGenome.GameLength / StrategyGenome.PurchasePhasesCount) + 1;

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
            var provincePile = game.Kingdom.Piles[CardEnum.Province];
            if (provincePile.Count < provincePile.InitialCount / 2)
            {
                purchasePhaseIndex = Genome.PurchasePhases.Length - 1;
            }

            var playPhaseIndex = (game.Turn - 2) / (StrategyGenome.GameLength / StrategyGenome.PlayPhasesCount);

            if (playPhaseIndex >= Genome.PlayPhases.Length
                || provincePile.Count < provincePile.InitialCount / 2)
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
        var actionsCardsInHand = State.Hand
            .OfType<IActionCard>()
            .OrderBy(c => playOrder.IndexOf(c.CardTypeId))
            .ToList();

        while (State.ActionsCount > 0 && actionsCardsInHand.Any())
        {
            IActionCard? playCard = actionsCardsInHand.FirstOrDefault();

            if (playCard == null)
            {
                break;
            }

            var argsCount = phase.CardsArguments[playCard.CardTypeId].ToList().Count;
            bool isPlayed = false;

            var argumentCombinationsCount = playCard.ArgTypes.Length == 0 ? 1 : argsCount / playCard.ArgTypes.Length;
            for (int i = 0; i < argumentCombinationsCount; i++)
            {
                var args = GetPlayCardArguments(playCard, phase, i);

                if (args == null || State.CanPlayCard(
                    game,
                    new PlayCardMessage(
                        playCard.CardTypeId,
                       args),
                    this))
                {
                    try
                    {
                        await State.PlayCard(game, this, new PlayCardMessage(
                            playCard.CardTypeId,
                            args));

                        actionsCardsInHand = State.Hand
                            .OfType<IActionCard>()
                            .OrderBy(c => playOrder.IndexOf(c.CardTypeId))
                            .ToList();
                        isPlayed = true;
                        break;
                    }
                    catch
                    {
                        actionsCardsInHand.Remove(playCard);
                        break;
                    }
                }
            }
            if (!isPlayed)
            {
                actionsCardsInHand.Remove(playCard);
            }
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

    public void UpdateState(IGameState game)
    {
        Game = game;
    }

    private List<CardEnum>? GetPlayCardArguments(IActionCard playCard, GenomePlayPhase playPhase, int argIterator)
    {
        List<CardEnum> result = new();

        if (playCard.ArgTypes.Length == 0)
        {
            return result;
        }

        var availableArgs = playPhase.CardsArguments[playCard.CardTypeId]
                .Skip(playCard.ArgTypes.Length * argIterator)
                .Take(playCard.ArgTypes.Length)
                .Cast<CardEnum?>()
                .ToList();

        var availableHand = new List<CardEnum>(State.Hand.Select(c => c.CardTypeId));
        availableHand.Remove(playCard.CardTypeId);

        for (int i = 0; i < playCard.ArgTypes.Length; i++)
        {
            var argType = playCard.ArgTypes[i];

            if (argType.Source == ActionArgSourceType.Any)
            {
                var arg = availableArgs.First()!.Value;
                result.Add(arg);
                availableArgs.Remove(arg);
            }
            else
            {
                var arg = availableArgs.FirstOrDefault(a => availableHand.Contains(a!.Value));
                if (arg == null)
                {
                    if (!argType.IsOptional)
                    {
                        return null;
                    }
                    else
                    {
                        continue;
                    }
                }
                result.Add(arg.Value);
                availableArgs.Remove(arg);
                availableHand.Remove(arg.Value);
            }
        }

        return result;
    }
}


