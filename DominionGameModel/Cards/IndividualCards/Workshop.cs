using GameModel.Infrastructure.Attributes;
using GameModel.Infrastructure.Exceptions;
using GameModel.Infrastructure;

namespace GameModel.Cards.IndividualCards;

public class WorkshopCard : AbstractActionCard
{
    public override string Name { get; } = "Workshop";

    public override int Cost { get; } = 4;

    public override string Text { get; } = "Gain a card costing up to $4.";

    public override CardEnum CardTypeId { get; } = CardEnum.Workshop;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        var getCardType = playMessage.Args[0];

        var gottenCard = game.Kingdom.Piles[getCardType].Pop();

        player.State.AddCardsToDiscard(gottenCard);
    }

    public override bool CanAct(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        if (playMessage.Args.Length < 1)
        {
            throw new BaseDominionException(ExceptionsEnum.MissingArguments);
        }

        var getCardType = playMessage.Args[0];

        if (game.Kingdom.IsPileEmpty(getCardType))
        {
            throw new PileIsEmptyException(getCardType);
        }

        var getCardCost = getCardType.GetAttribute<CardCostAttribute>()!.CardCost;
        if (getCardCost > 4)
        {
            throw new NotEnoughMoneyException(getCardCost, 5);
        }

        return true;
    }
}
