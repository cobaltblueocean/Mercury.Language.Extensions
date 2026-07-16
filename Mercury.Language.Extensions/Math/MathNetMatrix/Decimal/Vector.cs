// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by QuickMath.NET.
//
// <copyright file="Vector.cs" company="QuickMath.NET">
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
using MathNet.Numerics.LinearAlgebra.Storage;
using Mercury.Language.Threading;

namespace MathNet.Numerics.LinearAlgebra.Decimal
{
    /// <summary>
    /// <c>decimal</c> version of the <see cref="Vector{T}"/> class.
    /// </summary>
    [Serializable]
    public abstract class Vector : Vector<decimal>
    {
        /// <summary>
        /// Initializes a new instance of the Vector class.
        /// </summary>
        protected Vector(VectorStorage<decimal> storage)
            : base(storage)
        {
        }

        /// <summary>
        /// Set all values whose absolute value is smaller than the threshold to zero.
        /// </summary>
        public override void CoerceZero(double threshold)
        {
            MapInplace(x => QuickMath.Abs(x) < (decimal)threshold ? 0M : x, Zeros.AllowSkip);
        }

        /// <summary>
        /// Conjugates vector and save result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        protected sealed override void DoConjugate(Vector<decimal> result)
        {
            if (ReferenceEquals(this, result))
            {
                return;
            }

            CopyTo(result);
        }

