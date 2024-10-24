namespace GameModel.Cards.IndividualCards
{
    public class EstateCard : AbstractCard, IVictoryCard
    {
        public override string Name { get; } = "Estate";

        public override int Cost { get; } = 2;

        public int VictoryPoints { get; } = 1;

        public override string Text { get; } = "1 Victory point";

        public override CardEnum CardTypeId { get; } = CardEnum.Estate;

        public override List<CardType> Types { get; } = new List<CardType> { CardType.Victory };
    }
}
