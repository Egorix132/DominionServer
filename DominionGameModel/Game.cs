using GameModel.Infrastructure;
using GameModel.Infrastructure.Exceptions;
using System.Diagnostics;

namespace GameModel
{
    public class Game : IGameState
    {
        public Guid Id { get; set; }

        public List<IPlayer> Players { get; set; } = new();
        public List<IPlayer> Spectators { get; set; } = new();

        public IKingdomState Kingdom { get; set; }

        public IPlayer CurrentPlayer { get; private set; }

        public int Turn { get; private set; }
        public List<LogEntry> Logs { get; private set; } = new();

        public Game(List<IPlayer> players, Kingdom kingdom, List<IPlayer>? spectators = null)
        {
            Id = Guid.NewGuid();
            Kingdom = kingdom;

            foreach (var player in players)
            {
                Players.Add(player);
                player.State.SetDefaultState();
            }

            if(spectators != null)
            {
                foreach (var spectator in spectators!)
                {
                    Spectators.Add(spectator);
                    spectator.State.SetDefaultState();
                }
            }

            Players.Shuffle();
        }

        public async Task StartGame()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            Turn = 0;
            int playerTurnCounter = 0;
            GameEndType? gameEndType = null;
            while (gameEndType == null)
            {
                if (playerTurnCounter % Players.Count == 0)
                {
                    Turn++;
                }

                try
                {
                    CurrentPlayer = Players[playerTurnCounter % Players.Count];
                    var spectators = Spectators.Where(s => s.Name == CurrentPlayer.Name);

                    foreach (var spectator in spectators)
                    {
                        spectator.PlayTurnAsync(this);
                    }
                    await CurrentPlayer.PlayTurnAsync(this);

                    CurrentPlayer.State.EndTurn();

                }
                catch (Exception e)
                {
                    foreach (var log in Logs)
                    {
                        Console.WriteLine($"{log.Turn} {log.PlayerName} {log.MessageType} {log.Args}");
                    }
                    //Console.WriteLine(e.Message);
                }

                gameEndType = Kingdom.IsGameOver();
                playerTurnCounter++;

            }

            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);

            EndGame(gameEndType.Value);
        }

        public async Task<GameEndDto> StartEvoGame()
        {
            Turn = 0;
            int playerTurnCounter = 0;
            GameEndType? gameEndType = null;
            while (gameEndType == null)
            {
                if (Turn > 40)
                {
                    return GetGameResult(GameEndType.ToooLong);
                }
                if (playerTurnCounter % Players.Count == 0)
                {
                    Turn++;
                }

                try
                {

                    CurrentPlayer = Players[playerTurnCounter % Players.Count];
                    await CurrentPlayer.PlayTurnAsync(this);

                    CurrentPlayer.State.EndTurn();

                }
                catch (BaseDominionException e)
                {
                    var a = e;
                }
                catch (Exception e)
                {
                    foreach (var log in Logs)
                    {
                        Console.WriteLine($"{log.Turn} {log.PlayerName} {log.MessageType} {log.Args}");
                    }
                    //Console.WriteLine(e.Message);
                }

                gameEndType = Kingdom.IsGameOver();
                playerTurnCounter++;
            }

            return GetGameResult(gameEndType.Value);
        }


        private void EndGame(GameEndType gameEndType)
        {
            var gameResult = GetGameResult(gameEndType);
            Console.WriteLine($"{gameEndType} Winner: {gameResult.WinnerName}");
            foreach (var player in Players)
            {
                player.GameEnded(gameResult);
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
                .OrderByDescending(p => p.State.VictoryPoints)
                .Select((p, i) => new PlayerVictoryDto()
                {
                    Name = p.Name,
                    Place = i + 1,
                    VictoryPoints = p.State.VictoryPoints
                })
                .ToList();
            gameEndDto.WinnerName = gameEndDto.Players[0].Name;
            gameEndDto.WinnerVP = gameEndDto.Players[0].VictoryPoints;

            return gameEndDto;
        }

        public void AddLog(IPlayer player, BaseMessage message)
        {
            Logs.Add(new LogEntry()
            {
                Turn = Turn,
                PlayerName = player.Name,
                MessageType = message is BuyMessage ? MessageType.Buy : MessageType.Play,
                PlayedCard = message is PlayCardMessage playCardMessage ? playCardMessage.PlayedCard : null,
                Args = message.Args
            });
        }

        public void AddSpectator(IPlayer specatator)
        {
            Spectators.Add(specatator);
            specatator.State.SetDefaultState();
        }
    }
}
