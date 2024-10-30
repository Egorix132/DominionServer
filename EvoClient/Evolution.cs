using EvoClient.Evo;
using GameModel;
using GameModel.Cards;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace EvoClient
{
    public class Evolution
    {
        public List<StrategyGenome> Strategies { get; set; } = new();
        public ConcurrentDictionary<string, float> WinCountByStrategy { get; set; } = new();

        public int EvoTurn = 0;
        public int StrategyCount = 0;

        public Evolution(int strategyCount)
        {
            StrategyCount = strategyCount;
            RecreateStrategies(StrategyCount);
        }

        public Evolution(string filePath, int strategyCount)
        {
            StrategyCount = strategyCount;
            CreateFromFile(filePath);
        }

        public Evolution(StrategyGenome strategy, int strategyCount)
        {
            StrategyCount = strategyCount;
            for (int i = 0; i < StrategyCount; i++)
            {
                string name = $"EvoTurn-{EvoTurn} index-{i}";
                strategy.Name = name;
                Strategies.Add(strategy);
                WinCountByStrategy.TryAdd(name, 0);
            }
        }

        public void AddStrategy(StrategyGenome strategy)
        {
            Strategies[Strategies.Count - 1] = strategy;
            WinCountByStrategy[strategy.Name] = 0;
        }

        public void CreateFromFile(string filePath)
        {
            var text = File.ReadAllText(filePath);
            var cardTypes = text.Split(" ").Select(i => Enum.Parse<CardEnum>(i));

            for (int i = 0; i < StrategyCount; i++)
            {
                var strategy = StrategyGenome.FromInt(cardTypes.ToArray());
                string name = $"EvoTurn-{EvoTurn} index-{i}";
                strategy.Name = name;
                Strategies.Add(strategy);
                WinCountByStrategy.TryAdd(name, 0);
            }
        }

        public void RecreateStrategies(int strategyCount)
        {
            WinCountByStrategy.Clear();

            if (Strategies.Count > 0)
            {
                for (int i = 0; i < Strategies.Count; i++)
                {
                    var origin = Strategies[i];
                    var mutated = origin.Mutate(StrategyGenome.IntSize / 40);

                    mutated.Name = $"EvoTurn-{EvoTurn} index-{i}";
                    Strategies[i] = mutated;
                    WinCountByStrategy.TryAdd(mutated.Name, 0);
                }
                return;
            }
            Strategies.Clear();

            for (int i = 0; i < strategyCount; i++)
            {
                string name = $"EvoTurn-{EvoTurn} index-{i}";
                var strategy = StrategyGenome.GenerateRandom(name);
                Strategies.Add(strategy);
                WinCountByStrategy.TryAdd(name, 0);
            }
        }

        public async Task StartEducation()
        {
            var date = DateTime.Now.Date;
            while (true)
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                await PlayGeneration();
                stopWatch.Stop();
                Console.WriteLine($"Epoch: {EvoTurn}, Play gen Time: " + stopWatch.ElapsedMilliseconds);

                if (WinCountByStrategy.Values.All(v => v == 0))
                {
                    Console.WriteLine($"{EvoTurn} ended RECREATE");

                    RecreateStrategies(Strategies.Count);
                }
                else
                {
                    var winnerStrategies = WinCountByStrategy
                        .OrderByDescending(w => w.Value)
                        .Take(Strategies.Count / 5)
                        .Select(w => Strategies.FirstOrDefault(s => s.Name == w.Key))
                        .ToList();

                    if (EvoTurn % 100 == 0)
                    {
                        var strategy = winnerStrategies.First();
                        File.WriteAllText(strategy!.Name + StrategyGenome.GenomeVersion + ".txt", string.Join(" ", strategy.ToIntArray().Select(t => (int)t!)));
                        Console.WriteLine($"{EvoTurn} ended, Avg VP per turn {WinCountByStrategy[strategy.Name] / StrategyCount}\n {string.Join(",", strategy.PurchasePhases.SelectMany(c => c))}");
                    }

                    var offspringCountForEachStrategy = 2;
                    var superMutatedCountForEachStrategy = 2;

                    Strategies.Clear();
                    WinCountByStrategy.Clear();

                    for (int i = 0; i < winnerStrategies.Count; i++)
                    {
                        var parentStrategy = winnerStrategies[i];
                        parentStrategy!.Name = $"EvoTurn-{EvoTurn} index-{i} {date:dd-MM-yyyy}";

                        Strategies.Add(parentStrategy);
                        WinCountByStrategy.TryAdd(parentStrategy!.Name, 0);

                        for (int j = 0; j < offspringCountForEachStrategy; j++)
                        {
                            var mutated = parentStrategy.Mutate();
                            mutated!.Name = $"EvoTurn-{EvoTurn} parent-{parentStrategy!.Name} offSpring-{j}";

                            Strategies.Add(mutated);
                            WinCountByStrategy.TryAdd(mutated!.Name, 0);
                        }

                        for (int j = 0; j < superMutatedCountForEachStrategy; j++)
                        {
                            var newMutated = parentStrategy.Mutate(StrategyGenome.IntSize / 40);
                            newMutated!.Name = $"EvoTurn-{EvoTurn} parent-{parentStrategy!.Name} super mutated";

                            Strategies.Add(newMutated);
                            WinCountByStrategy.TryAdd(newMutated!.Name, 0);
                        }
                    }
                }
                EvoTurn++;
            }
        }

        public async Task PlayGeneration()
        {
            ConcurrentDictionary<(string, string), byte> playedPairs = new();

            Parallel.ForEach(Strategies, async (firstStrategy) =>
            {
                foreach (var secondStrategy in Strategies)
                {
                    if (firstStrategy == secondStrategy
                        || playedPairs.ContainsKey((firstStrategy.Name, secondStrategy.Name)))
                    {
                        continue;
                    }

                    playedPairs.TryAdd((firstStrategy.Name, secondStrategy.Name), 0);

                    var players = new List<IPlayer>
                    {
                        new GenomePlayer(firstStrategy.Name, firstStrategy.Name, firstStrategy),
                        new GenomePlayer(secondStrategy.Name, secondStrategy.Name, secondStrategy),
                    };
                    Game game = new(
                        players,
                        new Kingdom(
                            new List<CardEnum> {
                        CardEnum.Artisan, CardEnum.Cellar, CardEnum.Market, CardEnum.Merchant, CardEnum.Mine,
                        CardEnum.Moat, CardEnum.Moneylender, CardEnum.Poacher, CardEnum.Remodel, CardEnum.Witch
                            },
                            players.Count
                        )
                    );

                    var gameEnd = await game.StartEvoGame();

                    if (gameEnd.GameEndType == GameEndType.ToooLong)
                    {
                        return;
                    }
                    else
                    {
                        //Console.WriteLine("VP: " + gameEnd.WinnerVP);
                        WinCountByStrategy[gameEnd.WinnerName] += gameEnd.WinnerVP / (float)gameEnd.Turn;
                    }
                }
            });
        }
    }
}
