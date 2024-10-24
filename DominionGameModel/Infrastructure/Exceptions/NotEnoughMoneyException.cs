namespace GameModel.Infrastructure.Exceptions
{
    public class NotEnoughMoneyException : Exception
    {
        public int RequiredMoney { get; set; }
        public int HaveMoney { get; set; }
        public NotEnoughMoneyException(int required, int have) : base($"required money: {required}, but you have: {have}")
        {
            RequiredMoney = required;
            HaveMoney = have;
        }
    }
}
