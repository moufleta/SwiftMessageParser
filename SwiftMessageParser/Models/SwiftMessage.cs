using SwiftMessageParser.Models.SwiftMessageBlocks;
using SwiftMessageParser.Models.SwiftMessageTags.Contracts;

namespace SwiftMessageParser.Models
{
    public class SwiftMessage
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public BasicHeader BasicHeader { get; set; }

        public ApplicationHeader ApplicationHeader { get; set; }

        public UserHeader UserHeader { get; set; }

        public IList<ITag> Tags { get; set; } = new List<ITag>();
    }
}