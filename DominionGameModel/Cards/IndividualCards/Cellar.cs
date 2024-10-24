using GameModel.Infrastructure.Exceptions;

namespace GameModel.Cards.IndividualCards
{
    public class CellarCard : AbstractActionCard
    {
        public override string Name { get; } = "Cellar";

        public override int Cost { get; } = 2;

        public override string Text { get; } = "+1 Action\r\nDiscard any number of cards, then draw that many.";

        public override CardEnum CardTypeId { get; } = CardEnum.Cellar;

        public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

        protected override void Act(Game game, IPlayer player, PlayCardMessage playMessage)
        {
            player.State.DiscardFromHand(DiscardType.LastToPublic, playMessage.Args);

            player.State.DrawToHand(playMessage.Args.Length);
        }

        public override bool CanAct(Game game, IPlayer player, PlayCardMessage playMessage)
        {
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
