﻿namespace SwiftMessageParser.Business
{
    /// <summary>
    /// Represents a character stream that allows sequential access to characters within an input string.
    /// </summary>
    public class CharStream
    {
        private int position;
        private readonly string content;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharStream"/> class with the provided input string.
        /// </summary>
        /// <param name="input">The input string to be used as the character stream.</param>
        public CharStream(string input)
        {
            content = input;
            position = 0;
        }

        /// <summary>
        /// Gets a boolean value indicating whether the current position in the character stream has reached the end.
        /// </summary>
        public bool IsEndOfStream
        {
            get { return position >= content.Length; }
        }

        /// <summary>
        /// Retrieves the next character from the character stream and advances the current position by one.
        /// </summary>
        /// <returns>The next character from the stream. Returns '\0' if the end of the stream has been reached.</returns>
        public char GetNextChar()
        {
            if (IsEndOfStream)
            {
                return (char) 0;
            }

            return content[position++];
        }

        /// <summary>
        /// Retrieves the previous character from the character stream (the character before the current position)
        /// and decrements the current position by one.
        /// </summary>
        /// <returns>The previous character from the stream. Returns '\0' if the current position is at the beginning of the stream.</returns>
        public char GetPreviousChar()
        {
            if (position <= 0)
            {
                return (char) 0;
            }

            return content[position--];
        }
    }
}