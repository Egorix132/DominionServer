using GameModel.Cards;

namespace GameModel.Infrastructure.Exceptions
{
    public class MissingCardsException : Exception
    {
        public CardEnum[] CardEnum { get; set; }
        public MissingCardsException(params CardEnum[] cardEnum) : base("Missing cards: " + string.Join(", ", cardEnum))
        {
            CardEnum = cardEnum;
        }

        public MissingCardsException(IEnumerable<CardEnum> cardEnum) : base("Missing cards: " + string.Join(", ", cardEnum))
        {
            CardEnum = cardEnum.ToArray();
        }
    }
}
