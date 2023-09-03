namespace SwiftMessageParser.Models.SwiftMessageBlocks
{
    public class UserHeader
    {
        private const int BankPriorityCodeIdentifier = 113;
        private const int MessageUserReferenceIdentifier = 108;

        public UserHeader(IDictionary<int, string> dictionary)
        {
            if (dictionary.ContainsKey(BankPriorityCodeIdentifier))
            {
                BankPriorityCode = dictionary[BankPriorityCodeIdentifier];
            }

            MessageUserReference = dictionary[MessageUserReferenceIdentifier];
        }

        public string BankPriorityCode { get; set; }

        public string MessageUserReference { get; set; }
    }
}