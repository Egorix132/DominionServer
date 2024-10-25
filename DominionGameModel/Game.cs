using GameModel.Infrastructure;

namespace GameModel
{
    public class Game
    {
        public Guid Id { get; set; }

        public List<IPlayer> Players { get; set; } = new();

        public Kingdom Kingdom { get; set; }

        public IPlayer CurrentPlayer { get; private set; }

        public int Turn { get; private set; }
        public List<LogEntry> Logs { get; private set; } = new();

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
            GameEndType? gameEndType = null;
            while (gameEndType == null)
            {
                if (playerTurnCounter % Players.Count == 0)
                {
                    Turn++;
                }
                CurrentPlayer = Players[playerTurnCounter % Players.Count];
                await CurrentPlayer.PlayTurn(this);

                CurrentPlayer.State.EndTurn();

                gameEndType = Kingdom.IsGameOver();
                playerTurnCounter++;
            }

            EndGame(gameEndType.Value);
        }

        private void EndGame(GameEndType gameEndType)
        {
            foreach (var player in Players)
            {
                player.GameEnded(GetGameResult(gameEndType));
            }
        }

        public void Dispose()
        {
            foreach (var player in Players)
            {
                player.GameEnded(GetGameResult(GameEndType.Error));
            }
        }

        private GameEndDto GetGameResult(GameEndType gameEndType)
        {
            var gameEndDto = new GameEndDto();
            gameEndDto.GameEndType = gameEndType;
            gameEndDto.Turn = Turn;
            gameEndDto.Players = Players
                .OrderBy(p => p.State.VictoryPoints)
                .Select((p, i) => new PlayerVictoryDto()
                {
                    Name = p.Name,
                    Place = i + 1,
                    VictoryPoints = p.State.VictoryPoints
                })
                .ToList();
            gameEndDto.WinnerName = Players[0].Name;

            return gameEndDto;
        }

        public void AddLog(IPlayer player, BaseMessage message)
        {
            Logs.Add(new LogEntry()
            {
                Turn = Turn,
                PlayerName = player.Name,
                MessageType = message is BuyMessage ? MessageType.Buy : MessageType.Play,
                Args = message.Args
            });
        }
    }
}
