using HtmlTags;
using System;
using System.Collections.Generic;

namespace ChemicalEquationBalancer
{
    public class Utils
    {
        public static readonly long INT_MAX = 9007199254740992;  // 2^53

        // Unicode character constants (because this script file's character encoding is unspecified)
        public static readonly string MINUS = "\u2212";  // Minus sign

        public HtmlTag CreateSpan(string className, string text = "")
        {
            var result = CreateElement("span", text);
            result.AddClass(className);
            return result;
        }

        public HtmlTag CreateElement(string tagName, string text)
        {
            return new HtmlTag(tagName).Text(text);
        }

        internal int CheckedParseInt(string v)
        {
            return int.Parse(v);
        }

        /// <summary>
        /// Returns the sum of the given integers, or throws an exception if the result is too large.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal int CheckedAdd(int x, int y)
        {
            return CheckOverflow(x + y);
        }

        /// <summary>
        /// Returns the product of the given integers, or throws an exception if the result is too large.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal int CheckedMultiply(int x, int y)
        {
            return CheckOverflow(x * y);
        }

        // Throws an exception if the given integer is too large, otherwise returns it.
        public int CheckOverflow(int x)
        {
            if (Math.Abs(x) >= INT_MAX)
            {
                throw new OverflowException("Arithmetic overflow");
            }
            return x;
        }

        internal Matrix BuildMatrix(Equation eqn)
        {
            var elems = eqn.GetElements();
            var lhs = eqn.LeftSide;
            var rhs = eqn.RightSide;
            var matrix = new Matrix(elems.Count + 1, lhs.Count + rhs.Count + 1);

            var i = 0;
            foreach (var elem in elems)
            {
                var j = 0;
                foreach (var term in lhs)
                {
                    matrix.Set(i, j, term.CountElement(elem));
                    j++;
                }
                foreach (var term in rhs)
                {
                    matrix.Set(i, j, -term.CountElement(elem));
                    j++;
                }
                i++;
            }
            return matrix;
        }

        internal void Solve(Matrix matrix)
        {
            matrix.GaussJordanEliminate();

            int countNonzeroCoeffs(int row)
            {
                var count = 0;
                for (var i = 0; i < matrix.NumCols; i++)
                {
                    if (matrix.Get(row, i) != 0)
                    {
                        count++;
                    }
                }
                return count;
            }

            // Find row with more than one non-zero coefficient
            int i;
            for (i = 0; i < matrix.NumRows - 1; i++)
            {
                if (countNonzeroCoeffs(i) > 1)
                {
                    break;
                }
            }
            if (i == matrix.NumRows - 1)
            {
                throw new BalancerException("All-zero solution");  // Unique solution with all coefficients zero
            }

            // Add an inhomogeneous equation
            matrix.Set(matrix.NumRows - 1, i, 1);
            matrix.Set(matrix.NumRows - 1, matrix.NumCols - 1, 1);

            matrix.GaussJordanEliminate();
        }

        internal List<int> ExtractCoefficients(Matrix matrix)
        {
            var rows = matrix.NumRows;
            var cols = matrix.NumCols;

            if (cols - 1 > rows || matrix.Get(cols - 2, cols - 2) == 0)
            {
                throw new BalancerException("Multiple independent solutions");
            }

            var lcm = 1;
            for (var i = 0; i < cols - 1; i++)
            {
                lcm = this.CheckedMultiply(lcm / this.Gcd(lcm, matrix.Get(i, i)), matrix.Get(i, i));
            }

            var coefs = new List<int>();
            var allzero = true;
            for (var i = 0; i < cols - 1; i++)
            {
                var coef = CheckedMultiply(lcm / matrix.Get(i, i), matrix.Get(i, cols - 1));
                coefs.Add(coef);
                allzero = allzero && coef == 0;
            }
            if (allzero)
            {
                throw new BalancerException("Assertion error: All-zero solution");
            }
            return coefs;
        }

        internal void CheckAnswer(Equation eqn, List<int> coefs)
        {
            if (coefs.Count != eqn.LeftSide.Count + eqn.RightSide.Count)
                throw new BalancerException("Assertion error: Mismatched length");

            var allzero = true;
            foreach (var coef in coefs)
            {
                allzero = allzero && coef == 0;
            }
            if (allzero)
                throw new BalancerException("Assertion error: All-zero solution");

            foreach (var elem in eqn.GetElements())
            {
                var sum = 0;
                var j = 0;
                foreach (var term in eqn.LeftSide)
                {
                    sum = CheckedAdd(sum, CheckedMultiply(term.CountElement(elem), coefs[j]));
                    j++;
                }
                foreach (var term in eqn.RightSide)
                {
                    sum = CheckedAdd(sum, CheckedMultiply(term.CountElement(elem), -coefs[j]));
                    j++;
                }
                if (sum != 0)
                {
                    throw new BalancerException("Assertion error: Incorrect balance");
                }
            }
        }

        /// <summary>
        /// Returns the greatest common divisor of the given integers.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal int Gcd(int x, int y)
        {
            x = Math.Abs(x);
            y = Math.Abs(y);
            while (y != 0)
            {
                var z = x % y;
                x = y;
                y = z;
            }
            return x;
        }

    }
}
