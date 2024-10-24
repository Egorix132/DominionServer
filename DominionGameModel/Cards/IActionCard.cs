namespace GameModel.Cards
{
    public interface IActionCard : ICard
    {
        bool TryAct(Game game, IPlayer player, PlayCardMessage playMessage);
        bool CanAct(Game game, IPlayer player, PlayCardMessage playMessage);
    }
}
