using EvoClient.Utils;
using GameModel;
using GameModel.Cards;

namespace EvoClient.Evo;

internal class GenomePlayerV2 : IPlayer
{
    public string Id { get; set; }

    public string Name { get; set; }

    public StrategyGenomeV2 Genome { get; set; }

    public IGameState Game { get; set; }
    public PlayerState State { get; set; }

    public GenomePlayerV2(string id, string name, StrategyGenomeV2? genome = null, int? seed = null)
    {
        Id = id;
        Name = name;
        State = new PlayerState(seed);
        Genome = genome ?? StrategyGenomeV2.GenerateRandom();
    }

    public async Task PlayTurnAsync(IGameState game)
    {
        Game = game;
        if (game.Turn == 1 || game.Turn == 2)
        {
            Buy(game, Genome.FirstTwoTurnsBuy);

            return;
        }
        else
        {
            var purchasePhaseIndex = (game.Turn - 2) / (StrategyGenome.GameLength / StrategyGenome.PurchasePhasesCount);

            if (purchasePhaseIndex >= Genome.PurchasePhases.Length)
            {
                purchasePhaseIndex = Genome.PurchasePhases.Length - 1;
            }
            var playPhaseIndex = (game.Turn - 2) / (StrategyGenome.GameLength / StrategyGenome.PlayPhasesCount);

            var provincePile = game.Kingdom.Piles[CardEnum.Province];
            if (provincePile.Count <= provincePile.InitialCount / 2)
            {
                purchasePhaseIndex = Genome.PurchasePhases.Length - 1;
                playPhaseIndex = Genome.PlayPhases.Length - 1;
            }

            if (playPhaseIndex >= Genome.PlayPhases.Length)
            {
                playPhaseIndex = Genome.PlayPhases.Length - 1;
            }
            var purchasePhase = Genome.PurchasePhases[purchasePhaseIndex];
            var playPhase = Genome.PlayPhases[playPhaseIndex];

            await Play(game, playPhase);

            Buy(game, GetCardsToBuy(purchasePhase));

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

    private IEnumerable<CardEnum> GetCardsToBuy(GenomeV2PurchasePhase purchasePhase)
    {
        return purchasePhase.CardsCount
            .Where(count => State.AllCards
            .Count(c => c.CardTypeId == count.Key) < count.Value)
            .Select(c => c.Key);
    }

    private void Buy(IGameState game, IEnumerable<CardEnum> purchaseList)
    {
        var canBuyCards = purchaseList
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

        var hand = new List<CardEnum>(State.Hand.Select(c => c.CardTypeId));
        hand.Remove(playCard.CardTypeId);

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
                CardEnum? arg;
                if (argType.IsOptional)
                {
                    arg = availableArgs.FirstOrDefault(a => hand.Contains(a!.Value));
                }
                else
                {
                    arg = availableArgs.ElementAtOrDefault(i);

                    if (arg == null)
                    {
                        return null;
                    }
                    else if (!hand.Contains(arg!.Value))
                    {
                        return null;
                    }
                }
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
                hand.Remove(arg.Value);

            }
        }

        return result;
    }
}


