﻿// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by OpenGamma Inc.
//
// <copyright file="SparseVector.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2015 Math.NET
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
using MathNet.Numerics.LinearAlgebra.Storage;
using MathNet.Numerics.Providers.LinearAlgebra;
using Mercury.Language.Threading;

namespace MathNet.Numerics.LinearAlgebra.Decimal
{
    /// <summary>
    /// A vector with sparse storage, intended for very large vectors where most of the cells are zero.
    /// </summary>
    /// <remarks>The sparse vector is not thread safe.</remarks>
    [Serializable]
    [DebuggerDisplay("SparseVector {Count}-Decimal {NonZerosCount}-NonZero")]
    public class SparseVector : Vector
    {
        readonly SparseVectorStorage<decimal> _storage;

        /// <summary>
        /// Gets the number of non zero elements in the vector.
        /// </summary>
        /// <value>The number of non zero elements.</value>
        public int NonZerosCount => _storage.ValueCount;

        /// <summary>
        /// Create a new sparse vector straight from an initialized vector storage instance.
        /// The storage is used directly without copying.
        /// Intended for advanced scenarios where you're working directly with
        /// storage for performance or interop reasons.
        /// </summary>
        public SparseVector(SparseVectorStorage<decimal> storage)
            : base(storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Create a new sparse vector with the given length.
        /// All cells of the vector will be initialized to zero.
        /// </summary>
        /// <exception cref="ArgumentException">If length is less than one.</exception>
        public SparseVector(int length)
            : this(SparseVectorStorage<decimal>.OfEnumerable(new decimal[length]))
        {
        }

        /// <summary>
        /// Create a new sparse vector as a copy of the given other vector.
        /// This new vector will be independent from the other vector.
        /// A new memory block will be allocated for storing the vector.
        /// </summary>
        public static SparseVector OfVector(Vector<decimal> vector)
        {
            return new SparseVector(SparseVectorStorage<decimal>.OfVector(vector.Storage));
        }

        /// <summary>
        /// Create a new sparse vector as a copy of the given enumerable.
        /// This new vector will be independent from the enumerable.
        /// A new memory block will be allocated for storing the vector.
        /// </summary>
        public static SparseVector OfEnumerable(IEnumerable<decimal> enumerable)
        {
            return new SparseVector(SparseVectorStorage<decimal>.OfEnumerable(enumerable));
        }

        /// <summary>
        /// Create a new sparse vector as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new vector will be independent from the enumerable.
        /// A new memory block will be allocated for storing the vector.
        /// </summary>
        public static SparseVector OfIndexedEnumerable(int length, IEnumerable<Tuple<int, decimal>> enumerable)
        {
            return new SparseVector(SparseVectorStorage<decimal>.OfIndexedEnumerable(length, enumerable));
        }

        /// <summary>
        /// Create a new sparse vector as a copy of the given indexed enumerable.
        /// Keys must be provided at most once, zero is assumed if a key is omitted.
        /// This new vector will be independent from the enumerable.
        /// A new memory block will be allocated for storing the vector.
        /// </summary>
        public static SparseVector OfIndexedEnumerable(int length, IEnumerable<(int, decimal)> enumerable)
        {
            return new SparseVector(SparseVectorStorage<decimal>.OfIndexedEnumerable(length, enumerable.Select(x => new Tuple<int, decimal>(x.Item1, x.Item2)).ToArray()));
        }

        /// <summary>
        /// Create a new sparse vector and initialize each value using the provided value.
        /// </summary>
        public static SparseVector Create(int length, decimal value)
        {
            return new SparseVector(SparseVectorStorage<decimal>.OfValue(length, value));
        }

        /// <summary>
        /// Create a new sparse vector and initialize each value using the provided init function.
        /// </summary>
        public static SparseVector Create(int length, Func<int, decimal> init)
        {
            return new SparseVector(SparseVectorStorage<decimal>.OfInit(length, init));
        }

        /// <summary>
        /// Adds a scalar to each element of the vector and stores the result in the result vector.
        /// Warning, the new 'sparse vector' with a non-zero scalar added to it will be a 100% filled
        /// sparse vector and very inefficient. Would be better to work with a dense vector instead.
        /// </summary>
        /// <param name="scalar">
        /// The scalar to add.
        /// </param>
        /// <param name="result">
        /// The vector to store the result of the addition.
        /// </param>
        protected override void DoAdd(decimal scalar, Vector<decimal> result)
        {
            if (scalar == 0.0M)
            {
                if (!ReferenceEquals(this, result))
                {
                    CopyTo(result);
                }

                return;
            }

            if (ReferenceEquals(this, result))
            {
                // populate a new vector with the scalar
                var vnonZeroValues = new decimal[Count];
                var vnonZeroIndices = new int[Count];
                for (int index = 0; index < Count; index++)
                {
                    vnonZeroIndices[index] = index;
                    vnonZeroValues[index] = scalar;
                }

                // populate the non zero values from this
                var indices = _storage.Indices;
                var values = _storage.Values;
                for (int j = 0; j < _storage.ValueCount; j++)
                {
                    vnonZeroValues[indices[j]] = values[j] + scalar;
                }

                // assign this vectors array to the new arrays.
                _storage.Values = vnonZeroValues;
                _storage.Indices = vnonZeroIndices;
                _storage.ValueCount = Count;
            }
            else
            {
                for (var index = 0; index < Count; index++)
                {
                    result.At(index, At(index) + scalar);
                }
            }
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
            if (other is SparseVector otherSparse && result is SparseVector resultSparse)
            {
                // TODO (ruegg, 2011-10-11): Options to optimize?
                var otherStorage = otherSparse._storage;
                if (ReferenceEquals(this, resultSparse))
                {
                    int i = 0, j = 0;
                    while (j < otherStorage.ValueCount)
                    {
                        if (i >= _storage.ValueCount || _storage.Indices[i] > otherStorage.Indices[j])
                        {
                            var otherValue = otherStorage.Values[j];
                            if (otherValue != 0.0M)
                            {
                                //_storage.InsertAtIndex(i++, otherStorage.Indices[j], otherValue);
                                _storage.At(otherStorage.Indices[j], otherValue);
                            }

                            j++;
                        }
                        else if (_storage.Indices[i] == otherStorage.Indices[j])
                        {
                            // TODO: result can be zero, remove?
                            _storage.Values[i++] += otherStorage.Values[j++];
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
                else
                {
                    result.Clear();
                    int i = 0, j = 0, last = -1;
                    while (i < _storage.ValueCount || j < otherStorage.ValueCount)
                    {
                        if (j >= otherStorage.ValueCount || i < _storage.ValueCount && _storage.Indices[i] <= otherStorage.Indices[j])
                        {
                            var next = _storage.Indices[i];
                            if (next != last)
                            {
                                last = next;
                                result.At(next, _storage.Values[i] + otherSparse.At(next));
                            }

                            i++;
                        }
                        else
                        {
                            var next = otherStorage.Indices[j];
                            if (next != last)
                            {
                                last = next;
                                result.At(next, At(next) + otherStorage.Values[j]);
                            }

                            j++;
                        }
                    }
                }
            }
            else
            {
                base.DoAdd(other, result);
            }
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
            DoAdd(-scalar, result);
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
            if (ReferenceEquals(this, other))
            {
                result.Clear();
                return;
            }

            if (other is SparseVector otherSparse && result is SparseVector resultSparse)
            {
                // TODO (ruegg, 2011-10-11): Options to optimize?
                var otherStorage = otherSparse._storage;
                if (ReferenceEquals(this, resultSparse))
                {
                    int i = 0, j = 0;
                    while (j < otherStorage.ValueCount)
                    {
                        if (i >= _storage.ValueCount || _storage.Indices[i] > otherStorage.Indices[j])
                        {
                            var otherValue = otherStorage.Values[j];
                            if (otherValue != 0.0M)
                            {
                                //_storage.InsertAtIndexUnchecked(i++, otherStorage.Indices[j], -otherValue);
                                _storage.At(otherStorage.Indices[j], -otherValue);
                            }

                            j++;
                        }
                        else if (_storage.Indices[i] == otherStorage.Indices[j])
                        {
                            // TODO: result can be zero, remove?
                            _storage.Values[i++] -= otherStorage.Values[j++];
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
                else
                {
                    result.Clear();
                    int i = 0, j = 0, last = -1;
                    while (i < _storage.ValueCount || j < otherStorage.ValueCount)
                    {
                        if (j >= otherStorage.ValueCount || i < _storage.ValueCount && _storage.Indices[i] <= otherStorage.Indices[j])
                        {
                            var next = _storage.Indices[i];
                            if (next != last)
                            {
                                last = next;
                                result.At(next, _storage.Values[i] - otherSparse.At(next));
                            }

                            i++;
                        }
                        else
                        {
                            var next = otherStorage.Indices[j];
                            if (next != last)
                            {
                                last = next;
                                result.At(next, At(next) - otherStorage.Values[j]);
                            }

                            j++;
                        }
                    }
                }
            }
            else
            {
                base.DoSubtract(other, result);
            }
        }

        /// <summary>
        /// Negates vector and saves result to <paramref name="result"/>
        /// </summary>
        /// <param name="result">Target vector</param>
        protected override void DoNegate(Vector<decimal> result)
        {
            if (result is SparseVector sparseResult)
            {
                if (!ReferenceEquals(this, result))
                {
                    sparseResult._storage.ValueCount = _storage.ValueCount;
                    sparseResult._storage.Indices = new int[_storage.ValueCount];
                    Buffer.BlockCopy(_storage.Indices, 0, sparseResult._storage.Indices, 0, _storage.ValueCount * Constants.SizeOfInt);
                    sparseResult._storage.Values = new decimal[_storage.ValueCount];
                    Array.Copy(_storage.Values, 0, sparseResult._storage.Values, 0, _storage.ValueCount);
                }

                LinearAlgebraControl2.Provider.ScaleArray(-1.0M, sparseResult._storage.Values, sparseResult._storage.Values);
            }
            else
            {
                result.Clear();
                for (var index = 0; index < _storage.ValueCount; index++)
                {
                    result.At(_storage.Indices[index], -_storage.Values[index]);
                }
            }
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
            if (result is SparseVector sparseResult)
            {
                if (!ReferenceEquals(this, result))
                {
                    sparseResult._storage.ValueCount = _storage.ValueCount;
                    sparseResult._storage.Indices = new int[_storage.ValueCount];
                    Buffer.BlockCopy(_storage.Indices, 0, sparseResult._storage.Indices, 0, _storage.ValueCount * Constants.SizeOfInt);
                    sparseResult._storage.Values = new decimal[_storage.ValueCount];
                    Array.Copy(_storage.Values, 0, sparseResult._storage.Values, 0, _storage.ValueCount);
                }

                LinearAlgebraControl2.Provider.ScaleArray(scalar, sparseResult._storage.Values, sparseResult._storage.Values);
            }
            else
            {
                result.Clear();
                for (var index = 0; index < _storage.ValueCount; index++)
                {
                    result.At(_storage.Indices[index], scalar * _storage.Values[index]);
                }
            }
        }

        /// <summary>
        /// Computes the dot product between this vector and another vector.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns>The sum of a[i]*b[i] for all i.</returns>
        protected override decimal DoDotProduct(Vector<decimal> other)
        {
            var result = 0M;
            if (ReferenceEquals(this, other))
            {
                for (var i = 0; i < _storage.ValueCount; i++)
                {
                    result += _storage.Values[i] * _storage.Values[i];
                }
            }
            else
            {
                for (var i = 0; i < _storage.ValueCount; i++)
                {
                    result += _storage.Values[i] * other.At(_storage.Indices[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// Computes the canonical modulus, where the result has the sign of the divisor,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected override void DoModulus(decimal divisor, Vector<decimal> result)
        {
            if (ReferenceEquals(this, result))
            {
                for (var index = 0; index < _storage.ValueCount; index++)
                {
                    _storage.Values[index] = Euclid2.Modulus(_storage.Values[index], divisor);
                }
            }
            else
            {
                result.Clear();
                for (var index = 0; index < _storage.ValueCount; index++)
                {
                    result.At(_storage.Indices[index], Euclid2.Modulus(_storage.Values[index], divisor));
                }
            }
        }

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// for each element of the vector for the given divisor.
        /// </summary>
        /// <param name="divisor">The scalar denominator to use.</param>
        /// <param name="result">A vector to store the results in.</param>
        protected override void DoRemainder(decimal divisor, Vector<decimal> result)
        {
            if (ReferenceEquals(this, result))
            {
                for (var index = 0; index < _storage.ValueCount; index++)
                {
                    _storage.Values[index] %= divisor;
                }
            }
            else
            {
                result.Clear();
                for (var index = 0; index < _storage.ValueCount; index++)
                {
                    result.At(_storage.Indices[index], _storage.Values[index] % divisor);
                }
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
        public static SparseVector operator +(SparseVector leftSide, SparseVector rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (SparseVector)leftSide.Add(rightSide);
        }

        /// <summary>
        /// Returns a <strong>Vector</strong> containing the negated values of <paramref name="rightSide"/>.
        /// </summary>
        /// <param name="rightSide">The vector to get the values from.</param>
        /// <returns>A vector containing the negated values as <paramref name="rightSide"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseVector operator -(SparseVector rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            return (SparseVector)rightSide.Negate();
        }

        /// <summary>
        /// Subtracts two <strong>Vectors</strong> and returns the results.
        /// </summary>
        /// <param name="leftSide">The vector to subtract from.</param>
        /// <param name="rightSide">The vector to subtract.</param>
        /// <returns>The result of the subtraction.</returns>
        /// <exception cref="ArgumentException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> are not the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseVector operator -(SparseVector leftSide, SparseVector rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (SparseVector)leftSide.Subtract(rightSide);
        }

        /// <summary>
        /// Multiplies a vector with a scalar.
        /// </summary>
        /// <param name="leftSide">The vector to scale.</param>
        /// <param name="rightSide">The scalar value.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static SparseVector operator *(SparseVector leftSide, decimal rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (SparseVector)leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a vector with a scalar.
        /// </summary>
        /// <param name="leftSide">The scalar value.</param>
        /// <param name="rightSide">The vector to scale.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static SparseVector operator *(decimal leftSide, SparseVector rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException(nameof(rightSide));
            }

            return (SparseVector)rightSide.Multiply(leftSide);
        }

        /// <summary>
        /// Computes the dot product between two <strong>Vectors</strong>.
        /// </summary>
        /// <param name="leftSide">The left row vector.</param>
        /// <param name="rightSide">The right column vector.</param>
        /// <returns>The dot product between the two vectors.</returns>
        /// <exception cref="ArgumentException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> are not the same size.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static decimal operator *(SparseVector leftSide, SparseVector rightSide)
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
        public static SparseVector operator /(SparseVector leftSide, decimal rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (SparseVector)leftSide.Divide(rightSide);
        }

        /// <summary>
        /// Computes the remainder (% operator), where the result has the sign of the dividend,
        /// of each element of the vector of the given divisor.
        /// </summary>
        /// <param name="leftSide">The vector whose elements we want to compute the modulus of.</param>
        /// <param name="rightSide">The divisor to use,</param>
        /// <returns>The result of the calculation</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static SparseVector operator %(SparseVector leftSide, decimal rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException(nameof(leftSide));
            }

            return (SparseVector)leftSide.Remainder(rightSide);
        }

        /// <summary>
        /// Returns the index of the absolute minimum element.
        /// </summary>
        /// <returns>The index of absolute minimum element.</returns>
        public override int AbsoluteMinimumIndex()
        {
            if (_storage.ValueCount == 0)
            {
                // No non-zero elements. Return 0
                return 0;
            }

            var index = 0;
            var min = Math.Abs(_storage.Values[index]);
            for (var i = 1; i < _storage.ValueCount; i++)
            {
                var test = Math.Abs(_storage.Values[i]);
                if (test < min)
                {
                    index = i;
                    min = test;
                }
            }

            return _storage.Indices[index];
        }

        /// <summary>
        /// Returns the index of the absolute maximum element.
        /// </summary>
        /// <returns>The index of absolute maximum element.</returns>
        public override int AbsoluteMaximumIndex()
        {
            if (_storage.ValueCount == 0)
            {
                // No non-zero elements. Return 0
                return 0;
            }

            var index = 0;
            var max = Math.Abs(_storage.Values[index]);
            for (var i = 1; i < _storage.ValueCount; i++)
            {
                var test = Math.Abs(_storage.Values[i]);
                if (test > max)
                {
                    index = i;
                    max = test;
                }
            }

            return _storage.Indices[index];
        }

        /// <summary>
        /// Returns the index of the maximum element.
        /// </summary>
        /// <returns>The index of maximum element.</returns>
        public override int MaximumIndex()
        {
            if (_storage.ValueCount == 0)
            {
                return 0;
            }

            var index = 0;
            var max = _storage.Values[0];
            for (var i = 1; i < _storage.ValueCount; i++)
            {
                if (max < _storage.Values[i])
                {
                    index = i;
                    max = _storage.Values[i];
                }
            }

            return _storage.Indices[index];
        }

        /// <summary>
        /// Returns the index of the minimum element.
        /// </summary>
        /// <returns>The index of minimum element.</returns>
        public override int MinimumIndex()
        {
            if (_storage.ValueCount == 0)
            {
                return 0;
            }

            var index = 0;
            var min = _storage.Values[0];
            for (var i = 1; i < _storage.ValueCount; i++)
            {
                if (min > _storage.Values[i])
                {
                    index = i;
                    min = _storage.Values[i];
                }
            }

            return _storage.Indices[index];
        }

        /// <summary>
        /// Computes the sum of the vector's elements.
        /// </summary>
        /// <returns>The sum of the vector's elements.</returns>
        public override decimal Sum()
        {
            decimal result = 0;
            for (var i = 0; i < _storage.ValueCount; i++)
            {
                result += _storage.Values[i];
            }
            return result;
        }

        /// <summary>
        /// Calculates the L1 norm of the vector, also known as Manhattan norm.
        /// </summary>
        /// <returns>The sum of the absolute values.</returns>
        public override double L1Norm()
        {
            var result = 0M;
            for (var i = 0; i < _storage.ValueCount; i++)
            {
                result += QuickMath.Abs(_storage.Values[i]);
            }
            return (double)result;
        }

        /// <summary>
        /// Calculates the infinity norm of the vector.
        /// </summary>
        /// <returns>The maximum absolute value.</returns>
        public override double InfinityNorm()
        {
            return (double)CommonParallel.Aggregate(0, _storage.ValueCount, i => QuickMath.Abs(_storage.Values[i]), QuickMath.Max, 0M);
        }

        /// <summary>
        /// Computes the p-Norm.
        /// </summary>
        /// <param name="p">The p value.</param>
        /// <returns>Scalar <c>ret = ( ∑|this[i]|^p )^(1/p)</c></returns>
        public override double Norm(double p)
        {
            if (p < 0d) throw new ArgumentOutOfRangeException(nameof(p));

            if (_storage.ValueCount == 0)
            {
                return 0d;
            }

            if (p == 1d) return L1Norm();
            if (p == 2d) return L2Norm();
            if (double.IsPositiveInfinity(p)) return InfinityNorm();

            var sum = 0M;
            for (var index = 0; index < _storage.ValueCount; index++)
            {
                sum += QuickMath.Pow(QuickMath.Abs(_storage.Values[index]), (decimal)p);
            }
            return (double)QuickMath.Pow(sum, 1.0M / (decimal)p);
        }

        /// <summary>
        /// Pointwise multiplies this vector with another vector and stores the result into the result vector.
        /// </summary>
        /// <param name="other">The vector to pointwise multiply with this one.</param>
        /// <param name="result">The vector to store the result of the pointwise multiplication.</param>
        protected override void DoPointwiseMultiply(Vector<decimal> other, Vector<decimal> result)
        {
            if (ReferenceEquals(this, other) && ReferenceEquals(this, result))
            {
                for (var i = 0; i < _storage.ValueCount; i++)
                {
                    _storage.Values[i] *= _storage.Values[i];
                }
            }
            else
            {
                base.DoPointwiseMultiply(other, result);
            }
        }

        #region Parse Functions

        /// <summary>
        /// Creates a decimal sparse vector based on a string. The string can be in the following formats (without the
        /// quotes): 'n', 'n,n,..', '(n,n,..)', '[n,n,...]', where n is a decimal.
        /// </summary>
        /// <returns>
        /// A decimal sparse vector containing the values specified by the given string.
        /// </returns>
        /// <param name="value">
        /// the string to parse.
        /// </param>
        /// <param name="formatProvider">
        /// An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.
        /// </param>
        public static SparseVector Parse(string value, IFormatProvider formatProvider = null)
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
            var data = tokens.Select(t => decimal.Parse(t, NumberStyles.Any, formatProvider)).ToList();
            if (data.Count == 0) throw new FormatException();
            return new SparseVector(SparseVectorStorage<decimal>.OfEnumerable(data));
        }

        /// <summary>
        /// Converts the string representation of a real sparse vector to decimal-precision sparse vector equivalent.
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
        public static bool TryParse(string value, out SparseVector result)
        {
            return TryParse(value, null, out result);
        }

        /// <summary>
        /// Converts the string representation of a real sparse vector to decimal-precision sparse vector equivalent.
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
        public static bool TryParse(string value, IFormatProvider formatProvider, out SparseVector result)
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

        public override string ToTypeString()
        {
            return FormattableString.Invariant($"SparseVector {Count}-Double {NonZerosCount / (decimal)Count:P2} Filled");
        }
    }
}
