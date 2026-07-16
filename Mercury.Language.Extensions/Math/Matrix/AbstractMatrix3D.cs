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
    /// Abstract base class for three-dimensional numeric matrices.
    /// </summary>
    /// <typeparam name="T">Numeric element type. Must implement <see cref="INumber{T}"/> to support generic math operations.</typeparam>
    public abstract class AbstractMatrix3D<T> : IMatrix3D<T> where T : INumber<T>
    {
        /// <inheritdoc />
        public abstract T this[int indexX, int indexY, int indexZ] { get; }
        /// <inheritdoc />
        public abstract int NumberOfElements { get; }
        /// <inheritdoc />
        public abstract T[][][] Data { get; }
        /// <inheritdoc />
        public abstract bool Identity { get; }
        /// <inheritdoc />
        public abstract int Rank { get; }
        public abstract int ColumnCount { get; }
        public abstract int RowCount { get; }
        public abstract int SliceCount { get; }

        /// <inheritdoc />
        public abstract IMatrix3D<T> Add(IMatrix3D<T> x);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Abs();
        /// <inheritdoc />
        public abstract IMatrix3D<T> AbsoluteMaximum();
        /// <inheritdoc />
        public abstract IMatrix3D<T> AbsoluteMinimum();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Acos();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Asin();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Atan();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Atan2(IMatrix3D<T> other);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Ceiling();
        /// <inheritdoc />
        public abstract T Condition();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Cos();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Cosh();
        /// <inheritdoc />
        public abstract T Determinant();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Devide(T x);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Devide(IMatrix3D<T> x);
        /// <inheritdoc />
        public abstract IMatrix3D<T> DotProduct(double[] b);
        /// <inheritdoc />
        public abstract IMatrix3D<T> DotProduct(IMatrix3D<T> b);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Exp();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Floor();
        public abstract T GetEntry(params int[] indices);
        /// <inheritdoc />
        public abstract T Invert();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Log();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Log10();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Maximum();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Minimum();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Modulus(T divisor);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Multiply(T x);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Multiply(IMatrix3D<T> x);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Negate();
        /// <inheritdoc />
        public abstract T Norm();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Power(T exponent);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Prepend(IMatrix2D<T> value);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Remainder(T divisor);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Remeinder(IMatrix3D<T> divisor);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Reverse();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Rotate();
        /// <inheritdoc />
        public abstract IMatrix3D<T> RotateX();
        /// <inheritdoc />
        public abstract IMatrix3D<T> RotateX(double radians, bool clockwise = true, double precision = 1E-10);
        /// <inheritdoc />
        public abstract IMatrix3D<T> RotateY();
        /// <inheritdoc />
        public abstract IMatrix3D<T> RotateY(double radians, bool clockwise = true, double precision = 1E-10);
        /// <inheritdoc />
        public abstract IMatrix3D<T> RotateZ();
        /// <inheritdoc />
        public abstract IMatrix3D<T> RotateZ(double radians, bool clockwise = true, double precision = 1E-10);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Round();
        /// <inheritdoc />
        public abstract void SetValue(int indexX, int indexY, int indexZ, T value);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Sign();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Sin();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Sinh();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Sqrt();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Subtract(IMatrix3D<T> x);
        /// <inheritdoc />
        public abstract IMatrix3D<T> Tan();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Tanh();
        /// <inheritdoc />
        public abstract T Trace();
        /// <inheritdoc />
        public abstract IMatrix3D<T> Transpose();
    }
}
