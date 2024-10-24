namespace GameModel.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CardCostAttribute : Attribute
    {
        public int CardCost { get; }

        public CardCostAttribute(int cost)
        {
            CardCost = cost;
        }
    }
}
