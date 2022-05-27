namespace ExpressionParser
{
    public enum TokenType
    {
        None,
        Operand,
        Operator,
        UnaryOperator
    }

    public enum SymbolStatus
    {
        Correct,
        UndefinedSymbol
    }

    public enum FunctionStatus
    {
        Correct,
        UndefinedFunction,
        WrongParameterCount,
        WrongParameterType
    }
}
