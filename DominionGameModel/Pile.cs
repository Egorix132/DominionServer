using GameModel.Cards;
using GameModel.Infrastructure;
using GameModel.Infrastructure.Attributes;

namespace GameModel
{
    public class Pile
    {
        public CardEnum Type { get; set; }
        internal Stack<ICard> Cards { get; } = new();
        public int Count => Cards.Count;
        public int Cost => Type.GetAttribute<CardCostAttribute>()!.CardCost;

        public Pile(CardEnum cardType, int count = 10)
        {
            Type = cardType;

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
