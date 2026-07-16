/// <summary>
/// COPYRIGHT
/// See the COPYRIGHT.txt file
/// </summary>
/// <see cref="COPYRIGHT.txt"/>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Language.Math.Matrix
{
    /// <summary>
    /// Abstract base class for one-dimensional numeric matrices (vectors).
    /// </summary>
    /// <typeparam name="T">Numeric element type. Must implement <see cref="INumber{T}"/> to support generic math operations.</typeparam>
    public abstract class AbstractMatrix1D<T> : IMatrix1D<T> where T : INumber<T>
    {
        T[] _storage;
        public int NumberOfElements => _storage.Length;

        public abstract T[] Data { get; }

        public abstract T this[int index] { get; }

        public virtual T this[params int[] indices]
        {
            get { return GetEntry(indices); }
        }

        public AbstractMatrix1D(T[] data)
        {
            _storage = data;
        }

        public virtual T GetEntry(params int[] indices)
        {
            ArgumentChecker.NotNull(indices);
            ArgumentChecker.IsTrue(indices.Length == 1);

            return _storage[indices[0]];
        }

        private void EnsureCapacity(int size)
        {
            T[] _temp = new T[size];

            Array.Fill(_temp, default(T));

            AutoParallel.AutoParallelFor(0, _temp.Length, (i) =>
            {
                if (i < _storage.Length)
                {
                    _temp[i] = _storage[i];
                }
            });

            _storage = _temp;
        }

        /// <inheritdoc />
        public abstract void SetValue(int index, T value);

        /// <inheritdoc />
        public abstract IMatrix2D<T> AsMatrix2D();

        /// <inheritdoc />
        public abstract IMatrix1D<T> Reverse();

        /// <inheritdoc />
        public abstract IMatrix1D<T> Add(IMatrix1D<T> value);

        /// <inheritdoc />
        public abstract IMatrix1D<T> Subtract(IMatrix1D<T> value);

        /// <inheritdoc />
        public abstract T Distance(IMatrix1D<T> value);

        /// <inheritdoc />
        public abstract T L1Distance(IMatrix1D<T> value);

        /// <inheritdoc />
        public abstract T LInfDistance(IMatrix1D<T> value);

        /// <inheritdoc />
        public abstract IMatrix1D<T> Sin();

        /// <inheritdoc />
        public abstract IMatrix1D<T> Tan();

        /// <inheritdoc />
        public abstract IMatrix1D<T> Multiply(IMatrix1D<T> value);

        /// <inheritdoc />
        public abstract IMatrix1D<T> Multiply(T value);

        /// <inheritdoc />
        public abstract IMatrix1D<T> Devide(IMatrix1D<T> value);

        /// <inheritdoc />
        public abstract IMatrix1D<T> Devide(T value);

        /// <inheritdoc />
        public abstract IMatrix1D<T> Power(IMatrix1D<T> value);

        /// <inheritdoc />
        public abstract IMatrix1D<T> Negate();

        /// <inheritdoc />
        public abstract T DotProduct(IMatrix1D<T> value);

        /// <inheritdoc />
        public abstract IMatrix1D<T> Projection(IMatrix1D<T> value);

        /// <inheritdoc />
        public abstract T InnerProduct(IMatrix1D<T> value);

        /// <inheritdoc />
        public abstract IMatrix2D<T> OuterProduct(IMatrix1D<T> value);

        /// <inheritdoc />
        public abstract T Norm1();

        /// <inheritdoc />
        public abstract T Norm2();

        /// <inheritdoc />
        public abstract T NormInfinity();

        /// <inheritdoc />
        public abstract T Norm();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Minimum();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Maximum();
        /// <inheritdoc />
        public abstract IMatrix1D<T> AbsoluteMinimum();
        /// <inheritdoc />
        public abstract IMatrix1D<T> AbsoluteMaximum();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Exp();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Log();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Abs();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Ceiling();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Floor();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Round();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Cos();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Sinh();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Cosh();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Tanh();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Asin();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Acos();
        /// <inheritdoc />
        public abstract IMatrix1D<T> Atan();
    }
}
