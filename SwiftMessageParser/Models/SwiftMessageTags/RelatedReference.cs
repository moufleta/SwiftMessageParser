using SwiftMessageParser.Business;
using SwiftMessageParser.Models.SwiftMessageTags.Contracts;

namespace SwiftMessageParser.Models.SwiftMessageTags
{
    public class RelatedReference : ITag
    {
        private const string TagCodeValue = "21";

        public int Id { get; set; }

        public string TagCode => TagCodeValue;

        public string TagValue { get; set; }

        public bool IsValid()
        {
            if (!TagValidator.IsValidFormat(TagValue))
            {
                return false;
            }

            if (!TagValidator.FollowsNetworkValidatedRules(TagValue))
            {
                return false;
            }

            return true;
        }
    }
}