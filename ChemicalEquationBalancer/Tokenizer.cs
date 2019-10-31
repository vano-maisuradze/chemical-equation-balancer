using System;
using System.Text.RegularExpressions;

namespace ChemicalEquationBalancer
{
    public class Tokenizer
    {
        private readonly string _str;

        /// <summary>
        /// The index of the next character to tokenize
        /// </summary>
        public int Position { get; private set; }

        public Tokenizer(string str)
        {
            _str = str;
            Position = 0;
            SkipSpaces();
        }

        /// <summary>
        /// Returns the next token as a string, or null if the end of the token stream is reached
        /// </summary>
        /// <returns></returns>
        public string Peek()
        {
            if (Position == _str.Length)  // End of stream
                return null;

            var regex = new Regex(@"^([A-Za-z][a-z]*|[0-9]+|[+\-^=()])");

            var match = regex.Match(_str.Substring(Position));
            if (match == null)
                throw new Exception($"Invalid symbol. Position: {Position}");
            return match.Value;
        }

        /// <summary>
        /// Returns the next token as a string and advances this tokenizer past the token
        /// </summary>
        public string Take()
        {
            var result = Peek();
            if (result == null)
                throw new Exception("Advancing beyond last token");
            Position += result.Length;
            SkipSpaces();
            return result;
        }

        /// <summary>
        /// Takes the next token and checks that it matches the given string, or throws an exception
        /// </summary>
        public void Consume(string s)
        {
            if (Take() != s)
            {
                throw new Exception("Token mismatch");
            }
        }

        private void SkipSpaces()
        {
            var regex = new Regex(@"^[ \t]*");
            var match = regex.Matches(_str.Substring(Position));
            if (match == null)
                throw new Exception("Assertion error");
            Position += match[0].Length;
        }
    }
}



