namespace GameModel.Cards.IndividualCards;

public class ProvinceCard : AbstractCard, IVictoryCard
{
    public override string Name { get; } = "Province";

    public override int Cost { get; } = 8;

    public int VictoryPoints { get; } = 6;

    public override string Text { get; } = "6 Victory point";

    public override CardEnum CardTypeId { get; } = CardEnum.Province;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Victory };

    public int GetVictoryPoints(PlayerState _)
    {
        return VictoryPoints;
    }
}
