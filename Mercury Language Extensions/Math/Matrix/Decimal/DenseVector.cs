﻿// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by Math.NET.
//
// <copyright file="DenseVector.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2013 Math.NET
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
using System.Globalization;
using System.Linq;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Storage;
using MathNet.Numerics.Providers.LinearAlgebra;
using Mercury.Language;
using Mercury.Language.Threading;

namespace MathNet.Numerics.LinearAlgebra.Decimal
{
    /// <summary>
    /// A vector using dense storage.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("DenseVector {" + nameof(Count) + "}-Decimal")]
    public class DenseVector : Vector
    {
        /// <summary>
        /// Number of elements
        /// </summary>
        readonly int _length;

        /// <summary>
        /// Gets the vector's data.
        /// </summary>
        readonly decimal[] _values;

        /// <summary>
        /// Create a new dense vector straight from an initialized vector storage instance.
        /// The storage is used directly without copying.
        /// Intended for advanced scenarios where you're working directly with
        /// storage for performance or interop reasons.
        /// </summary>
        public DenseVector(DenseVectorStorage<decimal> storage)
            : base(storage)
        {
            _length = storage.Length;
            _values = storage.Data;
        }

        /// <summary>
        /// Create a new dense vector with the given length.
        /// All cells of the vector will be initialized to zero.
        /// </summary>
        /// <exception cref="ArgumentException">If length is less than one.</exception>
        public DenseVector(int length)
            : this(DenseVectorStorage<decimal>.OfEnumerable(new decimal[length]))
        {
        }

        /// <summary>
        /// Create a new dense vector directly binding to a raw array.
        /// The array is used directly without copying.
        /// Very efficient, but changes to the array and the vector will affect each other.
        /// </summary>
        public DenseVector(decimal[] storage)
            : this(DenseVectorStorage<decimal>.OfEnumerable(storage))
        {
        }

        /// <summary>
        /// Create a new dense vector as a copy of the given other vector.
        /// This new vector will be independent from the other vector.
        /// A new memory block will be allocated for storing the vector.
        /// </summary>
        public static DenseVector OfVector(Vector<decimal> vector)
        {
            return new DenseVector(DenseVectorStorage<decimal>.OfVector(vector.Storage));
        }

        /// <summary>
        /// Create a new dense vector as a copy of the given array.
        /// This new vector will be independent from the array.
        /// A new memory block will be allocated for storing the vector.
        /// </summary>
        public static DenseVector OfArray(decimal[] array)
        {
            return new DenseVector(DenseVectorStorage<decimal>.OfVector(DenseVectorStorage<decimal>.OfEnumerable(array)));
        }

        /// <summary>
        /// Create a new dense vector as a copy of the given enumerable.
        /// This new vector will be independent from the enumerable.
        /// A new memory block will be allocated for storing the vector.
        /// </summary>
        public static DenseVector OfEnumerable(IEnumerable<decimal> enumerable)
        {
            return new DenseVector(DenseVectorStorage<decimal>.OfEnumerable(enumerable));
        }

        /// <summary>
        /// Create a new dense vector as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new vector will be independent from the enumerable.
        /// A new memory block will be allocated for storing the vector.
        /// </summary>
        public static DenseVector OfIndexedEnumerable(int length, IEnumerable<Tuple<int, decimal>> enumerable)
        {
            return new DenseVector(DenseVectorStorage<decimal>.OfIndexedEnumerable(length, enumerable));
        }

        /// <summary>
        /// Create a new dense vector as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new vector will be independent from the enumerable.
        /// A new memory block will be allocated for storing the vector.
        /// </summary>
        public static DenseVector OfIndexedEnumerable(int length, IEnumerable<(int, decimal)> enumerable)
        {
            return new DenseVector(DenseVectorStorage<decimal>.OfIndexedEnumerable(length, enumerable.Select(x => new Tuple<int, decimal>(x.Item1, x.Item2)).ToArray()));
        }

        /// <summary>
        /// Create a new dense vector and initialize each value using the provided value.
        /// </summary>
        public static DenseVector Create(int length, decimal value)
        {
            if (value == 0M) return new DenseVector(length);
            return new DenseVector(DenseVectorStorage<decimal>.OfValue(length, value));
        }

