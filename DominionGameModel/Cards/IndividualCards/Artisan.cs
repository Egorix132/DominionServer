using GameModel.Infrastructure.Attributes;
using GameModel.Infrastructure;
using GameModel.Infrastructure.Exceptions;

namespace GameModel.Cards.IndividualCards
{
    public class ArtisanCard : AbstractActionCard
    {
        public override string Name { get; } = "Artisan";

        public override int Cost { get; } = 6;

        public override string Text { get; } = "Gain a card to your hand costing up to $5.\r\nPut a card from your hand onto your deck.";

        public override CardEnum CardTypeId { get; } = CardEnum.Artisan;

        public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

        protected override void Act(Game game, IPlayer player, PlayCardMessage playMessage)
        {
            var getCardType = playMessage.Args[0];
            var discardCardType = playMessage.Args[1];

            if(getCardType != discardCardType)
            {
                player.State.OnDeckFromHand(discardCardType);
            }

            var gottenCard = game.Kingdom.Piles[getCardType].Pop();

            player.State.AddCardsToHand(gottenCard);
            player.State.OnDeckFromHand(discardCardType);
        }

        public override bool CanAct(Game game, IPlayer player, PlayCardMessage playMessage)
        { 
            if (playMessage.Args.Length < 2)
            {
                throw new BaseDominionException(ExceptionsEnum.MissingArguments);
            }

            var getCardType = playMessage.Args[0];
            var discardCardType = playMessage.Args[1];

            if (game.Kingdom.IsPileEmpty(getCardType))
            {
                throw new PileIsEmptyException(getCardType);
            }

            var getCardCost = getCardType.GetAttribute<CardCostAttribute>()!.CardCost;
            if (getCardCost > 5)
            {
                throw new NotEnoughMoneyException(getCardCost, 5);
            }

            if (getCardType != discardCardType)
            {
                if (!player.State.HaveInHand(discardCardType))
                {
                    throw new MissedCardsInHandException(discardCardType);
                }
            }

            return true;
        }
    }
}
