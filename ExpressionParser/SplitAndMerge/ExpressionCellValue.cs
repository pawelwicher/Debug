using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.SplitAndMerge
{
    public class ExpressionCellValue
    {
        public DataValueCollection Data { get; set; }

        public decimal Scalar { get; set; }

        public bool IsScalar { get; }

        public ExpressionCellValue(DataValueCollection data)
        {
            Data = data;
            IsScalar = false;
        }

        public ExpressionCellValue(decimal scalar)
        {
            Scalar = scalar;
            IsScalar = true;
        }
    }
}
