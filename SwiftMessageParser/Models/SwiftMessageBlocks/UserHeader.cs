namespace SwiftMessageParser.Models.SwiftMessageBlocks
{
    public class UserHeader
    {
        public string PriorityCode { get; set; }

        public string SwiftMessageUserReference { get; set; }

        public UserHeader(IDictionary<int, string> dictionary)
        {
            if (dictionary.ContainsKey(113))
            {
                PriorityCode = dictionary[113];
            }

            SwiftMessageUserReference = dictionary[108];
        }
    }
}