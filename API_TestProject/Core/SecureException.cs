namespace API_TestProject.Core
{
    public class SecureException : Exception
    {
        public SecureException(string message) : base(message) { }
    }
}
