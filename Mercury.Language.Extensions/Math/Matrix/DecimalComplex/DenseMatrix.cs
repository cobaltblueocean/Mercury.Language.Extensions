﻿// <copyright file="DenseMatrix.cs" company="QuickMath.NET">
// QuickMath.NET Numerics, part of the QuickMath.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2013 QuickMath.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.DecimalComplex.Factorization;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Storage;
using MathNet.Numerics.Providers.LinearAlgebra;
using Mercury.Language;
using Mercury.Language.Threading;

namespace MathNet.Numerics.LinearAlgebra.DecimalComplex
{
    using DecimalComplex = System.Numerics.DecimalComplex;

    /// <summary>
    /// A Matrix class with dense storage. The underlying storage is a one dimensional array in column-major order (column by column).
    /// </summary>
    [Serializable]
    [DebuggerDisplay("DenseMatrix {RowCount}x{ColumnCount}-DecimalComplex")]
    public class DenseMatrix : Matrix
    {
        /// <summary>
        /// Number of rows.
        /// </summary>
        /// <remarks>Using this instead of the RowCount property to speed up calculating
        /// a matrix index in the data array.</remarks>
        readonly int _rowCount;

        /// <summary>
        /// Number of columns.
        /// </summary>
        /// <remarks>Using this instead of the ColumnCount property to speed up calculating
        /// a matrix index in the data array.</remarks>
        readonly int _columnCount;

        /// <summary>
        /// Gets the matrix's data.
        /// </summary>
        /// <value>The matrix's data.</value>
        readonly DecimalComplex[] _values;

        /// <summary>
        /// Create a new dense matrix straight from an initialized matrix storage instance.
        /// The storage is used directly without copying.
        /// Intended for advanced scenarios where you're working directly with
        /// storage for performance or interop reasons.
        /// </summary>
        public DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex> storage)
            : base(storage)
        {
            _rowCount = storage.RowCount;
            _columnCount = storage.ColumnCount;
            _values = storage.Data;
        }

        /// <summary>
        /// Create a new square dense matrix with the given number of rows and columns.
        /// All cells of the matrix will be initialized to zero.
        /// </summary>
        /// <exception cref="ArgumentException">If the order is less than one.</exception>
        public DenseMatrix(int order)
            : this(DenseColumnMajorMatrixStorage<DecimalComplex>.OfArray(new DecimalComplex[order, order]))
        {
        }

        /// <summary>
        /// Create a new dense matrix with the given number of rows and columns.
        /// All cells of the matrix will be initialized to zero.
        /// </summary>
        /// <exception cref="ArgumentException">If the row or column count is less than one.</exception>
        public DenseMatrix(int rows, int columns)
            : this(DenseColumnMajorMatrixStorage<DecimalComplex>.OfArray(new DecimalComplex[rows, columns]))
        {
        }

