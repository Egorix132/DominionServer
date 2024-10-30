using GameModel.Infrastructure;
using GameModel.Infrastructure.Exceptions;

namespace GameModel.Cards.IndividualCards;

public class VassalCard : AbstractActionCard
{
    public override string Name { get; } = "Vassal";

    public override int Cost { get; } = 3;

    public override string Text { get; } = "+$2\r\nDiscard the top card of your deck. If it is an Action card, you may play it.";

    public override CardEnum CardTypeId { get; } = CardEnum.Vassal;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        player.State.AdditionalMoney += 2;
        
        var topDeskCard = player.State.Deck.Pop();

        if (topDeskCard == null)
        {
            return;
        }

        if (topDeskCard.Types.Contains(CardType.Action))
        {
            var clarification = await player.ClarifyPlay(
                new ClarificationRequestMessage()
                {
                    PlayedCard = topDeskCard.CardTypeId,
                    PlayedBy = playMessage.PlayedCard,
                    Args = player.State.Hand.Select(c => c.CardTypeId).ToArray()
                });

            if (!clarification.Args.Any())
            {
                player.State.MoveCardsToDiscard(topDeskCard);
                return;
            }

            player.State.Hand.Add(topDeskCard);
            player.State.ActionsCount++;
            try
            {
                await player.State.PlayCard(
                    game,
                    player,
                    new PlayCardMessage(topDeskCard.CardTypeId, playMessage.Args.Skip(1).ToArray()));
            }
            catch (Exception e)
            {
                player.State.ActionsCount--;
                player.State.OnDeckFromHand(topDeskCard.CardTypeId);
                player.SendException(e);
            }
        }
        else
        {
            player.State.MoveCardsToDiscard(topDeskCard);
        }
    }
}
