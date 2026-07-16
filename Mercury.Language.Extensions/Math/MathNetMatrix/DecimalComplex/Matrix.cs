// <copyright file="Matrix.cs" company="QuickMath.NET">
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
using MathNet.Numerics.LinearAlgebra.DecimalComplex.Factorization;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra.Storage;
using Mercury.Language;

namespace MathNet.Numerics.LinearAlgebra.DecimalComplex
{
    using DecimalComplex = System.Numerics.DecimalComplex;

    /// <summary>
    /// <c>DecimalComplex</c> version of the <see cref="Matrix{T}"/> class.
    /// </summary>
    [Serializable]
    public abstract class Matrix : Matrix<DecimalComplex>
    {
        /// <summary>
        /// Initializes a new instance of the Matrix class.
        /// </summary>
        protected Matrix(MatrixStorage<DecimalComplex> storage)
            : base(storage)
        {
        }

        /// <summary>
        /// Set all values whose absolute value is smaller than the threshold to zero.
        /// </summary>
        public override void CoerceZero(double threshold)
        {
            MapInplace(x => x.Magnitude < (decimal)threshold ? DecimalComplex.Zero : x, Zeros.AllowSkip);
        }

        /// <summary>
        /// Returns the conjugate transpose of this matrix.
        /// </summary>
        /// <returns>The conjugate transpose of this matrix.</returns>
        public sealed override Matrix<DecimalComplex> ConjugateTranspose()
        {
            var ret = Transpose();
            ret.MapInplace(c => c.Conjugate(), Zeros.AllowSkip);
            return ret;
        }

        /// <summary>
        /// Puts the conjugate transpose of this matrix into the result matrix.
        /// </summary>
        public sealed override void ConjugateTranspose(Matrix<DecimalComplex> result)
        {
            Transpose(result);
            result.MapInplace(c => c.Conjugate(), Zeros.AllowSkip);
        }

