using GameModel.Infrastructure.Exceptions;

namespace GameModel.Cards.IndividualCards;

public class PoacherCard : AbstractActionCard
{
    public override string Name { get; } = "Poacher";

    public override int Cost { get; } = 4;

    public override string Text { get; } = "+1 Card\r\n+1 Action\r\n+$1\r\nDiscard a card per empty Supply pile.";

    public override CardEnum CardTypeId { get; } = CardEnum.Poacher;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        var drawedCard = player.State.DrawToHand(1);
        try
        {
            var emptyPilesCount = game.Kingdom.Piles.Where(p => p.Value.IsEmpty()).Count();

            if (emptyPilesCount == 0)
            {
                player.State.ActionsCount++;
                player.State.AdditionalMoney++;

                return;
            }

            var clarification = await player.ClarificatePlayAsync(
                new ClarificationRequestMessage()
                {
                    PlayedCard = playMessage.PlayedCard,
                    Args = player.State.Hand.Select(c => c.CardTypeId).ToArray()
                });

            if (clarification.Args.Length < emptyPilesCount
                && emptyPilesCount <= player.State.Hand.Count)
            {
                throw new BaseDominionException(ExceptionsEnum.MissingArguments);
            }
            if (!player.State.DiscardFromHand(DiscardType.LastToPublic, clarification.Args))
            {
                throw new MissingCardsException(clarification.Args
                    .GroupBy(t => t)
                    .Where(group => player.State.Hand.Where(c => c.CardTypeId == group.FirstOrDefault()).Count() < group.Count())
                    .Select(g => g.FirstOrDefault()).ToArray());
            }

            player.State.ActionsCount++;
            player.State.AdditionalMoney++;
        }
        catch
        {
            player.State.OnDeckFromHand(drawedCard.FirstOrDefault()?.CardTypeId);
            throw;
        }
    }
}