        /// <summary>
        /// Create a new dense matrix with the given number of rows and columns directly binding to a raw array.
        /// The array is assumed to be in column-major order (column by column) and is used directly without copying.
        /// Very efficient, but changes to the array and the matrix will affect each other.
        /// </summary>
        /// <seealso href="http://en.wikipedia.org/wiki/Row-major_order"/>
        public DenseMatrix(int rows, int columns, DecimalComplex[] storage)
            : this(DenseColumnMajorMatrixStorage<DecimalComplex>.OfColumnMajorArray(rows, columns, storage))
        {
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given other matrix.
        /// This new matrix will be independent from the other matrix.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfMatrix(Matrix<DecimalComplex> matrix)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfMatrix(matrix.Storage));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given two-dimensional array.
        /// This new matrix will be independent from the provided array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfArray(DecimalComplex[,] array)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfArray(array));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfIndexed(int rows, int columns, IEnumerable<Tuple<int, int, DecimalComplex>> enumerable)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfIndexedEnumerable(rows, columns, enumerable));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfIndexed(int rows, int columns, IEnumerable<(int, int, DecimalComplex)> enumerable)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfIndexedEnumerable(rows, columns, enumerable.Select(x => new Tuple<int, int, DecimalComplex>(x.Item1, x.Item2, x.Item3)).ToArray()));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable.
        /// The enumerable is assumed to be in column-major order (column by column).
        /// This new matrix will be independent from the enumerable.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfColumnMajor(int rows, int columns, IEnumerable<DecimalComplex> columnMajor)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfColumnMajorEnumerable(rows, columns, columnMajor));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable columns.
        /// Each enumerable in the master enumerable specifies a column.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfColumns(IEnumerable<IEnumerable<DecimalComplex>> data)
        {
            return OfColumnArrays(data.Select(v => v.ToArray()).ToArray());
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable columns.
        /// Each enumerable in the master enumerable specifies a column.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfColumns(int rows, int columns, IEnumerable<IEnumerable<DecimalComplex>> data)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfColumnEnumerables(rows, columns, data));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfColumnArrays(params DecimalComplex[][] columns)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfColumnArrays(columns));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfColumnArrays(IEnumerable<DecimalComplex[]> columns)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfColumnArrays((columns as DecimalComplex[][]) ?? columns.ToArray()));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfColumnVectors(params Vector<DecimalComplex>[] columns)
        {
            var storage = new VectorStorage<DecimalComplex>[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                storage[i] = columns[i].Storage;
            }
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfColumnVectors(storage));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given column vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfColumnVectors(IEnumerable<Vector<DecimalComplex>> columns)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfColumnVectors(columns.Select(c => c.Storage).ToArray()));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable rows.
        /// Each enumerable in the master enumerable specifies a row.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfRows(IEnumerable<IEnumerable<DecimalComplex>> data)
        {
            return OfRowArrays(data.Select(v => v.ToArray()).ToArray());
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given enumerable of enumerable rows.
        /// Each enumerable in the master enumerable specifies a row.
        /// This new matrix will be independent from the enumerables.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfRows(int rows, int columns, IEnumerable<IEnumerable<DecimalComplex>> data)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfRowEnumerables(rows, columns, data));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfRowArrays(params DecimalComplex[][] rows)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfRowArrays(rows));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row arrays.
        /// This new matrix will be independent from the arrays.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfRowArrays(IEnumerable<DecimalComplex[]> rows)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfRowArrays((rows as DecimalComplex[][]) ?? rows.ToArray()));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfRowVectors(params Vector<DecimalComplex>[] rows)
        {
            var storage = new VectorStorage<DecimalComplex>[rows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                storage[i] = rows[i].Storage;
            }
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfRowVectors(storage));
        }

        /// <summary>
        /// Create a new dense matrix as a copy of the given row vectors.
        /// This new matrix will be independent from the vectors.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfRowVectors(IEnumerable<Vector<DecimalComplex>> rows)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfRowVectors(rows.Select(r => r.Storage).ToArray()));
        }

        /// <summary>
        /// Create a new dense matrix with the diagonal as a copy of the given vector.
        /// This new matrix will be independent from the vector.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfDiagonalVector(Vector<DecimalComplex> diagonal)
        {
            var m = new DenseMatrix(diagonal.Count, diagonal.Count);
            m.SetDiagonal(diagonal);
            return m;
        }

        /// <summary>
        /// Create a new dense matrix with the diagonal as a copy of the given vector.
        /// This new matrix will be independent from the vector.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfDiagonalVector(int rows, int columns, Vector<DecimalComplex> diagonal)
        {
            var m = new DenseMatrix(rows, columns);
            m.SetDiagonal(diagonal);
            return m;
        }

        /// <summary>
        /// Create a new dense matrix with the diagonal as a copy of the given array.
        /// This new matrix will be independent from the array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfDiagonalArray(DecimalComplex[] diagonal)
        {
            var m = new DenseMatrix(diagonal.Length, diagonal.Length);
            m.SetDiagonal(diagonal);
            return m;
        }

        /// <summary>
        /// Create a new dense matrix with the diagonal as a copy of the given array.
        /// This new matrix will be independent from the array.
        /// A new memory block will be allocated for storing the matrix.
        /// </summary>
        public static DenseMatrix OfDiagonalArray(int rows, int columns, DecimalComplex[] diagonal)
        {
            var m = new DenseMatrix(rows, columns);
            m.SetDiagonal(diagonal);
            return m;
        }

