namespace SwiftMessageParser.Business.Exceptions
{
    public static class ExceptionMessage
    {
        public const string BlockNotFound = "Expected block {0}, found block {1}.";
        public const string DuplicateSubblock = "Duplicate subblock identifier detected. Data repetition is not allowed.";
        public const string EndOfStream = "End of stream reached.";
        public const string InvalidCharacter = "Invalid character encountered.";
        public const string InvalidTag = "Invalid tag used in the MT799 message. Check the tag code for correctness.";
        public const string MandatoryTagsMissing = "Mandatory tags not present in the message.";
        public const string MultipleTags = "Multiple {0} tags found in a single message. Only one is permitted.";
        public const string UnexpectedColon = "Unexpected colon encountered while parsing for a block or subblock number.";
        public const string UnexpectedEndOfText = "Unexpected end of text while reading block {0}. Ensure that the message content is complete.";
        public const string UnrecognizedCharacter = "Unrecognized character(s) at the beginning of the block.";
    }
}