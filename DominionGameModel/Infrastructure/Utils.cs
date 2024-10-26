using GameModel.Cards;
using Newtonsoft.Json;

namespace EvoClient.Utils
{
    static class ArrayExtension
    {
        private static readonly Random _random = new();

        public static T GetRandom<T>(this T[] array)
        {
            int point = _random.Next(0, array.Length);
            return array[point];
        }
    }

    static class CardEnumDict
    {
        public static Dictionary<CardEnum, ICard> Cards;
        static CardEnumDict()
        {
            var text =  File.ReadAllText("dominionCardsEnumJson.json");
            Cards = JsonConvert.DeserializeObject<Dictionary<CardEnum, ICard>>(text, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        }

        public static ICard GetCard(CardEnum type)
        {
            return Cards[type];
        }
    }
}
