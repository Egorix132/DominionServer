using GameModel.Infrastructure;
using GameModel.Infrastructure.Attributes;
using GameModel.Infrastructure.Exceptions;

namespace GameModel.Cards.IndividualCards;

public class LibraryCard : AbstractActionCard
{
    public override string Name { get; } = "Library";

    public override int Cost { get; } = 5;

    public override string Text { get; } = "Draw until you have 7 cards in hand, skipping any Action cards you choose to; set those aside, discarding them afterwards.";

    public override CardEnum CardTypeId { get; } = CardEnum.Library;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        var allDiscardedCards = new List<ICard>();
        var handSize = player.State.Hand.Count;
        while (handSize < 7)
        {
            var drawedCards = player.State.Draw(7 - handSize);
            var cardsToDraw = new List<ICard>(drawedCards);

            var clarification = await player.ClarificatePlayAsync(
                new ClarificationRequestMessage()
                {
                    PlayedCard = playMessage.PlayedCard,
                    Args = drawedCards.Select(c => c.CardTypeId).ToArray()
                });

            var discardedCards = clarification.Args.Take(7 - handSize).ToList();

            if (discardedCards.Any(c => c == null) 
                || discardedCards.GroupBy(t => t)
                    .Any(group => drawedCards.Where(c => c.CardTypeId == group.Key).Count() < group.Count())
                || discardedCards.Any(t => !t!.GetAttribute<CardTypesAttribute>()!.CardTypes.Contains(CardType.Action)))
            {
                player.State.Hand.AddRange(cardsToDraw);
                return;
            }

            foreach (var cardType in discardedCards)
            {
                var cardToDiscard = drawedCards.First(c => c.CardTypeId == cardType);
                allDiscardedCards.Add(cardToDiscard);
                cardsToDraw.Remove(cardToDiscard);
            }
            player.State.Hand.AddRange(cardsToDraw);
            handSize = player.State.Hand.Count;
        }

        player.State.MoveCardsToDiscard(allDiscardedCards);
    }
}