        /// <summary>
        /// Create a new dense matrix and initialize each value to the same provided value.
        /// </summary>
        public static DenseMatrix Create(int rows, int columns, DecimalComplex value)
        {
            if (value == DecimalComplex.Zero) return new DenseMatrix(rows, columns);
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfValue(rows, columns, value));
        }

        /// <summary>
        /// Create a new dense matrix and initialize each value using the provided init function.
        /// </summary>
        public static DenseMatrix Create(int rows, int columns, Func<int, int, DecimalComplex> init)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfInit(rows, columns, init));
        }

        /// <summary>
        /// Create a new diagonal dense matrix and initialize each diagonal value to the same provided value.
        /// </summary>
        public static DenseMatrix CreateDiagonal(int rows, int columns, DecimalComplex value)
        {
            if (value == DecimalComplex.Zero) return new DenseMatrix(rows, columns);
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfDiagonalInit(rows, columns, i => value));
        }

        /// <summary>
        /// Create a new diagonal dense matrix and initialize each diagonal value using the provided init function.
        /// </summary>
        public static DenseMatrix CreateDiagonal(int rows, int columns, Func<int, DecimalComplex> init)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfDiagonalInit(rows, columns, init));
        }

        /// <summary>
        /// Create a new square sparse identity matrix where each diagonal value is set to One.
        /// </summary>
        public static DenseMatrix CreateIdentity(int order)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfDiagonalInit(order, order, i => One));
        }

        /// <summary>
        /// Create a new dense matrix with values sampled from the provided random distribution.
        /// </summary>
        public static DenseMatrix CreateRandom(int rows, int columns, IContinuousDistribution distribution)
        {
            return new DenseMatrix(DenseColumnMajorMatrixStorage<DecimalComplex>.OfColumnMajorArray(rows, columns, Generate2.RandomDecimalComplex(rows*columns, distribution)));
        }

        /// <summary>
        /// Gets the matrix's data.
        /// </summary>
        /// <value>The matrix's data.</value>
        public DecimalComplex[] Values => _values;

        /// <summary>Calculates the induced L1 norm of this matrix.</summary>
        /// <returns>The maximum absolute column sum of the matrix.</returns>
        public override double L1Norm()
        {
            return LinearAlgebraControl2.Provider.MatrixNorm(Norm.OneNorm, _rowCount, _columnCount, _values);
        }

        /// <summary>Calculates the induced infinity norm of this matrix.</summary>
        /// <returns>The maximum absolute row sum of the matrix.</returns>
        public override double InfinityNorm()
        {
            return LinearAlgebraControl2.Provider.MatrixNorm(Norm.InfinityNorm, _rowCount, _columnCount, _values);
        }

        /// <summary>Calculates the entry-wise Frobenius norm of this matrix.</summary>
        /// <returns>The square root of the sum of the squared values.</returns>
        public override double FrobeniusNorm()
        {
            return LinearAlgebraControl2.Provider.MatrixNorm(Norm.FrobeniusNorm, _rowCount, _columnCount, _values);
        }

        /// <summary>
        /// Negate each element of this matrix and place the results into the result matrix.
        /// </summary>
        /// <param name="result">The result of the negation.</param>
        protected override void DoNegate(Matrix<DecimalComplex> result)
        {
            if (result is DenseMatrix denseResult)
            {
                LinearAlgebraControl2.Provider.ScaleArray(-1, _values, denseResult._values);
                return;
            }

            base.DoNegate(result);
        }

        /// <summary>
        /// DecimalComplex conjugates each element of this matrix and place the results into the result matrix.
        /// </summary>
        /// <param name="result">The result of the conjugation.</param>
        protected override void DoConjugate(Matrix<DecimalComplex> result)
        {
            if (result is DenseMatrix denseResult)
            {
                LinearAlgebraControl2.Provider.ConjugateArray(_values, denseResult._values);
                return;
            }

            base.DoConjugate(result);
        }

        /// <summary>
        /// Add a scalar to each element of the matrix and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        /// <param name="result">The matrix to store the result of the addition.</param>
        protected override void DoAdd(DecimalComplex scalar, Matrix<DecimalComplex> result)
        {
            if (result is DenseMatrix denseResult)
            {
                CommonParallel.For(0, _values.Length, 4096, (a, b) =>
                {
                    var v = denseResult._values;
                    for (int i = a; i < b; i++)
                    {
                        v[i] = _values[i] + scalar;
                    }
                });
            }
            else
            {
                base.DoAdd(scalar, result);
            }
        }

        /// <summary>
        /// Adds another matrix to this matrix.
        /// </summary>
        /// <param name="other">The matrix to add to this matrix.</param>
        /// <param name="result">The matrix to store the result of add</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoAdd(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            // dense + dense = dense
            if (other.Storage is DenseColumnMajorMatrixStorage<DecimalComplex> denseOther && result.Storage is DenseColumnMajorMatrixStorage<DecimalComplex> denseResult)
            {
                LinearAlgebraControl2.Provider.AddArrays(_values, denseOther.Data, denseResult.Data);
                return;
            }

            // dense + diagonal = any
            if (other.Storage is DiagonalMatrixStorage<DecimalComplex> diagonalOther)
            {
                Storage.CopyTo(result.Storage, ExistingData.Clear);
                var diagonal = diagonalOther.Data;
                for (int i = 0; i < diagonal.Length; i++)
                {
                    result.At(i, i, result.At(i, i) + diagonal[i]);
                }
                return;
            }

            base.DoAdd(other, result);
        }

        /// <summary>
        /// Subtracts a scalar from each element of the matrix and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        /// <param name="result">The matrix to store the result of the subtraction.</param>
        protected override void DoSubtract(DecimalComplex scalar, Matrix<DecimalComplex> result)
        {
            if (result is DenseMatrix denseResult)
            {
                CommonParallel.For(0, _values.Length, 4096, (a, b) =>
                {
                    var v = denseResult._values;
                    for (int i = a; i < b; i++)
                    {
                        v[i] = _values[i] - scalar;
                    }
                });
            }
            else
            {
                base.DoSubtract(scalar, result);
            }
        }

        /// <summary>
        /// Subtracts another matrix from this matrix.
        /// </summary>
        /// <param name="other">The matrix to subtract.</param>
        /// <param name="result">The matrix to store the result of the subtraction.</param>
        protected override void DoSubtract(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            // dense + dense = dense
            if (other.Storage is DenseColumnMajorMatrixStorage<DecimalComplex> denseOther && result.Storage is DenseColumnMajorMatrixStorage<DecimalComplex> denseResult)
            {
                LinearAlgebraControl2.Provider.SubtractArrays(_values, denseOther.Data, denseResult.Data);
                return;
            }

            // dense + diagonal = matrix
            if (other.Storage is DiagonalMatrixStorage<DecimalComplex> diagonalOther)
            {
                CopyTo(result);
                var diagonal = diagonalOther.Data;
                for (int i = 0; i < diagonal.Length; i++)
                {
                    result.At(i, i, result.At(i, i) - diagonal[i]);
                }
                return;
            }

            base.DoSubtract(other, result);
        }

        /// <summary>
        /// Multiplies each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="scalar">The scalar to multiply the matrix with.</param>
        /// <param name="result">The matrix to store the result of the multiplication.</param>
        protected override void DoMultiply(DecimalComplex scalar, Matrix<DecimalComplex> result)
        {
            if (result is DenseMatrix denseResult)
            {
                LinearAlgebraControl2.Provider.ScaleArray(scalar, _values, denseResult._values);
            }
            else
            {
                base.DoMultiply(scalar, result);
            }
        }

        /// <summary>
        /// Multiplies this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(Vector<DecimalComplex> rightSide, Vector<DecimalComplex> result)
        {
            if (rightSide is DenseVector denseRight && result is DenseVector denseResult)
            {
                LinearAlgebraControl2.Provider.MatrixMultiply(
                    _values,
                    _rowCount,
                    _columnCount,
                    denseRight.Values,
                    denseRight.Count,
                    1,
                    denseResult.Values);
            }
            else
            {
                base.DoMultiply(rightSide, result);
            }
        }

        /// <summary>
        /// Multiplies this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            if (other is DenseMatrix denseOther && result is DenseMatrix denseResult)
            {
                LinearAlgebraControl2.Provider.MatrixMultiply(
                    _values,
                    _rowCount,
                    _columnCount,
                    denseOther._values,
                    denseOther._rowCount,
                    denseOther._columnCount,
                    denseResult._values);
                return;
            }

            if (other.Storage is DiagonalMatrixStorage<DecimalComplex> diagonalOther)
            {
                var diagonal = diagonalOther.Data;
                var d = QuickMath.Min(ColumnCount, other.ColumnCount);
                if (d < other.ColumnCount)
                {
                    result.ClearSubMatrix(0, RowCount, ColumnCount, other.ColumnCount - ColumnCount);
                }
                int index = 0;
                for (int j = 0; j < d; j++)
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        result.At(i, j, _values[index]*diagonal[j]);
                        index++;
                    }
                }
                return;
            }

            base.DoMultiply(other, result);
        }

        /// <summary>
        /// Multiplies this matrix with transpose of another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeAndMultiply(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            if (other is DenseMatrix denseOther && result is DenseMatrix denseResult)
            {
                LinearAlgebraControl2.Provider.MatrixMultiplyWithUpdate(
                    Providers.LinearAlgebra.Transpose.DontTranspose,
                    Providers.LinearAlgebra.Transpose.Transpose,
                    1.0M,
                    _values,
                    _rowCount,
                    _columnCount,
                    denseOther._values,
                    denseOther._rowCount,
                    denseOther._columnCount,
                    0.0M,
                    denseResult._values);
                return;
            }

            if (other.Storage is DiagonalMatrixStorage<DecimalComplex> diagonalOther)
            {
                var diagonal = diagonalOther.Data;
                var d = QuickMath.Min(ColumnCount, other.RowCount);
                if (d < other.RowCount)
                {
                    result.ClearSubMatrix(0, RowCount, ColumnCount, other.RowCount - ColumnCount);
                }
                int index = 0;
                for (int j = 0; j < d; j++)
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        result.At(i, j, _values[index]*diagonal[j]);
                        index++;
                    }
                }
                return;
            }

            base.DoTransposeAndMultiply(other, result);
        }

        /// <summary>
        /// Multiplies this matrix with the conjugate transpose of another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoConjugateTransposeAndMultiply(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            if (other is DenseMatrix denseOther && result is DenseMatrix denseResult)
            {
                LinearAlgebraControl2.Provider.MatrixMultiplyWithUpdate(
                    Providers.LinearAlgebra.Transpose.DontTranspose,
                    Providers.LinearAlgebra.Transpose.ConjugateTranspose,
                    1.0M,
                    _values,
                    _rowCount,
                    _columnCount,
                    denseOther._values,
                    denseOther._rowCount,
                    denseOther._columnCount,
                    0.0M,
                    denseResult._values);
                return;
            }

            if (other.Storage is DiagonalMatrixStorage<DecimalComplex> diagonalOther)
            {
                var diagonal = diagonalOther.Data;
                var conjugateDiagonal = new DecimalComplex[diagonal.Length];
                for (int i = 0; i < diagonal.Length; i++)
                {
                    conjugateDiagonal[i] = diagonal[i].Conjugate();
                }

                var d = QuickMath.Min(ColumnCount, other.RowCount);
                if (d < other.RowCount)
                {
                    result.ClearSubMatrix(0, RowCount, ColumnCount, other.RowCount - ColumnCount);
                }
                int index = 0;
                for (int j = 0; j < d; j++)
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        result.At(i, j, _values[index]*conjugateDiagonal[j]);
                        index++;
                    }
                }
                return;
            }

            base.DoConjugateTransposeAndMultiply(other, result);
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeThisAndMultiply(Vector<DecimalComplex> rightSide, Vector<DecimalComplex> result)
        {
            if (rightSide is DenseVector denseRight && result is DenseVector denseResult)
            {
                LinearAlgebraControl2.Provider.MatrixMultiplyWithUpdate(
                    Providers.LinearAlgebra.Transpose.Transpose,
                    Providers.LinearAlgebra.Transpose.DontTranspose,
                    1.0M,
                    _values,
                    _rowCount,
                    _columnCount,
                    denseRight.Values,
                    denseRight.Count,
                    1,
                    0.0M,
                    denseResult.Values);
                return;
            }

            base.DoTransposeThisAndMultiply(rightSide, result);
        }

        /// <summary>
        /// Multiplies the conjugate transpose of this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoConjugateTransposeThisAndMultiply(Vector<DecimalComplex> rightSide, Vector<DecimalComplex> result)
        {
            if (rightSide is DenseVector denseRight && result is DenseVector denseResult)
            {
                LinearAlgebraControl2.Provider.MatrixMultiplyWithUpdate(
                    Providers.LinearAlgebra.Transpose.ConjugateTranspose,
                    Providers.LinearAlgebra.Transpose.DontTranspose,
                    1.0M,
                    _values,
                    _rowCount,
                    _columnCount,
                    denseRight.Values,
                    denseRight.Count,
                    1,
                    0.0M,
                    denseResult.Values);
                return;
            }

            base.DoConjugateTransposeThisAndMultiply(rightSide, result);
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeThisAndMultiply(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            if (other is DenseMatrix denseOther && result is DenseMatrix denseResult)
            {
                LinearAlgebraControl2.Provider.MatrixMultiplyWithUpdate(
                    Providers.LinearAlgebra.Transpose.Transpose,
                    Providers.LinearAlgebra.Transpose.DontTranspose,
                    1.0M,
                    _values,
                    _rowCount,
                    _columnCount,
                    denseOther._values,
                    denseOther._rowCount,
                    denseOther._columnCount,
                    0.0M,
                    denseResult._values);
                return;
            }

            if (other.Storage is DiagonalMatrixStorage<DecimalComplex> diagonalOther)
            {
                var diagonal = diagonalOther.Data;
                var d = QuickMath.Min(RowCount, other.ColumnCount);
                if (d < other.ColumnCount)
                {
                    result.ClearSubMatrix(0, ColumnCount, RowCount, other.ColumnCount - RowCount);
                }
                int index = 0;
                for (int i = 0; i < ColumnCount; i++)
                {
                    for (int j = 0; j < d; j++)
                    {
                        result.At(i, j, _values[index]*diagonal[j]);
                        index++;
                    }
                    index += (RowCount - d);
                }
                return;
            }

            base.DoTransposeThisAndMultiply(other, result);
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoConjugateTransposeThisAndMultiply(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            if (other is DenseMatrix denseOther && result is DenseMatrix denseResult)
            {
                LinearAlgebraControl2.Provider.MatrixMultiplyWithUpdate(
                    Providers.LinearAlgebra.Transpose.ConjugateTranspose,
                    Providers.LinearAlgebra.Transpose.DontTranspose,
                    1.0M,
                    _values,
                    _rowCount,
                    _columnCount,
                    denseOther._values,
                    denseOther._rowCount,
                    denseOther._columnCount,
                    0.0M,
                    denseResult._values);
                return;
            }

            if (other.Storage is DiagonalMatrixStorage<DecimalComplex> diagonalOther)
            {
                var diagonal = diagonalOther.Data;
                var d = QuickMath.Min(RowCount, other.ColumnCount);
                if (d < other.ColumnCount)
                {
                    result.ClearSubMatrix(0, ColumnCount, RowCount, other.ColumnCount - RowCount);
                }
                int index = 0;
                for (int i = 0; i < ColumnCount; i++)
                {
                    for (int j = 0; j < d; j++)
                    {
                        result.At(i, j, _values[index].Conjugate()*diagonal[j]);
                        index++;
                    }
                    index += (RowCount - d);
                }
                return;
            }

            base.DoConjugateTransposeThisAndMultiply(other, result);
        }

        /// <summary>
        /// Divides each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="divisor">The scalar to divide the matrix with.</param>
        /// <param name="result">The matrix to store the result of the division.</param>
        protected override void DoDivide(DecimalComplex divisor, Matrix<DecimalComplex> result)
        {
            if (result is DenseMatrix denseResult)
            {
                LinearAlgebraControl2.Provider.ScaleArray(1.0M / divisor, _values, denseResult._values);
            }
            else
            {
                base.DoDivide(divisor, result);
            }
        }

        /// <summary>
        /// Pointwise multiplies this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise multiply with this one.</param>
        /// <param name="result">The matrix to store the result of the pointwise multiplication.</param>
        protected override void DoPointwiseMultiply(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            if (other is DenseMatrix denseOther && result is DenseMatrix denseResult)
            {
                LinearAlgebraControl2.Provider.PointWiseMultiplyArrays(_values, denseOther._values, denseResult._values);
            }
            else
            {
                base.DoPointwiseMultiply(other, result);
            }
        }

        /// <summary>
        /// Pointwise divide this matrix by another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="divisor">The matrix to pointwise divide this one by.</param>
        /// <param name="result">The matrix to store the result of the pointwise division.</param>
        protected override void DoPointwiseDivide(Matrix<DecimalComplex> divisor, Matrix<DecimalComplex> result)
        {
            if (divisor is DenseMatrix denseDivisor && result is DenseMatrix denseResult)
            {
                LinearAlgebraControl2.Provider.PointWiseDivideArrays(_values, denseDivisor._values, denseResult._values);
            }
            else
            {
                base.DoPointwiseDivide(divisor, result);
            }
        }

        /// <summary>
        /// Pointwise raise this matrix to an exponent and store the result into the result matrix.
        /// </summary>
        /// <param name="exponent">The exponent to raise this matrix values to.</param>
        /// <param name="result">The vector to store the result of the pointwise power.</param>
        protected override void DoPointwisePower(Matrix<DecimalComplex> exponent, Matrix<DecimalComplex> result)
        {
            if (exponent is DenseMatrix denseExponent && result is DenseMatrix denseResult)
            {
                LinearAlgebraControl2.Provider.PointWisePowerArrays(_values, denseExponent._values, denseResult._values);
            }
            else
            {
                base.DoPointwisePower(exponent, result);
            }
        }

        /// <summary>
        /// Computes the trace of this matrix.
        /// </summary>
        /// <returns>The trace of this matrix</returns>
        /// <exception cref="ArgumentException">If the matrix is not square</exception>
        public override DecimalComplex Trace()
        {
            if (_rowCount != _columnCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_MUST_BE_SQUARE);
            }

            var sum = DecimalComplex.Zero;
            for (var i = 0; i < _rowCount; i++)
            {
                sum += _values[(i * _rowCount) + i];
            }

            return sum;
        }

        /// <summary>
        /// Adds two matrices together and returns the results.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to add.</param>
        /// <param name="rightSide">The right matrix to add.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> don't have the same dimensions.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseMatrix operator +(DenseMatrix leftSide, DenseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            if (leftSide._rowCount != rightSide._rowCount || leftSide._columnCount != rightSide._columnCount)
            {
                throw MatrixExceptionFactory.DimensionsDontMatch<ArgumentOutOfRangeException>(leftSide.RowCount, rightSide.RowCount, leftSide.ColumnCount, rightSide.ColumnCount);
            }

            return (DenseMatrix)leftSide.Add(rightSide);
        }

        /// <summary>
        /// Returns a <strong>Matrix</strong> containing the same values of <paramref name="rightSide"/>.
        /// </summary>
        /// <param name="rightSide">The matrix to get the values from.</param>
        /// <returns>A matrix containing a the same values as <paramref name="rightSide"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseMatrix operator +(DenseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            return (DenseMatrix)rightSide.Clone();
        }

        /// <summary>
        /// Subtracts two matrices together and returns the results.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to subtract.</param>
        /// <param name="rightSide">The right matrix to subtract.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> don't have the same dimensions.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseMatrix operator -(DenseMatrix leftSide, DenseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            if (leftSide._rowCount != rightSide._rowCount || leftSide._columnCount != rightSide._columnCount)
            {
                throw MatrixExceptionFactory.DimensionsDontMatch<ArgumentOutOfRangeException>(leftSide.RowCount, rightSide.RowCount, leftSide.ColumnCount, rightSide.ColumnCount);
            }

            return (DenseMatrix)leftSide.Subtract(rightSide);
        }

        /// <summary>
        /// Negates each element of the matrix.
        /// </summary>
        /// <param name="rightSide">The matrix to negate.</param>
        /// <returns>A matrix containing the negated values.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseMatrix operator -(DenseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            return (DenseMatrix)rightSide.Negate();
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static DenseMatrix operator *(DenseMatrix leftSide, DecimalComplex rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (DenseMatrix)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseMatrix operator *(DecimalComplex leftSide, DenseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            return (DenseMatrix)rightSide.Multiply(leftSide);
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to multiply.</param>
        /// <param name="rightSide">The right matrix to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the dimensions of <paramref name="leftSide"/> or <paramref name="rightSide"/> don't conform.</exception>
        public static DenseMatrix operator *(DenseMatrix leftSide, DenseMatrix rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            if (leftSide._columnCount != rightSide._rowCount)
            {
                throw MatrixExceptionFactory.DimensionsDontMatch<ArgumentException>(leftSide.RowCount, rightSide.RowCount, leftSide.ColumnCount, rightSide.ColumnCount);
            }

            return (DenseMatrix)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> and a Vector.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The vector to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator *(DenseMatrix leftSide, DenseVector rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (DenseVector)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a Vector and a <strong>Matrix</strong>.
        /// </summary>
        /// <param name="leftSide">The vector to multiply.</param>
        /// <param name="rightSide">The matrix to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator *(DenseVector leftSide, DenseMatrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            return (DenseVector)rightSide.LeftMultiply(leftSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static DenseMatrix operator %(DenseMatrix leftSide, DecimalComplex rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (DenseMatrix)leftSide.Remainder(rightSide);
        }

        /// <summary>
        /// Evaluates whether this matrix is symmetric.
        /// </summary>
        public override bool IsSymmetric()
        {
            if (RowCount != ColumnCount)
            {
                return false;
            }

            for (var j = 0; j < ColumnCount; j++)
            {
                var index = j * RowCount;
                for (var i = j + 1; i < RowCount; i++)
                {
                    if (_values[(i*ColumnCount) + j] != _values[index + i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Evaluates whether this matrix is Hermitian (conjugate symmetric).
        /// </summary>
        public override bool IsHermitian()
        {
            if (RowCount != ColumnCount)
            {
                return false;
            }

            int stride = RowCount + 1;
            for (var k = 0; k < _values.Length; k += stride)
            {
                if (!_values[k].IsReal())
                {
                    return false;
                }
            }

            for (var j = 0; j < ColumnCount; j++)
            {
                var index = j * RowCount;
                for (var i = j + 1; i < RowCount; i++)
                {
                    if (_values[(i*ColumnCount) + j] != _values[index + i].Conjugate())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override Cholesky<DecimalComplex> Cholesky()
        {
            return DenseCholesky.Create(this);
        }

        public override LU<DecimalComplex> LU()
        {
            return DenseLU.Create(this);
        }

        public override QR<DecimalComplex> QR(QRMethod method = QRMethod.Thin)
        {
            return DenseQR.Create(this, method);
        }

        public override GramSchmidt<DecimalComplex> GramSchmidt()
        {
            return DenseGramSchmidt.Create(this);
        }

        public override Svd<DecimalComplex> Svd(bool computeVectors = true)
        {
            return DenseSvd.Create(this, computeVectors);
        }

        public override Evd<DecimalComplex> Evd(Symmetricity symmetricity = Symmetricity.Unknown)
        {
            return DenseEvd.Create(this, symmetricity);
        }
    }
}
