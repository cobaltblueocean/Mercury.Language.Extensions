// <copyright file="DenseCholesky.cs" company="QuickMath.NET">
// QuickMath.NET Numerics, part of the QuickMath.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2015 QuickMath.NET
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
using Mercury.Language;
using MathNet.Numerics.Providers.LinearAlgebra;

namespace MathNet.Numerics.LinearAlgebra.DecimalComplex.Factorization
{
    using DecimalComplex = System.Numerics.DecimalComplex;

    /// <summary>
    /// <para>A class which encapsulates the functionality of a Cholesky factorization for dense matrices.</para>
    /// <para>For a symmetric, positive definite matrix A, the Cholesky factorization
    /// is an lower triangular matrix L so that A = L*L'.</para>
    /// </summary>
    /// <remarks>
    /// The computation of the Cholesky factorization is done at construction time. If the matrix is not symmetric
    /// or positive definite, the constructor will throw an exception.
    /// </remarks>
    internal sealed class DenseCholesky : Cholesky
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DenseCholesky"/> class. This object will compute the
        /// Cholesky factorization when the constructor is called and cache it's factorization.
        /// </summary>
        /// <param name="matrix">The matrix to factor.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="matrix"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="matrix"/> is not a square matrix.</exception>
        /// <exception cref="ArgumentException">If <paramref name="matrix"/> is not positive definite.</exception>
        public static DenseCholesky Create(DenseMatrix matrix)
        {
            if (matrix.RowCount != matrix.ColumnCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_MUST_BE_SQUARE);
            }

            // Create a new matrix for the Cholesky factor, then perform factorization (while overwriting).
            var factor = (DenseMatrix) matrix.Clone();
            LinearAlgebraControl2.Provider.CholeskyFactor(factor.Values, factor.RowCount);
            return new DenseCholesky(factor);
        }

        DenseCholesky(Matrix<DecimalComplex> factor)
            : base(factor)
        {
        }

        /// <summary>
        /// Solves a system of linear equations, <b>AX = B</b>, with A Cholesky factorized.
        /// </summary>
        /// <param name="input">The right hand side <see cref="Matrix{T}"/>, <b>B</b>.</param>
        /// <param name="result">The left hand side <see cref="Matrix{T}"/>, <b>X</b>.</param>
        public override void Solve(Matrix<DecimalComplex> input, Matrix<DecimalComplex> result)
        {
            if (result.RowCount != input.RowCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_ROW_DIMENSIONS_MUST_AGREE);
            }

            if (result.ColumnCount != input.ColumnCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_COLUMN_DIMENSIONS_MUST_AGREE);
            }

            if (input.RowCount != Factor.RowCount)
            {
                throw MatrixExceptionFactory.DimensionsDontMatch<ArgumentException>(input.RowCount, Factor.RowCount);
            }

            if (input is DenseMatrix dinput && result is DenseMatrix dresult)
            {
                // Copy the contents of input to result.
                Array.Copy(dinput.Values, 0, dresult.Values, 0, dinput.Values.Length);

                // Cholesky solve by overwriting result.
                var dfactor = (DenseMatrix) Factor;
                LinearAlgebraControl2.Provider.CholeskySolveFactored(dfactor.Values, dfactor.RowCount, dresult.Values, dresult.ColumnCount);
            }
            else
            {
                throw new NotSupportedException(String.Format(LocalizedResources.Instance().CAN_ONLY_DO_FACTORIZATION_FOR_DENSE_MATRICES_AT_THE_MOMENT, "Cholesky"));
            }
        }

        /// <summary>
        /// Solves a system of linear equations, <b>Ax = b</b>, with A Cholesky factorized.
        /// </summary>
        /// <param name="input">The right hand side vector, <b>b</b>.</param>
        /// <param name="result">The left hand side <see cref="Matrix{T}"/>, <b>x</b>.</param>
        public override void Solve(Vector<DecimalComplex> input, Vector<DecimalComplex> result)
        {
            if (input.Count != result.Count)
            {
                throw new ArgumentException(LocalizedResources.Instance().ALL_VECTORS_MUST_HAVE_THE_SAME_DIMENSIONALITY);
            }

            if (input.Count != Factor.RowCount)
            {
                throw MatrixExceptionFactory.DimensionsDontMatch<ArgumentException>(input.Count, Factor.RowCount);
            }

            if (input is DenseVector dinput && result is DenseVector dresult)
            {
                // Copy the contents of input to result.
                Array.Copy(dinput.Values, 0, dresult.Values, 0, dinput.Values.Length);

                // Cholesky solve by overwriting result.
                var dfactor = (DenseMatrix) Factor;
                LinearAlgebraControl2.Provider.CholeskySolveFactored(dfactor.Values, dfactor.RowCount, dresult.Values, 1);
            }
            else
            {
                throw new NotSupportedException(String.Format(LocalizedResources.Instance().CAN_ONLY_DO_FACTORIZATION_FOR_DENSE_MATRICES_AT_THE_MOMENT, "Cholesky"));
            }
        }

        /// <summary>
        /// Calculates the Cholesky factorization of the input matrix.
        /// </summary>
        /// <param name="matrix">The matrix to be factorized<see cref="Matrix{T}"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="matrix"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="matrix"/> is not a square matrix.</exception>
        /// <exception cref="ArgumentException">If <paramref name="matrix"/> is not positive definite.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="matrix"/> does not have the same dimensions as the existing factor.</exception>
        public override void Factorize(Matrix<DecimalComplex> matrix)
        {
            if (matrix.RowCount != matrix.ColumnCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_MUST_BE_SQUARE);
            }

            if (matrix.RowCount != Factor.RowCount || matrix.ColumnCount != Factor.ColumnCount)
            {
                throw MatrixExceptionFactory.DimensionsDontMatch<ArgumentException>(matrix.RowCount, Factor.RowCount, matrix.ColumnCount, Factor.ColumnCount);
            }

            if (matrix is DenseMatrix dmatrix)
            {
                var dfactor = (DenseMatrix) Factor;

                // Overwrite the existing Factor matrix with the input.
                Array.Copy(dmatrix.Values, 0, dfactor.Values, 0, dmatrix.Values.Length);

                // Perform factorization (while overwriting).
                LinearAlgebraControl2.Provider.CholeskyFactor(dfactor.Values, dfactor.RowCount);
            }
            else
            {
                throw new NotSupportedException(String.Format(LocalizedResources.Instance().CAN_ONLY_DO_FACTORIZATION_FOR_DENSE_MATRICES_AT_THE_MOMENT, "Cholesky"));
            }
        }
    }
}
