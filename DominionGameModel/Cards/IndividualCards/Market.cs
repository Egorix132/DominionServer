namespace GameModel.Cards.IndividualCards;

public class MarketCard : AbstractActionCard
{
    public override string Name { get; } = "Market";

    public override int Cost { get; } = 5;

    public override string Text { get; } = "+1 Card\r\n+1 Action\r\n+1 Buy\r\n+$1";

    public override CardEnum CardTypeId { get; } = CardEnum.Market;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        player.State.DrawToHand(1);
        player.State.ActionsCount++;
        player.State.BuyCount++;
        player.State.AdditionalMoney++;
    }
}
