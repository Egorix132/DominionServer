using GameModel.Cards;

namespace GameModel.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CardTypesAttribute : Attribute
    {
        public CardType[] CardTypes { get; }

        public CardTypesAttribute(params CardType[] types)
        {
            CardTypes = types;
        }
    }
}
