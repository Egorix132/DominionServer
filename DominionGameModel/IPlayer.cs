using GameModel.Cards;

namespace GameModel
{
    public interface IPlayer
    {
        string Id { get; }

        string Name { get; }

        PlayerState State { get; }

        Task PlayTurnAsync(Game game);

        Task<ClarificationResponseMessage> ClarifyPlay(ClarificationRequestMessage request);

        void GameEnded(GameEndDto gameEndDto);

        void SendException(Exception e);
    }
}
