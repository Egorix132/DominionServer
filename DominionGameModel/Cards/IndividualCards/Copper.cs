namespace GameModel.Cards.IndividualCards
{
    public class CopperCard : AbstractCard, ITreasureCard
    {
        public override string Name { get; } = "Copper";

        public override int Cost { get; } = 0;

        public int Money { get; } = 1;

        public override string Text { get; } = "1";

        public override CardEnum CardTypeId { get; } = CardEnum.Copper;

        public override List<CardType> Types { get; } = new List<CardType> { CardType.Treasure };
    }
}
