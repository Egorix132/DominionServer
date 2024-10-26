namespace GameModel.Infrastructure.Exceptions
{
    public enum ExceptionsEnum
    {
        PileIsEmpty = 0,
        MissingCard = 1,
        MissingArguments = 2,
        NotYourTurn = 3,
        DontHaveActions = 4,
        DontHaveBuy = 5,
        NotEnoughMoney = 6,
        InnerException = 7,
    }
}
