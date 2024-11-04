using EvoClient.Utils;

namespace GameModel.Cards.IndividualCards;

public class MineCard : AbstractActionCard
{
    public override string Name { get; } = "Mine";

    public override int Cost { get; } = 5;

    public override ActionArg[] ArgTypes { get; } = new[] { new ActionArg(ActionArgSourceType.FromHand), new ActionArg(), };

    public override string Text { get; } = "You may trash a Treasure from your hand. Gain a Treasure to your hand costing up to $3 more than it.";

    public override CardEnum CardTypeId { get; } = CardEnum.Mine;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        var trashCardType = playMessage.Args[0];
        var getCardType = playMessage.Args[1];

        player.State.TrashFromHand(game.Kingdom, trashCardType);

        var gottenCard = game.Kingdom.Piles[getCardType].Pop();

        player.State.AddCardsToHand(gottenCard);
    }

    public override bool CanAct(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        if (playMessage.Args.Count < 2)
        {
            return false;
        }

        var trashCardType = playMessage.Args[0];
        var getCardType = playMessage.Args[1];

        if (game.Kingdom.IsPileEmpty(getCardType))
        {
            return false;
        }

        if (CardEnumDict.GetCard(getCardType).Types.All(t => t != CardType.Treasure)
            || CardEnumDict.GetCard(trashCardType).Types.All(t => t != CardType.Treasure)
            || (CardEnumDict.GetCard(getCardType).Cost
            > CardEnumDict.GetCard(trashCardType).Cost + 3))
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
