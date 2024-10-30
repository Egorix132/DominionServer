using EvoClient.Utils;
using GameModel.Cards;

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

    public const string GenomeVersion = "0.3";

    public const int FirstBuySize = 4;

    public static int IntSize = FirstBuySize + PurchasePhasesCount * PurchasePhaseLength + PlayPhasesCount * GenomePlayPhase.intSize;

    public const int MutatePercent = 2;

    public static float MutateGensCountDefault = IntSize * (MutatePercent / 100f);

    public float MutateGensCount = MutateGensCountDefault;

    public string Name;

    public const int GameTurnLength = 30;

    public const int PurchasePhasesCount = 3;
    public const int PurchasePhaseLength = GameTurnLength / PurchasePhasesCount + 2;

    public const int PlayPhasesCount = 2;
    public const int PlayPhaseLength = GameTurnLength / PlayPhasesCount;


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

            for (int j = 0; j < KingdomActionPilesCount; j++)
            {
                var key = KingdomActionPiles[j];

                var argsCount = (CardEnumDict.GetCard(key) as IActionCard)!.ArgsCount;

                for (int k = 0; k < argsCount; k++)
                {
                    result[pointer + k] = playPhase.CardsArguments[key][k];
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
            var purchasePhase = new CardEnum[PurchasePhaseLength];
            if (i == 0)
            {
                purchasePhase[0] = ints[0];
                purchasePhase[1] = ints[1];
                purchasePhase[2] = ints[2];
                purchasePhase[3] = ints[3];

                pointer += 4;
                strategy.PurchasePhases[i] = purchasePhase;
                continue;
            }

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

            for (int j = 0; j < KingdomActionPilesCount; j++)
            {
                var key = KingdomActionPiles[j];

                var argsCount = (CardEnumDict.GetCard(key) as IActionCard)!.ArgsCount;

                for (int k = 0; k < argsCount; k++)
                {
                    playPhase.CardsArguments[key][k] = ints[pointer + k];
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
            var purchasePhase = new CardEnum[PurchasePhaseLength];
            if (i == 0)
            {
                purchasePhase[0] = AllCardEnum.GetRandom();
                purchasePhase[1] = AllCardEnum.GetRandom();
                purchasePhase[2] = AllCardEnum.GetRandom();
                purchasePhase[3] = AllCardEnum.GetRandom();

                strategy.PurchasePhases[i] = purchasePhase;
                continue;
            }

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

            for (int j = 0; j < KingdomActionPilesCount; j++)
            {
                var key = KingdomActionPiles[j];

                var argsCount = (CardEnumDict.GetCard(key) as IActionCard)!.ArgsCount;

                for (int k = 0; k < argsCount; k++)
                {
                    playPhase.CardsArguments[key][k] = AllCardEnum.GetRandom();
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
            var newPurchasePhase = new CardEnum[PurchasePhaseLength];
            if (i == 0)
            {
                newPurchasePhase[0] = MutateGen(PurchasePhases[i][0], false, 3);
                newPurchasePhase[1] = MutateGen(PurchasePhases[i][1], false, 3);
                newPurchasePhase[2] = MutateGen(PurchasePhases[i][2], false, 3);
                newPurchasePhase[3] = MutateGen(PurchasePhases[i][3], false, 3);

                strategy.PurchasePhases[i] = newPurchasePhase;
                continue;
            }

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

            for (int j = 0; j < KingdomActionPilesCount; j++)
            {
                var key = KingdomActionPiles[j];

                var argsCount = (CardEnumDict.GetCard(key) as IActionCard)!.ArgsCount;

                for (int k = 0; k < argsCount; k++)
                {
                    newPlayPhase.CardsArguments[key][k] = MutateGen(oldPlayPhase.CardsArguments[key][k]);
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
            var purchasePhase = new CardEnum[PurchasePhaseLength];
            if (i == 0)
            {
                Console.WriteLine($"Input 4 cards to buy on {i} turn");

                purchasePhase[0] = AskFromConsole();
                purchasePhase[1] = AskFromConsole();
                purchasePhase[2] = AskFromConsole();
                purchasePhase[3] = AskFromConsole();

                strategy.PurchasePhases[i] = purchasePhase;
                continue;
            }

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

            Console.WriteLine($"Input 10 action cards to order play on {i} turn");

            for (int j = 0; j < GenomePlayPhase.PlayOrderCount; j++)
            {
                playPhase.PlayOrder[j] = AskFromConsole();
            }

            for (int j = 0; j < KingdomActionPilesCount; j++)
            {
                var key = KingdomActionPiles[j];

                Console.WriteLine($"Input 5 action cards to pass them to play {key} card on {i} turn");

                var argsCount = (CardEnumDict.GetCard(key) as IActionCard)!.ArgsCount;

                for (int k = 0; k < argsCount; k++)
                {
                    playPhase.CardsArguments[key][k] = AskFromConsole();
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
    public static int TotalCardsArgumentsCount = StrategyGenome.KingdomActionPiles.Select(p =>
        CardEnumDict.GetCard(p)).OfType<IActionCard>().Sum(c => c.ArgsCount);

    public static int intSize = PlayOrderCount + TotalCardsArgumentsCount;

    public CardEnum[] PlayOrder { get; set; } = new CardEnum[PlayOrderCount];

    public Dictionary<CardEnum, CardEnum[]> CardsArguments { get; set; } = StrategyGenome.KingdomActionPiles.Select(p =>
        CardEnumDict.GetCard(p)).OfType<IActionCard>().ToDictionary(k => k.CardTypeId, c => new CardEnum[c.ArgsCount]);
}
