namespace SwiftMessageParser.Models.SwiftMessageBlocks
{
    public class ApplicationHeader
    {
        public string SwiftMessageDirection { get; set; }

        public string SwiftMessageType { get; set; }

        public string RecipientBusinessIdentifierCode { get; set; }

        public string SwiftMessagePriority { get; set; }

        public string DeliveryMonitoring { get; set; }

        public string NonDeliveryNotificationPeriod { get; set; }

        public ApplicationHeader(string block2)
        {
            SwiftMessageDirection = block2.Substring(0, 1);
            SwiftMessageType = block2.Substring(1, 3);
            RecipientBusinessIdentifierCode = block2.Substring(4, 11);
            SwiftMessagePriority = block2.Substring(16, 1);

            if (block2.Length == 18)
            {
                DeliveryMonitoring = block2.Substring(17, 1);
            }
            else if (block2.Length == 20)
            {
                NonDeliveryNotificationPeriod = block2.Substring(17, 3);
            }
            else if (block2.Length == 21)
            {
                DeliveryMonitoring = block2.Substring(17, 1);
                NonDeliveryNotificationPeriod = block2.Substring(18, 3);
            }
        }
    }
}