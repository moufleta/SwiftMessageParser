using SwiftMessageParser.Models.SwiftMessageTags.Contracts;

namespace SwiftMessageParser.Models.SwiftMessageTags
{
    public class Narrative : ITag
    {
        public int Id { get; set; }

        public string TagCode => "79";

        public string TagValue { get; set; }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}