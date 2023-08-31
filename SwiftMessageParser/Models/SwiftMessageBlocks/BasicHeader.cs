namespace SwiftMessageParser.Models.SwiftMessageBlocks
{
    public class BasicHeader
    {
        public int Id { get; set; }

        public int SwiftMessageId { get; set; }

        public string ApplicationId { get; set; }

        public string ServiceId { get; set; }

        public string LogicalTerminal { get; set; }

        public string SessionNumber { get; set; }

        public string SequenceNumber { get; set; }
    }
}