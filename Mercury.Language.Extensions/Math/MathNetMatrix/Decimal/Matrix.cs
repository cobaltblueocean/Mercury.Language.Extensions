// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by OpenGamma Inc.
//
// Copyright (C) 2012 - present by OpenGamma Inc. and the OpenGamma group of c0Mpanies
//
// Please see distribution for license.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in c0Mpliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
//     
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Decimal.Factorization;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Storage;
using Mercury.Language;
using Mercury.Language.Exceptions;

namespace MathNet.Numerics.LinearAlgebra.Decimal
{
    /// <summary>
    /// <c>decimal</c> version of the <see cref="Matrix{T}"/> class.
    /// </summary>
    [Serializable]
    public abstract class Matrix : Matrix<decimal>
    {
        /// <summary>
        /// Initializes a new instance of the Matrix class.
        /// </summary>
        protected Matrix(MatrixStorage<decimal> storage)
            : base(storage)
        {
        }

        /// <summary>
        /// Set all values whose absolute value is smaller than the threshold to zero.
        /// </summary>
        public override void CoerceZero(double threshold)
        {
            MapInplace(x => System.QuickMath.Abs(x) < (decimal)threshold ? 0M : x, Zeros.AllowSkip);
        }

        /// <summary>
        /// Returns the conjugate transpose of this matrix.
        /// </summary>
        /// <returns>The conjugate transpose of this matrix.</returns>
        public sealed override Matrix<decimal> ConjugateTranspose()
        {
            return Transpose();
        }

        /// <summary>
        /// Puts the conjugate transpose of this matrix into the result matrix.
        /// </summary>
        public sealed override void ConjugateTranspose(Matrix<decimal> result)
        {
            Transpose(result);
        }

        /// <summary>
        /// C0Mplex conjugates each element of this matrix and place the results into the result matrix.
        /// </summary>
        /// <param name="result">The result of the conjugation.</param>
        protected sealed override void DoConjugate(Matrix<decimal> result)
        {
            if (ReferenceEquals(this, result))
            {
                return;
            }

            CopyTo(result);
        }

