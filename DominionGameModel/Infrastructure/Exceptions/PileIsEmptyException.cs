using GameModel.Cards;

namespace GameModel.Infrastructure.Exceptions
{
    public class PileIsEmptyException : BaseDominionException
    {
        public CardEnum[] CardEnum { get; set; }
        public PileIsEmptyException(params CardEnum[] cardEnum) : base(ExceptionsEnum.PileIsEmpty, "Empty piles" + string.Join(", ", cardEnum))
        {
            CardEnum = cardEnum;
        }
    }
}
