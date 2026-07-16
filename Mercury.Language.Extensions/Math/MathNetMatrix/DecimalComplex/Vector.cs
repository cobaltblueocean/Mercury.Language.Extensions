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

namespace MathNet.Numerics.LinearAlgebra.DecimalComplex
{
    using DecimalComplex = System.Numerics.DecimalComplex;

    /// <summary>
    /// <c>DecimalComplex</c> version of the <see cref="Vector{T}"/> class.
    /// </summary>
    [Serializable]
    public abstract class Vector : Vector<DecimalComplex>
    {
        /// <summary>
        /// Initializes a new instance of the Vector class.
        /// </summary>
        protected Vector(VectorStorage<DecimalComplex> storage)
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
        /// Conjugates vector and save result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        protected override void DoConjugate(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Conjugate, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Negates vector and saves result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        protected override void DoNegate(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Negate, result, Zeros.AllowSkip);
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
        protected override void DoAdd(DecimalComplex scalar, Vector<DecimalComplex> result)
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
        protected override void DoAdd(Vector<DecimalComplex> other, Vector<DecimalComplex> result)
        {
            Map2(DecimalComplex.Add, other, result, Zeros.AllowSkip);
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
        protected override void DoSubtract(DecimalComplex scalar, Vector<DecimalComplex> result)
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
        protected override void DoSubtract(Vector<DecimalComplex> other, Vector<DecimalComplex> result)
        {
            Map2(DecimalComplex.Subtract, other, result, Zeros.AllowSkip);
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
        protected override void DoMultiply(DecimalComplex scalar, Vector<DecimalComplex> result)
        {
            Map(x => x*scalar, result, Zeros.AllowSkip);
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
        protected override void DoDivide(DecimalComplex divisor, Vector<DecimalComplex> result)
        {
            Map(x => x/divisor, result, divisor.IsZero() ? Zeros.Include : Zeros.AllowSkip);
        }

        /// <summary>
        /// Divides a scalar by each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="dividend">The scalar to divide.</param>
        /// <param name="result">The vector to store the result of the division.</param>
        protected override void DoDivideByThis(DecimalComplex dividend, Vector<DecimalComplex> result)
        {
            Map(x => dividend/x, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise multiplies this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise multiply with this one.</param>
        /// <param name="result">The vector to store the result of the pointwise multiplication.</param>
        protected override void DoPointwiseMultiply(Vector<DecimalComplex> other, Vector<DecimalComplex> result)
        {
            Map2(DecimalComplex.Multiply, other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Pointwise divide this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="divisor">The vector to pointwise divide this one by.</param>
        /// <param name="result">The vector to store the result of the pointwise division.</param>
        protected override void DoPointwiseDivide(Vector<DecimalComplex> divisor, Vector<DecimalComplex> result)
        {
            Map2(DecimalComplex.Divide, divisor, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise raise this vector to an exponent and store the result into the result vector.
        /// </summary>
        /// <param name="exponent">The exponent to raise this vector values to.</param>
        /// <param name="result">The vector to store the result of the pointwise power.</param>
        protected override void DoPointwisePower(DecimalComplex exponent, Vector<DecimalComplex> result)
        {
            Map(x => x.Power(exponent), result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise raise this vector to an exponent vector and store the result into the result vector.
        /// </summary>
        /// <param name="exponent">The exponent vector to raise this vector values to.</param>
        /// <param name="result">The vector to store the result of the pointwise power.</param>
        protected override void DoPointwisePower(Vector<DecimalComplex> exponent, Vector<DecimalComplex> result)
        {
            Map2(DecimalComplex.Pow, exponent, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise canonical modulus, where the result has the sign of the divisor,
        /// of this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="divisor">The pointwise denominator vector to use.</param>
        /// <param name="result">The result of the modulus.</param>
        protected sealed override void DoPointwiseModulus(Vector<DecimalComplex> divisor, Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Pointwise remainder (% operator), where the result has the sign of the dividend,
        /// of this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="divisor">The pointwise denominator vector to use.</param>
        /// <param name="result">The result of the modulus.</param>
        protected sealed override void DoPointwiseRemainder(Vector<DecimalComplex> divisor, Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Pointwise applies the exponential function to each value and stores the result into the result vector.
        /// </summary>
        /// <param name="result">The vector to store the result.</param>
        protected override void DoPointwiseExp(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Exp, result, Zeros.Include);
        }

        /// <summary>
        /// Pointwise applies the natural logarithm function to each value and stores the result into the result vector.
        /// </summary>
        /// <param name="result">The vector to store the result.</param>
        protected override void DoPointwiseLog(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Log, result, Zeros.Include);
        }

        protected override void DoPointwiseAbs(Vector<DecimalComplex> result)
        {
            Map(x => DecimalComplex.Abs(x), result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAcos(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Acos, result, Zeros.Include);
        }
        protected override void DoPointwiseAsin(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Asin, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAtan(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Atan, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseAtan2(Vector<DecimalComplex> other, Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseAtan2(DecimalComplex scalar, Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseCeiling(Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseCos(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Cos, result, Zeros.Include);
        }
        protected override void DoPointwiseCosh(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Cosh, result, Zeros.Include);
        }
        protected override void DoPointwiseFloor(Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseLog10(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Log10, result, Zeros.Include);
        }
        protected override void DoPointwiseRound(Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseSign(Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }
        protected override void DoPointwiseSin(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Sin, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSinh(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Sinh, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseSqrt(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Sqrt, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseTan(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Tan, result, Zeros.AllowSkip);
        }
        protected override void DoPointwiseTanh(Vector<DecimalComplex> result)
        {
            Map(DecimalComplex.Tanh, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Computes the dot product between this vector and another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The sum of a[i]*b[i] for all i.</returns>
        protected override DecimalComplex DoDotProduct(Vector<DecimalComplex> other)
        {
            var dot = DecimalComplex.Zero;
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
        protected override DecimalComplex DoConjugateDotProduct(Vector<DecimalComplex> other)
        {
            var dot = DecimalComplex.Zero;
            for (var i = 0; i < Count; i++)
            {
                dot += At(i).Conjugate() * other.At(i);
            }
            return dot;
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected sealed override void DoModulus(DecimalComplex divisor, Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for the given dividend for each element of the vector.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected sealed override void DoModulusByThis(DecimalComplex dividend, Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected sealed override void DoRemainder(DecimalComplex divisor, Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for the given dividend for each element of the vector.
        /// </summary>
        /// <param name="dividend">The scalar numerator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected sealed override void DoRemainderByThis(DecimalComplex dividend, Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        protected override void DoPointwiseMinimum(DecimalComplex scalar, Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        protected override void DoPointwiseMaximum(DecimalComplex scalar, Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        protected override void DoPointwiseAbsoluteMinimum(DecimalComplex scalar, Vector<DecimalComplex> result)
        {
            decimal absolute = scalar.Magnitude;
            Map(x => QuickMath.Min(absolute, x.Magnitude), result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseAbsoluteMaximum(DecimalComplex scalar, Vector<DecimalComplex> result)
        {
            decimal absolute = scalar.Magnitude;
            Map(x => QuickMath.Max(absolute, x.Magnitude), result, Zeros.Include);
        }

        protected override void DoPointwiseMinimum(Vector<DecimalComplex> other, Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        protected override void DoPointwiseMaximum(Vector<DecimalComplex> other, Vector<DecimalComplex> result)
        {
            throw new NotSupportedException();
        }

        protected override void DoPointwiseAbsoluteMinimum(Vector<DecimalComplex> other, Vector<DecimalComplex> result)
        {
            Map2((x, y) => QuickMath.Min(x.Magnitude, y.Magnitude), other, result, Zeros.AllowSkip);
        }

        protected override void DoPointwiseAbsoluteMaximum(Vector<DecimalComplex> other, Vector<DecimalComplex> result)
        {
            Map2((x, y) => QuickMath.Max(x.Magnitude, y.Magnitude), other, result, Zeros.AllowSkip);
        }

        /// <summary>
        /// Returns the value of the absolute minimum element.
        /// </summary>
        /// <returns>The value of the absolute minimum element.</returns>
        public sealed override DecimalComplex AbsoluteMinimum()
        {
            return At(AbsoluteMinimumIndex()).Magnitude;
        }

        /// <summary>
        /// Returns the index of the absolute minimum element.
        /// </summary>
        /// <returns>The index of absolute minimum element.</returns>
        public override int AbsoluteMinimumIndex()
        {
            var index = 0;
            var min = At(index).Magnitude;
            for (var i = 1; i < Count; i++)
            {
                var test = At(i).Magnitude;
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
        public override DecimalComplex AbsoluteMaximum()
        {
            return At(AbsoluteMaximumIndex()).Magnitude;
        }

        /// <summary>
        /// Returns the index of the absolute maximum element.
        /// </summary>
        /// <returns>The index of absolute maximum element.</returns>
        public override int AbsoluteMaximumIndex()
        {
            var index = 0;
            var max = At(index).Magnitude;
            for (var i = 1; i < Count; i++)
            {
                var test = At(i).Magnitude;
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
        public override DecimalComplex Sum()
        {
            var sum = DecimalComplex.Zero;
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
            var sum = 0M;
            for (var i = 0; i < Count; i++)
            {
                sum += At(i).Magnitude;
            }
            return (double)sum;
        }

        /// <summary>
        /// Calculates the L2 norm of the vector, also known as Euclidean norm.
        /// </summary>
        /// <returns>The square root of the sum of the squared values.</returns>
        public override double L2Norm()
        {
            return (double)DoConjugateDotProduct(this).SquareRoot().Real;
        }

        /// <summary>
        /// Calculates the infinity norm of the vector.
        /// </summary>
        /// <returns>The maximum absolute value.</returns>
        public override double InfinityNorm()
        {
            return (double)CommonParallel.Aggregate(0, Count, i => At(i).Magnitude, QuickMath.Max, 0M);
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
            if (p < 0d) throw new ArgumentOutOfRangeException(nameof(p));

            if (p == 1d) return L1Norm();
            if (p == 2d) return L2Norm();
            if (double.IsPositiveInfinity(p)) return InfinityNorm();

            var sum = 0M;
            for (var index = 0; index < Count; index++)
            {
                sum += QuickMath.Pow(At(index).Magnitude, (decimal)p);
            }
            return (double)QuickMath.Pow(sum, 1.0M/(decimal)p);
        }

        /// <summary>
        /// Returns the index of the maximum element.
        /// </summary>
        /// <returns>The index of maximum element.</returns>
        public override int MaximumIndex()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the index of the minimum element.
        /// </summary>
        /// <returns>The index of minimum element.</returns>
        public override int MinimumIndex()
        {
            throw new NotSupportedException();
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
        public override Vector<DecimalComplex> Normalize(double p)
        {
            if (p < 0d)
            {
                throw new ArgumentOutOfRangeException(nameof(p));
            }

            double norm = Norm(p);
            var clone = Clone();
            if (norm == 0d)
            {
                return clone;
            }

            clone.Multiply(1M / (decimal)norm, clone);

            return clone;
        }
    }
}
