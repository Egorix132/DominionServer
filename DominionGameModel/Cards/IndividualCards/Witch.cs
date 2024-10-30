namespace GameModel.Cards.IndividualCards;

public class WitchCard : AbstractActionCard
{
    public override string Name { get; } = "Witch";

    public override int Cost { get; } = 5;

    public override string Text { get; } = "+2 Cards\r\nEach other player gains a Curse.";

    public override CardEnum CardTypeId { get; } = CardEnum.Witch;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action, CardType.Attack };

    protected override async Task Act(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        player.State.DrawToHand(2);

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

            if(gamePlayer.State.Hand.Any(c => c.CardTypeId == CardEnum.Moat))
            {
                continue;
            }

            var gottenCard = game.Kingdom.Piles[CardEnum.Curse].Pop();

            gamePlayer.State.AddCardsToDiscard(gottenCard);
        }
    }
}
