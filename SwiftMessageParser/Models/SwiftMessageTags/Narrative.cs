using SwiftMessageParser.Business;
using SwiftMessageParser.Models.SwiftMessageTags.Contracts;

namespace SwiftMessageParser.Models.SwiftMessageTags
{
    public class Narrative : ITag
    {
        private const int MaxLineLength = 50;
        private const int MaxNumberOfLines = 35;
        private const string TagCodeValue = "79";

        public int Id { get; set; }

        public string TagCode => TagCodeValue;

        public string TagValue { get; set; }

        public bool IsValid()
        {
            return TagValidator.IsValidFormat(MaxLineLength, MaxNumberOfLines, TagValue);
        }
    }
}