using GameModel;
using GameModel.Cards;

namespace DominionServer.Services
{
    public class GameService : IGameService
    {
        private List<Game> _games = new();

        public GameService() { 
            
        }

        public Game StartGame(string playerName)
        {
            var newGame = new Game(
                new List<IPlayer>(),
                new Kingdom(
                    new List<CardEnum> {
                        CardEnum.Artisan, CardEnum.Cellar, CardEnum.Market, CardEnum.Merchant, CardEnum.Mine,
                        CardEnum.Moat, CardEnum.Moneylender, CardEnum.Poacher, CardEnum.Remodel, CardEnum.Witch
                    },
                    2)
                );

            return newGame;
        }
    }
}
