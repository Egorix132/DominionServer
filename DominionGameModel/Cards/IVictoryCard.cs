namespace GameModel.Cards
{
    public interface IVictoryCard : ICard
    {
        int GetVictoryPoints(PlayerState player);
    }
}
