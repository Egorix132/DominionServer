using GameModel.Cards;

namespace GameModel
{
    public interface IPlayer
    {
        string Id { get; }

        string Name { get; }

        PlayerState State { get; }

        Task PlayTurn(Game game);

        Task<ClarificationResponseMessage> ClarificatePlayAsync(ClarificationRequestMessage request);

        void GameEnded(GameEndDto gameEndDto);
    }
}
