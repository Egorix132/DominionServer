namespace GameModel.Cards
{
    public abstract class AbstractActionCard : AbstractCard, IActionCard
    {
        public virtual ActionArg[] ArgTypes { get; } = Array.Empty<ActionArg>();
        public virtual ActionArg[] ClarifyArgTypes { get; } = Array.Empty<ActionArg>();

        public async Task<bool> TryAct(IGameState game, IPlayer player, PlayCardMessage playMessage)
        {
            if (!CanAct(game, player, playMessage))
            {
                return false;
            }

            await Act(game, player, playMessage);

            return true;
        }

        protected abstract Task Act(IGameState game, IPlayer player, PlayCardMessage playMessage);

        public virtual bool CanAct(IGameState game, IPlayer player, PlayCardMessage playMessage)
        {
            return true;
        }
    }
}
