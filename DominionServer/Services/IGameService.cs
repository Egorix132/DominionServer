using GameModel;

namespace DominionServer.Services
{
    public interface IGameService
    {
        public Game StartGame(string playerName);
    }
}
