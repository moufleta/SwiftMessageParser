namespace SwiftMessageParser.Business.Contracts
{
    public interface IMyLogger
    {
        void Debug(string message, string argument = null);

        void Error(string message, string argument = null);

        void Info(string message, string argument = null);

        void Warn(string message, string argument = null);
    }
}