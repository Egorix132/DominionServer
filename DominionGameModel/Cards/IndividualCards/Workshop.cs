using EvoClient.Utils;

namespace GameModel.Cards.IndividualCards;

public class WorkshopCard : AbstractActionCard
{
    public override string Name { get; } = "Workshop";

    public override int Cost { get; } = 3;

    public override ActionArg[] ArgTypes { get; } = new[] { new ActionArg(ActionArgSourceType.FromHand), new ActionArg(), };

    public override string Text { get; } = "Gain a card costing up to $4.";

    public override CardEnum CardTypeId { get; } = CardEnum.Workshop;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        var getCardType = playMessage.Args[0];

        var gottenCard = game.Kingdom.Piles[getCardType].Pop();

        player.State.AddCardsToDiscard(gottenCard);
    }

    public override bool CanAct(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        if (playMessage.Args.Count < 1)
        {
            return false;
        }

        var getCardType = playMessage.Args[0];

        if (game.Kingdom.IsPileEmpty(getCardType))
        {
            return false;
        }

        var getCardCost = CardEnumDict.GetCard(getCardType).Cost;
        if (getCardCost > 4)
        {
            return false;
        }

        return true;
    }
}
