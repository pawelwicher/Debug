using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.SplitAndMerge
{
    public class ExpressionCell
    {
        public ExpressionCellValue Value { get; set; }

        public char Action { get; set; }

        public ExpressionCell(ExpressionCellValue value, char action)
        {
            Value = value;
            Action = action;
        }
    }
}
