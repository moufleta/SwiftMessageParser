namespace SwiftMessageParser.Models.SwiftMessageBlocks
{
    public class BasicHeader
    {
        public string ApplicationId { get; set; }

        public string ServiceId { get; set; }

        public string LogicalTerminal { get; set; }

        public string SessionNumber { get; set; }

        public string SequenceNumber { get; set; }

        public BasicHeader(string block1)
        {
            ApplicationId = block1.Substring(0, 1);
            ServiceId = block1.Substring(1, 2);
            LogicalTerminal = block1.Substring(3, 12);
            SessionNumber = block1.Substring(15, 4);
            SequenceNumber = block1.Substring(19, 6);
        }
    }
}