namespace API_TestProject.Core.Model
{
    public class SecureException : Exception
    {
        public SecureException(string message) : base(message) { }
    }
}
