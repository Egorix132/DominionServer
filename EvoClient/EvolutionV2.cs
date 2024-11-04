using EvoClient.Evo;
using GameModel;
using GameModel.Cards;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace EvoClient
{
    public class EvolutionV2
    {
        private static Random _random = new Random();

        public const string BaseSavePath = "E:\\MyProjects\\DominionServer\\EvoClient\\strategies\\";

        public List<StrategyGenomeV2> Strategies { get; set; } = new();
        public ConcurrentDictionary<string, List<GameEndDto>> WinCountByStrategy { get; set; } = new();   

        public int EvoTurn = 0;
        public int StrategyCount = 0;

        public EvolutionV2(int strategyCount)
        {
            StrategyCount = strategyCount;
            RecreateStrategies(StrategyCount);
        }

        public EvolutionV2(string filePath, int strategyCount)
        {
            StrategyCount = strategyCount;
            CreateFromFile(filePath);
        }

        public EvolutionV2(StrategyGenomeV2 strategy, int strategyCount)
        {
            StrategyCount = strategyCount;
            for (int i = 0; i < StrategyCount; i++)
            {
                string name = $"EvoTurn-{EvoTurn} index-{i}";
                strategy.Name = name;
                Strategies.Add(strategy);
                WinCountByStrategy.TryAdd(name, new List<GameEndDto>());
            }
        }

        public void AddStrategy(StrategyGenomeV2 strategy)
        {
            Strategies[Strategies.Count - 1] = strategy;
            WinCountByStrategy[strategy.Name] = new List<GameEndDto>();
        }

        public void CreateFromFile(string filePath)
        {
            var text = File.ReadAllText(BaseSavePath + filePath);
            var cardTypes = text.Split(" ").Select(i => int.Parse(i));

            for (int i = 0; i < StrategyCount; i++)
            {
                var strategy = StrategyGenomeV2.FromInt(cardTypes.ToArray());
                string name = $"EvoTurn-{EvoTurn} index-{i}";
                strategy.Name = name;
                Strategies.Add(strategy);
                WinCountByStrategy.TryAdd(name, new List<GameEndDto>());
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
                    var mutated = origin.Mutate(StrategyGenome.IntSize / 4);

                    mutated.Name = $"EvoTurn-{EvoTurn} index-{i}";
                    Strategies[i] = mutated;
                    WinCountByStrategy.TryAdd(mutated.Name, new List<GameEndDto>());
                }
                return;
            }
            Strategies.Clear();

            for (int i = 0; i < strategyCount; i++)
            {
                string name = $"EvoTurn-{EvoTurn} index-{i}";
                var strategy = StrategyGenomeV2.GenerateRandom(name);
                Strategies.Add(strategy);
                WinCountByStrategy.TryAdd(name, new List<GameEndDto>());
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

                if (WinCountByStrategy.Values.All(v => v.Count == 0))
                {
                    Console.WriteLine($"{EvoTurn} ended RECREATE");

                    RecreateStrategies(Strategies.Count);
                }
                else
                {
                    var winnerStrategies = WinCountByStrategy
                        .OrderByDescending(w => w.Value.Select(w => w.WinnerVP / (float)w.Turn).DefaultIfEmpty(0).Sum())
                        .Take(Strategies.Count / 5)
                        .Select(w => Strategies.FirstOrDefault(s => s.Name == w.Key))
                        .ToList();

                    var strategy = winnerStrategies.First();
                    var wins = WinCountByStrategy[strategy.Name];

                    Console.WriteLine($"Epoch: {EvoTurn}, Play gen Time: {stopWatch.ElapsedMilliseconds} avg game {wins.Sum(w => w.Turn) / (float)wins.Count}");
                    if (EvoTurn % 100 == 0)
                    {
                        
                        Console.WriteLine($"{EvoTurn} ended, Avg VP per turn {wins.Sum(w => w.WinnerVP / (float)w.Turn) / wins.Count}\n {string.Join(",", strategy.PurchasePhases.Last().CardsCount.Select(c => $"{c.Key}: {c.Value}"))}");

                        if (EvoTurn != 0)
                        {
                            File.WriteAllText(
                            BaseSavePath + strategy!.Name.Substring(0, Math.Min(50, strategy!.Name.Length)) + " V" + StrategyGenomeV2.GenomeVersion + ".txt",
                            string.Join(" ", strategy.ToIntArray().Select(t => (int)t!)));
                        }
                    }

                    var offspringCountForEachStrategy = 2;
                    var superMutatedCountForEachStrategy = 2;

                    Strategies.Clear();
                    WinCountByStrategy.Clear();

                    for (int i = 0; i < winnerStrategies.Count; i++)
                    {
                        var parentStrategy = winnerStrategies[i];

                        for (int j = 0; j < offspringCountForEachStrategy; j++)
                        {
                            var mutated = parentStrategy.Mutate();
                            mutated!.Name = $"EvoTurn-{EvoTurn} parent-{parentStrategy!.Name} offSpring-{j}";

                            Strategies.Add(mutated);
                            WinCountByStrategy.TryAdd(mutated!.Name, new List<GameEndDto>());
                        }

                        for (int j = 0; j < superMutatedCountForEachStrategy; j++)
                        {
                            var newMutated = parentStrategy.Mutate(StrategyGenome.IntSize / 15);
                            newMutated!.Name = $"EvoTurn-{EvoTurn} parent-{parentStrategy!.Name} super mutated";

                            Strategies.Add(newMutated);
                            WinCountByStrategy.TryAdd(newMutated!.Name, new List<GameEndDto>());
                        }

                        parentStrategy!.Name = $"EvoTurn-{EvoTurn} index-{i} {date:dd-MM-yyyy}";

                        Strategies.Add(parentStrategy);
                        WinCountByStrategy.TryAdd(parentStrategy!.Name, new List<GameEndDto>());
                    }
                }
                EvoTurn++;
            }
        }

        public async Task PlayGeneration()
        {
            ConcurrentDictionary<(string, string), byte> playedPairs = new();
            var seed = _random.Next();


            for (int i = 0; i < 3; i++)
            {
                await Parallel.ForEachAsync(Strategies, async (firstStrategy, ct) =>
                {
                    foreach (var secondStrategy in Strategies)
                    {
                        if (firstStrategy == secondStrategy
                            || playedPairs.ContainsKey((firstStrategy.Name, secondStrategy.Name))
                            || playedPairs.ContainsKey((secondStrategy.Name, firstStrategy.Name)))
                        {
                            continue;
                        }

                        playedPairs.TryAdd((firstStrategy.Name, secondStrategy.Name), 0);

                        var players = new List<IPlayer>
                        {
                            new GenomePlayerV2(firstStrategy.Name, firstStrategy.Name, firstStrategy),
                            new GenomePlayerV2(secondStrategy.Name, secondStrategy.Name, secondStrategy),
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
                            continue;
                        }
                        else
                        {
                            //Console.WriteLine("VP: " + gameEnd.WinnerVP);
                            WinCountByStrategy[gameEnd.WinnerName].Add(gameEnd);
                        }
                    }
                });

                playedPairs.Clear();
            }
        }
    }
}
