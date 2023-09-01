namespace SwiftMessageParser.Business
{
    public class CharStream
    {
        private int position;
        private readonly string content;

        public CharStream(string inputContent)
        {
            content = inputContent;
            position = 0;
        }

        public bool IsEndOfStream
        {
            get { return position >= content.Length; }
        }

        public char GetNextChar()
        {
            if (IsEndOfStream)
            {
                return (char) 0;
            }

            return content[position++];
        }
    }
}