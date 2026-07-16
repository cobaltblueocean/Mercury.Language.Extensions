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
    /// Represents a 1‑dimensional numeric array (vector) API for mathematical and linear‑algebra operations.
    /// </summary>
    /// <typeparam name="T">a generic numeric element type (supports arithmetic via .NET generic math).</typeparam>
    public interface IMatrix1D<T>:IMatrix<T> where T : INumber<T>
    {
        /// <summary>
        /// Gets the entry specified by the index.
        /// </summary>
        /// <param name="index">The index, not null. The number of index must match the dimension of the matrix</param>
        /// <returns>The entry</returns>
        T this[int index] { get; }

        /// <summary>
        /// Returns the underlying vector data. If this is changed so is the vector.
        /// </summary>
        T[] Data { get; }

        /// <summary>
        /// Set value at the index
        /// </summary>
        /// <param name="index">Index to set the value</param>
        /// <param name="value">Value to be set</param>
        public void SetValue(int index, T value);

        /// <summary>
        /// Return a reversed order Matrix1D
        /// </summary>
        /// <returns>Reversed Matrix</returns>
        public IMatrix1D<T> Reverse();

        /// <summary>
        /// Compute the Sum of this and <code>value</code>.
        /// </summary>
        /// <param name="">matrix to be added</param>
        /// <returns>Added Matrix</returns>
        public IMatrix1D<T> Add(IMatrix1D<T> value);

        /// <summary>
        /// Compute  this minus <code>value</code>.
        /// </summary>
        /// <param name="">matrix to be subtracted</param>
        /// <returns>Subtarcted Matrix</returns>
        public IMatrix1D<T> Subtract(IMatrix1D<T> value);

        /// <summary>
        /// Distance between two Matrixes.
        /// <p>This method computes the distance consistent with the L<sub>2</sub> norm, i.e. the square root of the Sum of element differences, or Euclidean distance.</p>
        /// </summary>
        /// <param name="">matrix to which distance is requested</param>
        /// <returns>the distance between two vectors.</returns>
        public T Distance(IMatrix1D<T> value);

        /// <summary>
        /// Distance between two Matrixes.
        /// <p>This method computes the distance consistent with the L<sub>1</sub> norm, i.e. the square root of the Sum of element differences, or Euclidean distance.</p>
        /// </summary>
        /// <param name="">matrix to which distance is requested</param>
        /// <returns>the distance between two vectors.</returns>
        public T L1Distance(IMatrix1D<T> value);

        /// <summary>
        /// Distance between two Matrixes.
        /// <p>This method computes the distance consistent with the L<sub>&infin;</sub> norm, i.e. the square root of the Sum of element differences, or Euclidean distance.</p>
        /// </summary>
        /// <param name="">matrix to which distance is requested</param>
        /// <returns>the distance between two vectors.</returns>
        public T LInfDistance(IMatrix1D<T> value);

        /// <summary>
        /// Element-by-element multiplication.
        /// </summary>
        /// <param name="">Vector by which instance elements must be multiplied</param>
        /// <returns>a vector containing this[i] * value[i] for all i.</returns>
        public IMatrix1D<T> Multiply(IMatrix1D<T> value);

        /// <summary>
        /// Element-by-element multiplication.
        /// </summary>
        /// <param name="">Vector by which instance elements must be multiplied</param>
        /// <returns>a vector containing this[i] * value for all i.</returns>
        public IMatrix1D<T> Multiply(T value);

        /// <summary>
        /// Element-by-element division.
        /// </summary>
        /// <param name="">Vector by which instance elements must be divided.</param>
        /// <returns>a vector containing this[i] / value[i] for all i.</returns>
        public IMatrix1D<T> Devide(IMatrix1D<T> value);

        /// <summary>
        /// Element-by-element division.
        /// </summary>
        /// <param name="">Vector by which instance elements must be divided.</param>
        /// <returns>a vector containing this[i] / value for all i.</returns>
        public IMatrix1D<T> Devide(T value);

        /// <summary>
        /// Pointwise raise this vector to an exponent vector and store the result into the result vector.
        /// </summary>
        /// <param name="">The exponent vector to raise this vector values to.</param>
        /// <returns>The vector to store the result of the pointwise power.</returns>
        public IMatrix1D<T> Power(IMatrix1D<T> value);

        /// <summary>
        /// Negates vector
        /// </summary>
        /// <returns>A vector which negated</returns>
        public IMatrix1D<T> Negate();

        /// <summary>
        /// Computes the dot Product between this vector and another vector.
        /// </summary>
        /// <param name="b">The other vector.</param>
        /// <returns>The Sum of a[i]*b[i] for all i.</returns>
        public T DotProduct(IMatrix1D<T> value);

        /// <summary>
        /// Find the orthogonal projection of this vector onto another vector.
        /// </summary>
        /// <param name="b">vector onto which instance must be projected.</param>
        /// <returns>projection of the instance onto <code>b</code>.</returns>
        public IMatrix1D<T> Projection(IMatrix1D<T> value);

        /// <summary>
        /// Calculates the inner Product of this vector and given <code>b</code>.
        /// Alias of <code>DotProduct()</code>
        /// </summary>
        /// <param name="b">the right hand vector for inner Product</param>
        /// <returns>The Sum of a[i]*b[i] for all i.</returns>
        public T InnerProduct(IMatrix1D<T> value);

        /// <summary>
        /// Compute the outer Product.
        /// </summary>
        /// <param name="b">Vector with which outer Product should be computed.</param>
        /// <returns>the matrix outer Product between this instance and <code>b</code>.</returns>
        public IMatrix2D<T> OuterProduct(IMatrix1D<T> value);

        /// <summary>
        /// Calculates the L1 norm of the vector, also known as Manhattan norm.
        /// </summary>
        /// <returns>The Sum of the absolute values.</returns>
        public T Norm1();

        /// <summary>
        /// Calculates the L2 norm of the vector, also known as Euclidean norm.
        /// </summary>
        /// <returns>The square root of the Sum of the squared values.</returns>
        public T Norm2();

        /// <summary>
        /// Returns the L<sub>&infin;</sub> norm of the matrix.
        /// <p>The L<sub>&infin;</sub> norm is the Max of the absolute values of the elements.</p>
        /// </summary>
        /// <returns>the norm.</returns>
        /// <see cref="Norm1"/>
        /// <see cref="Norm2"/>
        /// <see cref=""/>
        public T NormInfinity();

        /// <summary>
        /// Returns the L<sub>2</sub> norm of the matrix.
        /// <p>The L<sub>2</sub> norm is the root of the Sum of the squared elements.</p>
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <returns>the norm.</returns>
        /// <see cref="Norm1"/>
        /// <see cref="Norm2"/>
        /// <see cref="NormInfinity"/>
        public T Norm();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public IMatrix1D<T> Minimum();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public IMatrix1D<T> Maximum();

        /// <summary>
        /// Create a Matrix which filled by absolute minimum value
        /// </summary>
        /// <returns>Matrix of absolute minimum value</returns>
        public IMatrix1D<T> AbsoluteMinimum();

        /// <summary>
        /// Create a Matrix which filled by absolute maximum value
        /// </summary>
        /// <returns>Matrix of absolute maximum value</returns>
        public IMatrix1D<T> AbsoluteMaximum();
        /// <summary>
        /// Returns a new matrix whose elements are the exponential of the corresponding elements in the current matrix.
        /// </summary>
        /// <remarks>This operation applies the exponential function to each element individually. The
        /// result matrix has the same dimensions as the original.</remarks>
        /// <returns>An <see cref="IMatrix1D{T}"/> containing the element-wise exponential values of the current matrix.</returns>
        public IMatrix1D<T> Exp();

        /// <summary>
        /// Returns a new matrix whose elements are the natural logarithms of the corresponding elements in the current
        /// matrix.
        /// </summary>
        /// <remarks>Elements with non-positive values may result in undefined or special values (such as
        /// NaN or negative infinity), depending on the underlying numeric type. The original matrix is not
        /// modified.</remarks>
        /// <returns>An <see cref="IMatrix1D{T}"/> containing the natural logarithms of the elements of this matrix. Each element
        /// is computed as ln(x), where x is the value of the corresponding element in the original matrix.</returns>
        public IMatrix1D<T> Log();

        /// <summary>
        /// Returns a matrix whose elements are the absolute values of the corresponding elements in the current matrix.
        /// </summary>
        /// <returns>An <see cref="IMatrix1D{T}"/> containing the absolute values of the elements in the current matrix.</returns>
        public IMatrix1D<T> Abs();

        /// <summary>
        /// Returns a new matrix with each element rounded up to the nearest integer value.
        /// </summary>
        /// <remarks>This method does not modify the original matrix. The operation is typically used to
        /// eliminate fractional components by rounding values upward.</remarks>
        /// <returns>An <see cref="IMatrix1D{T}"/> whose elements are the smallest integers greater than or equal to the
        /// corresponding elements of the original matrix.</returns>
        public IMatrix1D<T> Ceiling();

        /// <summary>
        /// Returns a new matrix with each element rounded down to the nearest integer value.
        /// </summary>
        /// <remarks>This method does not modify the original matrix. The operation is typically used to
        /// eliminate fractional components by rounding values upward.</remarks>
        /// <returns>An <see cref="IMatrix1D{T}"/> whose elements are the smallest integers greater than or equal to the
        /// corresponding elements of the original matrix.</returns>
        public IMatrix1D<T> Floor();

        /// <summary>
        /// Returns a new matrix with each element rounded to the nearest integer value.
        /// </summary>
        /// <remarks>This method does not modify the original matrix. The operation is typically used to
        /// eliminate fractional components by rounding values upward.</remarks>
        /// <returns>An <see cref="IMatrix1D{T}"/> whose elements are the smallest integers greater than or equal to the
        /// corresponding elements of the original matrix.</returns>
        public IMatrix1D<T> Round();

        /// <summary>
        /// Computes the cosine of the angle between this vector and the argument.
        /// </summary>
        /// <returns>the cosine of the angle between this matrix and <code>value</code>.</returns>
        public IMatrix1D<T> Cos();


        /// <summary>
        /// Computes the sin of the angle between this vector and the argument.
        /// </summary>
        /// <returns>the sin of the angle between this matrix and <code>value</code>.</returns>
        public IMatrix1D<T> Sin();


        /// <summary>
        /// Computes the tangent of the angle between this vector and the argument.
        /// </summary>
        /// <returns>the tangent of the angle between this matrix and <code>value</code>.</returns>
        public IMatrix1D<T> Tan();


        /// <summary>
        /// Computes the hyperbolic sine of the corresponding elements in this vector and the argument.
        /// </summary>
        /// <remarks>Each element in the resulting matrix is computed as Math.Sinh(element) for the
        /// corresponding element in the original matrix. The original matrix is not modified.</remarks>
        /// <returns>A matrix containing the sine of each element of the current matrix.</returns>
        public IMatrix1D<T> Sinh();

        /// <summary>
        /// Computes the hyperbolic cosine of the corresponding elements in this vector and the argument.
        /// </summary>
        /// <remarks>The cosine is computed element-wise. The type parameter <typeparamref name="T"/> must
        /// support cosine operations; otherwise, an exception may be thrown at runtime.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> containing the cosine of each element of the current matrix.</returns>
        public IMatrix1D<T> Cosh();

        /// <summary>
        /// Computes the hyperbolic tangent of the corresponding elements in this vector and the argument.
        /// </summary>
        /// <remarks>The tangent is computed element-wise. The resulting matrix has the same dimensions as
        /// the original. If any element is outside the valid domain for the tangent function, the corresponding result
        /// may be NaN or infinity, depending on the underlying numeric type.</remarks>
        /// <returns>A matrix containing the tangent of each element in the current matrix.</returns>
        public IMatrix1D<T> Tanh();

        /// <summary>
        /// Computes the arc sine (inverse sine) of the corresponding elements in this vector and the argument.
        /// </summary>
        /// <remarks>Each element in the resulting matrix is computed as Math.Asin(x), where x is the
        /// corresponding element in the original matrix. The input values should be in the range [-1, 1]; values
        /// outside this range will result in NaN elements in the output matrix.</remarks>
        /// <returns>A matrix containing the arc sine of each element of the current matrix, with the same dimensions and element
        /// type.</returns>
        public IMatrix1D<T> Asin();

        /// <summary>
        /// Computes the arc cosine (inverse cosine) of the corresponding elements in this vector and the argument.
        /// </summary>
        /// <remarks>Each element in the resulting matrix is computed as Math.Acos(x), where x is the
        /// corresponding element in the original matrix. The input values must be in the range [-1, 1]; otherwise, the
        /// result for those elements will be NaN.</remarks>
        /// <returns>A matrix containing the arc-cosine of each element of the current matrix, with the same dimensions and
        /// element type.</returns>
        public IMatrix1D<T> Acos();

        /// <summary>
        /// Returns a new matrix whose elements are the arctangent (inverse tangent) of the corresponding elements in
        /// the current matrix.
        /// </summary>
        /// <remarks>The arctangent is computed element-wise. The result is expressed in radians. The
        /// behavior for elements outside the valid domain of the arctangent function depends on the implementation of
        /// <typeparamref name="T"/>.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> containing the arctangent of each element in the current matrix. The returned
        /// matrix has the same dimensions as the original.</returns>
        public IMatrix1D<T> Atan();

        /// <summary>
        /// Create 2D Matrix with this Matrix
        /// </summary>
        /// <returns>2D Matrix by this Matrix data</returns>
        public IMatrix2D<T> AsMatrix2D();
    }
}
