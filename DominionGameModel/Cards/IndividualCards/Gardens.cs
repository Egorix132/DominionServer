namespace GameModel.Cards.IndividualCards;

public class GardensCard : AbstractCard, IVictoryCard
{
    public override string Name { get; } = "Gardens";

    public override int Cost { get; } = 4;

    public override string Text { get; } = "Worth 1 VP per 10 cards you have (round down).";

    public override CardEnum CardTypeId { get; } = CardEnum.Gardens;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Victory };

    public int GetVictoryPoints(PlayerState playerState)
    {
        return playerState.AllCards.Count / 10;
    }
}