        /// <summary>
        /// Negates vector and saves result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        protected override void DoNegate(Vector<decimal> result)
        {
            Map(x => -x, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Adds a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">
        /// The scalar to add.
        /// </param>
        /// <param name="result">
        /// The vector to store the result of the addition.
        /// </param>
        protected override void DoAdd(decimal scalar, Vector<decimal> result)
        {
            Map(x => x + scalar, result, Zeros.Include);
        }

        /// <summary>
        /// Adds another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">
        /// The vector to add to this one.
        /// </param>
        /// <param name="result">
        /// The vector to store the result of the addition.
        /// </param>
        protected override void DoAdd(Vector<decimal> other, Vector<decimal> result)
        {
            Map2((x, y) => x + y, other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Subtracts a scalar from each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">
        /// The scalar to subtract.
        /// </param>
        /// <param name="result">
        /// The vector to store the result of the subtraction.
        /// </param>
        protected override void DoSubtract(decimal scalar, Vector<decimal> result)
        {
            Map(x => x - scalar, result, Zeros.Include);
        }

        /// <summary>
        /// Subtracts another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">
        /// The vector to subtract from this one.
        /// </param>
        /// <param name="result">
        /// The vector to store the result of the subtraction.
        /// </param>
        protected override void DoSubtract(Vector<decimal> other, Vector<decimal> result)
        {
            Map2((x, y) => x - y, other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Multiplies a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">
        /// The scalar to multiply.
        /// </param>
        /// <param name="result">
        /// The vector to store the result of the multiplication.
        /// </param>
        protected override void DoMultiply(decimal scalar, Vector<decimal> result)
        {
            Map(x => x * scalar, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Divides each element of the vector by a scalar and stores the result in the result vector.
        /// </summary>
        /// <param name="divisor">
        /// The scalar to divide with.
        /// </param>
        /// <param name="result">
        /// The vector to store the result of the division.
        /// </param>
        protected override void DoDivide(decimal divisor, Vector<decimal> result)
        {
            Map(x => x / divisor, result, divisor == 0.0M ? Zeros.Include : Zeros.AllowSkip);
        }

        /// <summary>
        /// Divides a scalar by each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="dividend">The scalar to divide.</param>
        /// <param name="result">The vector to store the result of the division.</param>
        protected override void DoDivideByThis(decimal dividend, Vector<decimal> result)
        {
            Map(x => dividend / x, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise multiplies this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise multiply with this one.</param>
        /// <param name="result">The vector to store the result of the pointwise multiplication.</param>
        protected override void DoPointwiseMultiply(Vector<decimal> other, Vector<decimal> result)
        {
            Map2((x, y) => x * y, other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Pointwise divide this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="divisor">The vector to pointwise divide this one by.</param>
        /// <param name="result">The vector to store the result of the pointwise division.</param>
        protected override void DoPointwiseDivide(Vector<decimal> divisor, Vector<decimal> result)
        {
            Map2((x, y) => x / y, divisor, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise raise this vector to an exponent and store the result into the result vector.
        /// </summary>
        /// <param name="exponent">The exponent to raise this vector values to.</param>
        /// <param name="result">The vector to store the result of the pointwise power.</param>
        protected override void DoPointwisePower(decimal exponent, Vector<decimal> result)
        {
            Map(x => QuickMath.Pow(x, exponent), result, exponent > 0.0M ? Zeros.AllowSkip : Zeros.Include);
        }

        /// <summary>
        /// Pointwise raise this vector to an exponent vector and store the result into the result vector.
        /// </summary>
        /// <param name="exponent">The exponent vector to raise this vector values to.</param>
        /// <param name="result">The vector to store the result of the pointwise power.</param>
        protected override void DoPointwisePower(Vector<decimal> exponent, Vector<decimal> result)
        {
            Map2(QuickMath.Pow, exponent, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise canonical modulus, where the result has the sign of the divisor,
        /// of this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="divisor">The pointwise denominator vector to use.</param>
        /// <param name="result">The result of the modulus.</param>
        protected override void DoPointwiseModulus(Vector<decimal> divisor, Vector<decimal> result)
        {
            Map2(Euclid2.Modulus, divisor, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise remainder (% operator), where the result has the sign of the dividend,
        /// of this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="divisor">The pointwise denominator vector to use.</param>
        /// <param name="result">The result of the modulus.</param>
        protected override void DoPointwiseRemainder(Vector<decimal> divisor, Vector<decimal> result)
        {
            Map2(Euclid2.Remainder, divisor, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise applies the exponential function to each value and stores the result into the result vector.
        /// </summary>
        /// <param name="result">The vector to store the result.</param>
        protected override void DoPointwiseExp(Vector<decimal> result)
        {
            Map(QuickMath.Exp, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise applies the natural logarithm function to each value and stores the result into the result vector.
        /// </summary>
        /// <param name="result">The vector to store the result.</param>
        protected override void DoPointwiseLog(Vector<decimal> result)
        {
            Map(QuickMath.Log, result, Zeros.Include);
        }

        protected override void DoPointwiseAbs(Vector<decimal> result)
        {
            Map(QuickMath.Abs, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAcos(Vector<decimal> result)
        {
            Map(QuickMath.Acos, result, Zeros.Include);
        }
        protected override void DoPointwiseAsin(Vector<decimal> result)
        {
            Map(QuickMath.Asin, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAtan(Vector<decimal> result)
        {
            Map(QuickMath.Atan, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAtan2(Vector<decimal> other, Vector<decimal> result)
        {
            Map2(QuickMath.Atan2, other, result, Zeros.Include);
        }
        protected override void DoPointwiseAtan2(decimal scalar, Vector<decimal> result)
        {
            Map(x => QuickMath.Atan2(x, scalar), result, Zeros.Include);
        }
        protected override void DoPointwiseCeiling(Vector<decimal> result)
        {
            Map(QuickMath.Ceiling, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseCos(Vector<decimal> result)
        {
            Map(QuickMath.Cos, result, Zeros.Include);
        }
        protected override void DoPointwiseCosh(Vector<decimal> result)
        {
            Map(QuickMath.Cosh, result, Zeros.Include);
        }
        protected override void DoPointwiseFloor(Vector<decimal> result)
        {
            Map(QuickMath.Floor, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseLog10(Vector<decimal> result)
        {
            Map(QuickMath.Log10, result, Zeros.Include);
        }
        protected override void DoPointwiseRound(Vector<decimal> result)
        {
            Map(QuickMath.Round, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSign(Vector<decimal> result)
        {
            Map(x => QuickMath.Sign(x), result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSin(Vector<decimal> result)
        {
            Map(QuickMath.Sin, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSinh(Vector<decimal> result)
        {
            Map(QuickMath.Sinh, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSqrt(Vector<decimal> result)
        {
            Map(QuickMath.Sqrt, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseTan(Vector<decimal> result)
        {
            Map(QuickMath.Tan, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseTanh(Vector<decimal> result)
        {
            Map(QuickMath.Tanh, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Computes the dot product between this vector and another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The sum of a[i]*b[i] for all i.</returns>
        protected override decimal DoDotProduct(Vector<decimal> other)
        {
            var dot = 0.0M;
            for (var i = 0; i < Count; i++)
            {
                dot += At(i) * other.At(i);
            }
            return dot;
        }

        /// <summary>
        /// Computes the dot product between the conjugate of this vector and another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The sum of conj(a[i])*b[i] for all i.</returns>
        protected sealed override decimal DoConjugateDotProduct(Vector<decimal> other)
        {
            return DoDotProduct(other);
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected override void DoModulus(decimal divisor, Vector<decimal> result)
        {
            Map(x => Euclid2.Modulus(x, divisor), result, Zeros.Include);
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for the given dividend for each element of the vector.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected override void DoModulusByThis(decimal dividend, Vector<decimal> result)
        {
            Map(x => Euclid2.Modulus(dividend, x), result, Zeros.Include);
        }

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected override void DoRemainder(decimal divisor, Vector<decimal> result)
        {
            Map(x => Euclid2.Remainder(x, divisor), result, Zeros.Include);
        }

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// for the given dividend for each element of the vector.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected override void DoRemainderByThis(decimal dividend, Vector<decimal> result)
        {
            Map(x => Euclid2.Remainder(dividend, x), result, Zeros.Include);
        }

        protected override void DoPointwiseMinimum(decimal scalar, Vector<decimal> result)
        {
            Map(x => QuickMath.Min(scalar, x), result, scalar >= 0M ? Zeros.AllowSkip : Zeros.Include);
        }

        protected override void DoPointwiseMaximum(decimal scalar, Vector<decimal> result)
        {
            Map(x => QuickMath.Max(scalar, x), result, scalar <= 0M ? Zeros.AllowSkip : Zeros.Include);
        }

        protected override void DoPointwiseAbsoluteMinimum(decimal scalar, Vector<decimal> result)
        {
            decimal absolute = QuickMath.Abs(scalar);
            Map(x => QuickMath.Min(absolute, QuickMath.Abs(x)), result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseAbsoluteMaximum(decimal scalar, Vector<decimal> result)
        {
            decimal absolute = QuickMath.Abs(scalar);
            Map(x => QuickMath.Max(absolute, QuickMath.Abs(x)), result, Zeros.Include);
        }

        protected override void DoPointwiseMinimum(Vector<decimal> other, Vector<decimal> result)
        {
            Map2(QuickMath.Min, other, result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseMaximum(Vector<decimal> other, Vector<decimal> result)
        {
            Map2(QuickMath.Max, other, result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseAbsoluteMinimum(Vector<decimal> other, Vector<decimal> result)
        {
            Map2((x, y) => QuickMath.Min(QuickMath.Abs(x), QuickMath.Abs(y)), other, result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseAbsoluteMaximum(Vector<decimal> other, Vector<decimal> result)
        {
            Map2((x, y) => QuickMath.Max(QuickMath.Abs(x), QuickMath.Abs(y)), other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Returns the value of the absolute minimum element.
        /// </summary>
        /// <returns>The value of the absolute minimum element.</returns>
        public override decimal AbsoluteMinimum()
        {
            return QuickMath.Abs(At(AbsoluteMinimumIndex()));
        }

        /// <summary>
        /// Returns the index of the absolute minimum element.
        /// </summary>
        /// <returns>The index of absolute minimum element.</returns>
        public override int AbsoluteMinimumIndex()
        {
            var index = 0;
            var min = QuickMath.Abs(At(index));
            for (var i = 1; i < Count; i++)
            {
                var test = QuickMath.Abs(At(i));
                if (test < min)
                {
                    index = i;
                    min = test;
                }
            }

            return index;
        }

        /// <summary>
        /// Returns the value of the absolute maximum element.
        /// </summary>
        /// <returns>The value of the absolute maximum element.</returns>
        public override decimal AbsoluteMaximum()
        {
            return QuickMath.Abs(At(AbsoluteMaximumIndex()));
        }

        /// <summary>
        /// Returns the index of the absolute maximum element.
        /// </summary>
        /// <returns>The index of absolute maximum element.</returns>
        public override int AbsoluteMaximumIndex()
        {
            var index = 0;
            var max = QuickMath.Abs(At(index));
            for (var i = 1; i < Count; i++)
            {
                var test = QuickMath.Abs(At(i));
                if (test > max)
                {
                    index = i;
                    max = test;
                }
            }

            return index;
        }

        /// <summary>
        /// Computes the sum of the vector's elements.
        /// </summary>
        /// <returns>The sum of the vector's elements.</returns>
        public override decimal Sum()
        {
            var sum = 0.0M;
            for (var i = 0; i < Count; i++)
            {
                sum += At(i);
            }
            return sum;
        }

        /// <summary>
        /// Calculates the L1 norm of the vector, also known as Manhattan norm.
        /// </summary>
        /// <returns>The sum of the absolute values.</returns>
        public override double L1Norm()
        {
            var sum = 0.0;
            for (var i = 0; i < Count; i++)
            {
                sum += Math.Abs((double)At(i));
            }
            return sum;
        }

        /// <summary>
        /// Calculates the L2 norm of the vector, also known as Euclidean norm.
        /// </summary>
        /// <returns>The square root of the sum of the squared values.</returns>
        public override double L2Norm()
        {
            return Math.Sqrt((double)DoDotProduct(this));
        }

        /// <summary>
        /// Calculates the infinity norm of the vector.
        /// </summary>
        /// <returns>The maximum absolute value.</returns>
        public override double InfinityNorm()
        {
            return (double)CommonParallel.Aggregate(0, Count, i => QuickMath.Abs(At(i)), QuickMath.Max, 0M);
        }

        /// <summary>
        /// Computes the p-Norm.
        /// </summary>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <returns>
        /// <c>Scalar ret = ( ∑|At(i)|^p )^(1/p)</c>
        /// </returns>
        public override double Norm(double p)
        {
            if (p < 0) throw new ArgumentOutOfRangeException(nameof(p));

            if (p == 1d) return L1Norm();
            if (p == 2d) return L2Norm();

            var sum = 0M;
            for (var index = 0; index < Count; index++)
            {
                sum += QuickMath.Pow(QuickMath.Abs(At(index)), (decimal)p);
            }
            return (double)QuickMath.Pow(sum, 1.0M / (decimal)p);
        }

        /// <summary>
        /// Returns the index of the maximum element.
        /// </summary>
        /// <returns>The index of maximum element.</returns>
        public override int MaximumIndex()
        {
            var index = 0;
            var max = At(index);
            for (var i = 1; i < Count; i++)
            {
                var test = At(i);
                if (test > max)
                {
                    index = i;
                    max = test;
                }
            }

            return index;
        }

        /// <summary>
        /// Returns the index of the minimum element.
        /// </summary>
        /// <returns>The index of minimum element.</returns>
        public override int MinimumIndex()
        {
            var index = 0;
            var min = At(index);
            for (var i = 1; i < Count; i++)
            {
                var test = At(i);
                if (test < min)
                {
                    index = i;
                    min = test;
                }
            }

            return index;
        }

        /// <summary>
        /// Normalizes this vector to a unit vector with respect to the p-norm.
        /// </summary>
        /// <param name="p">
        /// The p value.
        /// </param>
        /// <returns>
        /// This vector normalized to a unit vector with respect to the p-norm.
        /// </returns>
        public override Vector<decimal> Normalize(double p)
        {
            if (p < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(p));
            }

            decimal norm = (decimal)Norm(p);
            var clone = Clone();
            if (norm == 0M)
            {
                return clone;
            }

            clone.Multiply(1M / norm, clone);

            return clone;
        }
    }
}
