using GameModel.Infrastructure.Exceptions;

namespace GameModel.Cards.IndividualCards;

public class ThroneRoomCard : AbstractActionCard
{
    public override string Name { get; } = "ThroneRoom";

    public override int Cost { get; } = 4;

    public override string Text { get; } = "You may play an Action card from your hand twice.";

    public override CardEnum CardTypeId { get; } = CardEnum.ThroneRoom;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        var doubledCardType = playMessage.Args.FirstOrDefault();
        var doubledCard = player.State.Hand.FirstOrDefault(c => c.CardTypeId == doubledCardType);

        await player.State.PlayCard(game, player, new PlayCardMessage { PlayedCard = doubledCardType, Args = playMessage.Args.Skip(1).ToArray() });

        var clarification = await player.ClarificatePlayAsync(
            new ClarificationRequestMessage()
            {
                PlayedCard = doubledCardType,
                PlayedBy = playMessage.PlayedCard,
                Args = player.State.Hand.Select(c => c.CardTypeId).ToArray()
            });

        player.State.OnPlay.Remove(doubledCard!);
        player.State.Hand.Add(doubledCard!);

        await player.State.PlayCard(
            game, 
            player, 
            new PlayCardMessage { PlayedCard = doubledCardType, Args = clarification.Args });
    }

    public override bool CanAct(Game game, IPlayer player, PlayCardMessage playMessage)
    {
        var doubledCardType = playMessage.Args.FirstOrDefault();
        if (playMessage.Args.Length < 1 || !player.State.HaveInHand(doubledCardType))
        {
            throw new MissingCardsException(doubledCardType);
        }
        return true;
    }
}
