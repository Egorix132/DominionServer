using GameModel.Infrastructure;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace GameModel
{
    public class Game
    {
        public Guid Id { get; set; }

        public List<IPlayer> Players { get; set; } = new();

        public Kingdom Kingdom { get; set; }

        public IPlayer CurrentPlayer { get; private set; }

        public int Turn { get; private set; }

        public Game(List<IPlayer> players, Kingdom kingdom)
        {
            Id = Guid.NewGuid();
            Kingdom = kingdom;


            foreach (var player in players)
            {
                Players.Add(player);
                player.State.SetDefaultState();
            }

            Players.Shuffle();
        }

        public async Task StartGame()
        {
            Turn = 0;
            int playerTurnCounter = 0;
            bool isGameOver = false;
            while (!isGameOver)
            {
                if (playerTurnCounter % Players.Count == 0)
                {
                    Turn++;
                }
                CurrentPlayer = Players[playerTurnCounter % Players.Count];
                await CurrentPlayer.PlayTurn(this);

                CurrentPlayer.State.EndTurn();

                isGameOver = Kingdom.IsGameOver();
                playerTurnCounter++;
            }
        }

        public void Dispose()
        {
            foreach (var player in Players)
            {
                player.GameStopped();
            }
        }
    }
}
