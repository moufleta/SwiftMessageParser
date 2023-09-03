using System.Data.SQLite;

namespace SwiftMessageParser.Models.SwiftMessageTags.Contracts
{
    public interface ITag
    {
        int Id { get; set; }

        int SwiftMessageId { get; set; }

        string TagCode { get; }

        string TagValue { get; set; }

        bool IsValid();

        SQLiteCommand GenerateInsertCommand(SQLiteConnection connection);
    }
}