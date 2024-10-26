namespace GameModel.Infrastructure.Exceptions
{
    public class BaseDominionException : Exception
    {
        public ExceptionsEnum ExceptionType { get; set; }
        public BaseDominionException(ExceptionsEnum exception, string message = "") : base(exception.ToString())
        {
            ExceptionType = exception;
        }
    }
}
