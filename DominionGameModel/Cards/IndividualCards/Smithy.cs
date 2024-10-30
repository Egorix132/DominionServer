namespace GameModel.Cards.IndividualCards;

public class SmithyCard : AbstractActionCard
{
    public override string Name { get; } = "Smithy";

    public override int Cost { get; } = 5;

    public override string Text { get; } = "+3 Cards";

    public override CardEnum CardTypeId { get; } = CardEnum.Smithy;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        player.State.DrawToHand(3);
    }
}
