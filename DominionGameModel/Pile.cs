using EvoClient.Utils;
using GameModel.Cards;

namespace GameModel
{
    public class Pile
    {
        public CardEnum Type { get; set; }
        public Stack<ICard> Cards { get; set; } = new();

        public int Count => Cards.Count;

        public int InitialCount;

        public int Cost => CardEnumDict.GetCard(Type).Cost;

        public Pile() { }

        public Pile(CardEnum cardType, int count = 10)
        {
            Type = cardType;
            InitialCount = count;

            for (int i = 0; i < count; i++)
            {
                Cards.Push(CardFactory.CreateCard(cardType));
            }
        }

        public ICard Pop()
        {
            return Cards.Pop();
        }

        public bool IsEmpty()
        {
            return !Cards.Any();
        }
    }
}
