using EvoClient.Utils;
using GameModel.Cards;
using System.Reflection;

namespace EvoClient.Evo;

public class StrategyGenomeV2
{
    private Random _random = new();

    public static CardEnum[] KingdomActionPiles = new CardEnum[]
    {
        CardEnum.Artisan, CardEnum.Cellar, CardEnum.Market, CardEnum.Merchant, CardEnum.Mine,
        CardEnum.Moat, CardEnum.Moneylender, CardEnum.Poacher, CardEnum.Remodel, CardEnum.Witch
    };

    public static CardEnum[] BasePiles = new CardEnum[]
    {
        CardEnum.Copper, CardEnum.Silver, CardEnum.Gold, CardEnum.Estate, CardEnum.Duchy, CardEnum.Province, CardEnum.Curse
    };

    public static CardEnum[] AllCardEnum = KingdomActionPiles.Concat(BasePiles).ToArray();

    public const int KingdomActionPilesCount = 10;

    public const string GenomeVersion = "1.1";

    public const int FirstBuySize = 4;

    public static readonly int IntSize = KingdomActionPiles.Length + FirstBuySize 
        + PurchasePhasesCount * GenomeV2PurchasePhase.intSize + PlayPhasesCount * GenomePlayPhase.intSize;

    public const int MutatePercent = 1;

    public static readonly float MutateGensCountDefault = IntSize * (MutatePercent / 100f);

    public float MutateGensCount = MutateGensCountDefault;

    public string Name;

    public const int GameLength = 20;

    public const int PurchasePhasesCount = 4;
    public const int PurchasePhaseLength = GameLength / PurchasePhasesCount + 2;

    public const int PlayPhasesCount = 3;
    public const int PlayPhaseLength = GameLength / PlayPhasesCount;

    public CardEnum[] FirstTwoTurnsBuy { get; set; } = new CardEnum[4];

    public GenomeV2PurchasePhase[] PurchasePhases { get; set; } = new GenomeV2PurchasePhase[PurchasePhasesCount];
    public GenomePlayPhase[] PlayPhases { get; set; } = new GenomePlayPhase[PlayPhasesCount];

    public StrategyGenomeV2(string name, CardEnum[]? kingdomPiles = null)
    {
        Name = name;
        KingdomActionPiles = kingdomPiles ?? KingdomActionPiles;
    }

    public int?[] ToIntArray()
    {
        var result = new int?[IntSize];

        result[0] = (int?)FirstTwoTurnsBuy[0];
        result[1] = (int?)FirstTwoTurnsBuy[1];
        result[2] = (int?)FirstTwoTurnsBuy[2];
        result[3] = (int?)FirstTwoTurnsBuy[3];

        int pointer = 4;

        for (int i = 0; i < PurchasePhases.Length; i++)
        {
            var purchasePhase = PurchasePhases[i];

            for (int j = 0; j < purchasePhase.CardsCount.Count; j++)
            {
                var el = purchasePhase.CardsCount.ElementAt(j);

                result[pointer + j] = purchasePhase.CardsCount[el.Key];
            }
            pointer += purchasePhase.CardsCount.Count;
        }

        for (int i = 0; i < PlayPhases.Length; i++)
        {
            var playPhase = PlayPhases[i];

            for (int j = 0; j < GenomePlayPhase.PlayOrderCount; j++)
            {
                result[pointer + j] = (int?)playPhase.PlayOrder[j];
            }

            pointer += GenomePlayPhase.PlayOrderCount;

            for (int j = 0; j < playPhase.CardsArguments.Count; j++)
            {
                var el = playPhase.CardsArguments.ElementAt(j);

                var argsCount = el.Value.Length;

                for (int k = 0; k < argsCount; k++)
                {
                    result[pointer + k] = (int?)playPhase.CardsArguments[el.Key][k];
                }

                pointer += argsCount;
            }
        }

        return result;
    }

    public static StrategyGenomeV2 FromFile(string filePath, CardEnum[]? kingdomPiles = null)
    {
        KingdomActionPiles = kingdomPiles ?? KingdomActionPiles;
        var text = File.ReadAllText(filePath);
        var cardTypes = text.Split(" ").Select(i => int.Parse(i));

        var strategy = FromInt(cardTypes.ToArray());
        strategy.Name = Path.GetFileNameWithoutExtension(filePath);
        return strategy;
    }

