using SwiftMessageParser.Business.Contracts;
using SwiftMessageParser.Business.Exceptions;
using SwiftMessageParser.Data.Contracts;
using SwiftMessageParser.Models;
using SwiftMessageParser.Models.SwiftMessageBlocks;
using SwiftMessageParser.Models.SwiftMessageTags;
using SwiftMessageParser.Models.SwiftMessageTags.Contracts;
using System.Text;

namespace SwiftMessageParser.Business
{
    /// <summary>
    /// Represents a parser for Swift messages.
    /// </summary>
    public class Parser : IParser
    {
        private readonly ISwiftMessageRepository swiftMessageRepository;

        public Parser(ISwiftMessageRepository swiftMessageRepository)
        {
            this.swiftMessageRepository = swiftMessageRepository;
        }

        /// <summary>
        /// Parses the content of a SWIFT message file and processes it, storing the parsed data in the database.
        /// </summary>
        /// <param name="fileContent">The content of the SWIFT message file.</param>
        public void ParseSwiftMessageFile(string fileContent)
        {
            MyLogger.GetInstance().Info("Parsing SWIFT message file...");

            CharStream stream = new CharStream(fileContent);

            string block1 = ParseBlock(stream, 1);
            Validator.ValidateBlock1Length(block1);
            MyLogger.GetInstance().Info("Block 1 parsed successfully.");

            string block2 = ParseBlock(stream, 2);
            Validator.ValidateBlock2Length(block2);
            MyLogger.GetInstance().Info("Block 2 parsed successfully.");

            string block3 = ParseBlock(stream, 3);
            var dictionary = ParseBlock3(new CharStream(block3));

            if (dictionary.ContainsKey(113))
            {
                Validator.ValidateBankPriorityCode(dictionary[113]);
            }

            Validator.ValidateMessageUserReference(dictionary[108]);
            MyLogger.GetInstance().Info("Block 3 parsed successfully.");

            string block4 = ParseBlock(stream, 4);
            IList<ITag> tags = ParseBlock4(new CharStream(block4));

            SwiftMessage swiftMessage = new SwiftMessage();

            if (Validator.ValidateTags(tags))
            {
                swiftMessage.Tags = tags;
            }

            MyLogger.GetInstance().Info("Block 4 parsed successfully.");

            swiftMessage.CreatedOn = DateTime.UtcNow;
            swiftMessage.BasicHeader = new BasicHeader(block1);
            swiftMessage.ApplicationHeader = new ApplicationHeader(block2);
            swiftMessage.UserHeader = new UserHeader(dictionary);
            swiftMessageRepository.SaveSwiftMessage(swiftMessage);
        }

        /// <summary>
        /// Parses the third block of a SWIFT message.
        /// </summary>
        /// <param name="stream">The character stream containing the third block.</param>
        /// <returns>A dictionary containing subblock identifiers and their corresponding content.</returns>
        private Dictionary<int, string> ParseBlock3(CharStream stream)
        {
            MyLogger.GetInstance().Info("Parsing the third block of the SWIFT message...");
            var dictionary = new Dictionary<int, string>();

            while (!stream.IsEndOfStream)
            {
                string subblockBody = GetBlock(stream, out int subblockIdentifier);

                if (string.IsNullOrEmpty(subblockBody))
                {
                    MyLogger.GetInstance().Error("Required input is missing.");
                    throw new SyntaxException(ExceptionMessage.RequiredInputMissing);
                }

                if (!dictionary.ContainsKey(subblockIdentifier))
                {
                    dictionary.Add(subblockIdentifier, subblockBody);
                    MyLogger.GetInstance().Info($"Subblock {subblockIdentifier} parsed successfully.");
                }
                else
                {
                    MyLogger.GetInstance().Error($"Duplicate subblock {subblockIdentifier} found.");
                    throw new SyntaxException(ExceptionMessage.DuplicateSubblock);
                }
            }

            MyLogger.GetInstance().Info("Parsing of the third block completed.");
            return dictionary;
        }

