namespace SwiftMessageParser.Business
{
    public static class TagValidator
    {
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

        public static bool IsValidFormat(int expectedLength, string tagValue)
        {
            if (expectedLength != tagValue.Length)
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

        public static bool IsValidSwiftCharacter(char character)
        {
            string allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789/-?:().,'+ \r\n";

            return allowedCharacters.Contains(character);
        }
    }
}