    public static StrategyGenomeV2 FromInt(int[] ints)
    {
        var strategy = new StrategyGenomeV2(Guid.NewGuid().ToString());

        for (int i = 0; i < KingdomActionPiles.Length; i++)
        {
            KingdomActionPiles[i] = (CardEnum)ints[i];
        }
        int pointer = KingdomActionPiles.Length;

        strategy.FirstTwoTurnsBuy[pointer + 0] = (CardEnum)ints[0];
        strategy.FirstTwoTurnsBuy[pointer + 1] = (CardEnum)ints[1];
        strategy.FirstTwoTurnsBuy[pointer + 2] = (CardEnum)ints[2];
        strategy.FirstTwoTurnsBuy[pointer + 3] = (CardEnum)ints[3];

        for (int i = 0; i < PurchasePhasesCount; i++)
        {
            var purchasePhase = new GenomeV2PurchasePhase();

            for (int j = 0; j < purchasePhase.CardsCount.Count; j++)
            {
                var el = purchasePhase.CardsCount.ElementAt(j);

                purchasePhase.CardsCount[el.Key] = ints[pointer + j];
            }
            strategy.PurchasePhases[i] = purchasePhase;
            pointer += purchasePhase.CardsCount.Count;
        }

        for (int i = 0; i < PlayPhasesCount; i++)
        {
            var playPhase = new GenomePlayPhase();

            for (int j = 0; j < GenomePlayPhase.PlayOrderCount; j++)
            {
                playPhase.PlayOrder[j] = (CardEnum)ints[pointer + j];
            }

            pointer += GenomePlayPhase.PlayOrderCount;

            for (int j = 0; j < playPhase.CardsArguments.Count; j++)
            {
                var el = playPhase.CardsArguments.ElementAt(j);

                var argsCount = el.Value.Length;

                for (int k = 0; k < argsCount; k++)
                {
                    playPhase.CardsArguments[el.Key][k] = (CardEnum)ints[pointer + k];
                }

                pointer += argsCount;
            }

            strategy.PlayPhases[i] = playPhase;
        }

        return strategy;
    }

    public static StrategyGenomeV2 GenerateRandom(string? name = null)
    {
        var strategy = new StrategyGenomeV2(name ?? Guid.NewGuid().ToString());

        strategy.FirstTwoTurnsBuy[0] = AllCardEnum.GetRandom();
        strategy.FirstTwoTurnsBuy[1] = AllCardEnum.GetRandom();
        strategy.FirstTwoTurnsBuy[2] = AllCardEnum.GetRandom();
        strategy.FirstTwoTurnsBuy[3] = AllCardEnum.GetRandom();

        int pointer = 4;

        for (int i = 0; i < PurchasePhasesCount; i++)
        {
            var purchasePhase = new GenomeV2PurchasePhase();

            for (int j = 0; j < purchasePhase.CardsCount.Count; j++)
            {
                var el = purchasePhase.CardsCount.ElementAt(j);

                purchasePhase.CardsCount[el.Key] = strategy._random.Next(0, 10);
            }

            strategy.PurchasePhases[i] = purchasePhase;
            pointer += purchasePhase.CardsCount.Count;
        }

        for (int i = 0; i < PlayPhasesCount; i++)
        {
            var playPhase = new GenomePlayPhase();

            for (int j = 0; j < GenomePlayPhase.PlayOrderCount; j++)
            {
                playPhase.PlayOrder[j] = KingdomActionPiles.GetRandom();
            }

            for (int j = 0; j < playPhase.CardsArguments.Count; j++)
            {
                var el = playPhase.CardsArguments.ElementAt(j);

                var argsCount = el.Value.Length;

                for (int k = 0; k < argsCount; k++)
                {
                    playPhase.CardsArguments[el.Key][k] = AllCardEnum.GetRandom();
                }
            }
            strategy.PlayPhases[i] = playPhase;
        }

        return strategy;
    }

    public StrategyGenomeV2 Mutate(float? mutateCount = null)
    {
        MutateGensCount = mutateCount ?? MutateGensCountDefault;
        var strategy = new StrategyGenomeV2(Name);

        strategy.FirstTwoTurnsBuy[0] = MutateGen(FirstTwoTurnsBuy[0], false, 3);
        strategy.FirstTwoTurnsBuy[1] = MutateGen(FirstTwoTurnsBuy[0], false, 3);
        strategy.FirstTwoTurnsBuy[2] = MutateGen(FirstTwoTurnsBuy[0], false, 3);
        strategy.FirstTwoTurnsBuy[3] = MutateGen(FirstTwoTurnsBuy[0], false, 3);

        int pointer = 4;

        for (int i = 0; i < PurchasePhasesCount; i++)
        {
            var newPurchasePhase = new GenomeV2PurchasePhase();

            for (int j = 0; j < newPurchasePhase.CardsCount.Count; j++)
            {
                var el = newPurchasePhase.CardsCount.ElementAt(j);

                newPurchasePhase.CardsCount[el.Key] = MutateGen(PurchasePhases[i].CardsCount[el.Key], 10, 3 / (i + 1));
            }
            strategy.PurchasePhases[i] = newPurchasePhase;
            pointer += newPurchasePhase.CardsCount.Count;
        }

        for (int i = 0; i < PlayPhasesCount; i++)
        {
            var oldPlayPhase = PlayPhases[i];
            var newPlayPhase = new GenomePlayPhase();

            for (int j = 0; j < GenomePlayPhase.PlayOrderCount; j++)
            {
                newPlayPhase.PlayOrder[j] = MutateGen(oldPlayPhase.PlayOrder[j], true);
            }

            for (int j = 0; j < newPlayPhase.CardsArguments.Count; j++)
            {
                var el = oldPlayPhase.CardsArguments.ElementAt(j);

                var argsCount = el.Value.Length;

                for (int k = 0; k < argsCount; k++)
                {
                    newPlayPhase.CardsArguments[el.Key][k] = MutateGen(oldPlayPhase.CardsArguments[el.Key][k]);
                }
            }

            strategy.PlayPhases[i] = newPlayPhase;
        }

        return strategy;
    }

