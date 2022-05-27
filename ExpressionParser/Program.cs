using System;
using System.Diagnostics;

namespace ExpressionParser
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            PerfTests();

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private static void PerfTests()
        {
            var expression = "(2 + 3) * 5 - (2 + 4) * 6 + 3 * 4";
            var p = 0M;
            var q = 0M;
            var t = 0D;
            var x = 0;
            var iterations = 10000000;
            var sw = new Stopwatch();
            var parser1 = new SplitAndMerge.ExpressionParser();
            var parser2 = new ExpressionParser2();

            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                var result = parser1.Evaluate(expression);
                q += result.Scalar;
            }
            sw.Stop();
            Console.WriteLine("SplitAndMerge");
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine();

            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                var result = parser2.EvaluateExpression(expression);
                p += result;
            }
            sw.Stop();
            Console.WriteLine("RPN");
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine();

            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                var result = SimpleExpressionEngine.Parser.Parse(expression).Eval(null);
                t += result;
            }
            sw.Stop();
            Console.WriteLine("Nodes");
            Console.WriteLine(sw.Elapsed);

            Console.WriteLine();

            sw.Restart();
            for (int i = 0; i < iterations; i++)
            {
                var result = (2 + 3) * 5 - (2 + 4) * 6 + 3 * 4;
                x += result;
            }
            sw.Stop();
            Console.WriteLine("C#");
            Console.WriteLine(sw.Elapsed);
        }
    }
}