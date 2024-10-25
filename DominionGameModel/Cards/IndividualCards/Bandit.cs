using GameModel.Infrastructure;

namespace GameModel.Cards.IndividualCards;

public class BanditCard : AbstractActionCard
{
    public override string Name { get; } = "Bandit";

    public override int Cost { get; } = 5;

    public override string Text { get; } = "Gain a Gold. Each other player reveals the top 2 cards of their deck, trashes a revealed Treasure other than Copper, and discards the rest.";

    public override CardEnum CardTypeId { get; } = CardEnum.Bandit;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action, CardType.Attack };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        if (!game.Kingdom.IsPileEmpty(CardEnum.Gold))
        {
            var gottenCard = game.Kingdom.Piles[CardEnum.Gold].Pop();

            player.State.AddCardsToDiscard(gottenCard);
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

            var firstCard = gamePlayer.State.Deck.Pop();
            var secondCard = gamePlayer.State.Deck.Pop();

            bool firstCardIsNotCopperTreashure = (firstCard?.Types.Contains(CardType.Treasure) ?? false)
                && (firstCard?.CardTypeId != CardEnum.Copper);
            bool secondCardIsNotCopperTreashure = (firstCard?.Types.Contains(CardType.Treasure) ?? false)
                && (firstCard?.CardTypeId != CardEnum.Copper);

            var lookingCards = new List<ICard> { firstCard, secondCard }.Where(c => c != null);
            ICard? cardToTrash = lookingCards.FirstOrDefault();

            if (firstCardIsNotCopperTreashure && secondCardIsNotCopperTreashure)
            {
                var clarification = await player.ClarificatePlayAsync(
                    new ClarificationRequestMessage()
                    {
                        PlayedCard = playMessage.PlayedCard,
                        Args = lookingCards.Select(c => c.CardTypeId).ToArray()
                    });

                if (firstCard!.CardTypeId == clarification.Args.FirstOrDefault())
                {
                    cardToTrash = firstCard!;
                }
                else if(secondCard!.CardTypeId == clarification.Args.FirstOrDefault())
                {
                    cardToTrash = secondCard!;
                }
            }
            else if (firstCardIsNotCopperTreashure)
            {
                cardToTrash = firstCard!;
            }
            else if (secondCardIsNotCopperTreashure)
            {
                cardToTrash = secondCard!;
            }
            if(cardToTrash != null)
            {
                game.Kingdom.ToTrash(secondCard);
            }

            gamePlayer.State.MoveCardsToDiscard(lookingCards.Where(c => c != cardToTrash));
        }
    }
}