        /// <summary>
        /// DecimalComplex conjugates each element of this matrix and place the results into the result matrix.
        /// </summary>
        /// <param name="result">The result of the conjugation.</param>
        protected override void DoConjugate(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Conjugate, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Negate each element of this matrix and place the results into the result matrix.
        /// </summary>
        /// <param name="result">The result of the negation.</param>
        protected override void DoNegate(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Negate, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Add a scalar to each element of the matrix and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        /// <param name="result">The matrix to store the result of the addition.</param>
        protected override void DoAdd(DecimalComplex scalar, Matrix<DecimalComplex> result)
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
        protected override void DoAdd(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            Map2(DecimalComplex.Add, other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Subtracts a scalar from each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        /// <param name="result">The matrix to store the result of the subtraction.</param>
        protected override void DoSubtract(DecimalComplex scalar, Matrix<DecimalComplex> result)
        {
            Map(x => x - scalar, result, Zeros.Include);
        }

        /// <summary>
        /// Subtracts another matrix from this matrix.
        /// </summary>
        /// <param name="other">The matrix to subtract to this matrix.</param>
        /// <param name="result">The matrix to store the result of subtraction.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        protected override void DoSubtract(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            Map2(DecimalComplex.Subtract, other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Multiplies each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="scalar">The scalar to multiply the matrix with.</param>
        /// <param name="result">The matrix to store the result of the multiplication.</param>
        protected override void DoMultiply(DecimalComplex scalar, Matrix<DecimalComplex> result)
        {
            Map(x => x*scalar, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Multiplies this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(Vector<DecimalComplex> rightSide, Vector<DecimalComplex> result)
        {
            for (var i = 0; i < RowCount; i++)
            {
                var s = DecimalComplex.Zero;
                for (var j = 0; j < ColumnCount; j++)
                {
                    s += At(i, j)*rightSide[j];
                }
                result[i] = s;
            }
        }

        /// <summary>
        /// Multiplies this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoMultiply(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j != other.ColumnCount; j++)
                {
                    var s = DecimalComplex.Zero;
                    for (var k = 0; k < ColumnCount; k++)
                    {
                        s += At(i, k)*other.At(k, j);
                    }
                    result.At(i, j, s);
                }
            }
        }

        /// <summary>
        /// Divides each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="divisor">The scalar to divide the matrix with.</param>
        /// <param name="result">The matrix to store the result of the division.</param>
        protected override void DoDivide(DecimalComplex divisor, Matrix<DecimalComplex> result)
        {
            Map(x => x/divisor, result, divisor.IsZero() ? Zeros.Include : Zeros.AllowSkip);
        }

        /// <summary>
        /// Divides a scalar by each element of the matrix and stores the result in the result matrix.
        /// </summary>
        /// <param name="dividend">The scalar to divide by each element of the matrix.</param>
        /// <param name="result">The matrix to store the result of the division.</param>
        protected override void DoDivideByThis(DecimalComplex dividend, Matrix<DecimalComplex> result)
        {
            Map(x => dividend/x, result, Zeros.Include);
        }

        /// <summary>
        /// Multiplies this matrix with transpose of another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeAndMultiply(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            for (var j = 0; j < other.RowCount; j++)
            {
                for (var i = 0; i < RowCount; i++)
                {
                    var s = DecimalComplex.Zero;
                    for (var k = 0; k < ColumnCount; k++)
                    {
                        s += At(i, k)*other.At(j, k);
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
        protected override void DoConjugateTransposeAndMultiply(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            for (var j = 0; j < other.RowCount; j++)
            {
                for (var i = 0; i < RowCount; i++)
                {
                    var s = DecimalComplex.Zero;
                    for (var k = 0; k < ColumnCount; k++)
                    {
                        s += At(i, k)*other.At(j, k).Conjugate();
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
        protected override void DoTransposeThisAndMultiply(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            for (var j = 0; j < other.ColumnCount; j++)
            {
                for (var i = 0; i < ColumnCount; i++)
                {
                    var s = DecimalComplex.Zero;
                    for (var k = 0; k < RowCount; k++)
                    {
                        s += At(k, i)*other.At(k, j);
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
        protected override void DoConjugateTransposeThisAndMultiply(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            for (var j = 0; j < other.ColumnCount; j++)
            {
                for (var i = 0; i < ColumnCount; i++)
                {
                    var s = DecimalComplex.Zero;
                    for (var k = 0; k < RowCount; k++)
                    {
                        s += At(k, i).Conjugate()*other.At(k, j);
                    }
                    result.At(i, j, s);
                }
            }
        }

        /// <summary>
        /// Multiplies the transpose of this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoTransposeThisAndMultiply(Vector<DecimalComplex> rightSide, Vector<DecimalComplex> result)
        {
            for (var j = 0; j < ColumnCount; j++)
            {
                var s = DecimalComplex.Zero;
                for (var i = 0; i < RowCount; i++)
                {
                    s += At(i, j)*rightSide[i];
                }
                result[j] = s;
            }
        }

        /// <summary>
        /// Multiplies the conjugate transpose of this matrix with a vector and places the results into the result vector.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        protected override void DoConjugateTransposeThisAndMultiply(Vector<DecimalComplex> rightSide, Vector<DecimalComplex> result)
        {
            for (var j = 0; j < ColumnCount; j++)
            {
                var s = DecimalComplex.Zero;
                for (var i = 0; i < RowCount; i++)
                {
                    s += At(i, j).Conjugate()*rightSide[i];
                }
                result[j] = s;
            }
        }

        /// <summary>
        /// Pointwise multiplies this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise multiply with this one.</param>
        /// <param name="result">The matrix to store the result of the pointwise multiplication.</param>
        protected override void DoPointwiseMultiply(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            Map2(DecimalComplex.Multiply, other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Pointwise divide this matrix by another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="divisor">The matrix to pointwise divide this one by.</param>
        /// <param name="result">The matrix to store the result of the pointwise division.</param>
        protected override void DoPointwiseDivide(Matrix<DecimalComplex> divisor, Matrix<DecimalComplex> result)
        {
            Map2(DecimalComplex.Divide, divisor, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise raise this matrix to an exponent and store the result into the result matrix.
        /// </summary>
        /// <param name="exponent">The exponent to raise this matrix values to.</param>
        /// <param name="result">The matrix to store the result of the pointwise power.</param>
        protected override void DoPointwisePower(DecimalComplex exponent, Matrix<DecimalComplex> result)
        {
            Map(x => x.Power(exponent), result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise raise this matrix to an exponent and store the result into the result matrix.
        /// </summary>
        /// <param name="exponent">The exponent to raise this matrix values to.</param>
        /// <param name="result">The vector to store the result of the pointwise power.</param>
        protected override void DoPointwisePower(Matrix<DecimalComplex> exponent, Matrix<DecimalComplex> result)
        {
            Map2(DecimalComplex.Pow, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise canonical modulus, where the result has the sign of the divisor,
        /// of this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="divisor">The pointwise denominator matrix to use</param>
        /// <param name="result">The result of the modulus.</param>
        protected sealed override void DoPointwiseModulus(Matrix<DecimalComplex> divisor, Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Pointwise remainder (% operator), where the result has the sign of the dividend,
        /// of this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="divisor">The pointwise denominator matrix to use</param>
        /// <param name="result">The result of the modulus.</param>
        protected sealed override void DoPointwiseRemainder(Matrix<DecimalComplex> divisor, Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for the given divisor each element of the matrix.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">Matrix to store the results in.</param>
        protected sealed override void DoModulus(DecimalComplex divisor, Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for the given dividend for each element of the matrix.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected sealed override void DoModulusByThis(DecimalComplex dividend, Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// for the given divisor each element of the matrix.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">Matrix to store the results in.</param>
        protected sealed override void DoRemainder(DecimalComplex divisor, Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// for the given dividend for each element of the matrix.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected sealed override void DoRemainderByThis(DecimalComplex dividend, Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Pointwise applies the exponential function to each value and stores the result into the result matrix.
        /// </summary>
        /// <param name="result">The matrix to store the result.</param>
        protected override void DoPointwiseExp(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Exp, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise applies the natural logarithm function to each value and stores the result into the result matrix.
        /// </summary>
        /// <param name="result">The matrix to store the result.</param>
        protected override void DoPointwiseLog(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Log, result, Zeros.Include);
        }

        protected override void DoPointwiseAbs(Matrix<DecimalComplex> result)
        {
            Map(x => DecimalComplex.Abs(x), result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAcos(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Acos, result, Zeros.Include);
        }
        protected override void DoPointwiseAsin(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Asin, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAtan(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Atan, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAtan2(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseCeiling(Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseCos(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Cos, result, Zeros.Include);
        }
        protected override void DoPointwiseCosh(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Cosh, result, Zeros.Include);
        }
        protected override void DoPointwiseFloor(Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseLog10(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Log10, result, Zeros.Include);
        }
        protected override void DoPointwiseRound(Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseSign(Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseSin(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Sin, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSinh(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Sinh, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSqrt(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Sqrt, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseTan(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Tan, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseTanh(Matrix<DecimalComplex> result)
        {
            Map(DecimalComplex.Tanh, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Computes the Moore-Penrose Pseudo-Inverse of this matrix.
        /// </summary>
        public override Matrix<DecimalComplex> PseudoInverse()
        {
            var svd = Svd(true);
            var w = svd.W;
            var s = svd.S;
            double tolerance = QuickMath.Max(RowCount, ColumnCount) * svd.L2Norm * Precision.DoublePrecision;

            for (int i = 0; i < s.Count; i++)
            {
                s[i] = s[i].Magnitude < (decimal)tolerance ? 0 : 1/s[i];
            }

            w.SetDiagonal(s);
            return (svd.U * (w * svd.VT)).ConjugateTranspose();
        }

        /// <summary>
        /// Computes the trace of this matrix.
        /// </summary>
        /// <returns>The trace of this matrix</returns>
        /// <exception cref="ArgumentException">If the matrix is not square</exception>
        public override DecimalComplex Trace()
        {
            if (RowCount != ColumnCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_MUST_BE_SQUARE);
            }

            var sum = DecimalComplex.Zero;
            for (var i = 0; i < RowCount; i++)
            {
                sum += At(i, i);
            }

            return sum;
        }

        protected override void DoPointwiseMinimum(DecimalComplex scalar, Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        protected override void DoPointwiseMaximum(DecimalComplex scalar, Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        protected override void DoPointwiseAbsoluteMinimum(DecimalComplex scalar, Matrix<DecimalComplex> result)
        {
            decimal absolute = scalar.Magnitude;
            Map(x => QuickMath.Min(absolute, x.Magnitude), result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseAbsoluteMaximum(DecimalComplex scalar, Matrix<DecimalComplex> result)
        {
            decimal absolute = scalar.Magnitude;
            Map(x => QuickMath.Max(absolute, x.Magnitude), result, Zeros.Include);
        }

        protected override void DoPointwiseMinimum(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        protected override void DoPointwiseMaximum(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        protected override void DoPointwiseAbsoluteMinimum(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            Map2((x, y) => QuickMath.Min(x.Magnitude, y.Magnitude), other, result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseAbsoluteMaximum(Matrix<DecimalComplex> other, Matrix<DecimalComplex> result)
        {
            Map2((x, y) => QuickMath.Max(x.Magnitude, y.Magnitude), other, result, Zeros.AllowSkip);
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
                    s += At(i, j).Magnitude;
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
                    s += At(i, j).Magnitude;
                }
                norm = QuickMath.Max(norm, s);
            }
            return (double)norm;
        }

        /// <summary>Calculates the entry-wise Frobenius norm of this matrix.</summary>
        /// <returns>The square root of the sum of the squared values.</returns>
        public override double FrobeniusNorm()
        {
            var transpose = ConjugateTranspose();
            var aat = this*transpose;
            var norm = 0M;
            for (var i = 0; i < RowCount; i++)
            {
                norm += aat.At(i, i).Magnitude;
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

            var ret = new double[RowCount];
            if (norm == 2.0)
            {
                Storage.FoldByRow(ret, (s, x) => s + (double)x.MagnitudeSquared(), (x, c) => Math.Sqrt(x), ret, Zeros.AllowSkip);
            }
            else if (norm == 1.0)
            {
                Storage.FoldByRow(ret, (s, x) => s + (double)x.Magnitude, (x, c) => x, ret, Zeros.AllowSkip);
            }
            else if (double.IsPositiveInfinity(norm))
            {
                Storage.FoldByRow(ret, (s, x) => Math.Max(s, (double)x.Magnitude), (x, c) => x, ret, Zeros.AllowSkip);
            }
            else
            {
                double invnorm = 1.0/norm;
                Storage.FoldByRow(ret, (s, x) => s + Math.Pow((double)x.Magnitude, norm), (x, c) => QuickMath.Pow(x, invnorm), ret, Zeros.AllowSkip);
            }
            return Vector<double>.Build.Dense(ret);
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

            var ret = new double[ColumnCount];
            if (norm == 2.0)
            {
                Storage.FoldByColumn(ret, (s, x) => s + (double)x.MagnitudeSquared(), (x, c) => Math.Sqrt(x), ret, Zeros.AllowSkip);
            }
            else if (norm == 1.0)
            {
                Storage.FoldByColumn(ret, (s, x) => s + (double)x.Magnitude, (x, c) => x, ret, Zeros.AllowSkip);
            }
            else if (double.IsPositiveInfinity(norm))
            {
                Storage.FoldByColumn(ret, (s, x) => Math.Max(s, (double)x.Magnitude), (x, c) => x, ret, Zeros.AllowSkip);
            }
            else
            {
                double invnorm = 1.0/norm;
                Storage.FoldByColumn(ret, (s, x) => s + Math.Pow((double)x.Magnitude, norm), (x, c) => QuickMath.Pow(x, invnorm), ret, Zeros.AllowSkip);
            }
            return Vector<double>.Build.Dense(ret);
        }

        /// <summary>
        /// Normalizes all row vectors to a unit p-norm.
        /// Typical values for p are 1.0 (L1, Manhattan norm), 2.0 (L2, Euclidean norm) and positive infinity (infinity norm)
        /// </summary>
        public sealed override Matrix<DecimalComplex> NormalizeRows(double norm)
        {
            var norminv = ((DenseVectorStorage<double>)RowNorms(norm).Storage).Data;
            for (int i = 0; i < norminv.Length; i++)
            {
                norminv[i] = norminv[i] == 0d ? 1d : 1d/norminv[i];
            }

            var result = Build.SameAs(this, RowCount, ColumnCount);
            Storage.MapIndexedTo(result.Storage, (i, j, x) => (decimal)norminv[i]*x, Zeros.AllowSkip, ExistingData.AssumeZeros);
            return result;
        }

        /// <summary>
        /// Normalizes all column vectors to a unit p-norm.
        /// Typical values for p are 1.0 (L1, Manhattan norm), 2.0 (L2, Euclidean norm) and positive infinity (infinity norm)
        /// </summary>
        public sealed override Matrix<DecimalComplex> NormalizeColumns(double norm)
        {
            var norminv = ((DenseVectorStorage<double>)ColumnNorms(norm).Storage).Data;
            for (int i = 0; i < norminv.Length; i++)
            {
                norminv[i] = norminv[i] == 0d ? 1d : 1d/norminv[i];
            }

            var result = Build.SameAs(this, RowCount, ColumnCount);
            Storage.MapIndexedTo(result.Storage, (i, j, x) => (decimal)norminv[j]*x, Zeros.AllowSkip, ExistingData.AssumeZeros);
            return result;
        }

        /// <summary>
        /// Calculates the value sum of each row vector.
        /// </summary>
        public override Vector<DecimalComplex> RowSums()
        {
            var ret = new DecimalComplex[RowCount];
            Storage.FoldByRow(ret, (s, x) => s + x, (x, c) => x, ret, Zeros.AllowSkip);
            return Vector<DecimalComplex>.Build.Dense(ret);
        }

        /// <summary>
        /// Calculates the absolute value sum of each row vector.
        /// </summary>
        public override Vector<DecimalComplex> RowAbsoluteSums()
        {
            var ret = new DecimalComplex[RowCount];
            Storage.FoldByRow(ret, (s, x) => s + x.Magnitude, (x, c) => x, ret, Zeros.AllowSkip);
            return Vector<DecimalComplex>.Build.Dense(ret);
        }

        /// <summary>
        /// Calculates the value sum of each column vector.
        /// </summary>
        public override Vector<DecimalComplex> ColumnSums()
        {
            var ret = new DecimalComplex[ColumnCount];
            Storage.FoldByColumn(ret, (s, x) => s + x, (x, c) => x, ret, Zeros.AllowSkip);
            return Vector<DecimalComplex>.Build.Dense(ret);
        }

        /// <summary>
        /// Calculates the absolute value sum of each column vector.
        /// </summary>
        public override Vector<DecimalComplex> ColumnAbsoluteSums()
        {
            var ret = new DecimalComplex[ColumnCount];
            Storage.FoldByColumn(ret, (s, x) => s + x.Magnitude, (x, c) => x, ret, Zeros.AllowSkip);
            return Vector<DecimalComplex>.Build.Dense(ret);
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

            for (var k = 0; k < RowCount; k++)
            {
                if (!At(k, k).IsReal())
                {
                    return false;
                }
            }

            for (var row = 0; row < RowCount; row++)
            {
                for (var column = row + 1; column < ColumnCount; column++)
                {
                    if (!At(row, column).Equals(At(column, row).Conjugate()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override Cholesky<DecimalComplex> Cholesky()
        {
            return UserCholesky.Create(this);
        }

        public override LU<DecimalComplex> LU()
        {
            return UserLU.Create(this);
        }

        public override QR<DecimalComplex> QR(QRMethod method = QRMethod.Thin)
        {
            return UserQR.Create(this, method);
        }

        public override GramSchmidt<DecimalComplex> GramSchmidt()
        {
            return UserGramSchmidt.Create(this);
        }

        public override Svd<DecimalComplex> Svd(bool computeVectors = true)
        {
            return UserSvd.Create(this, computeVectors);
        }

        public override Evd<DecimalComplex> Evd(Symmetricity symmetricity = Symmetricity.Unknown)
        {
            return UserEvd.Create(this, symmetricity);
        }
    }
}
