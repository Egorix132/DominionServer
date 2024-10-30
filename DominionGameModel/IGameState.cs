namespace GameModel
{
    public interface IGameState
    {
        public Guid Id { get; }

        public int Turn { get; }

        public List<IPlayer> Players { get; }

        public IKingdomState Kingdom { get; }

        public IPlayer CurrentPlayer { get; }

    }
}
