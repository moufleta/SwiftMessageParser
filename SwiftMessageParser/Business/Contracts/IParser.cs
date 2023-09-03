namespace SwiftMessageParser.Business.Contracts
{
    public interface IParser
    {
        void ParseSwiftMessageFile(string fileContent);
    }
}