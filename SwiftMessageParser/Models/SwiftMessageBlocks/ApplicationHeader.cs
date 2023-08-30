namespace SwiftMessageParser.Models.SwiftMessageBlocks
{
    public class ApplicationHeader
    {
        public int Id { get; set; }

        public string MessageDirection { get; set; }

        public string MessageType { get; set; }

        public string RecipientBusinessIdentifierCode { get; set; }

        public string MessagePriority { get; set; }

        public string DeliveryMonitoring { get; set; }

        public string NonDeliveryNotificationPeriod { get; set; }
    }
}