namespace SwiftMessageParser.Business.Exceptions
{
    public class SyntaxException : ApplicationException
    {
        public SyntaxException(string message) : base(message) { }
    }
}