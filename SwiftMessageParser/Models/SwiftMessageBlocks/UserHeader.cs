namespace SwiftMessageParser.Models.SwiftMessageBlocks
{
    public class UserHeader
    {
        public int Id { get; set; }

        public int SwiftMessageId { get; set; }

        public string PriorityCode { get; set; }

        public string SwiftMessageUserReference { get; set; }
    }
}