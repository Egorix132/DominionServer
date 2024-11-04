using EvoClient.Utils;
using GameModel.Cards;
using System.Collections.ObjectModel;

namespace EvoClient.Evo;

public class StrategyGenome
{
    private Random _random = new();

    public static CardEnum[] KingdomActionPiles = new CardEnum[]
    {
        CardEnum.Artisan, CardEnum.Cellar, CardEnum.Market, CardEnum.Merchant, CardEnum.Mine,
        CardEnum.Moat, CardEnum.Moneylender, CardEnum.Poacher, CardEnum.Remodel, CardEnum.Witch
    };

    public static CardEnum[] AllCardEnum = new CardEnum[]
    {
        CardEnum.Artisan, CardEnum.Cellar, CardEnum.Market, CardEnum.Merchant, CardEnum.Mine,
        CardEnum.Moat, CardEnum.Moneylender, CardEnum.Poacher, CardEnum.Remodel, CardEnum.Witch,
        CardEnum.Copper, CardEnum.Silver, CardEnum.Gold, CardEnum.Estate, CardEnum.Duchy, CardEnum.Province, CardEnum.Curse
    };

    public const int KingdomActionPilesCount = 10;

    public const string GenomeVersion = "0.7";

    public const int FirstBuySize = 4;

    public static int IntSize = FirstBuySize + PurchasePhasesCount * PurchasePhaseLength + PlayPhasesCount * GenomePlayPhase.intSize;

    public const int MutatePercent = 1;

    public static float MutateGensCountDefault = IntSize * (MutatePercent / 100f);

    public float MutateGensCount = MutateGensCountDefault;

    public string Name;

    public const int GameLength = 20;

    public const int PurchasePhasesCount = 4;
    public const int PurchasePhaseLength = GameLength / PurchasePhasesCount + 2;

    public const int PlayPhasesCount = 3;
    public const int PlayPhaseLength = GameLength / PlayPhasesCount;


    public CardEnum[][] PurchasePhases { get; set; } = new CardEnum[1 + PurchasePhasesCount][];
    public GenomePlayPhase[] PlayPhases { get; set; } = new GenomePlayPhase[PlayPhasesCount];

    public StrategyGenome(string name)
    {
        Name = name;
    }

    public CardEnum?[] ToIntArray()
    {
        var result = new CardEnum?[IntSize];
        int pointer = 0;

        for (int i = 0; i < PurchasePhases.Length; i++)
        {
            var purchasePhase = PurchasePhases[i];
            if (i == 0)
            {
                result[i + 0] = purchasePhase[0];
                result[i + 1] = purchasePhase[1];
                result[i + 2] = purchasePhase[2];
                result[i + 3] = purchasePhase[3];

                pointer += 4;

                continue;
            }

            for (int j = 0; j < PurchasePhaseLength; j++)
            {
                result[pointer + j] = purchasePhase[j];
            }
            pointer += PurchasePhaseLength;
        }

        for (int i = 0; i < PlayPhases.Length; i++)
        {
            var playPhase = PlayPhases[i];

            for (int j = 0; j < GenomePlayPhase.PlayOrderCount; j++)
            {
                result[pointer + j] = playPhase.PlayOrder[j];
            }

            pointer += GenomePlayPhase.PlayOrderCount;

            for (int j = 0; j < playPhase.CardsArguments.Count; j++)
            {
                var el = playPhase.CardsArguments.ElementAt(j);

                var argsCount = el.Value.Length;

                for (int k = 0; k < argsCount; k++)
                {
                    result[pointer + k] = playPhase.CardsArguments[el.Key][k];
                }

                pointer += argsCount;
            }
        }

        return result;
    }

    public static StrategyGenome FromFile(string filePath)
    {
        var text = File.ReadAllText(filePath);
        var cardTypes = text.Split(" ").Select(i => Enum.Parse<CardEnum>(i));

        var strategy = FromInt(cardTypes.ToArray());
        strategy.Name = Path.GetFileNameWithoutExtension(filePath);
        return strategy;
    }

    public static StrategyGenome FromInt(CardEnum[] ints)
    {
        var strategy = new StrategyGenome(Guid.NewGuid().ToString());

        int pointer = 0;

        for (int i = 0; i < PurchasePhasesCount + 1; i++)
        {
            CardEnum[] purchasePhase;
            if (i == 0)
            {
                purchasePhase = new CardEnum[4];

                purchasePhase[0] = ints[0];
                purchasePhase[1] = ints[1];
                purchasePhase[2] = ints[2];
                purchasePhase[3] = ints[3];

                pointer += 4;
                strategy.PurchasePhases[i] = purchasePhase;
                continue;
            }

            purchasePhase = new CardEnum[PurchasePhaseLength];

            for (int j = 0; j < PurchasePhaseLength; j++)
            {
                purchasePhase[j] = ints[pointer + j];
            }
            strategy.PurchasePhases[i] = purchasePhase;
            pointer += PurchasePhaseLength;
        }

        for (int i = 0; i < PlayPhasesCount; i++)
        {
            var playPhase = new GenomePlayPhase();

            for (int j = 0; j < GenomePlayPhase.PlayOrderCount; j++)
            {
                playPhase.PlayOrder[j] = ints[pointer + j];
            }

            pointer += GenomePlayPhase.PlayOrderCount;

            for (int j = 0; j < playPhase.CardsArguments.Count; j++)
            {
                var el = playPhase.CardsArguments.ElementAt(j);

                var argsCount = el.Value.Length;

                for (int k = 0; k < argsCount; k++)
                {
                    playPhase.CardsArguments[el.Key][k] = ints[pointer + k];
                }

                pointer += argsCount;
            }

            strategy.PlayPhases[i] = playPhase;
        }

        return strategy;
    }

