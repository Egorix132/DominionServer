using GameModel.Infrastructure.Exceptions;

namespace GameModel.Cards.IndividualCards;

public class CellarCard : AbstractActionCard
{
    public override string Name { get; } = "Cellar";

    public override int Cost { get; } = 2;

    public override int ArgsCount { get; } = 4;

    public override string Text { get; } = "+1 Action\r\nDiscard any number of cards, then draw that many.";

    public override CardEnum CardTypeId { get; } = CardEnum.Cellar;

    public override List<CardType> Types { get; } = new List<CardType> { CardType.Action };

    protected override async Task Act(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        player.State.DiscardFromHand(DiscardType.LastToPublic, playMessage.Args);

        player.State.DrawToHand(playMessage.Args.Length);

        player.State.ActionsCount++;
    }

    public override bool CanAct(IGameState game, IPlayer player, PlayCardMessage playMessage)
    {
        if (!player.State.HaveInHand(playMessage.Args))
        {
            return false;
        }

        return true;
    }
}
