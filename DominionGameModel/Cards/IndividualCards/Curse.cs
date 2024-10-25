namespace GameModel.Cards.IndividualCards;

public class CurseCard : AbstractCard, IVictoryCard
{
    public override string Name { get; } = "Curse";

    public override int Cost { get; } = 0;

    public int VictoryPoints { get; set; } = -1;

    public override string Text { get; } = "-1 Victory point";

    public override CardEnum CardTypeId { get; } = CardEnum.Curse;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Curse };

    public int GetVictoryPoints(PlayerState _)
    {
        return VictoryPoints;
    }
}
