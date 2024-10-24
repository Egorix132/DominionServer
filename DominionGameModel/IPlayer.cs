using System.Text.Json.Serialization;

namespace GameModel
{
    public interface IPlayer
    {
        string Id { get; }

        string Name { get; }

        PlayerState State { get; }

        Task PlayTurn(Game game);
    }
}
