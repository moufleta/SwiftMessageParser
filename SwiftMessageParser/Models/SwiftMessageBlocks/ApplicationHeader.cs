namespace SwiftMessageParser.Models.SwiftMessageBlocks
{
    public class ApplicationHeader
    {
        public int Id { get; set; }

        public int SwiftMessageId { get; set; }

        public string SwiftMessageDirection { get; set; }

        public string SwiftMessageType { get; set; }

        public string RecipientBusinessIdentifierCode { get; set; }

        public string SwiftMessagePriority { get; set; }

        public string DeliveryMonitoring { get; set; }

        public string NonDeliveryNotificationPeriod { get; set; }
    }
}