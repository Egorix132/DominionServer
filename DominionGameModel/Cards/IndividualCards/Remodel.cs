using GameModel.Infrastructure;
using GameModel.Infrastructure.Attributes;
using GameModel.Infrastructure.Exceptions;

namespace GameModel.Cards.IndividualCards;

public class RemodelCard : AbstractActionCard
{
    public override string Name { get; } = "Remodel";

    public override int Cost { get; } = 4;

    public override string Text { get; } = "Trash a card from your hand.\r\nGain a card costing up to $2 more than it.";

    public override CardEnum CardTypeId { get; } = CardEnum.Remodel;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        var trashCardType = playMessage.Args[0];
        var getCardType = playMessage.Args[1];

        player.State.TrashFromHand(game.Kingdom, trashCardType);

        var gottenCard = game.Kingdom.Piles[getCardType].Pop();

        player.State.AddCardsToDiscard(gottenCard);
    }

    public override bool CanAct(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        if (playMessage.Args.Count() < 2)
        {
            throw new BaseDominionException(ExceptionsEnum.MissingArguments);
        }

        var trashCardType = playMessage.Args[0];
        var getCardType = playMessage.Args[1];

        if (game.Kingdom.IsPileEmpty(getCardType))
        {
            throw new PileIsEmptyException(getCardType);
        }

        var getCardCost = getCardType.GetAttribute<CardCostAttribute>()!.CardCost;
        var trashCardCost = trashCardType.GetAttribute<CardCostAttribute>()!.CardCost;

        if (getCardCost
            > trashCardCost + 2)
        {
            throw new NotEnoughMoneyException(getCardCost, trashCardCost + 2);
        }

        if (!player.State.HaveInHand(trashCardType))
        {
            throw new MissingCardsException(trashCardType);
        }
        return true;
    }
}
