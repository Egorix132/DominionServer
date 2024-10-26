using GameModel.Cards;

namespace GameModel.Infrastructure.Exceptions
{
    public class MissingCardsException : BaseDominionException
    {
        public CardEnum[] CardEnum { get; set; }
        public MissingCardsException(params CardEnum[] cardEnum) : base(ExceptionsEnum.MissingCard, "Missing cards: " + string.Join(", ", cardEnum))
        {
            CardEnum = cardEnum;
        }

        public MissingCardsException(IEnumerable<CardEnum> cardEnum) : base(ExceptionsEnum.MissingCard, "Missing cards: " + string.Join(", ", cardEnum))
        {
            CardEnum = cardEnum.ToArray();
        }
    }
}