        /// <summary>
        /// Parses the fourth block of a SWIFT message.
        /// </summary>
        /// <param name="stream">The character stream containing the fourth block.</param>
        /// <returns>A list of SWIFT message tags parsed from the fourth block.</returns>
        private IList<ITag> ParseBlock4(CharStream stream)
        {
            MyLogger.GetInstance().Info("Parsing the fourth block of the SWIFT message...");

            IList<ITag> tags = new List<ITag>();
            int narrativeCount = 0;
            int relatedReferenceCount = 0;
            int transactionReferenceNumberCount = 0;

            while (!stream.IsEndOfStream)
            {
                var tag = ParseBlock4Tag(stream);

                if (tag == null)
                {
                    MyLogger.GetInstance().Info("End of fourth block reached.");

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
                            MyLogger.GetInstance().Error("Multiple 'TransactionReferenceNumber' tags found.");
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
                            MyLogger.GetInstance().Error("Multiple 'RelatedReference' tags found.");
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
                MyLogger.GetInstance().Error("Mandatory tags missing.");
                throw new SyntaxException(ExceptionMessage.MandatoryTagsMissing);
            }

            MyLogger.GetInstance().Info("Parsing of the fourth block completed.");
            return tags;
        }

        /// <summary>
        /// Parses a tag within the fourth block of a SWIFT message.
        /// </summary>
        /// <param name="stream">The character stream containing the tag.</param>
        /// <returns>An instance of a specific tag type representing the parsed tag.</returns>
        private ITag ParseBlock4Tag(CharStream stream)
        {
            char c;
            ITag tag = null;
            MyLogger.GetInstance().Info("Parsing a tag within the fourth block of the SWIFT message...");

            // Loop to find the beginning of a tag (indicated by ':')
            while (true)
            {
                c = stream.GetNextChar();

                if (c == '\n')
                {
                    continue;
                }
                else if (c == '\r')
                {
                    continue;
                }
                else if (c == ':')
                {
                    MyLogger.GetInstance().Info("Beginning of a tag found.");

                    break; // Found the beginning of a tag
                }
                else if (c == '-')
                {
                    MyLogger.GetInstance().Info("Tag separator '-' found; indicating the end of tags.");
                    return null; // Tag separator '-' indicates the end of tags
                }
                else
                {
                    MyLogger.GetInstance().Error("Unrecognized character encountered: " + c);
                    throw new SyntaxException(ExceptionMessage.UnrecognizedCharacter);
                }
            }

            int tagIdentifier = ParseBlockIdentifier(stream);

            switch (tagIdentifier)
            {
                case 20:
                    MyLogger.GetInstance().Info("Tag identifier 20 found; parsing as 'TransactionReferenceNumber'.");
                    tag = new TransactionReferenceNumber();

                    break;
                case 21:
                    MyLogger.GetInstance().Info("Tag identifier 21 found; parsing as 'RelatedReference'.");
                    tag = new RelatedReference();

                    break;
                case 79:
                    MyLogger.GetInstance().Info("Tag identifier 79 found; parsing as 'Narrative'.");
                    tag = new Narrative();

                    break;
                default:
                    MyLogger.GetInstance().Error("Invalid tag identifier: " + tagIdentifier);
                    throw new SyntaxException(ExceptionMessage.InvalidTag);
            }

            StringBuilder content = new StringBuilder();

            if (tagIdentifier == 79)
            {
                // Handle multi-line content for :79: tag
                bool newLineDetected = false;

                while (true)
                {
                    c = stream.GetNextChar();

                    if (c == '\n')
                    {
                        content.Append(Environment.NewLine);
                        newLineDetected = true;

                        continue;
                    }
                    else if (c == (char) 0)
                    {
                        MyLogger.GetInstance().Error("Unexpected end of text for tag identifier " + tagIdentifier);
                        throw new SyntaxException(string.Format(ExceptionMessage.UnexpectedEndOfText, tagIdentifier));
                    }
                    else if ((c == ':' || c == '-') && newLineDetected)
                    {
                        // If a ':' or '-' is found and a newline was detected,
                        // it indicates the start of a new tag, so we stop accumulating content.
                        stream.SetToPreviousChar();
                        tag.TagValue = content.ToString();

                        MyLogger.GetInstance().Info("Content of tag " + tagIdentifier + " parsed successfully.");

                        break;
                    }
                    else
                    {
                        content.Append(c);
                        newLineDetected = false; // Reset newline detection
                    }
                }
            }
            else
            {
                // Handle single-line content for other tags
                while (true)
                {
                    c = stream.GetNextChar();

                    if (c == '\n')
                    {
                        // If a newline character is encountered, it marks the end of the tag's content
                        tag.TagValue = content.ToString();

                        MyLogger.GetInstance().Info("Content of tag " + tagIdentifier + " parsed successfully.");

                        break;
                    }
                    else if (c == (char) 0)
                    {
                        MyLogger.GetInstance().Error("Unexpected end of text for tag identifier " + tagIdentifier);
                        throw new SyntaxException(string.Format(ExceptionMessage.UnexpectedEndOfText, tagIdentifier));
                    }
                    else
                    {
                        content.Append(c);
                    }
                }
            }

            return tag;
        }

        /// <summary>
        /// Parses and retrieves the identifier of a block within a SWIFT message.
        /// </summary>
        /// <param name="stream">The character stream containing the block's identifier.</param>
        /// <returns>The identifier of the parsed block.</returns>
        private int ParseBlockIdentifier(CharStream stream)
        {
            bool foundAtLeastOneDigit = false;
            char c;
            int result = 0;
            MyLogger.GetInstance().Info("Parsing the identifier of a block within a SWIFT message...");

            // Continue reading characters until a colon (:) or the end of stream is encountered
            while (!stream.IsEndOfStream)
            {
                c = stream.GetNextChar();

                if (Char.IsDigit(c))
                {
                    result = result * 10 + (int) (c - '0'); // Accumulate the numeric identifier

                    foundAtLeastOneDigit = true;
                }
                else if (c == ':')
                {
                    if (!foundAtLeastOneDigit)
                    {
                        MyLogger.GetInstance().Error("Unexpected colon encountered without a numeric identifier.");
                        throw new SyntaxException(ExceptionMessage.UnexpectedColon);
                    }

                    MyLogger.GetInstance().Info("Block identifier parsed successfully: " + result);
                    return result;
                }
                else
                {
                    MyLogger.GetInstance().Error("Invalid character encountered: " + c);
                    throw new SyntaxException(ExceptionMessage.InvalidCharacter);
                }
            }

            MyLogger.GetInstance().Error("End of stream reached without finding a colon.");
            throw new SyntaxException(ExceptionMessage.EndOfStream);
        }

        /// <summary>
        /// Retrieves the content of a block within a SWIFT message.
        /// </summary>
        /// <param name="stream">The character stream containing the block.</param>
        /// <param name="foundBlockIdentifier">The identifier of the found block.</param>
        /// <returns>The content of the block.</returns>
        private string GetBlock(CharStream stream, out int foundBlockIdentifier)
        {
            char c;
            MyLogger.GetInstance().Info("Retrieving the content of a block within a SWIFT message...");

            // Skip whitespace characters until an opening curly brace is found
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

                MyLogger.GetInstance().Error("Unrecognized character encountered: " + c);
                throw new SyntaxException(ExceptionMessage.UnrecognizedCharacter);
            }

            foundBlockIdentifier = ParseBlockIdentifier(stream);
            int openCurlyBraceCount = 1; // Initialize the count of opening curly braces
            StringBuilder content = new StringBuilder();

            // Continue reading characters until the matching closing curly brace is found
            while (true)
            {
                c = stream.GetNextChar();

                if (c == '{')
                {
                    openCurlyBraceCount++; // Increment the count for nested curly braces

                    if (openCurlyBraceCount > 1)
                    {
                        content.Append(c);
                    }
                }
                else if (c == '}')
                {
                    if (openCurlyBraceCount > 1)
                    {
                        content.Append(c);
                    }

                    openCurlyBraceCount--;

                    if (openCurlyBraceCount == 0)
                    {
                        break;
                    }
                }
                else if (c == (char) 0)
                {
                    MyLogger.GetInstance().Error("Unexpected end of text encountered.");
                    throw new SyntaxException(string.Format(ExceptionMessage.UnexpectedEndOfText, foundBlockIdentifier));
                }
                else
                {
                    content.Append(c);
                }
            }

            MyLogger.GetInstance().Info("Content of the block retrieved successfully.");
            return content.ToString();
        }

        /// <summary>
        /// Parses a block within a SWIFT message.
        /// </summary>
        /// <param name="stream">The character stream containing the block.</param>
        /// <param name="expectedBlockIdentifier">The expected identifier of the block.</param>
        /// <returns>The content of the parsed block.</returns>
        private string ParseBlock(CharStream stream, int expectedBlockIdentifier)
        {
            MyLogger.GetInstance().Info("Parsing a block within a SWIFT message...");
            string block = GetBlock(stream, out int foundBlockIdentifier);

            if (foundBlockIdentifier != expectedBlockIdentifier)
            {
                MyLogger.GetInstance().Error($"Block not found. Expected: {expectedBlockIdentifier}, Found: {foundBlockIdentifier}");
                throw new SyntaxException(string.Format(ExceptionMessage.BlockNotFound, expectedBlockIdentifier, foundBlockIdentifier));
            }

            MyLogger.GetInstance().Info("Block parsed successfully.");
            return block;
        }
    }
}