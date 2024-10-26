namespace GameModel.Cards.IndividualCards;

public class MilitiaCard : AbstractActionCard
{
    public override string Name { get; } = "Militia";

    public override int Cost { get; } = 4;

    public override string Text { get; } = "+$2\r\nEach other player discards down to 3 cards in hand.";

    public override CardEnum CardTypeId { get; } = CardEnum.Militia;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action, CardType.Attack };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        player.State.AdditionalMoney += 2;

        foreach (var gamePlayer in game.Players)
        {
            if (gamePlayer.Id == player.Id)
            {
                continue;
            }

            if (gamePlayer.State.Hand.Any(c => c.CardTypeId == CardEnum.Moat))
            {
                continue;
            }
            
            var clarification = await player.ClarifyPlay(
                new ClarificationRequestMessage()
                {
                    PlayedCard = playMessage.PlayedCard,
                    Args = player.State.Hand.Select(c => c.CardTypeId).ToArray()
                });

            var handSize = player.State.Hand.Count;
            IEnumerable<CardEnum> cardsToDiscard;
            try
            {
                cardsToDiscard = clarification.Args.Take(handSize - 3);
            }
            catch
            {
                cardsToDiscard = player.State.Hand.Take(handSize - 3).Select(c => c.CardTypeId);
            }

            if (cardsToDiscard.Count() < handSize - 3
                || !player.State.HaveInHand(cardsToDiscard))
            {
                cardsToDiscard = player.State.Hand.Take(handSize - 3).Select(c => c.CardTypeId).ToList();
                return;
            }

            gamePlayer.State.DiscardFromHand(DiscardType.LastToPublic, cardsToDiscard);
        }
    }
}
