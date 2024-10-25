namespace GameModel.Cards.IndividualCards;

public class LaboratoryCard : AbstractActionCard
{
    public override string Name { get; } = "Laboratory";

    public override int Cost { get; } = 5;

    public override string Text { get; } = "+2 Cards\r\n+1 Action";

    public override CardEnum CardTypeId { get; } = CardEnum.Laboratory;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        player.State.DrawToHand(2);
        player.State.ActionsCount += 1;
    }
}
