using GameModel.Cards;

namespace GameModel.Infrastructure.Exceptions
{
    public class MissingCardsInHandException : Exception
    {
        public CardEnum[] CardEnum { get; set; }
        public MissingCardsInHandException(params CardEnum[] cardEnum) : base("Missing cards: " + string.Join(", ", cardEnum))
        {
            CardEnum = cardEnum;
        }
    }
}
