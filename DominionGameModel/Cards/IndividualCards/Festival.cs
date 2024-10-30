namespace GameModel.Cards.IndividualCards;

public class FestivalCard : AbstractActionCard
{
    public override string Name { get; } = "Festival";

    public override int Cost { get; } = 5;

    public override string Text { get; } = "+2 Actions\r\n+1 Buy\r\n+$2";

    public override CardEnum CardTypeId { get; } = CardEnum.Festival;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        player.State.ActionsCount += 2;
        player.State.BuyCount++;
        player.State.AdditionalMoney += 2;
    }
}
