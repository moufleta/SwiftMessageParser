using SwiftMessageParser.Models;

namespace SwiftMessageParser.Data.Contracts
{
    public interface ISwiftMessageRepository
    {
        void SaveSwiftMessage(SwiftMessage swiftMessage);
    }
}