namespace GameModel.Cards.IndividualCards;

public class MerchantCard : AbstractActionCard
{
    public override string Name { get; } = "Merchant";

    public override int Cost { get; } = 3;

    public override string Text { get; } = "+1 Card\r\n+1 Action\r\nThe first time you play a Silver this turn, +$1.";

    public override CardEnum CardTypeId { get; } = CardEnum.Merchant;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        player.State.DrawToHand(1);
        player.State.ActionsCount++;
    }
}
