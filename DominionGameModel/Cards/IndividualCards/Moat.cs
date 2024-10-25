namespace GameModel.Cards.IndividualCards
{
    public class MoatCard : AbstractActionCard
    {
        public override string Name { get; } = "Moat";

        public override int Cost { get; } = 2;

        public override string Text { get; } = "+2 Cards\r\nWhen another player plays an Attack card, you may first reveal this from your hand, to be unaffected by it.";

        public override CardEnum CardTypeId { get; } = CardEnum.Moat;

        public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

        protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
        {
            player.State.DrawToHand(2);
        }
    }
}
