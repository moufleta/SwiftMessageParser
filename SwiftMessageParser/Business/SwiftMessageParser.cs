using SwiftMessageParser.Business.Exceptions;
using SwiftMessageParser.Models.SwiftMessageTags;
using SwiftMessageParser.Models.SwiftMessageTags.Contracts;
using System.Text;

namespace SwiftMessageParser.Business
{
    public class SwiftMessageParser
    {
        public int GetBlockIdentifier(CharStream stream)
        {
            bool foundAtLeastOneDigit = false;
            int result = 0;

            while (!stream.IsEndOfStream)
            {
                char c = stream.GetNextChar();

                if (Char.IsDigit(c))
                {
                    result = result * 10 + (int)(c - '0');

                    foundAtLeastOneDigit = true;
                }
                else if (c == ':')
                {
                    if (!foundAtLeastOneDigit)
                    {
                        throw new SyntaxException("Unexpected colon encountered while parsing for a block or subblock number.");
                    }

                    return result;
                }
                else
                {
                    throw new SyntaxException("Invalid character encountered while parsing for a block or subblock number.");
                }
            }

            throw new SyntaxException("End of stream reached before finding a colon.");
        }

        // get block's content as string
        public string GetBlock(CharStream stream, out int foundBlockNum)
        {
            char c;

            // Skip whitespace and look for the opening curly brace '{'
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

                throw new SyntaxException("Unrecognized character(s) at the beginning of the block.");
            }

            // Successfully found the opening curly brace, get the block number
            foundBlockNum = GetBlockIdentifier(stream);

            // Start collecting the inner content
            int openCurlyBraceCount = 1;
            StringBuilder content = new StringBuilder();

            while (true)
            {
                c = stream.GetNextChar();

                if (c == '{') // can happen in block 3 > {3:{108:SEC0001016754675}}
                {
                    openCurlyBraceCount++;
                }
                else if (c == '}')
                {
                    openCurlyBraceCount--;

                    if (openCurlyBraceCount == 0)
                        break; // Block is fully closed -> exit
                }
                else if (c == (char)0)
                {
                    throw new SyntaxException("Unexpected end of text while reading block " + foundBlockNum.ToString());
                }
                else
                {
                    // It's not an opening/closing curly brace -> it's part of the block's content/body
                    content.Append(c);
                }
            }

            // Block content is collected, return it
            return content.ToString();
        }

        public ITag GetBlock4Tag(CharStream stream)
        {
            char c;

            while (true)
            {
                c = stream.GetNextChar();

                if (c == '-')
                {
                    return null;
                }

                if (c == '\n')
                {
                    continue;
                }

                if (c == ':')
                {
                    break; // tag opening
                }

                throw new SyntaxException("Unrecognized character(s) at the beginning of the block.");
            }

            // you're at the opening semicolon (:) of the tag before you execute this step
            int tagCode = GetBlockIdentifier(stream);

            ITag tag = null;

            switch (tagCode)
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
                    throw new SyntaxException("Use of incorrect tag for MT799 message");
            }

            // Start collecting tag value content
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
                    throw new SyntaxException("Unexpected end of text while reading block " + tagCode.ToString());
                }
                else
                {
                    content.Append(c);
                }
            }

            // Block content is collected, return it
            return tag;
        }

        private string ParseBlock(CharStream stream, int expectedBlockNumber)
        {
            string block = GetBlock(stream, out int foundBlock);

            if (foundBlock != expectedBlockNumber)
            {
                throw new SyntaxException("Expected block " + expectedBlockNumber + ", found block " + foundBlock);
            }

            return block;
        }

        // TODO
        private Dictionary<int, string> ParseUserHeaderBlock(CharStream stream)
        {
            var dict = new Dictionary<int, string>();

            while (!stream.IsEndOfStream)
            {
                string bodySubblock = GetBlock(stream, out int foundSubblock);

                if (!dict.ContainsKey(foundSubblock))
                {
                    dict.Add(foundSubblock, bodySubblock);
                }
                else
                {
                    throw new SyntaxException("Data repetition present");
                }
            }

            return dict;
        }

        // TODO
        private List<ITag> ParseTextBlock(CharStream stream)
        {
            List<ITag> tags = new List<ITag>();
            int transactionReferenceNumberCount = 0;
            int relatedReferenceCount = 0;
            int narrativeCount = 0;

            while (!stream.IsEndOfStream)
            {
                var tag = GetBlock4Tag(stream);

                if (tag == null) break;

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
                            throw new SyntaxException("Can't have more than one TransactionReferenceNumber tag in one message");
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
                            throw new SyntaxException("Can't have more than one RelatedReference tag in one message");
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

            if (transactionReferenceNumberCount != 1 || narrativeCount < 1)
            {
                throw new SyntaxException("Mandatory tags not present in message");
            }

            return tags;
        }

        // receives file content from controller (user input) and reads it
        public void ParseFile(string fileContent)
        {
            CharStream stream = new CharStream(fileContent);

            string block1 = ParseBlock(stream, 1);
            string block2 = ParseBlock(stream, 2);

            // Block 3 has subblocks
            string block3 = ParseBlock(stream, 3);
            var dict = ParseUserHeaderBlock(new CharStream(block3)); // now you have each sub-tag associated with its corresponding value

            // Block 4
            string block4 = ParseBlock(stream, 4);
            List<ITag> tags = ParseTextBlock(new CharStream(block4));
        }
    }
}