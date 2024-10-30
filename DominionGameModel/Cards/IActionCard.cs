namespace GameModel.Cards
{
    public interface IActionCard : ICard
    {
        public int ArgsCount { get; }

        Task<bool> TryAct(IGameState game, IPlayer player, PlayCardMessage playMessage);

        bool CanAct(IGameState game, IPlayer player, PlayCardMessage playMessage);
    }
}
