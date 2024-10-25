using GameModel.Infrastructure;
using GameModel.Infrastructure.Exceptions;

namespace GameModel.Cards.IndividualCards;

public class SentryCard : AbstractActionCard
{
    public override string Name { get; } = "Sentry";

    public override int Cost { get; } = 5;

    public override string Text { get; } = "+1 Card\r\n+1 Action\r\nLook at the top 2 cards of your deck. Trash and/or discard any number of them. Put the rest back on top in any order.";

    public override CardEnum CardTypeId { get; } = CardEnum.Sentry;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        var drawedCard = player.State.DrawToHand(1).FirstOrDefault();
        
        var firstCard = player.State.Deck.Pop();
        var secondCard = player.State.Deck.Pop();
        try
        {
            var lookingCards = new List<ICard> { firstCard, secondCard }.Where(c => c != null).ToList();

            if(lookingCards.Count() == 0)
            {
                return;
            }

            var clarification = await player.ClarificatePlayAsync(
                new ClarificationRequestMessage()
                {
                    PlayedCard = playMessage.PlayedCard,
                    Args = lookingCards.Select(c => c.CardTypeId).ToArray()
                });

            var cardsToTrash = clarification.Args;
            var cardsToDiscard = clarification.SecondArgs;
            var cardsToDeck = clarification.ThirdArgs;

            foreach (var lookingCard in lookingCards)
            {
                if (cardsToTrash.Contains(lookingCard.CardTypeId))
                {
                    game.Kingdom.ToTrash(lookingCard);
                }
                else if (cardsToDiscard.Contains(lookingCard.CardTypeId))
                {
                    player.State.MoveCardsToDiscard(lookingCard);
                }
                else if (cardsToDeck.Contains(lookingCard.CardTypeId))
                {
                    player.State.MoveOnDeck(lookingCard);
                }
                else
                {
                    throw new MissingCardsException(lookingCard.CardTypeId);
                }
            }

            player.State.ActionsCount++;
        }
        catch
        {
            player.State.MoveOnDeck(secondCard);
            player.State.MoveOnDeck(firstCard);
            player.State.MoveOnDeck(drawedCard);
            throw;
        }
    }
}
