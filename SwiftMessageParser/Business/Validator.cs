using SwiftMessageParser.Models.SwiftMessageTags.Contracts;

namespace SwiftMessageParser.Business
{
    /// <summary>
    /// Provides validation methods for Swift message tags and related data.
    /// </summary>
    public static class Validator
    {
        private const int MaxTagValueLength = 16;

        /// <summary>
        /// Determines whether the provided tag value follows network-validated rules.
        /// </summary>
        /// <param name="tagValue">The tag value to be validated.</param>
        /// <returns>True if the tag value follows network-validated rules; otherwise, false.</returns>
        public static bool FollowsNetworkValidatedRules(string tagValue)
        {
            if (tagValue.StartsWith("/") || tagValue.EndsWith("/"))
            {
                return false;
            }

            if (tagValue.Contains("//"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the provided tag value has a valid format based on length constraints.
        /// </summary>
        /// <param name="maxLineLength">The maximum allowed line length.</param>
        /// <param name="maxNumberOfLines">The maximum allowed number of lines.</param>
        /// <param name="tagValue">The tag value to be validated.</param>
        /// <returns>True if the tag value has a valid format; otherwise, false.</returns>
        public static bool IsValidFormat(int maxLineLength, int maxNumberOfLines, string tagValue)
        {
            string[] lines = tagValue.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            if (lines.Length > maxNumberOfLines)
            {
                return false;
            }

            foreach (string line in lines)
            {
                if (line.Length > maxLineLength)
                {
                    return false;
                }

                foreach (char character in line)
                {
                    if (!IsValidSwiftCharacter(character))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the provided tag value has a valid format.
        /// </summary>
        /// <param name="tagValue">The tag value to be validated.</param>
        /// <returns>True if the tag value has a valid format; otherwise, false.</returns>
        public static bool IsValidFormat(string tagValue)
        {
            if (MaxTagValueLength < tagValue.Length)
            {
                return false;
            }

            foreach (char character in tagValue)
            {
                if (!IsValidSwiftCharacter(character))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the provided character is a valid Swift character.
        /// </summary>
        /// <param name="character">The character to be validated.</param>
        /// <returns>True if the character is valid; otherwise, false.</returns>
        public static bool IsValidSwiftCharacter(char character)
        {
            string allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789/-?:().,'+ \r\n";

            return allowedCharacters.Contains(character);
        }

        /// <summary>
        /// Validates a collection of Swift message tags.
        /// </summary>
        /// <param name="tags">The collection of tags to be validated.</param>
        /// <returns>True if all tags are valid; otherwise, false.</returns>
        public static bool ValidateTags(IList<ITag> tags)
        {
            foreach (var tag in tags)
            {
                if (!tag.IsValid())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Validates the Bank Priority Code based on length and format.
        /// </summary>
        /// <param name="bankPriorityCode">The Bank Priority Code to be validated.</param>
        /// <exception cref="ArgumentException">Thrown if the code is invalid.</exception>
        public static void ValidateBankPriorityCode(string bankPriorityCode)
        {
            if (string.IsNullOrEmpty(bankPriorityCode) || bankPriorityCode.Length != 4)
            {
                throw new ArgumentException("BankPriorityCode is invalid. It should consist of exactly 4 characters.");
            }
        }

        /// <summary>
        /// Validates the length of Block 1 based on the expected length of 25 characters.
        /// </summary>
        /// <param name="block1">Block 1 value to be validated.</param>
        /// <exception cref="ArgumentException">Thrown if the length is invalid.</exception>
        public static void ValidateBlock1Length(string block1)
        {
            if (block1.Length != 25)
            {
                throw new ArgumentException("Block1 length is invalid. It should be exactly 25 characters.");
            }
        }

        /// <summary>
        /// Validates the length of Block 2 based on allowed lengths: 17, 18, 20, or 21 characters.
        /// </summary>
        /// <param name="block2">Block 2 value to be validated.</param>
        /// <exception cref="ArgumentException">Thrown if the length is invalid.</exception>
        public static void ValidateBlock2Length(string block2)
        {
            if (block2.Length != 17 && block2.Length != 18 && block2.Length != 20 && block2.Length != 21)
            {
                throw new ArgumentException("Block2 length is invalid. It should be 17, 18, 20, or 21 characters.");
            }
        }

        /// <summary>
        /// Validates the Message User Reference based on length constraints.
        /// </summary>
        /// <param name="messageUserReference">The Message User Reference to be validated.</param>
        /// <exception cref="ArgumentException">Thrown if the reference is invalid.</exception>
        public static void ValidateMessageUserReference(string messageUserReference)
        {
            if (string.IsNullOrEmpty(messageUserReference) || messageUserReference.Length > 16)
            {
                throw new ArgumentException("MessageUserReference is invalid. It cannot be null or empty and should be up to 16 characters long.");
            }
        }
    }
}