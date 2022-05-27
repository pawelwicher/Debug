using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ExpressionParser.SplitAndMerge
{
    public class ExpressionParser
    {
        private const char PAREN_START = '(';
        private const char PAREN_END = ')';
        private const char SPACE = ' ';
        private const char TAB = '\t';
        private const char END_LINE = '\n';

        public ExpressionCellValue Evaluate(string expression)
        {
            var from = 0;
            expression = Preprocess(expression);
            return Evaluate(expression, ref from, END_LINE);
        }

        private string Preprocess(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentException("Expression is empty");
            }

            var parentheses = 0;
            var result = new StringBuilder(expression.Length);

            for (var i = 0; i < expression.Length; i++)
            {
                var ch = expression[i];

                switch (ch)
                {
                    case SPACE:
                    case TAB:
                    case END_LINE:
                        continue;

                    case PAREN_END:
                        parentheses--;
                        break;

                    case PAREN_START:
                        parentheses++;
                        break;
                }
                result.Append(ch);
            }


            if (parentheses != 0)
            {
                throw new ArgumentException("Unbalanced parentheses");
            }

            expression = result.ToString();

            if (expression.StartsWith('-') || expression.StartsWith('+'))
            {
                expression = '0' + expression;
            }

            return expression;
        }

        private ExpressionCellValue Evaluate(string expression, ref int position, char endChar)
        {
            ExpressionCellValue value;
            var cells = new List<ExpressionCell>();
            var item = string.Empty;

            if (position >= expression.Length || expression[position] == endChar)
            {
                throw new ArgumentException($"Invalid expression: {expression}");
            }

            do
            {
                var currentChar = expression[position++];

                if (StillCollecting(item.ToString(), currentChar, endChar))
                {
                    item += currentChar;

                    if (position < expression.Length && expression[position] != endChar)
                    {
                        continue;
                    }
                }

                if (item.Length == 0 && currentChar == PAREN_START)
                {
                    value = Evaluate(expression, ref position, PAREN_END);
                }
                else if (item == "add")
                {
                    var arg1 = Evaluate(expression, ref position, ',');
                    var arg2 = Evaluate(expression, ref position, ',');
                    var arg3 = Evaluate(expression, ref position, PAREN_END);
                    value = new ExpressionCellValue(arg1.Scalar + arg2.Scalar + arg3.Scalar);
                }
                else
                {
                    if (!decimal.TryParse(item, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal num))
                    {
                        throw new ArgumentException($"Could not parse token: {item}");
                    }

                    value = new ExpressionCellValue(num);
                }

                var action = IsActionValid(currentChar) ? currentChar : UpdateAction(expression, ref position, currentChar, endChar);

                cells.Add(new ExpressionCell(value, action));
                item = string.Empty;

            }
            while (position < expression.Length && expression[position] != endChar);

            if (position < expression.Length && (expression[position] == PAREN_END || expression[position] == endChar))
            {
                position++;
            }

            var baseCell = cells[0];
            var index = 1;

            return Merge(baseCell, ref index, cells);
        }

        private bool StillCollecting(string item, char c, char endPosition)
        {
            char stopCollecting = (endPosition == PAREN_END || endPosition == END_LINE) ? PAREN_END : endPosition;

            return (item.Length == 0 && (c == '-' || c == '+' || c == PAREN_END)) || !(IsActionValid(c) || c == PAREN_START || c == stopCollecting);
        }

        private bool IsActionValid(char ch)
        {
            return ch == '*' || ch == '/' || ch == '+' || ch == '-';
        }

        private char UpdateAction(string item, ref int from, char ch, char to)
        {
            if (from >= item.Length || item[from] == PAREN_END || item[from] == to)
            {
                return PAREN_END;
            }

            var index = from;
            var res = ch;

            while (!IsActionValid(res) && index < item.Length)
            {
                res = item[index++];
            }

            from = IsActionValid(res) ? index : index > from ? index - 1 : from;

            return res;
        }

        private ExpressionCellValue Merge(ExpressionCell current, ref int index, List<ExpressionCell> cells, bool mergeOneOnly = false)
        {
            while (index < cells.Count)
            {
                var next = cells[index++];

                while (!CanMergeCells(current, next))
                {
                    Merge(next, ref index, cells, true);
                }

                MergeCells(current, next);

                if (mergeOneOnly)
                {
                    return current.Value;
                }
            }

            return current.Value;
        }

        private void MergeCells(ExpressionCell left, ExpressionCell right)
        {
            switch (left.Action)
            {
                case '+':
                    left.Value.Scalar += right.Value.Scalar;
                    break;

                case '-':
                    left.Value.Scalar -= right.Value.Scalar;
                    break;

                case '*':
                    left.Value.Scalar *= right.Value.Scalar;
                    break;

                case '/':
                    left.Value.Scalar /= right.Value.Scalar;
                    break;
            }

            left.Action = right.Action;
        }

        private bool CanMergeCells(ExpressionCell left, ExpressionCell right)
        {
            return GetPriority(left.Action) >= GetPriority(right.Action);
        }

        private int GetPriority(char action)
        {
            switch (action)
            {
                case '+':
                case '-': return 2;
                case '*':
                case '/': return 3;
            }

            return 0;
        }
    }
}