    private CardEnum MutateGen(CardEnum genInCurrentGenom, bool onlyActions = false, float mutateCoef = 1)
    {
        
        if (_random.Next(0, IntSize) <= MutateGensCount * mutateCoef)
        {
            if (onlyActions)
            {
                return KingdomActionPiles.GetRandom();
            }
            else
            {
                return AllCardEnum.GetRandom();
            }
        }
        else
        {
            return genInCurrentGenom;
        }
    }

    private int MutateGen(int genInCurrentGenom, int maxSize = 10, float mutateCoef = 1)
    {

        if (_random.Next(0, IntSize) <= MutateGensCount * mutateCoef)
        {
            return _random.Next(0, maxSize);
        }
        else
        {
            return genInCurrentGenom;
        }
    }

    public static StrategyGenomeV2 FromConsole()
    {
        Console.WriteLine("Input strategy name:");
        string strategyName = Console.ReadLine();

        var strategy = new StrategyGenomeV2(strategyName ?? Guid.NewGuid().ToString());

        Console.WriteLine($"Input 4 cards to buy on first two turns");

        strategy.FirstTwoTurnsBuy[0] = AskFromConsole();
        strategy.FirstTwoTurnsBuy[1] = AskFromConsole();
        strategy.FirstTwoTurnsBuy[2] = AskFromConsole();
        strategy.FirstTwoTurnsBuy[3] = AskFromConsole();

        int pointer = 4;

        for (int i = 0; i < PurchasePhasesCount; i++)
        {
            var newPurchasePhase = new GenomeV2PurchasePhase();

            for (int j = 0; j < newPurchasePhase.CardsCount.Count; j++)
            {
                var el = newPurchasePhase.CardsCount.ElementAt(j);

                Console.WriteLine($"Input count of {el.Key} cards to buy on {i} phase");

                newPurchasePhase.CardsCount[el.Key] = (int)AskFromConsole();
            }
            strategy.PurchasePhases[i] = newPurchasePhase;
            pointer += newPurchasePhase.CardsCount.Count;
        }

        for (int i = 0; i < PlayPhasesCount; i++)
        {
            var playPhase = new GenomePlayPhase();

            Console.WriteLine($"Input {GenomePlayPhase.PlayOrderCount} action cards to order play on {i} turn");

            for (int j = 0; j < GenomePlayPhase.PlayOrderCount; j++)
            {
                playPhase.PlayOrder[j] = AskFromConsole();
            }

            for (int j = 0; j < playPhase.CardsArguments.Count; j++)
            {
                var el = playPhase.CardsArguments.ElementAt(j);

                var argsCount = el.Value.Length;

                Console.WriteLine($"Input {argsCount} cards to pass them to play {el.Key} card on {i} turn");

                for (int k = 0; k < argsCount; k++)
                {
                    playPhase.CardsArguments[el.Key][k] = AskFromConsole();
                }
            }
            strategy.PlayPhases[i] = playPhase;
        }

        return strategy;
    }

    private static CardEnum AskFromConsole(string? ask = null)
    {
        if (ask != null)
        {
            Console.WriteLine(ask);
        }
        string cardString;

        CardEnum card;
        do
        {
            cardString = Console.ReadLine();
        }
        while (!Enum.TryParse(cardString, out card));

        return card;
    }
}

public class GenomeV2PurchasePhase
{
    public static int intSize;

    public GenomeV2PurchasePhase()
    {
        intSize = StrategyGenomeV2.AllCardEnum.Length;
    }


    public Dictionary<CardEnum, int> CardsCount { get; set; } = StrategyGenomeV2.AllCardEnum.ToDictionary(entry => entry,
                                                                entry => 0);
}
