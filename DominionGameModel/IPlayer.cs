namespace GameModel
{
    public interface IPlayer
    {
        string Id { get; }

        string Name { get; }

        PlayerState State { get; }

        Task PlayTurnAsync(IGameState game);

        void UpdateState(IGameState game);

        Task<ClarificationResponseMessage> ClarifyPlay(ClarificationRequestMessage request);

        void GameEnded(GameEndDto gameEndDto);

        void SendException(Exception e);
    }
}
