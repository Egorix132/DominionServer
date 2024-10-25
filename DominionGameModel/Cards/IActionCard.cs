namespace GameModel.Cards
{
    public interface IActionCard : ICard
    {
        Task<bool> TryAct(Game game, IPlayer player, PlayCardMessage playMessage);
        bool CanAct(Game game, IPlayer player, PlayCardMessage playMessage);
    }
}
