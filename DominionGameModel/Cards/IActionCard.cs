namespace GameModel.Cards
{
    public interface IActionCard : ICard
    {
        public ActionArg[] ArgTypes { get; }
        public ActionArg[] ClarifyArgTypes { get; }

        Task<bool> TryAct(IGameState game, IPlayer player, PlayCardMessage playMessage);

        bool CanAct(IGameState game, IPlayer player, PlayCardMessage playMessage);
    }
}
