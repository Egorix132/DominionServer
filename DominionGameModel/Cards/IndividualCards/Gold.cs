namespace GameModel.Cards.IndividualCards;

public class GoldCard : AbstractCard, ITreasureCard
{
    public override string Name { get; } = "Gold";

    public override int Cost { get; } = 6;

    public int Money { get; } = 3;

    public override string Text { get; } = "3";

    public override CardEnum CardTypeId { get; } = CardEnum.Gold;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Treasure };
}
