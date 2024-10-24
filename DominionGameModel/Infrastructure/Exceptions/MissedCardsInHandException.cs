using GameModel.Cards;

namespace GameModel.Infrastructure.Exceptions
{
    public class MissedCardsInHandException : Exception
    {
        public CardEnum[] CardEnum { get; set; }
        public MissedCardsInHandException(params CardEnum[] cardEnum) : base(string.Join(", ", cardEnum))
        {
            CardEnum = cardEnum;
        }
    }
}
