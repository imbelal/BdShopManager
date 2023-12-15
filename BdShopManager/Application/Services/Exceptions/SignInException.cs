namespace Application.Services.Exceptions
{
    public class SignInException : Exception
    {
        private SignInException() : base("Error occured while signing in user")
        {
        }

        public static SignInException Instance { get; } = new();
    }
}
