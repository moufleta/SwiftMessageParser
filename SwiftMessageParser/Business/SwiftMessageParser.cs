using SwiftMessageParser.Business.Exceptions;
using SwiftMessageParser.Models.SwiftMessageTags;
using SwiftMessageParser.Models.SwiftMessageTags.Contracts;
using System.Text;

namespace SwiftMessageParser.Business
{
    public class SwiftMessageParser
    {
        public const int Block1 = 1;
        public const int Block2 = 2;
        public const int Block3 = 3;
        public const int Block4 = 4;

        public void ParseSwiftMessageFile(string fileContent)
        {
            CharStream stream = new CharStream(fileContent);

            // Section related to Block 1 and 2
            string block1 = ParseBlock(stream, Block1);
            string block2 = ParseBlock(stream, Block2);

            // Section related to Block 3
            string block3 = ParseBlock(stream, Block3);
            var dictionary = ParseBlock3(new CharStream(block3));

            // Section related to Block 4
            string block4 = ParseBlock(stream, Block4);

            IList<ITag> tags = ParseBlock4(new CharStream(block4));

            // ...
        }

        private Dictionary<int, string> ParseBlock3(CharStream stream)
        {
            var dictionary = new Dictionary<int, string>();

            while (!stream.IsEndOfStream)
            {
                string subblockBody = GetBlock(stream, out int subblockIdentifier);

                if (!dictionary.ContainsKey(subblockIdentifier))
                {
                    dictionary.Add(subblockIdentifier, subblockBody);
                }
                else
                {
                    throw new SyntaxException(ExceptionMessage.DuplicateSubblock);
                }
            }

            return dictionary;
        }

        private IList<ITag> ParseBlock4(CharStream stream)
        {
            IList<ITag> tags = new List<ITag>();
            int narrativeCount = 0;
            int relatedReferenceCount = 0;
            int transactionReferenceNumberCount = 0;

            while (!stream.IsEndOfStream)
            {
                var tag = ParseBlock4Tag(stream);

                if (tag == null)
                {
                    break;
                }

                switch (tag.TagCode)
                {
                    case "20":
                        transactionReferenceNumberCount++;

                        if (transactionReferenceNumberCount == 1)
                        {
                            tags.Add(tag);
                        }
                        else
                        {
                            throw new SyntaxException(string.Format(ExceptionMessage.MultipleTags, "TransactionReferenceNumber"));
                        }

                        break;
                    case "21":
                        relatedReferenceCount++;

                        if (relatedReferenceCount == 1)
                        {
                            tags.Add(tag);
                        }
                        else
                        {
                            throw new SyntaxException(string.Format(ExceptionMessage.MultipleTags, "RelatedReference"));
                        }

                        break;
                    case "79":
                        narrativeCount++;
                        tags.Add(tag);

                        break;
                    default:
                        break;
                }
            }

            if (narrativeCount < 1 || transactionReferenceNumberCount != 1)
            {
                throw new SyntaxException(ExceptionMessage.MandatoryTagsMissing);
            }

            return tags;
        }

        private ITag ParseBlock4Tag(CharStream stream)
        {
            char c;
            ITag tag = null;

            while (true)
            {
                c = stream.GetNextChar();

                if (c == '\n')
                {
                    continue;
                }
                else if (c == ':')
                {
                    break;
                }
                else if (c == '-')
                {
                    return tag;
                }
                else
                {
                    throw new SyntaxException(ExceptionMessage.UnrecognizedCharacter);
                }
            }

            int tagIdentifier = ParseBlockIdentifier(stream);

            switch (tagIdentifier)
            {
                case 20:
                    tag = new TransactionReferenceNumber();

                    break;
                case 21:
                    tag = new RelatedReference();

                    break;
                case 79:
                    tag = new Narrative();

                    break;
                default:
                    throw new SyntaxException(ExceptionMessage.InvalidTag);
            }

            StringBuilder content = new StringBuilder();

            while (true)
            {
                c = stream.GetNextChar();

                if (c == '\n')
                {
                    tag.TagValue = content.ToString();

                    break;
                }
                else if (c == (char) 0)
                {
                    throw new SyntaxException(string.Format(ExceptionMessage.UnexpectedEndOfText, tagIdentifier));
                }
                else
                {
                    content.Append(c);
                }
            }

            return tag;
        }

        private int ParseBlockIdentifier(CharStream stream)
        {
            bool foundAtLeastOneDigit = false;
            char c;
            int result = 0;

            while (!stream.IsEndOfStream)
            {
                c = stream.GetNextChar();

                if (Char.IsDigit(c))
                {
                    result = result * 10 + (int) (c - '0');

                    foundAtLeastOneDigit = true;
                }
                else if (c == ':')
                {
                    if (!foundAtLeastOneDigit)
                    {
                        throw new SyntaxException(ExceptionMessage.UnexpectedColon);
                    }

                    return result;
                }
                else
                {
                    throw new SyntaxException(ExceptionMessage.InvalidCharacter);
                }
            }

            throw new SyntaxException(ExceptionMessage.EndOfStream);
        }

        private string GetBlock(CharStream stream, out int foundBlockIdentifier)
        {
            char c;

            while (true)
            {
                c = stream.GetNextChar();

                if (c == ' ')
                {
                    continue;
                }

                if (c == '{')
                {
                    break;
                }

                throw new SyntaxException(ExceptionMessage.UnrecognizedCharacter);
            }

            foundBlockIdentifier = ParseBlockIdentifier(stream);
            int openCurlyBraceCount = 1;
            StringBuilder content = new StringBuilder();

            while (true)
            {
                c = stream.GetNextChar();

                if (c == '{')
                {
                    openCurlyBraceCount++;
                }
                else if (c == '}')
                {
                    openCurlyBraceCount--;

                    if (openCurlyBraceCount == 0)
                    {
                        break;
                    }
                }
                else if (c == (char) 0)
                {
                    throw new SyntaxException(string.Format(ExceptionMessage.UnexpectedEndOfText, foundBlockIdentifier));
                }
                else
                {
                    content.Append(c);
                }
            }

            return content.ToString();
        }

        private string ParseBlock(CharStream stream, int expectedBlockIdentifier)
        {
            string block = GetBlock(stream, out int foundBlockIdentifier);

            if (foundBlockIdentifier != expectedBlockIdentifier)
            {
                throw new SyntaxException(string.Format(ExceptionMessage.BlockNotFound, expectedBlockIdentifier, foundBlockIdentifier));
            }

            return block;
        }
    }
}