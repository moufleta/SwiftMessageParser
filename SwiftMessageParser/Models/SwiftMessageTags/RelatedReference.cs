using SwiftMessageParser.Models.SwiftMessageTags.Contracts;

namespace SwiftMessageParser.Models.SwiftMessageTags
{
    public class RelatedReference : ITag
    {
        public int Id { get; set; }

        public string TagCode => "21";

        public string TagValue { get; set; }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}