using GameModel.Infrastructure.Exceptions;

namespace GameModel.Cards.IndividualCards
{
    public class PoacherCard : AbstractActionCard
    {
        public override string Name { get; } = "Poacher";

        public override int Cost { get; } = 4;

        public override string Text { get; } = "+1 Card\r\n+1 Action\r\n+$1\r\nDiscard a card per empty Supply pile.";

        public override CardEnum CardTypeId { get; } = CardEnum.Poacher;

        public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

        protected override void Act(Game game, IPlayer player, PlayCardMessage playMessage)
        {
            player.State.DrawToHand(1);
            player.State.ActionsCount++;
            player.State.AdditionalMoney++;
        }

        public override bool CanAct(Game game, IPlayer player, PlayCardMessage playMessage)
        {
            var emptyPilesCount = game.Kingdom.Piles.Where(p => p.Value.IsEmpty()).Count();
            if (playMessage.Args.Count() < emptyPilesCount)
            {
                throw new BaseDominionException(ExceptionsEnum.MissingArguments);
            }

            if (!player.State.HaveInHand(playMessage.Args))
            {
                throw new MissedCardsInHandException(playMessage.Args
                    .GroupBy(t => t)
                    .Where(group => player.State.Hand.Where(c => c.CardTypeId == group.FirstOrDefault()).Count() < group.Count())
                    .Select(g => g.FirstOrDefault()).ToArray());
            }
            return true;
        }
    }
}
