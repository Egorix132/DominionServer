namespace GameModel.Cards
{
    public abstract class AbstractActionCard : AbstractCard, IActionCard
    {
        public bool TryAct(Game game, IPlayer player, PlayCardMessage playMessage)
        {
            if (!CanAct(game, player, playMessage))
            {
                return false;
            }

            player.State.DrawToHand(2);

            return true;
        }

        protected abstract void Act(Game game, IPlayer player, PlayCardMessage playMessage);

        public virtual bool CanAct(Game game, IPlayer player, PlayCardMessage playMessage)
        {
            return true;
        }
    }
}
