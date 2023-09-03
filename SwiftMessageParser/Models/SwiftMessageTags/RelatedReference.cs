using SwiftMessageParser.Business;
using SwiftMessageParser.Models.SwiftMessageTags.Contracts;
using System.Data.SQLite;

namespace SwiftMessageParser.Models.SwiftMessageTags
{
    public class RelatedReference : ITag
    {
        private const string TagCodeValue = "21";

        public int Id { get; set; }

        public int SwiftMessageId { get; set; }

        public string TagCode => TagCodeValue;

        public string TagValue { get; set; }

        public bool IsValid()
        {
            if (!Validator.IsValidFormat(TagValue))
            {
                return false;
            }

            if (!Validator.FollowsNetworkValidatedRules(TagValue))
            {
                return false;
            }

            return true;
        }

        public SQLiteCommand GenerateInsertCommand(SQLiteConnection connection)
        {
            string insertSql = "INSERT INTO RelatedReference (SwiftMessageId, TagCode, TagValue) VALUES (@SwiftMessageId, @TagCode, @TagValue)";
            var command = new SQLiteCommand(insertSql, connection);

            command.Parameters.AddWithValue("@SwiftMessageId", SwiftMessageId);
            command.Parameters.AddWithValue("@TagCode", TagCode);
            command.Parameters.AddWithValue("@TagValue", TagValue);
            return command;
        }
    }
}