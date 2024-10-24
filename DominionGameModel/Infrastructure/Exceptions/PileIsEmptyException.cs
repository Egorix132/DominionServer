using GameModel.Cards;

namespace GameModel.Infrastructure.Exceptions
{
    public class PileIsEmptyException : Exception
    {
        public CardEnum[] CardEnum { get; set; }
        public PileIsEmptyException(params CardEnum[] cardEnum) : base(string.Join(", ", cardEnum))
        {
            CardEnum = cardEnum;
        }
    }
}
