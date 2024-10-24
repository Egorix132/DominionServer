namespace GameModel.Cards.IndividualCards
{
    public class DuchyCard : AbstractCard, IVictoryCard
    {
        public override string Name { get; } = "Duchy";

        public override int Cost { get; } = 5;

        public int VictoryPoints { get; } = 3;

        public override string Text { get; } = "3 Victory point";

        public override CardEnum CardTypeId { get; } = CardEnum.Duchy;

        public override List<CardType> Types { get; } = new List<CardType> { CardType.Victory };
    }
}
