/// <summary>
/// COPYRIGHT
/// See the COPYRIGHT.txt file
/// </summary>
/// <see cref="COPYRIGHT.txt"/>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Language.Math.Matrix
{
    /// <summary>
    /// Abstract base class for two-dimensional numeric matrices.
    /// </summary>
    /// <typeparam name="T">Numeric element type. Must implement <see cref="INumber{T}"/> to support generic math operations.</typeparam>   
    public abstract class AbstractMatrix2D<T> : IMatrix2D<T> where T : INumber<T>
    {
        /// <inheritdoc />
        public abstract T this[int indexX, int indexY] { get; }

        /// <inheritdoc />
        public abstract T[][] Data { get; }
        /// <inheritdoc />
        public abstract bool Identity { get; }
        /// <inheritdoc />
        public abstract int NumberOfElements { get; }

        /// <inheritdoc />
        int IMatrix2D<T>.Rank => throw new NotImplementedException();

        /// <inheritdoc />
        public abstract int ColumnCount { get; }
        /// <inheritdoc />
        public abstract int RowCount { get; }
        /// <inheritdoc />
        public abstract IMatrix2D<T> Abs();
        /// <inheritdoc />
        public abstract IMatrix2D<T> AbsoluteMaximum();
        /// <inheritdoc />
        public abstract IMatrix2D<T> AbsoluteMinimum();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Acos();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Add(IMatrix2D<T> value);
        /// <inheritdoc />
        public abstract IMatrix2D<T> Asin();
        /// <inheritdoc />
        public abstract IMatrix3D<T> AsMatrix3D();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Atan();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Atan2(IMatrix2D<T> other);
        /// <inheritdoc />
        public abstract IMatrix2D<T> Ceiling();
        /// <inheritdoc />
        /// <inheritdoc />
        public abstract T Condition();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Cos();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Cosh();
        /// <inheritdoc />
        public abstract T Determinant();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Devide(IMatrix2D<T> value);
        /// <inheritdoc />
        public abstract IMatrix2D<T> DotProduct(double[] b);
        /// <inheritdoc />
        public abstract IMatrix2D<T> DotProduct(IMatrix2D<T> b);
        /// <inheritdoc />
        public abstract IMatrix2D<T> Floor();
        /// <inheritdoc />
        public abstract T GetEntry(params int[] indices);
        /// <inheritdoc />
        public abstract IMatrix2D<T> Log();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Log10();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Maximum();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Minimum();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Modulus(T divisor);
        /// <inheritdoc />
        public abstract IMatrix2D<T> Multiply(T value);
        /// <inheritdoc />
        public abstract IMatrix2D<T> Multiply(IMatrix2D<T> value);
        /// <inheritdoc />
        public abstract IMatrix2D<T> Negate();
        /// <inheritdoc />
        public abstract T L1Norm();
        /// <inheritdoc />
        public abstract T FrobeniusNorm();
        /// <inheritdoc />
        public abstract T InfinityNorm();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Power(T exponent);
        /// <inheritdoc />
        public abstract IMatrix2D<T> Remainder(T divisor);
        /// <inheritdoc />
        public abstract IMatrix2D<T> Remeinder(IMatrix2D<T> divisor);
        /// <inheritdoc />
        public abstract IMatrix2D<T> Reverse();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Rotate();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Rotate(double radians, bool clockwise = true, double precision = 1E-10);
        /// <inheritdoc />
        public abstract IMatrix2D<T> Round();
        /// <inheritdoc />
        public abstract void SetValue(int indexX, int indexY, T value);
        /// <inheritdoc />
        public abstract IMatrix2D<T> Sign();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Sin();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Sinh();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Sqrt();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Subtract(IMatrix2D<T> value);
        /// <inheritdoc />
        public abstract IMatrix2D<T> Tan();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Tanh();
        /// <inheritdoc />
        public abstract T Trace();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Transpose();
        /// <inheritdoc />
        public abstract IMatrix2D<T> Invert();
    }
}
