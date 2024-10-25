namespace GameModel.Cards.IndividualCards;

public class VillageCard : AbstractActionCard
{
    public override string Name { get; } = "Village";

    public override int Cost { get; } = 3;

    public override string Text { get; } = "+1 Card\r\n+2 Actions";

    public override CardEnum CardTypeId { get; } = CardEnum.Village;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        player.State.DrawToHand(1);
        player.State.ActionsCount += 2;
    }
}