        /// <summary>
        /// Negate each element of this matrix and place the results into the result matrix.
        /// </summary>
        /// <param name="result">The result of the negation.</param>
        protected override void DoNegate(Matrix<decimal> result)
        {
            Map(x => -x, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Add a scalar to each element of the matrix and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        /// <param name="result">The matrix to store the result of the addition.</param>
        protected override void DoAdd(decimal scalar, Matrix<decimal> result)
        {
            Map(x => x + scalar, result, Zeros.Include);
        }

        /// <summary>
        /// Adds another matrix to this matrix.
        /// </summary>
        /// <param name="other">The matrix to add to this matrix.</param>
        /// <param name="result">The matrix to store the result of the addition.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoAdd(Matrix<decimal> other, Matrix<decimal> result)
        {
            Map2((x, y) => x + y, other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Subtracts a scalar fr0M each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        /// <param name="result">The matrix to store the result of the subtraction.</param>
        protected override void DoSubtract(decimal scalar, Matrix<decimal> result)
        {
            Map(x => x - scalar, result, Zeros.Include);
        }

        /// <summary>
        /// Subtracts another matrix fr0M this matrix.
        /// </summary>
        /// <param name="other">The matrix to subtract to this matrix.</param>
        /// <param name="result">The matrix to store the result of subtraction.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoSubtract(Matrix<decimal> other, Matrix<decimal> result)
        {
            Map2((x, y) => x - y, other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Multiplies each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="scalar">The scalar to multiply the matrix with.</param>
        /// <param name="result">The matrix to store the result of the multiplication.</param>
        protected override void DoMultiply(decimal scalar, Matrix<decimal> result)
        {
            Map(x => x * scalar, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Multiplies this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(Vector<decimal> rightSide, Vector<decimal> result)
        {
            for (var i = 0; i < RowCount; i++)
            {
                var s = 0.0M;
                for (var j = 0; j < ColumnCount; j++)
                {
                    s += At(i, j) * rightSide[j];
                }
                result[i] = s;
            }
        }

        /// <summary>
        /// Divides each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="divisor">The scalar to divide the matrix with.</param>
        /// <param name="result">The matrix to store the result of the division.</param>
        protected override void DoDivide(decimal divisor, Matrix<decimal> result)
        {
            Map(x => x / divisor, result, divisor == 0.0M ? Zeros.Include : Zeros.AllowSkip);
        }

        /// <summary>
        /// Divides a scalar by each element of the matrix and stores the result in the result matrix.
        /// </summary>
        /// <param name="dividend">The scalar to divide by each element of the matrix.</param>
        /// <param name="result">The matrix to store the result of the division.</param>
        protected override void DoDivideByThis(decimal dividend, Matrix<decimal> result)
        {
            Map(x => dividend / x, result, Zeros.Include);
        }

        /// <summary>
        /// Multiplies this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(Matrix<decimal> other, Matrix<decimal> result)
        {
            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j < other.ColumnCount; j++)
                {
                    var s = 0.0M;
                    for (var k = 0; k < ColumnCount; k++)
                    {
                        s += At(i, k) * other.At(k, j);
                    }
                    result.At(i, j, s);
                }
            }
        }

        /// <summary>
        /// Multiplies this matrix with transpose of another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeAndMultiply(Matrix<decimal> other, Matrix<decimal> result)
        {
            for (var j = 0; j < other.RowCount; j++)
            {
                for (var i = 0; i < RowCount; i++)
                {
                    var s = 0.0M;
                    for (var k = 0; k < ColumnCount; k++)
                    {
                        s += At(i, k) * other.At(j, k);
                    }
                    result.At(i, j, s);
                }
            }
        }

        /// <summary>
        /// Multiplies this matrix with the conjugate transpose of another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected sealed override void DoConjugateTransposeAndMultiply(Matrix<decimal> other, Matrix<decimal> result)
        {
            DoTransposeAndMultiply(other, result);
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeThisAndMultiply(Matrix<decimal> other, Matrix<decimal> result)
        {
            for (var j = 0; j < other.ColumnCount; j++)
            {
                for (var i = 0; i < ColumnCount; i++)
                {
                    var s = 0.0M;
                    for (var k = 0; k < RowCount; k++)
                    {
                        s += At(k, i) * other.At(k, j);
                    }
                    result.At(i, j, s);
                }
            }
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected sealed override void DoConjugateTransposeThisAndMultiply(Matrix<decimal> other, Matrix<decimal> result)
        {
            DoTransposeThisAndMultiply(other, result);
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeThisAndMultiply(Vector<decimal> rightSide, Vector<decimal> result)
        {
            for (var j = 0; j < ColumnCount; j++)
            {
                var s = 0.0M;
                for (var i = 0; i < RowCount; i++)
                {
                    s += At(i, j) * rightSide[i];
                }
                result[j] = s;
            }
        }

        /// <summary>
        /// Multiplies the conjugate transpose of this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected sealed override void DoConjugateTransposeThisAndMultiply(Vector<decimal> rightSide, Vector<decimal> result)
        {
            DoTransposeThisAndMultiply(rightSide, result);
        }

        /// <summary>
        /// C0Mputes the canonical modulus, where the result has the sign of the divisor,
        /// for the given divisor each element of the matrix.
        /// </summary>
        /// <param name="divisor">The scalar den0Minator to use.</param>
        /// <param name="result">Matrix to store the results in.</param>
        protected override void DoModulus(decimal divisor, Matrix<decimal> result)
        {
            Map(x => Euclid2.Modulus(x, divisor), result, Zeros.Include);
        }

        /// <summary>
        /// C0Mputes the canonical modulus, where the result has the sign of the divisor,
        /// for the given dividend for each element of the matrix.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected override void DoModulusByThis(decimal dividend, Matrix<decimal> result)
        {
            Map(x => Euclid2.Modulus(dividend, x), result, Zeros.Include);
        }

        /// <summary>
        /// C0Mputes the remainder (% operator), where the result has the sign of the dividend,
        /// for the given divisor each element of the matrix.
        /// </summary>
        /// <param name="divisor">The scalar den0Minator to use.</param>
        /// <param name="result">Matrix to store the results in.</param>
        protected override void DoRemainder(decimal divisor, Matrix<decimal> result)
        {
            Map(x => Euclid2.Remainder(x, divisor), result, Zeros.Include);
        }

        /// <summary>
        /// C0Mputes the remainder (% operator), where the result has the sign of the dividend,
        /// for the given dividend for each element of the matrix.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected override void DoRemainderByThis(decimal dividend, Matrix<decimal> result)
        {
            Map(x => Euclid2.Remainder(dividend, x), result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise multiplies this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise multiply with this one.</param>
        /// <param name="result">The matrix to store the result of the pointwise multiplication.</param>
        protected override void DoPointwiseMultiply(Matrix<decimal> other, Matrix<decimal> result)
        {
            Map2((x, y) => x * y, other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Pointwise divide this matrix by another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="divisor">The matrix to pointwise divide this one by.</param>
        /// <param name="result">The matrix to store the result of the pointwise division.</param>
        protected override void DoPointwiseDivide(Matrix<decimal> divisor, Matrix<decimal> result)
        {
            Map2((x, y) => x / y, divisor, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise raise this matrix to an exponent and store the result into the result matrix.
        /// </summary>
        /// <param name="exponent">The exponent to raise this matrix values to.</param>
        /// <param name="result">The matrix to store the result of the pointwise power.</param>
        protected override void DoPointwisePower(decimal exponent, Matrix<decimal> result)
        {
            Map(x => QuickMath.Pow(x, exponent), result, exponent > 0.0M ? Zeros.AllowSkip : Zeros.Include);
        }

        /// <summary>
        /// Pointwise raise this matrix to an exponent and store the result into the result matrix.
        /// </summary>
        /// <param name="exponent">The exponent to raise this matrix values to.</param>
        /// <param name="result">The vector to store the result of the pointwise power.</param>
        protected override void DoPointwisePower(Matrix<decimal> exponent, Matrix<decimal> result)
        {
            Map2(QuickMath.Pow, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise canonical modulus, where the result has the sign of the divisor,
        /// of this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="divisor">The pointwise den0Minator matrix to use</param>
        /// <param name="result">The result of the modulus.</param>
        protected override void DoPointwiseModulus(Matrix<decimal> divisor, Matrix<decimal> result)
        {
            Map2(Euclid2.Modulus, divisor, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise remainder (% operator), where the result has the sign of the dividend,
        /// of this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="divisor">The pointwise den0Minator matrix to use</param>
        /// <param name="result">The result of the modulus.</param>
        protected override void DoPointwiseRemainder(Matrix<decimal> divisor, Matrix<decimal> result)
        {
            Map2(Euclid2.Remainder, divisor, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise applies the exponential function to each value and stores the result into the result matrix.
        /// </summary>
        /// <param name="result">The matrix to store the result.</param>
        protected override void DoPointwiseExp(Matrix<decimal> result)
        {
            Map(QuickMath.Exp, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise applies the natural logarithm function to each value and stores the result into the result matrix.
        /// </summary>
        /// <param name="result">The matrix to store the result.</param>
        protected override void DoPointwiseLog(Matrix<decimal> result)
        {
            Map(QuickMath.Log, result, Zeros.Include);
        }

        protected override void DoPointwiseAbs(Matrix<decimal> result)
        {
            Map(QuickMath.Abs, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAcos(Matrix<decimal> result)
        {
            Map(QuickMath.Acos, result, Zeros.Include);
        }
        protected override void DoPointwiseAsin(Matrix<decimal> result)
        {
            Map(QuickMath.Asin, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAtan(Matrix<decimal> result)
        {
            Map(QuickMath.Atan, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAtan2(Matrix<decimal> other, Matrix<decimal> result)
        {
            Map2(QuickMath.Atan2, other, result, Zeros.Include);
        }
        protected override void DoPointwiseCeiling(Matrix<decimal> result)
        {
            Map(QuickMath.Ceiling, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseCos(Matrix<decimal> result)
        {
            Map(QuickMath.Cos, result, Zeros.Include);
        }
        protected override void DoPointwiseCosh(Matrix<decimal> result)
        {
            Map(QuickMath.Cosh, result, Zeros.Include);
        }
        protected override void DoPointwiseFloor(Matrix<decimal> result)
        {
            Map(QuickMath.Floor, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseLog10(Matrix<decimal> result)
        {
            Map(QuickMath.Log10, result, Zeros.Include);
        }
        protected override void DoPointwiseRound(Matrix<decimal> result)
        {
            Map(QuickMath.Round, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSign(Matrix<decimal> result)
        {
            Map(x => QuickMath.Sign(x), result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSin(Matrix<decimal> result)
        {
            Map(QuickMath.Sin, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSinh(Matrix<decimal> result)
        {
            Map(QuickMath.Sinh, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSqrt(Matrix<decimal> result)
        {
            Map(QuickMath.Sqrt, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseTan(Matrix<decimal> result)
        {
            Map(QuickMath.Tan, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseTanh(Matrix<decimal> result)
        {
            Map(QuickMath.Tanh, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// C0Mputes the Moore-Penrose Pseudo-Inverse of this matrix.
        /// </summary>
        public override Matrix<decimal> PseudoInverse()
        {
            var svd = Svd(true);
            var w = svd.W;
            var s = svd.S;
            decimal tolerance = QuickMath.Max(RowCount, ColumnCount) * System.Decimal.Parse(svd.L2Norm.ToString()) *  QuickMath.DecimalPrecision;

            for (int i = 0; i < s.Count; i++)
            {
                s[i] = s[i] < tolerance ? 0 : 1 / s[i];
            }

            w.SetDiagonal(s);
            return (svd.U * (w * svd.VT)).Transpose();
        }

        /// <summary>
        /// C0Mputes the trace of this matrix.
        /// </summary>
        /// <returns>The trace of this matrix</returns>
        /// <exception cref="ArgumentException">If the matrix is not square</exception>
        public override decimal Trace()
        {
            if (RowCount != ColumnCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_MUST_BE_SQUARE);
            }

            var sum = 0.0M;
            for (var i = 0; i < RowCount; i++)
            {
                sum += At(i, i);
            }

            return sum;
        }

        protected override void DoPointwiseMinimum(decimal scalar, Matrix<decimal> result)
        {
            Map(x => QuickMath.Min(scalar, x), result, scalar >= 0M ? Zeros.AllowSkip : Zeros.Include);
        }

        protected override void DoPointwiseMaximum(decimal scalar, Matrix<decimal> result)
        {
            Map(x => QuickMath.Max(scalar, x), result, scalar <= 0M ? Zeros.AllowSkip : Zeros.Include);
        }

        protected override void DoPointwiseAbsoluteMinimum(decimal scalar, Matrix<decimal> result)
        {
            decimal absolute = QuickMath.Abs(scalar);
            Map(x => QuickMath.Min(absolute, QuickMath.Abs(x)), result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseAbsoluteMaximum(decimal scalar, Matrix<decimal> result)
        {
            decimal absolute = QuickMath.Abs(scalar);
            Map(x => QuickMath.Max(absolute, QuickMath.Abs(x)), result, Zeros.Include);
        }

        protected override void DoPointwiseMinimum(Matrix<decimal> other, Matrix<decimal> result)
        {
            Map2(QuickMath.Min, other, result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseMaximum(Matrix<decimal> other, Matrix<decimal> result)
        {
            Map2(QuickMath.Max, other, result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseAbsoluteMinimum(Matrix<decimal> other, Matrix<decimal> result)
        {
            Map2((x, y) => QuickMath.Min(QuickMath.Abs(x), QuickMath.Abs(y)), other, result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseAbsoluteMaximum(Matrix<decimal> other, Matrix<decimal> result)
        {
            Map2((x, y) => QuickMath.Max(QuickMath.Abs(x), QuickMath.Abs(y)), other, result, Zeros.AllowSkip);
        }

        /// <summary>Calculates the induced L1 norm of this matrix.</summary>
        /// <returns>The maximum absolute column sum of the matrix.</returns>
        public override double L1Norm()
        {
            var norm = 0M;
            for (var j = 0; j < ColumnCount; j++)
            {
                var s = 0M;
                for (var i = 0; i < RowCount; i++)
                {
                    s += QuickMath.Abs(At(i, j));
                }
                norm = QuickMath.Max(norm, s);
            }
            return (double)norm;
        }

        /// <summary>Calculates the induced infinity norm of this matrix.</summary>
        /// <returns>The maximum absolute row sum of the matrix.</returns>
        public override double InfinityNorm()
        {
            var norm = 0M;
            for (var i = 0; i < RowCount; i++)
            {
                var s = 0M;
                for (var j = 0; j < ColumnCount; j++)
                {
                    s += QuickMath.Abs(At(i, j));
                }
                norm = QuickMath.Max(norm, s);
            }
            return (double)norm;
        }

        /// <summary>Calculates the entry-wise Frobenius norm of this matrix.</summary>
        /// <returns>The square root of the sum of the squared values.</returns>
        public override double FrobeniusNorm()
        {
            var transpose = Transpose();
            var aat = this * transpose;
            var norm = 0M;
            for (var i = 0; i < RowCount; i++)
            {
                norm += aat.At(i, i);
            }
            return (double)QuickMath.Sqrt(norm);
        }

        /// <summary>
        /// Calculates the p-norms of all row vectors.
        /// Typical values for p are 1.0 (L1, Manhattan norm), 2.0 (L2, Euclidean norm) and positive infinity (infinity norm)
        /// </summary>
        public override Vector<double> RowNorms(double norm)
        {
            if (norm <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(norm), "Value must be positive.");
            }

            var ret = new decimal[RowCount];
            if (norm == 2.0)
            {
                Storage.FoldByRow(ret, (s, x) => s + x * x, (x, c) => QuickMath.Sqrt(x), ret, Zeros.AllowSkip);
            }
            else if (norm == 1.0)
            {
                Storage.FoldByRow(ret, (s, x) => s + QuickMath.Abs(x), (x, c) => x, ret, Zeros.AllowSkip);
            }
            //else if (Double.IsPositiveInfinity(norm))
            //{
            //    Storage.FoldByRow(ret, (s, x) => QuickMath.Max(s, QuickMath.Abs(x)), (x, c) => x, ret, Zeros.AllowSkip);
            //}
            else
            {
                decimal invnorm = 1.0M / (decimal)norm;
                Storage.FoldByRow(ret, (s, x) => s + QuickMath.Pow(QuickMath.Abs(x), (decimal)norm), (x, c) => QuickMath.Pow(x, invnorm), ret, Zeros.AllowSkip);
            }

            var ret2 = ret.Select(x => (double)x).ToArray();

            return Vector<double>.Build.Dense(ret2);
        }

        /// <summary>
        /// Calculates the p-norms of all column vectors.
        /// Typical values for p are 1.0 (L1, Manhattan norm), 2.0 (L2, Euclidean norm) and positive infinity (infinity norm)
        /// </summary>
        public override Vector<double> ColumnNorms(double norm)
        {
            if (norm <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(norm), "Value must be positive.");
            }

            var ret = new decimal[ColumnCount];
            if (norm == 2.0)
            {
                Storage.FoldByColumn(ret, (s, x) => s + x * x, (x, c) => QuickMath.Sqrt(x), ret, Zeros.AllowSkip);
            }
            else if (norm == 1.0)
            {
                Storage.FoldByColumn(ret, (s, x) => s + QuickMath.Abs(x), (x, c) => x, ret, Zeros.AllowSkip);
            }
            else if (double.IsPositiveInfinity(norm))
            {
                Storage.FoldByColumn(ret, (s, x) => QuickMath.Max(s, QuickMath.Abs(x)), (x, c) => x, ret, Zeros.AllowSkip);
            }
            else
            {
                decimal invnorm = 1.0M / (decimal)norm;
                Storage.FoldByColumn(ret, (s, x) => s + QuickMath.Pow(QuickMath.Abs(x), (decimal)norm), (x, c) => QuickMath.Pow(x, invnorm), ret, Zeros.AllowSkip);
            }

            var ret2 = ret.Select(x => (double)x).ToArray();

            return Vector<double>.Build.Dense(ret2);
        }

        /// <summary>
        /// Normalizes all row vectors to a unit p-norm.
        /// Typical values for p are 1.0 (L1, Manhattan norm), 2.0 (L2, Euclidean norm) and positive infinity (infinity norm)
        /// </summary>
        public sealed override Matrix<decimal> NormalizeRows(double norm)
        {
            var norminv = DenseVectorStorage<decimal>.OfEnumerable(RowNorms(norm).AsArrayEx().Select(x => (decimal)x).ToArray()).Data;
            for (int i = 0; i < norminv.Length; i++)
            {
                norminv[i] = norminv[i] == 0M ? 1M : 1M / norminv[i];
            }

            var result = Build.SameAs(this, RowCount, ColumnCount);
            Storage.MapIndexedTo(result.Storage, (i, j, x) => norminv[i] * x, Zeros.AllowSkip, ExistingData.AssumeZeros);
            return result;
        }

        /// <summary>
        /// Normalizes all column vectors to a unit p-norm.
        /// Typical values for p are 1.0 (L1, Manhattan norm), 2.0 (L2, Euclidean norm) and positive infinity (infinity norm)
        /// </summary>
        public sealed override Matrix<decimal> NormalizeColumns(double norm)
        {
            var norminv = DenseVectorStorage<decimal>.OfEnumerable(ColumnNorms(norm).AsArrayEx().Select(x => (decimal)x).ToArray()).Data; //((DenseVectorStorage<decimal>)ColumnNorms(norm).Storage).Data;
            for (int i = 0; i < norminv.Length; i++)
            {
                norminv[i] = norminv[i] == 0M ? 1M : 1M / norminv[i];
            }

            var result = Build.SameAs(this, RowCount, ColumnCount);
            Storage.MapIndexedTo(result.Storage, (i, j, x) => norminv[j] * x, Zeros.AllowSkip, ExistingData.AssumeZeros);
            return result;
        }

        /// <summary>
        /// Calculates the value sum of each row vector.
        /// </summary>
        public override Vector<decimal> RowSums()
        {
            var ret = new decimal[RowCount];
            Storage.FoldByRow(ret, (s, x) => s + x, (x, c) => x, ret, Zeros.AllowSkip);
            return Vector<decimal>.Build.Dense(ret);
        }

        /// <summary>
        /// Calculates the absolute value sum of each row vector.
        /// </summary>
        public override Vector<decimal> RowAbsoluteSums()
        {
            var ret = new decimal[RowCount];
            Storage.FoldByRow(ret, (s, x) => s + QuickMath.Abs(x), (x, c) => x, ret, Zeros.AllowSkip);
            return Vector<decimal>.Build.Dense(ret);
        }

        /// <summary>
        /// Calculates the value sum of each column vector.
        /// </summary>
        public override Vector<decimal> ColumnSums()
        {
            var ret = new decimal[ColumnCount];
            Storage.FoldByColumn(ret, (s, x) => s + x, (x, c) => x, ret, Zeros.AllowSkip);
            return Vector<decimal>.Build.Dense(ret);
        }

        /// <summary>
        /// Calculates the absolute value sum of each column vector.
        /// </summary>
        public override Vector<decimal> ColumnAbsoluteSums()
        {
            var ret = new decimal[ColumnCount];
            Storage.FoldByColumn(ret, (s, x) => s + QuickMath.Abs(x), (x, c) => x, ret, Zeros.AllowSkip);
            return Vector<decimal>.Build.Dense(ret);
        }

        /// <summary>
        /// Evaluates whether this matrix is Hermitian (conjugate symmetric).
        /// </summary>
        public sealed override bool IsHermitian()
        {
            return IsSymmetric();
        }

        public override Cholesky<decimal> Cholesky()
        {
            return UserCholesky.Create(this);
        }

        public override LU<decimal> LU()
        {
            return UserLU.Create(this);
        }

        public override QR<decimal> QR(QRMethod method = QRMethod.Thin)
        {
            return UserQR.Create(this, method);
        }

        public override GramSchmidt<decimal> GramSchmidt()
        {
            return UserGramSchmidt.Create(this);
        }

        public override Svd<decimal> Svd(bool c0MputeVectors = true)
        {
            return UserSvd.Create(this, c0MputeVectors);
        }

        public override Evd<decimal> Evd(Symmetricity symmetricity = Symmetricity.Unknown)
        {
            return UserEvd.Create(this, symmetricity);
        }
    }
}
