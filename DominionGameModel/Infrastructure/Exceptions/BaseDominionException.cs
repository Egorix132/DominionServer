namespace GameModel.Infrastructure.Exceptions
{
    public class BaseDominionException : Exception
    {
        public ExceptionsEnum Exception { get; set; }
        public BaseDominionException(ExceptionsEnum exception) : base(exception.ToString())
        {
            Exception = exception;
        }
    }
}
