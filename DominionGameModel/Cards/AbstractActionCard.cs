namespace GameModel.Cards
{
    public abstract class AbstractActionCard : AbstractCard, IActionCard
    {
        public async Task<bool> TryAct(Game game, IPlayer player, PlayCardMessage playMessage)
        {
            if (!CanAct(game, player, playMessage))
            {
                return false;
            }

            await Act(game, player, playMessage);

            return true;
        }

        protected abstract Task Act(Game game, IPlayer player, PlayCardMessage playMessage);

        public virtual bool CanAct(Game game, IPlayer player, PlayCardMessage playMessage)
        {
            return true;
        }
    }
}
