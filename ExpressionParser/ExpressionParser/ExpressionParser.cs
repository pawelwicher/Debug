using System;
using System.Linq;
using System.Collections.Generic;

namespace ExpressionParser
{
    public class ExpressionParser2
    {
        private const string UnaryMinus = "\x80";

        private const string InvalidOperandError = "Invalid operand";
        private const string OperandExpectedError = "Operand expected";
        private const string OperatorExpectedError = "Operator expected";
        private const string UnmatchedClosingParenthesisError = "Closing parenthesis without matching open parenthesis";
        private const string MultipleDecimalPointsError = "Operand contains multiple decimal points";
        private const string UnexpectedCharacterError = "Unexpected character encountered \"{0}\"";
        private const string UndefinedSymbolError = "Undefined symbol \"{0}\"";
        private const string UndefinedFunctionError = "Undefined function \"{0}\"";
        private const string ClosingParenthesisExpectedError = "Closing parenthesis expected";
        private const string WrongParametersCountError = "Wrong number of function parameters";

        public decimal EvaluateExpression(string expression)
        {
            return this.EvaluateTokens(this.TokenizeExpression(expression));
        }

        private decimal EvaluateTokens(IEnumerable<string> tokens)
        {
            var stack = new Stack<decimal>();
            decimal temp1 = 0M, temp2 = 0M; ;

            foreach (string token in tokens)
            {
                var count = token.Count(c => char.IsDigit(c) || c == '.');

                if (count == token.Length)
                {
                    stack.Push(decimal.Parse(token));
                }
                else if (token == "+")
                {
                    stack.Push(stack.Pop() + stack.Pop());
                }
                else if (token == "-")
                {
                    temp1 = stack.Pop();
                    temp2 = stack.Pop();
                    stack.Push(temp2 - temp1);
                }
                else if (token == "*")
                {
                    stack.Push(stack.Pop() * stack.Pop());
                }
                else if (token == "/")
                {
                    temp1 = stack.Pop();
                    temp2 = stack.Pop();
                    stack.Push(temp2 / temp1);
                }
                else if (token == UnaryMinus)
                {
                    stack.Push(-stack.Pop());
                }
            }

            return stack.Count > 0 ? stack.Pop() : 0M;
        }

        private IEnumerable<string> TokenizeExpression(string expression)
        {
            var tokens = new List<string>();
            var stack = new Stack<string>();
            var expressionStream = new ExpressionStream(expression);
            var tokenType = TokenType.None;
            var parenthesisCount = 0;
            string temp;

            while (!expressionStream.EndOfText)
            {
                if (char.IsWhiteSpace(expressionStream.Peek()))
                {
                }
                else if (expressionStream.Peek() == '(')
                {
                    if (tokenType == TokenType.Operand)
                    {
                        throw new ExpressionParserException(OperatorExpectedError, expressionStream.Position);
                    }

                    if (tokenType == TokenType.UnaryOperator)
                    {
                        tokenType = TokenType.Operator;
                    }

                    stack.Push(expressionStream.Peek().ToString());
                    parenthesisCount++;
                }
                else if (expressionStream.Peek() == ')')
                {
                    if (tokenType != TokenType.Operand)
                    {
                        throw new ExpressionParserException(OperandExpectedError, expressionStream.Position);
                    }

                    if (parenthesisCount == 0)
                    {
                        throw new ExpressionParserException(UnmatchedClosingParenthesisError, expressionStream.Position);
                    }

                    temp = stack.Pop();

                    while (temp != "(")
                    {
                        tokens.Add(temp);
                        temp = stack.Pop();
                    }

                    parenthesisCount--;
                }
                else if ("+-*/".Contains(expressionStream.Peek()))
                {
                    if (tokenType == TokenType.Operand)
                    {
                        var currPrecedence = GetPrecedence(expressionStream.Peek().ToString());

                        while (stack.Count > 0 && GetPrecedence(stack.Peek()) >= currPrecedence)
                        {
                            tokens.Add(stack.Pop());
                        }

                        stack.Push(expressionStream.Peek().ToString());
                        tokenType = TokenType.Operator;
                    }
                    else if (tokenType == TokenType.UnaryOperator)
                    {
                        throw new ExpressionParserException(OperandExpectedError, expressionStream.Position);
                    }
                    else
                    {
                        if (expressionStream.Peek() == '-')
                        {
                            stack.Push(UnaryMinus);
                            tokenType = TokenType.UnaryOperator;
                        }
                        else if (expressionStream.Peek() == '+')
                        {
                            tokenType = TokenType.UnaryOperator;
                        }
                        else
                        {
                            throw new ExpressionParserException(OperandExpectedError, expressionStream.Position);
                        }
                    }
                }
                else if (char.IsDigit(expressionStream.Peek()) || expressionStream.Peek() == '.')
                {
                    if (tokenType == TokenType.Operand)
                    {
                        throw new ExpressionParserException(OperatorExpectedError, expressionStream.Position);
                    }

                    temp = this.ParseNumberToken(expressionStream);
                    tokens.Add(temp);
                    tokenType = TokenType.Operand;
                    continue;
                }
                else
                {
                    if (tokenType == TokenType.Operand)
                    {
                        throw new ExpressionParserException(OperatorExpectedError, expressionStream.Position);
                    }

                    if (!(char.IsLetter(expressionStream.Peek()) || expressionStream.Peek() == '_'))
                    {
                        throw new ExpressionParserException(string.Format(UnexpectedCharacterError, expressionStream.Peek()), expressionStream.Position);
                    }

                    var symbolPosition = expressionStream.Position;
                    temp = this.ParseSymbolToken(expressionStream);
                    expressionStream.MovePastWhitespace();

                    decimal result;

                    if (expressionStream.Peek() == '(')
                    {
                        result = this.EvaluateFunction(expressionStream, temp, symbolPosition);
                    }
                    else
                    {
                        result = this.EvaluateSymbol(temp, symbolPosition);
                    }

                    if (result < 0)
                    {
                        stack.Push(UnaryMinus);
                        result = Math.Abs(result);
                    }

                    tokens.Add(result.ToString());
                    tokenType = TokenType.Operand;
                    continue;
                }

                expressionStream.MoveAhead();
            }


            if (tokenType == TokenType.Operator || tokenType == TokenType.UnaryOperator)
            {
                throw new ExpressionParserException(OperandExpectedError, expressionStream.Position);
            }

            if (parenthesisCount > 0)
            {
                throw new ExpressionParserException(ClosingParenthesisExpectedError, expressionStream.Position);
            }

            while (stack.Count > 0)
            {
                tokens.Add(stack.Pop());
            }

            return tokens;
        }