    public static StrategyGenome GenerateRandom(string? name = null)
    {
        var strategy = new StrategyGenome(name ?? Guid.NewGuid().ToString());

        for (int i = 0; i < PurchasePhasesCount + 1; i++)
        {
            CardEnum[] purchasePhase;
            if (i == 0)
            {
                purchasePhase = new CardEnum[4];

                purchasePhase[0] = AllCardEnum.GetRandom();
                purchasePhase[1] = AllCardEnum.GetRandom();
                purchasePhase[2] = AllCardEnum.GetRandom();
                purchasePhase[3] = AllCardEnum.GetRandom();

                strategy.PurchasePhases[i] = purchasePhase;
                continue;
            }
            purchasePhase = new CardEnum[PurchasePhaseLength];

            for (int j = 0; j < PurchasePhaseLength; j++)
            {
                purchasePhase[j] = AllCardEnum.GetRandom();
            }
            strategy.PurchasePhases[i] = purchasePhase;
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

    public StrategyGenome Mutate(float? mutateCount = null)
    {
        MutateGensCount = mutateCount ?? MutateGensCountDefault;
        var strategy = new StrategyGenome(Name);

        for (int i = 0; i < PurchasePhases.Length; i++)
        {
            CardEnum[] newPurchasePhase;
            if (i == 0)
            {
                newPurchasePhase = new CardEnum[PurchasePhaseLength];

                newPurchasePhase[0] = MutateGen(PurchasePhases[i][0], false, 3);
                newPurchasePhase[1] = MutateGen(PurchasePhases[i][1], false, 3);
                newPurchasePhase[2] = MutateGen(PurchasePhases[i][2], false, 3);
                newPurchasePhase[3] = MutateGen(PurchasePhases[i][3], false, 3);

                strategy.PurchasePhases[i] = newPurchasePhase;
                continue;
            }
            newPurchasePhase = new CardEnum[PurchasePhaseLength];

            for (int j = 0; j < PurchasePhaseLength; j++)
            {
                newPurchasePhase[j] = MutateGen(PurchasePhases[i][j], false, 3 / (i + 1));
            }

            strategy.PurchasePhases[i] = newPurchasePhase;
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

    public static StrategyGenome FromConsole()
    {
        Console.WriteLine("Input strategy name:");
        string strategyName = Console.ReadLine();
        var strategy = new StrategyGenome(strategyName ?? Guid.NewGuid().ToString());

        for (int i = 0; i < PurchasePhasesCount + 1; i++)
        {
            CardEnum[] purchasePhase;
            if (i == 0)
            {
                Console.WriteLine($"Input 4 cards to buy on {i} turn");
                purchasePhase = new CardEnum[4];

                purchasePhase[0] = AskFromConsole();
                purchasePhase[1] = AskFromConsole();
                purchasePhase[2] = AskFromConsole();
                purchasePhase[3] = AskFromConsole();

                strategy.PurchasePhases[i] = purchasePhase;
                continue;
            }
            purchasePhase = new CardEnum[PurchasePhaseLength];

            Console.WriteLine($"Input {PurchasePhaseLength} cards to buy on {i} turn");

            for (int j = 0; j < PurchasePhaseLength; j++)
            {
                purchasePhase[j] = AskFromConsole();
            }
            strategy.PurchasePhases[i] = purchasePhase;
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

public class GenomePlayPhase
{
    public const int PlayOrderCount = 10;

    private static readonly IReadOnlyDictionary<CardEnum, CardEnum[]> EmptyCardArguments;
    public static readonly int TotalCardsArgumentsCount;
    public static readonly int intSize;

    static GenomePlayPhase()
    {
        EmptyCardArguments = new ReadOnlyDictionary<CardEnum, CardEnum[]>(StrategyGenome.KingdomActionPiles.Select(p =>
            CardEnumDict.GetCard(p)).OfType<IActionCard>().ToDictionary(k => k.CardTypeId, c => new CardEnum[c.ArgTypes.Length * 2]));

        TotalCardsArgumentsCount = EmptyCardArguments.Sum(c => c.Value.Length);
        intSize = PlayOrderCount + TotalCardsArgumentsCount;
    }

    public GenomePlayPhase()
    {
        CardsArguments = EmptyCardArguments.ToDictionary(entry => entry.Key,
                                                        entry => (CardEnum[])entry.Value.Clone());
    }


    public CardEnum[] PlayOrder { get; set; } = new CardEnum[PlayOrderCount];

    public Dictionary<CardEnum, CardEnum[]> CardsArguments { get; set; }
}
