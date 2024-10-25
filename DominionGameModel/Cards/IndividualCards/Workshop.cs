namespace GameModel.Cards.IndividualCards
{
    public class WorkshopCard : AbstractActionCard
    {
        public override string Name { get; } = "Workshop";

        public override int Cost { get; } = 4;

        public override string Text { get; } = "Gain a card costing up to $4.";

        public override CardEnum CardTypeId { get; } = CardEnum.Workshop;

        public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

        public override bool CanAct(Game game, IPlayer player, PlayCardMessage playMessage)
        {
            throw new NotImplementedException();
        }

        protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
        {
        }
    }
}