        private string ParseNumberToken(ExpressionStream expressionStream)
        {
            var hasDecimal = false;
            var start = expressionStream.Position;

            while (char.IsDigit(expressionStream.Peek()) || expressionStream.Peek() == '.')
            {
                if (expressionStream.Peek() == '.')
                {
                    if (hasDecimal)
                    {
                        throw new ExpressionParserException(MultipleDecimalPointsError, expressionStream.Position);
                    }

                    hasDecimal = true;
                }

                expressionStream.MoveAhead();
            }

            var token = expressionStream.Extract(start, expressionStream.Position);

            if (token == ".")
            {
                throw new ExpressionParserException(InvalidOperandError, expressionStream.Position - 1);
            }

            return token;
        }

        private string ParseSymbolToken(ExpressionStream expressionStream)
        {
            var start = expressionStream.Position;

            while (char.IsLetterOrDigit(expressionStream.Peek()) || expressionStream.Peek() == '_')
            {
                expressionStream.MoveAhead();
            }

            return expressionStream.Extract(start, expressionStream.Position);
        }

        private IEnumerable<decimal> ParseParameters(ExpressionStream expressionStream)
        {
            expressionStream.MoveAhead();

            var parameters = new List<decimal>();

            expressionStream.MovePastWhitespace();

            if (expressionStream.Peek() != ')')
            {
                var paramStart = expressionStream.Position;
                var parenCount = 1;

                while (!expressionStream.EndOfText)
                {
                    if (expressionStream.Peek() == ',')
                    {
                        if (parenCount == 1)
                        {
                            parameters.Add(EvaluateParameter(expressionStream, paramStart));
                            paramStart = expressionStream.Position + 1;
                        }
                    }

                    if (expressionStream.Peek() == ')')
                    {
                        parenCount--;
                        if (parenCount == 0)
                        {
                            parameters.Add(EvaluateParameter(expressionStream, paramStart));
                            break;
                        }
                    }
                    else if (expressionStream.Peek() == '(')
                    {
                        parenCount++;
                    }

                    expressionStream.MoveAhead();
                }
            }

            if (expressionStream.Peek() != ')')
            {
                throw new ExpressionParserException(ClosingParenthesisExpectedError, expressionStream.Position);
            }

            expressionStream.MoveAhead();

            return parameters;
        }

        private decimal EvaluateParameter(ExpressionStream expressionStream, int paramStart)
        {
            try
            {
                return this.EvaluateExpression(expressionStream.Extract(paramStart, expressionStream.Position));
            }
            catch (ExpressionParserException ex)
            {
                ex.Position += paramStart;
                throw ex;
            }
        }

        private decimal EvaluateFunction(ExpressionStream expressionStream, string name, int pos)
        {
            var result = 0M;
            var parameters = this.ParseParameters(expressionStream).ToList();
            var status = FunctionStatus.UndefinedFunction;

            if (name == "add")
            {
                if (parameters.Count == 2)
                {
                    status = FunctionStatus.Correct;
                    result = parameters[0] + parameters[1];
                }
                else
                {
                    status = FunctionStatus.WrongParameterCount;
                }
            }

            if (status == FunctionStatus.UndefinedFunction)
            {
                throw new ExpressionParserException(string.Format(UndefinedFunctionError, name), pos);
            }

            if (status == FunctionStatus.WrongParameterCount)
            {
                throw new ExpressionParserException(WrongParametersCountError, pos);
            }

            return result;
        }

        private decimal EvaluateSymbol(string name, int pos)
        {
            var result = 0M;
            SymbolStatus status;

            if (name == "x")
            {
                status = SymbolStatus.Correct;
                result = 42M;
            }
            else
            {
                status = SymbolStatus.UndefinedSymbol;
            }

            if (status == SymbolStatus.UndefinedSymbol)
            {
                throw new ExpressionParserException(String.Format(UndefinedSymbolError, name), pos);
            }

            return result;
        }

        private int GetPrecedence(string operatorSymbol)
        {
            switch (operatorSymbol)
            {
                case "+":
                case "-": return 1;
                case "*":
                case "/": return 2;
                case UnaryMinus: return 10;
            }

            return 0;
        }
    }
}