using SwiftMessageParser.Business;
using SwiftMessageParser.Models.SwiftMessageTags.Contracts;
using System.Data.SQLite;

namespace SwiftMessageParser.Models.SwiftMessageTags
{
    public class Narrative : ITag
    {
        private const int MaxLineLength = 50;
        private const int MaxNumberOfLines = 35;
        private const string TagCodeValue = "79";

        public int Id { get; set; }

        public int SwiftMessageId { get; set; }

        public string TagCode => TagCodeValue;

        public string TagValue { get; set; }

        public bool IsValid()
        {
            return Validator.IsValidFormat(MaxLineLength, MaxNumberOfLines, TagValue);
        }

        public SQLiteCommand GenerateInsertCommand(SQLiteConnection connection)
        {
            string insertSql = "INSERT INTO Narrative (SwiftMessageId, TagCode, TagValue) VALUES (@SwiftMessageId, @TagCode, @TagValue)";
            var command = new SQLiteCommand(insertSql, connection);

            command.Parameters.AddWithValue("@SwiftMessageId", SwiftMessageId);
            command.Parameters.AddWithValue("@TagCode", TagCode);
            command.Parameters.AddWithValue("@TagValue", TagValue);
            return command;
        }
    }
}