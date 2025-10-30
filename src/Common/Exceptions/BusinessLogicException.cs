namespace Common.Exceptions
{
    /// <summary>
    /// Base exception for all business logic errors that should be displayed to the user.
    /// Use this exception for domain validation errors and business rule violations.
    /// </summary>
    public class BusinessLogicException : Exception
    {
        public BusinessLogicException(string message) : base(message)
        {
        }

        public BusinessLogicException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
