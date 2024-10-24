using GameModel.Infrastructure;
using System.Text.Json.Serialization;

namespace GameModel
{
    public class Game
    {
        public Guid Id { get; set; }

        public List<IPlayer> Players { get; set; } = new();

        public Kingdom Kingdom { get; set; }

        public IPlayer CurrentPlayer { get; private set; }

        public Game(List<IPlayer> players, Kingdom kingdom)
        {
            Id = Guid.NewGuid();
            Kingdom = kingdom;


            foreach (var player in players)
            {
                Players.Add(player);
            }

            Players.Shuffle();
        }

        public async Task StartGame()
        {
            int turn = 0;
            int playerTurnCounter = 0;
            bool isGameOver = false;
            while (!isGameOver)
            {
                if (playerTurnCounter % Players.Count == 0)
                {
                    turn++;
                }
                CurrentPlayer = Players[playerTurnCounter % Players.Count];
                await CurrentPlayer.PlayTurn(this);

                CurrentPlayer.State.EndTurn();

                isGameOver = Kingdom.IsGameOver();
                playerTurnCounter++;
            }
        }
    }
}
