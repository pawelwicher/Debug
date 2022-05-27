using System;

namespace ExpressionParser
{
    public class ExpressionParserException : Exception
    {
        public int Position { get; set; }

        public ExpressionParserException(string message, int position)
            : base(message)
        {
            this.Position = position;
        }

        public override string Message
        {
            get
            {
                return $"{base.Message} (column {this.Position + 1})";
            }
        }
    }
}
