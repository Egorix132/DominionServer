namespace GameModel.Infrastructure.Exceptions
{
    public class NotEnoughMoneyException : BaseDominionException
    {
        public int RequiredMoney { get; set; }
        public int HaveMoney { get; set; }
        public NotEnoughMoneyException(int required, int have) : base(ExceptionsEnum.NotEnoughMoney, $"required money: {required}, but you have: {have}")
        {
            RequiredMoney = required;
            HaveMoney = have;
        }
    }
}
