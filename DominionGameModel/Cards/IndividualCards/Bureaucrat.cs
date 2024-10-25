namespace GameModel.Cards.IndividualCards;

public class BureaucratCard : AbstractActionCard
{
    public override string Name { get; } = "Bureaucrat";

    public override int Cost { get; } = 4;

    public override string Text { get; } = "Gain a Silver onto your deck. Each other player reveals a Victory card from their hand and puts it onto their deck (or reveals a hand with no Victory cards).";

    public override CardEnum CardTypeId { get; } = CardEnum.Bureaucrat;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action, CardType.Attack };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        if (!game.Kingdom.IsPileEmpty(CardEnum.Silver))
        {
            var gottenCard = game.Kingdom.Piles[CardEnum.Silver].Pop();
            player.State.AddCardsOntoDeck(gottenCard);
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


            var victoryCards = gamePlayer.State.Hand.Where(c => c.Types.Contains(CardType.Victory));
            ICard? discardCard = victoryCards.FirstOrDefault();

            if (victoryCards.Count() > 1)
            {
                var clarification = await player.ClarificatePlayAsync(
                    new ClarificationRequestMessage()
                    {
                        PlayedCard = playMessage.PlayedCard,
                        Args = victoryCards.Select(c => c.CardTypeId).ToArray()
                    });

                var selectedCard = victoryCards.FirstOrDefault(c => c.CardTypeId == clarification.Args.FirstOrDefault());
                if (selectedCard != null)
                {
                    discardCard = selectedCard;
                }
            }
            if(discardCard != null)
            {
                gamePlayer.State.OnDeckFromHand(discardCard.CardTypeId);
            }
        }
    }
}
