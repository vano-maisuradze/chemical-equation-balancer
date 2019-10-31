using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChemicalEquationBalancer
{
    public class Matrix
    {
        public int NumCols { get; }
        public int NumRows { get; }

        /// <summary>
        /// Main data (the matrix)
        /// </summary>
        private List<List<int>> _cells;

        public Matrix(int numRows, int numCols)
        {
            if (numRows < 0 || numCols < 0)
                throw new Exception("Illegal argument");

            // Initialize with zeros
            _cells = new List<List<int>>();
            for (var i = 0; i < numRows; i++)
            {
                var row = new List<int>();
                for (var j = 0; j < numCols; j++)
                {
                    row.Add(0);
                }
                _cells.Add(row);
            }

            this.NumRows = numRows;
            this.NumCols = numCols;
        }

        /// <summary>
        /// Returns the value of the given cell in the matrix, where r is the row and c is the column.
        /// </summary>
        public int Get(int r, int c)
        {
            if (r < 0 || r >= NumRows || c < 0 || c >= NumCols)
            {
                throw new IndexOutOfRangeException("Index out of bounds");
            }
            return _cells[r][c];
        }

        /// <summary>
        /// Sets the given cell in the matrix to the given value, where r is the row and c is the column.
        /// </summary>
        public void Set(int r, int c, int val)
        {
            if (r < 0 || r >= NumRows || c < 0 || c >= NumCols)
            {
                throw new IndexOutOfRangeException("Index out of bounds");
            }
            _cells[r][c] = val;
        }

        
        /// <summary>
        /// Swaps the two rows of the given indices in this matrix. The degenerate case of i == j is allowed.
        /// </summary>
        private void SwapRows(int i, int j)
        {
            if (i < 0 || i >= this.NumRows || j < 0 || j >= this.NumRows)
            {
                throw new IndexOutOfRangeException("Index out of bounds");
            }

            var temp = _cells[i];
            _cells[i] = _cells[j];
            _cells[j] = temp;
        }

        /// <summary>
        /// Returns a new row that is the sum of the two given rows. The rows are not indices.
        /// For example, addRow([3, 1, 4], [1, 5, 6]) = [4, 6, 10].
        /// </summary>
        private static List<int> AddRows(List<int> x, List<int> y)
        {
            var utils = new Utils();
            var z = new List<int>();
            for (var i = 0; i < x.Count; i++)
                z.Add(utils.CheckedAdd(x[i], y[i]));
            return z;
        }

        /// <summary>
        /// Returns a new row that is the product of the given row with the given scalar. 
        /// The row is is not an index. For example, multiplyRow([0, 1, 3], 4) = [0, 4, 12].
        /// </summary>
        private static List<int> MultiplyRow(List<int> x, int c)
        {
            var utils = new Utils();
            return x.Select(val => utils.CheckedMultiply(val, c)).ToList();
        }

        /// <summary>
        /// Returns the GCD of all the numbers in the given row. The row is is not an index.
        /// For example, gcdRow([3, 6, 9, 12]) = 3.
        /// </summary>
        private static int GcdRow(List<int> x)
        {
            var result = 0;
            var utils = new Utils();
            foreach (var val in x)
            {
                result = utils.Gcd(val, result);
            }
            return result;
        }

        /// <summary>
        /// Returns a new row where the leading non-zero number (if any) is positive, and the GCD of the row is 0 or 1.
        /// For example, simplifyRow([0, -2, 2, 4]) = [0, 1, -1, -2].
        /// </summary>
        private static List<int> SimplifyRow(List<int> x)
        {
            var sign = 0;
            foreach (var val in x)
            {
                if (val != 0)
                {
                    sign = Math.Sign(val);
                    break;
                }
            }
            if (sign == 0)
            {
                return x;
            }
            var g = GcdRow(x) * sign;
            return x.Select(val => val / g).ToList();
        }

        /// <summary>
        /// Changes this matrix to reduced row echelon form (RREF), 
        /// except that each leading coefficient is not necessarily 1. Each row is simplified.
        /// </summary>
        public void GaussJordanEliminate()
        {
            // Simplify all rows
            var cells = this._cells = this._cells.Select(SimplifyRow).ToList();

            var utils = new Utils();
            // Compute row echelon form (REF)
            var numPivots = 0;
            for (var i = 0; i < NumCols; i++)
            {
                // Find pivot
                var pivotRow = numPivots;
                while (pivotRow < NumRows && cells[pivotRow][i] == 0)
                {
                    pivotRow++;
                }
                if (pivotRow == NumRows)
                {
                    continue;
                }
                var pivot = cells[pivotRow][i];
                SwapRows(numPivots, pivotRow);
                numPivots++;

                // Eliminate below
                for (var j = numPivots; j < NumRows; j++)
                {
                    var g = utils.Gcd(pivot, cells[j][i]);
                    cells[j] = SimplifyRow(AddRows(MultiplyRow(cells[j], pivot / g), MultiplyRow(cells[i], -cells[j][i] / g)));
                }
            }

            // Compute reduced row echelon form (RREF), but the leading coefficient need not be 1
            for (var i = NumRows - 1; i >= 0; i--)
            {
                // Find pivot
                var pivotCol = 0;
                while (pivotCol < NumCols && cells[i][pivotCol] == 0)
                    pivotCol++;
                if (pivotCol == NumCols)
                    continue;
                var pivot = cells[i][pivotCol];

                // Eliminate above
                for (var j = i - 1; j >= 0; j--)
                {
                    var g = utils.Gcd(pivot, cells[j][pivotCol]);
                    cells[j] = SimplifyRow(AddRows(MultiplyRow(cells[j], pivot / g), MultiplyRow(cells[i], -cells[j][pivotCol] / g)));
                }
            }
        }
    }
}