        /// <summary>
        /// Create a new dense vector and initialize each value using the provided init function.
        /// </summary>
        public static DenseVector Create(int length, Func<int, decimal> init)
        {
            return new DenseVector(DenseVectorStorage<decimal>.OfInit(length, init));
        }

        /// <summary>
        /// Create a new dense vector with values sampled from the provided random distribution.
        /// </summary>
        public static DenseVector CreateRandom(int length, IContinuousDistribution distribution)
        {
            var samples = Generate.Random(length, distribution);
            return new DenseVector(DenseVectorStorage<decimal>.OfEnumerable(samples.Select(x => (decimal)x).ToArray()));
        }

        /// <summary>
        /// Gets the vector's data.
        /// </summary>
        /// <value>The vector's data.</value>
        public decimal[] Values => _values;

        /// <summary>
        /// Returns a reference to the internal data structure.
        /// </summary>
        /// <param name="vector">The <c>DenseVector</c> whose internal data we are
        /// returning.</param>
        /// <returns>
        /// A reference to the internal date of the given vector.
        /// </returns>
        public static explicit operator decimal[](DenseVector vector)
        {
            if (vector == null)
            {
                throw new ArgumentNullException(nameof(vector));
            }

            return vector.Values;
        }

        /// <summary>
        /// Returns a vector bound directly to a reference of the provided array.
        /// </summary>
        /// <param name="array">The array to bind to the <c>DenseVector</c> object.</param>
        /// <returns>
        /// A <c>DenseVector</c> whose values are bound to the given array.
        /// </returns>
        public static implicit operator DenseVector(decimal[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            return new DenseVector(array);
        }

        /// <summary>
        /// Adds a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to add.</param>
        /// <param name="result">The vector to store the result of the addition.</param>
        protected override void DoAdd(decimal scalar, Vector<decimal> result)
        {
            if (result is DenseVector dense)
            {
                CommonParallel.For(0, _values.Length, 4096, (a, b) =>
                {
                    for (int i = a; i < b; i++)
                    {
                        dense._values[i] = _values[i] + scalar;
                    }
                });
            }
            else
            {
                base.DoAdd(scalar, result);
            }
        }

        /// <summary>
        /// Adds another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to add to this one.</param>
        /// <param name="result">The vector to store the result of the addition.</param>
        protected override void DoAdd(Vector<decimal> other, Vector<decimal> result)
        {
            if (other is DenseVector otherDense && result is DenseVector resultDense)
            {
                LinearAlgebraControl2.Provider.AddArrays(_values, otherDense._values, resultDense._values);
            }
            else
            {
                base.DoAdd(other, result);
            }
        }

        /// <summary>
        /// Adds two <strong>Vectors</strong> together and returns the results.
        /// </summary>
        /// <param name="leftSide">One of the vectors to add.</param>
        /// <param name="rightSide">The other vector to add.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> are not the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator +(DenseVector leftSide, DenseVector rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            if (leftSide.Count != rightSide.Count)
            {
                throw new ArgumentException(LocalizedResources.Instance().ALL_VECTORS_MUST_HAVE_THE_SAME_DIMENSIONALITY, nameof(rightSide));
            }

            return (DenseVector)leftSide.Add(rightSide);
        }

        /// <summary>
        /// Subtracts a scalar from each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to subtract.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        protected override void DoSubtract(decimal scalar, Vector<decimal> result)
        {
            if (result is DenseVector dense)
            {
                CommonParallel.For(0, _values.Length, 4096, (a, b) =>
                {
                    for (int i = a; i < b; i++)
                    {
                        dense._values[i] = _values[i] - scalar;
                    }
                });
            }
            else
            {
                base.DoSubtract(scalar, result);
            }
        }

        /// <summary>
        /// Subtracts another vector to this vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to subtract from this one.</param>
        /// <param name="result">The vector to store the result of the subtraction.</param>
        protected override void DoSubtract(Vector<decimal> other, Vector<decimal> result)
        {
            if (other is DenseVector otherDense && result is DenseVector resultDense)
            {
                LinearAlgebraControl2.Provider.SubtractArrays(_values, otherDense._values, resultDense._values);
            }
            else
            {
                base.DoSubtract(other, result);
            }
        }

