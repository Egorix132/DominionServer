using EvoClient.Utils;

namespace GameModel.Cards.IndividualCards;

public class RemodelCard : AbstractActionCard
{
    public override string Name { get; } = "Remodel";

    public override int Cost { get; } = 4;

    public override ActionArg[] ArgTypes { get; } = new[] { new ActionArg(ActionArgSourceType.FromHand), new ActionArg(), };

    public override string Text { get; } = "Trash a card from your hand.\r\nGain a card costing up to $2 more than it.";

    public override CardEnum CardTypeId { get; } = CardEnum.Remodel;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        var trashCardType = playMessage.Args[0];
        var getCardType = playMessage.Args[1];

        player.State.TrashFromHand(game.Kingdom, trashCardType);

        var gottenCard = game.Kingdom.Piles[getCardType].Pop();

        player.State.AddCardsToDiscard(gottenCard);
    }

    public override bool CanAct(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        if (playMessage.Args.Count() < 2)
        {
            return false;
        }

        var trashCardType = playMessage.Args[0];
        var getCardType = playMessage.Args[1];

        if (game.Kingdom.IsPileEmpty(getCardType))
        {
            return false;
        }

        var getCardCost = CardEnumDict.GetCard(getCardType).Cost;
        var trashCardCost = CardEnumDict.GetCard(trashCardType).Cost;

        if (getCardCost
            > trashCardCost + 2)
        {
            return false;
        }

        if (!player.State.HaveInHand(trashCardType))
        {
            return false;
        }
        return true;
    }
}
