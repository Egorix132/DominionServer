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
        var declinedCards = new List<ICard>();
        var handSize = player.State.Hand.Count;
        while (handSize < 7)
        {
            var drawedCard = player.State.Draw(1).FirstOrDefault();

            if(drawedCard == null)
            {
                return;
            }

            if (!drawedCard.Types.Contains(CardType.Action))
            {
                player.State.Hand.Add(drawedCard);
                handSize = player.State.Hand.Count;
                continue;
            }

            var clarification = await player.ClarifyPlay(
                new ClarificationRequestMessage()
                {
                    PlayedCard = playMessage.PlayedCard,
                    Args = new CardEnum[] { drawedCard.CardTypeId }
                });

            var discardedCards = clarification.Args.Take(1).ToList();

            if (!discardedCards.Any() || drawedCard.CardTypeId != discardedCards.First())
            {
                declinedCards.Add(drawedCard);
                continue;
            }

            player.State.Hand.Add(drawedCard);
            handSize = player.State.Hand.Count;
        }

        player.State.MoveCardsToDiscard(declinedCards);
    }
}