        /// <summary>
        /// Returns a <strong>Vector</strong> containing the negated values of <paramref name="rightSide"/>.
        /// </summary>
        /// <param name="rightSide">The vector to get the values from.</param>
        /// <returns>A vector containing the negated values as <paramref name="rightSide"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator -(DenseVector rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            return (DenseVector)rightSide.Negate();
        }

        /// <summary>
        /// Subtracts two <strong>Vectors</strong> and returns the results.
        /// </summary>
        /// <param name="leftSide">The vector to subtract from.</param>
        /// <param name="rightSide">The vector to subtract.</param>
        /// <returns>The result of the subtraction.</returns>
        /// <exception cref="ArgumentException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> are not the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator -(DenseVector leftSide, DenseVector rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (DenseVector)leftSide.Subtract(rightSide);
        }

        /// <summary>
        /// Negates vector and saves result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        protected override void DoNegate(Vector<decimal> result)
        {
            if (result is DenseVector denseResult)
            {
                LinearAlgebraControl2.Provider.ScaleArray(-1.0M, _values, denseResult.Values);
            }
            else
            {
                base.DoNegate(result);
            }
        }

        /// <summary>
        /// Multiplies a scalar to each element of the vector and stores the result in the result vector.
        /// </summary>
        /// <param name="scalar">The scalar to multiply.</param>
        /// <param name="result">The vector to store the result of the multiplication.</param>
        /// <remarks></remarks>
        protected override void DoMultiply(decimal scalar, Vector<decimal> result)
        {
            if (result is DenseVector denseResult)
            {
                LinearAlgebraControl2.Provider.ScaleArray(scalar, _values, denseResult.Values);
            }
            else
            {
                base.DoMultiply(scalar, result);
            }
        }

        /// <summary>
        /// Computes the dot product between this vector and another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The sum of a[i]*b[i] for all i.</returns>
        protected override decimal DoDotProduct(Vector<decimal> other)
        {
            return other is DenseVector denseVector
                ? LinearAlgebraControl2.Provider.DotProduct(_values, denseVector.Values)
                : base.DoDotProduct(other);
        }

        /// <summary>
        /// Multiplies a vector with a scalar.
        /// </summary>
        /// <param name="leftSide">The vector to scale.</param>
        /// <param name="rightSide">The scalar value.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator *(DenseVector leftSide, decimal rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (DenseVector)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a vector with a scalar.
        /// </summary>
        /// <param name="leftSide">The scalar value.</param>
        /// <param name="rightSide">The vector to scale.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator *(decimal leftSide, DenseVector rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            return (DenseVector)rightSide.Multiply(leftSide);
        }

        /// <summary>
        /// Computes the dot product between two <strong>Vectors</strong>.
        /// </summary>
        /// <param name="leftSide">The left row vector.</param>
        /// <param name="rightSide">The right column vector.</param>
        /// <returns>The dot product between the two vectors.</returns>
        /// <exception cref="ArgumentException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> are not the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static decimal operator *(DenseVector leftSide, DenseVector rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return leftSide.DotProduct(rightSide);
        }

        /// <summary>
        /// Divides a vector with a scalar.
        /// </summary>
        /// <param name="leftSide">The vector to divide.</param>
        /// <param name="rightSide">The scalar value.</param>
        /// <returns>The result of the division.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator /(DenseVector leftSide, decimal rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (DenseVector)leftSide.Divide(rightSide);
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The divisor to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected override void DoModulus(decimal divisor, Vector<decimal> result)
        {
            if (result is DenseVector dense)
            {
                CommonParallel.For(0, _length, 4096, (a, b) =>
                {
                    for (int i = a; i < b; i++)
                    {
                        dense._values[i] = Euclid2.Modulus(_values[i], divisor);
                    }
                });
            }
            else
            {
                base.DoModulus(divisor, result);
            }
        }

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The divisor to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected override void DoRemainder(decimal divisor, Vector<decimal> result)
        {
            if (result is DenseVector dense)
            {
                CommonParallel.For(0, _length, 4096, (a, b) =>
                {
                    for (int i = a; i < b; i++)
                    {
                        dense._values[i] = _values[i] % divisor;
                    }
                });
            }
            else
            {
                base.DoRemainder(divisor, result);
            }
        }

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// of each element of the vector of the given divisor.
        /// </summary>
        /// <param name="leftSide">The vector whose elements we want to compute the remainder of.</param>
        /// <param name="rightSide">The divisor to use,</param>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static DenseVector operator %(DenseVector leftSide, decimal rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (DenseVector)leftSide.Remainder(rightSide);
        }

        /// <summary>
        /// Returns the index of the absolute minimum element.
        /// </summary>
        /// <returns>The index of absolute minimum element.</returns>
        public override int AbsoluteMinimumIndex()
        {
            var index = 0;
            var min = Math.Abs(_values[index]);
            for (var i = 1; i < _length; i++)
            {
                var test = Math.Abs(_values[i]);
                if (test < min)
                {
                    index = i;
                    min = test;
                }
            }

            return index;
        }

        /// <summary>
        /// Returns the index of the absolute maximum element.
        /// </summary>
        /// <returns>The index of absolute maximum element.</returns>
        public override int AbsoluteMaximumIndex()
        {
            var index = 0;
            var max = Math.Abs(_values[index]);
            for (var i = 1; i < _length; i++)
            {
                var test = Math.Abs(_values[i]);
                if (test > max)
                {
                    index = i;
                    max = test;
                }
            }

            return index;
        }

        /// <summary>
        /// Returns the index of the maximum element.
        /// </summary>
        /// <returns>The index of maximum element.</returns>
        public override int MaximumIndex()
        {
            var index = 0;
            var max = _values[0];
            for (var i = 1; i < _length; i++)
            {
                if (max < _values[i])
                {
                    index = i;
                    max = _values[i];
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
            var min = _values[0];
            for (var i = 1; i < _length; i++)
            {
                if (min > _values[i])
                {
                    index = i;
                    min = _values[i];
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
            for (var index = 0; index < _length; index++)
            {
                sum += _values[index];
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
            for (var index = 0; index < _length; index++)
            {
                sum += Math.Abs(_values[index]);
            }
            return (double)sum;
        }

        /// <summary>
        /// Calculates the L2 norm of the vector, also known as Euclidean norm.
        /// </summary>
        /// <returns>The square root of the sum of the squared values.</returns>
        public override double L2Norm()
        {
            // TODO: native provider
            return (double)_values.Aggregate(0M, SpecialFunctions2.Hypotenuse);
        }

        /// <summary>
        /// Calculates the infinity norm of the vector.
        /// </summary>
        /// <returns>The maximum absolute value.</returns>
        public override double InfinityNorm()
        {
            return (double)CommonParallel.Aggregate(_values, (i, v) => Math.Abs(v), Math.Max, 0M);
        }

        /// <summary>
        /// Computes the p-Norm.
        /// </summary>
        /// <param name="p">The p value.</param>
        /// <returns>Scalar <c>ret = ( ∑|this[i]|^p )^(1/p)</c></returns>
        public override double Norm(double p)
        {
            if (p < 0d) throw new ArgumentOutOfRangeException(nameof(p));

            if (p == 1d) return L1Norm();
            if (p == 2d) return L2Norm();
            if (double.IsPositiveInfinity(p)) return InfinityNorm();

            var sum = 0M;
            for (var index = 0; index < _length; index++)
            {
                sum += Math2.Pow(Math2.Abs(_values[index]), (decimal)p);
            }
            return (double)Math2.Pow(sum, 1.0M / (decimal)p);
        }

        /// <summary>
        /// Pointwise divide this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise divide this one by.</param>
        /// <param name="result">The vector to store the result of the pointwise division.</param>
        protected override void DoPointwiseMultiply(Vector<decimal> other, Vector<decimal> result)
        {
            if (other is DenseVector denseOther && result is DenseVector denseResult)
            {
                LinearAlgebraControl2.Provider.PointWiseMultiplyArrays(_values, denseOther._values, denseResult._values);
            }
            else
            {
                base.DoPointwiseMultiply(other, result);
            }
        }

        /// <summary>
        /// Pointwise divide this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="divisor">The vector to pointwise divide this one by.</param>
        /// <param name="result">The vector to store the result of the pointwise division.</param>
        /// <remarks></remarks>
        protected override void DoPointwiseDivide(Vector<decimal> divisor, Vector<decimal> result)
        {
            if (divisor is DenseVector denseOther && result is DenseVector denseResult)
            {
                LinearAlgebraControl2.Provider.PointWiseDivideArrays(_values, denseOther._values, denseResult._values);
            }
            else
            {
                base.DoPointwiseDivide(divisor, result);
            }
        }

        /// <summary>
        /// Pointwise raise this vector to an exponent vector and store the result into the result vector.
        /// </summary>
        /// <param name="exponent">The exponent vector to raise this vector values to.</param>
        /// <param name="result">The vector to store the result of the pointwise power.</param>
        protected override void DoPointwisePower(Vector<decimal> exponent, Vector<decimal> result)
        {
            if (exponent is DenseVector denseExponent && result is DenseVector denseResult)
            {
                LinearAlgebraControl2.Provider.PointWisePowerArrays(_values, denseExponent._values, denseResult._values);
            }
            else
            {
                base.DoPointwisePower(exponent, result);
            }
        }

        #region Parse Functions

        /// <summary>
        /// Creates a decimal dense vector based on a string. The string can be in the following formats (without the
        /// quotes): 'n', 'n,n,..', '(n,n,..)', '[n,n,...]', where n is a decimal.
        /// </summary>
        /// <returns>
        /// A decimal dense vector containing the values specified by the given string.
        /// </returns>
        /// <param name="value">
        /// the string to parse.
        /// </param>
        /// <param name="formatProvider">
        /// An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.
        /// </param>
        public static DenseVector Parse(string value, IFormatProvider formatProvider = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            value = value.Trim();
            if (value.Length == 0)
            {
                throw new FormatException();
            }

            // strip out parens
            if (value.StartsWith("(", StringComparison.Ordinal))
            {
                if (!value.EndsWith(")", StringComparison.Ordinal))
                {
                    throw new FormatException();
                }

                value = value.Substring(1, value.Length - 2).Trim();
            }

            if (value.StartsWith("[", StringComparison.Ordinal))
            {
                if (!value.EndsWith("]", StringComparison.Ordinal))
                {
                    throw new FormatException();
                }

                value = value.Substring(1, value.Length - 2).Trim();
            }

            // parsing
            var tokens = value.Split(new[] { formatProvider.GetTextInfo().ListSeparator, " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            var data = tokens.Select(t => decimal.Parse(t, NumberStyles.Any, formatProvider)).ToArray();
            if (data.Length == 0) throw new FormatException();
            return new DenseVector(data);
        }

        /// <summary>
        /// Converts the string representation of a real dense vector to decimal-precision dense vector equivalent.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="value">
        /// A string containing a real vector to convert.
        /// </param>
        /// <param name="result">
        /// The parsed value.
        /// </param>
        /// <returns>
        /// If the conversion succeeds, the result will contain a complex number equivalent to value.
        /// Otherwise the result will be <c>null</c>.
        /// </returns>
        public static bool TryParse(string value, out DenseVector result)
        {
            return TryParse(value, null, out result);
        }

        /// <summary>
        /// Converts the string representation of a real dense vector to decimal-precision dense vector equivalent.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="value">
        /// A string containing a real vector to convert.
        /// </param>
        /// <param name="formatProvider">
        /// An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about value.
        /// </param>
        /// <param name="result">
        /// The parsed value.
        /// </param>
        /// <returns>
        /// If the conversion succeeds, the result will contain a complex number equivalent to value.
        /// Otherwise the result will be <c>null</c>.
        /// </returns>
        public static bool TryParse(string value, IFormatProvider formatProvider, out DenseVector result)
        {
            bool ret;
            try
            {
                result = Parse(value, formatProvider);
                ret = true;
            }
            catch (ArgumentNullException)
            {
                result = null;
                ret = false;
            }
            catch (FormatException)
            {
                result = null;
                ret = false;
            }

            return ret;
        }

        #endregion
    }
}
