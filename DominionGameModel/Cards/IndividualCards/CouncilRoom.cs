namespace GameModel.Cards.IndividualCards;

public class CouncilRoomCard : AbstractActionCard
{
    public override string Name { get; } = "Council Room";

    public override int Cost { get; } = 5;

    public override string Text { get; } = "+4 Cards\r\n+1 Buy\r\nEach other player draws a card.";

    public override CardEnum CardTypeId { get; } = CardEnum.CouncilRoom;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        player.State.DrawToHand(4);
        player.State.BuyCount++;

        if (game.Kingdom.IsPileEmpty(CardEnum.Curse))
        {
            return;
        }

        foreach (var gamePlayer in game.Players)
        {
            if(gamePlayer.Id == player.Id)
            {
                continue;
            }

            gamePlayer.State.DrawToHand(1);
        }
    }
}
