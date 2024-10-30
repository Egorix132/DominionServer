namespace GameModel.Cards.IndividualCards;

public class SilverCard : AbstractCard, ITreasureCard
{
    public override string Name { get; } = "Silver";

    public override int Cost { get; } = 3;

    public int Money { get; } = 2;

    public override string Text { get; } = "2";

    public override CardEnum CardTypeId { get; } = CardEnum.Silver;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Treasure };
}
