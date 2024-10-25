using GameModel.Infrastructure.Exceptions;

namespace GameModel.Cards.IndividualCards;

public class HarbingerCard : AbstractActionCard
{
    public override string Name { get; } = "Harbinger";

    public override int Cost { get; } = 3;

    public override string Text { get; } = "+1 Card\r\n+1 Action\r\nLook through your discard pile. You may put a card from it onto your deck.";

    public override CardEnum CardTypeId { get; } = CardEnum.Harbinger;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        var drawedCard = player.State.DrawToHand(1);
        try
        {
            var clarification = await player.ClarificatePlayAsync(
                new ClarificationRequestMessage()
                {
                    PlayedCard = playMessage.PlayedCard,
                    Args = player.State.Discard.Select(c => c.CardTypeId).ToArray()
                });

            if (clarification.Args.Length == 0)
            {
                return;
            }

            var getDiscardedCardType = clarification.Args.FirstOrDefault();

            var getDiscardedCard = player.State.Discard.FirstOrDefault(c => c.CardTypeId == getDiscardedCardType);

            if (getDiscardedCard == null)
            {
                throw new MissingCardsException(getDiscardedCardType);
            }

            player.State.Discard.Remove(getDiscardedCard);
            player.State.MoveOnDeck(getDiscardedCard);
        }
        catch
        {
            player.State.OnDeckFromHand(drawedCard.FirstOrDefault()?.CardTypeId);
            throw;
        }
    }
}
