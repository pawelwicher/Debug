using System;
using System.Linq;

namespace ExpressionParser
{
    public class ExpressionStream
    {
        private const char NullChar = (char)0;

        public string Text { get; }

        public int Position { get; private set; }

        public bool EndOfText
        {
            get { return (Position >= Text.Length); }
        }

        public ExpressionStream(string expression)
        {
            this.Text = expression;
            this.Position = 0;
        }

        public char Peek()
        {
            return Peek(0);
        }

        public char Peek(int ahead)
        {
            var newPos = this.Position + ahead;

            if (newPos < this.Text.Length)
            {
                return this.Text[newPos];
            }

            return NullChar;
        }

        public string Extract(int start, int end)
        {
            return Text.Substring(start, end - start);
        }

        public void MoveAhead()
        {
            MoveAhead(1);
        }

        public void MoveAhead(int ahead)
        {
            this.Position = Math.Min(this.Position + ahead, this.Text.Length);
        }

        public bool IsInArray(char c, char[] chars)
        {
            return chars.Contains(c);
        }

        public void MovePastWhitespace()
        {
            while (char.IsWhiteSpace(Peek()))
            {
                MoveAhead();
            }
        }
    }
}
