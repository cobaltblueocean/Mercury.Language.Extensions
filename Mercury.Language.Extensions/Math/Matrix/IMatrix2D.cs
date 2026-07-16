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
    /// Represents a two-dimensional numeric matrix.
    /// </summary>
    /// <typeparam name="T">Numeric element type. Must implement <see cref="INumber{T}"/> to support generic math operations.</typeparam>
    /// <remarks>
    /// Implementations expose the underlying jagged array via <see cref="Data"/> and provide common matrix
    /// operations such as elementwise arithmetic, matrix multiplication, scalar operations, and
    /// structural transforms (transpose, rotate, invert). Methods that combine matrices require matching
    /// dimensions and should throw an <see cref="ArgumentException"/> when dimensions do not match.
    /// </remarks>
    public interface IMatrix2D<T>:IMatrix<T> where T : INumber<T>
    {
        /// <summary>
        /// Gets the element at the specified row and column.
        /// </summary>
        /// <param name="indexX">Zero-based row index.</param>
        /// <param name="indexY">Zero-based column index.</param>
        /// <returns>The element at [indexX, indexY].</returns>
        T this[int indexX, int indexY] { get; }


        /// <summary>
        /// Returns the underlying vector data. If this is changed so is the vector.
        /// </summary>
        T[][] Data { get; }

        /// <summary>
        /// Return if the matrix is identity matrix
        /// </summary>
        Boolean Identity { get; }

        /// <summary>
        /// Gets the number of columns in the matrix.
        /// </summary>
        /// <remarks>Use this property to determine the total number of columns available for iteration,
        /// validation, or configuration purposes. The value is guaranteed to be non-negative.</remarks>
        int ColumnCount {  get; }

        /// <summary>
        /// Gets the number of rows contained in the matrix.
        /// </summary>
        /// <remarks>This property is useful for iterating over the matrix or validating its dimensions.
        /// It can be used in scenarios such as data processing, matrix operations, or when implementing algorithms that
        /// require knowledge of the matrix size.</remarks>
        int RowCount { get; }

        /// <summary>
        /// Get the number of dimensions
        /// </summary>
        public int Rank { get; }

        /// <summary>
        /// Set value at the index
        /// </summary>
        /// <param name="indexX">Row Index to set the value</param>
        /// <param name="indexY">Column Index to set the value</param>
        /// <param name="value">Value to be set</param>
        public void SetValue(int indexX, int indexY, T value);

        /// <summary>
        /// Returns a new matrix with the elements in reverse order along both rows and columns.
        /// </summary>
        /// <remarks>Use this method to obtain a matrix that is a mirror image of the original, flipped both horizontally
        /// and vertically. This can be useful for image processing, data analysis, or algorithms that require reversed data
        /// orientation.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> representing the reversed matrix. The original matrix is not modified.</returns>
        public IMatrix2D<T> Reverse();

        /// <summary>
        /// Returns a new matrix that is the element-wise sum of this matrix and the specified matrix.
        /// </summary>
        /// <remarks>The original matrices are not modified. The returned matrix contains the sum of
        /// corresponding elements from this matrix and the specified matrix.</remarks>
        /// <param name="value">The matrix to add to this matrix. Must have the same dimensions as this matrix.</param>
        /// <returns>An <see cref="IMatrix2D{T}"/> representing the element-wise sum of the two matrices.</returns>
        public IMatrix2D<T> Add(IMatrix2D<T> value);

        /// <summary>
        /// Returns a new matrix that is the result of subtracting the specified matrix from the current matrix.
        /// </summary>
        /// <param name="value">The matrix to subtract from the current matrix. Must have the same dimensions as the current matrix.</param>
        /// <returns>A new matrix representing the element-wise difference between the current matrix and the specified matrix.</returns>
        public IMatrix2D<T> Subtract(IMatrix2D<T> value);

        /// <summary>
        /// Returns a new matrix that is the result of multiplying each element of the current matrix by the specified
        /// scalar value.
        /// </summary>
        /// <param name="value">The scalar value to multiply each element of the matrix by.</param>
        /// <returns>A new matrix containing the products of each element and the specified value.</returns>
        public IMatrix2D<T> Multiply(T value);


        /// <summary>
        /// Returns the matrix product of the current matrix and the specified matrix.
        /// </summary>
        /// <remarks>The number of columns in the current matrix must equal the number of rows in
        /// <paramref name="value"/>. The resulting matrix will have the same number of rows as the current matrix and
        /// the same number of columns as <paramref name="value"/>.</remarks>
        /// <param name="value">The matrix to multiply with the current matrix. Must have dimensions compatible for matrix multiplication.</param>
        /// <returns>An <see cref="IMatrix2D{T}"/> representing the result of the matrix multiplication.</returns>
        public IMatrix2D<T> Multiply(IMatrix2D<T> value);

        /// <summary>
        /// Divides the elements of the current matrix by the corresponding elements of the specified matrix.
        /// </summary>
        /// <remarks>Element-wise division is performed; each element in the resulting matrix is the
        /// result of dividing the corresponding element in the current matrix by the element in the specified matrix.
        /// Division by zero in any element may result in an exception or special value, depending on the implementation
        /// of the matrix type T.</remarks>
        /// <param name="value">The matrix whose elements are used as divisors for the corresponding elements of the current matrix. Must
        /// have the same dimensions as the current matrix.</param>
        /// <returns>A new matrix containing the result of the element-wise division.</returns>
        public IMatrix2D<T> Devide(IMatrix2D<T> value);

        /// <summary>
        /// Returns a new matrix with each element negated.
        /// </summary>
        /// <remarks>The original matrix is not modified. The returned matrix contains the result of
        /// multiplying each element by -1.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> whose elements are the additive inverses of the corresponding elements in the
        /// current matrix.</returns>
        public IMatrix2D<T> Negate();

        /// <summary>
        /// Returns a new matrix that is the transpose of the current matrix.
        /// </summary>
        /// <remarks>Transposing a matrix exchanges its rows and columns. The original matrix is not
        /// modified.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> representing the transposed matrix. The dimensions of the returned matrix are
        /// swapped compared to the original.</returns>
        public IMatrix2D<T> Transpose();

        /// <summary>
        /// Returns a new matrix whose elements are the remainders of dividing the corresponding elements of this matrix
        /// by the specified divisor.
        /// </summary>
        /// <param name="divisor">The value by which each element of the matrix is divided to compute the remainder. Cannot be zero.</param>
        /// <returns>A new matrix containing the remainder of each element divided by the specified divisor.</returns>
        public IMatrix2D<T> Remainder(T divisor);

        /// <summary>
        /// Computes the element-wise remainder of the current matrix divided by the specified matrix.
        /// </summary>
        /// <remarks>Both matrices must have matching dimensions. The operation is performed element-wise,
        /// and the result at each position is the remainder of dividing the corresponding elements of the current
        /// matrix by those of the divisor matrix.</remarks>
        /// <param name="divisor">The matrix by which to divide each element of the current matrix to obtain the remainder. Must have the same
        /// dimensions as the current matrix.</param>
        /// <returns>A new matrix containing the element-wise remainders of the division operation.</returns>
        public IMatrix2D<T> Remeinder(IMatrix2D<T> divisor);

        /// <summary>
        /// Calculates the matrix-vector dot product using the specified vector.
        /// </summary>
        /// <param name="b">The vector to multiply with the matrix. The length of the vector must match the number of columns in the
        /// matrix.</param>
        /// <returns>An <see cref="IMatrix2D{T}"/> representing the result of the matrix-vector multiplication.</returns>
        public IMatrix2D<T> DotProduct(double[] b);

        /// <summary>
        /// Calculates the matrix product of the current matrix and the specified matrix.
        /// </summary>
        /// <remarks>The resulting matrix will have the number of rows of the current matrix and the
        /// number of columns of the specified matrix. This operation does not modify the original matrices.</remarks>
        /// <param name="b">The matrix to multiply with the current matrix. Must have a number of rows equal to the number of columns in
        /// the current matrix.</param>
        /// <returns>A new matrix representing the result of the matrix multiplication.</returns>
        public IMatrix2D<T> DotProduct(IMatrix2D<T> b);

        /// <summary>
        /// Calculates the L1 norm (sum of absolute values) of the current vector or value.
        /// </summary>
        /// <returns>The L1 norm of the current instance, represented as a value of type <typeparamref name="T"/>.  The sum of the absolute values of the elements.</returns>
        public T L1Norm();

        /// <summary>
        /// Calculates the Frobenius norm (magnitude) of the current matrix.
        /// </summary>
        /// <returns>The Frobenius norm of the current instance, represented as a value of type <typeparamref name="T"/>.  The square root of the sum of the squared values.</returns>
        public T FrobeniusNorm();

        /// <summary>
        /// Calculates the infinity norm (maximum absolute row sum) of the current matrix.
        /// </summary>
        /// <returns>The infinity norm of the current instance, represented as a value of type <typeparamref name="T"/>.  The maximum absolute row sum of the matrix.</returns>
        public T InfinityNorm();

        /// <summary>
        /// Returns a new matrix in which each element is the remainder after division by the specified divisor.
        /// </summary>
        /// <remarks>The original matrix is not modified. The operation is applied element-wise. If the
        /// divisor is zero, an exception may be thrown depending on the implementation of the underlying
        /// type.</remarks>
        /// <param name="divisor">The value by which each element of the matrix is divided to obtain the remainder. Must not be zero.</param>
        /// <returns>A new matrix containing the result of the modulus operation applied to each element.</returns>
        public IMatrix2D<T> Modulus(T divisor);

        /// <summary>
        /// Returns a new matrix with each element raised to the specified exponent.
        /// </summary>
        /// <param name="exponent">The exponent to which each element of the matrix is raised.</param>
        /// <returns>A new matrix containing the result of raising each element to the specified exponent.</returns>
        public IMatrix2D<T> Power(T exponent);

        /// <summary>
        /// Returns a new matrix that is the result of rotating the current matrix by 90 degrees clockwise.
        /// </summary>
        /// <remarks>The original matrix is not modified. The returned matrix contains the same elements
        /// as the original, but with their positions rotated 90 degrees clockwise. The dimensions of the resulting
        /// matrix may differ if the original matrix is not square.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> representing the rotated matrix.</returns>
        public IMatrix2D<T> Rotate();

        /// <summary>
        /// Returns a new matrix that is the result of rotating the current matrix by 90 degrees clockwise.
        /// </summary>
        /// <param name="radians">The angle in radians to rotate the matrix.</param>
        /// <param name="clockwise">Indicates whether the rotation should be clockwise. Default is true.</param>
        /// <param name="precision">The precision to use for the rotation calculations. Default is 1e-10.</param>   
        /// <remarks>The original matrix is not modified. The returned matrix represents a rotation in the
        /// positive direction. The dimensions of the resulting
        /// matrix may differ if the original matrix is not square.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> representing the rotated matrix.</returns>
        public IMatrix2D<T> Rotate(double radians, bool clockwise = true, double precision = 1e-10);

        /// <summary>
        /// Returns a new matrix whose elements are the natural logarithms of the corresponding elements in the current
        /// matrix.
        /// </summary>
        /// <remarks>Elements with non-positive values may result in undefined or special values (such as
        /// NaN or negative infinity), depending on the underlying numeric type. The original matrix is not
        /// modified.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> containing the natural logarithms of the elements of this matrix. Each element
        /// is computed as ln(x), where x is the value of the corresponding element in the original matrix.</returns>
        public IMatrix2D<T> Log();

        /// <summary>
        /// Returns a matrix whose elements are the absolute values of the corresponding elements in the current matrix.
        /// </summary>
        /// <returns>An <see cref="IMatrix2D{T}"/> containing the absolute values of the elements in the current matrix.</returns>
        public IMatrix2D<T> Abs();

        /// <summary>
        /// Returns a new matrix with each element rounded up to the nearest integer value.
        /// </summary>
        /// <remarks>This method does not modify the original matrix. The operation is typically used to
        /// eliminate fractional components by rounding values upward.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> whose elements are the smallest integers greater than or equal to the
        /// corresponding elements of the original matrix.</returns>
        public IMatrix2D<T> Ceiling();

        /// <summary>
        /// Returns a new matrix with each element rounded down to the nearest integer value.
        /// </summary>
        /// <remarks>This method does not modify the original matrix. The operation is typically used to
        /// eliminate fractional components by rounding values upward.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> whose elements are the smallest integers greater than or equal to the
        /// corresponding elements of the original matrix.</returns>
        public IMatrix2D<T> Floor();

        /// <summary>
        /// Returns a new matrix with each element rounded to the nearest integer value.
        /// </summary>
        /// <remarks>This method does not modify the original matrix. The operation is typically used to
        /// eliminate fractional components by rounding values upward.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> whose elements are the smallest integers greater than or equal to the
        /// corresponding elements of the original matrix.</returns>
        public IMatrix2D<T> Round();

        /// <summary>
        /// Returns a new matrix whose elements are the sine of the corresponding elements in the current matrix.
        /// </summary>
        /// <remarks>Each element in the resulting matrix is computed as Math.Sin(element) for the
        /// corresponding element in the original matrix. The original matrix is not modified.</remarks>
        /// <returns>A matrix containing the sine of each element of the current matrix.</returns>
        public IMatrix2D<T> Sin();

        /// <summary>
        /// Returns a new matrix whose elements are the cosine of the corresponding elements in the current matrix.
        /// </summary>
        /// <remarks>The cosine is computed element-wise. The type parameter <typeparamref name="T"/> must
        /// support cosine operations; otherwise, an exception may be thrown at runtime.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> containing the cosine of each element of the current matrix.</returns>
        public IMatrix2D<T> Cos();

        /// <summary>
        /// Returns a new matrix whose elements are the tangent of the corresponding elements in the current matrix.
        /// </summary>
        /// <remarks>The tangent is computed element-wise. The resulting matrix has the same dimensions as
        /// the original. If any element is outside the valid domain for the tangent function, the corresponding result
        /// may be NaN or infinity, depending on the underlying numeric type.</remarks>
        /// <returns>A matrix containing the tangent of each element in the current matrix.</returns>
        public IMatrix2D<T> Tan();


        /// <summary>
        /// Returns a new matrix whose elements are the hyperbolic sine of the corresponding elements in the current matrix.
        /// </summary>
        /// <remarks>Each element in the resulting matrix is computed as Math.Sinh(element) for the
        /// corresponding element in the original matrix. The original matrix is not modified.</remarks>
        /// <returns>A matrix containing the sine of each element of the current matrix.</returns>
        public IMatrix2D<T> Sinh();

        /// <summary>
        /// Returns a new matrix whose elements are the hyperbolic cosine of the corresponding elements in the current matrix.
        /// </summary>
        /// <remarks>The cosine is computed element-wise. The type parameter <typeparamref name="T"/> must
        /// support cosine operations; otherwise, an exception may be thrown at runtime.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> containing the cosine of each element of the current matrix.</returns>
        public IMatrix2D<T> Cosh();

        /// <summary>
        /// Returns a new matrix whose elements are the hyperbolic tangent of the corresponding elements in the current matrix.
        /// </summary>
        /// <remarks>The tangent is computed element-wise. The resulting matrix has the same dimensions as
        /// the original. If any element is outside the valid domain for the tangent function, the corresponding result
        /// may be NaN or infinity, depending on the underlying numeric type.</remarks>
        /// <returns>A matrix containing the tangent of each element in the current matrix.</returns>
        public IMatrix2D<T> Tanh();

        /// <summary>
        /// Returns a new matrix whose elements are the arc sine (inverse sine) of the corresponding elements in the
        /// current matrix.
        /// </summary>
        /// <remarks>Each element in the resulting matrix is computed as Math.Asin(x), where x is the
        /// corresponding element in the original matrix. The input values should be in the range [-1, 1]; values
        /// outside this range will result in NaN elements in the output matrix.</remarks>
        /// <returns>A matrix containing the arc sine of each element of the current matrix, with the same dimensions and element
        /// type.</returns>
        public IMatrix2D<T> Asin();

        /// <summary>
        /// Returns a new matrix whose elements are the arc-cosine of the corresponding elements in the current matrix.
        /// </summary>
        /// <remarks>Each element in the resulting matrix is computed as Math.Acos(x), where x is the
        /// corresponding element in the original matrix. The input values must be in the range [-1, 1]; otherwise, the
        /// result for those elements will be NaN.</remarks>
        /// <returns>A matrix containing the arc-cosine of each element of the current matrix, with the same dimensions and
        /// element type.</returns>
        public IMatrix2D<T> Acos();

        /// <summary>
        /// Returns a new matrix whose elements are the arctangent (inverse tangent) of the corresponding elements in
        /// the current matrix.
        /// </summary>
        /// <remarks>The arctangent is computed element-wise. The result is expressed in radians. The
        /// behavior for elements outside the valid domain of the arctangent function depends on the implementation of
        /// <typeparamref name="T"/>.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> containing the arctangent of each element in the current matrix. The returned
        /// matrix has the same dimensions as the original.</returns>
        public IMatrix2D<T> Atan();

        /// <summary>
        /// Returns a new matrix whose elements are the base 10 logarithm of the corresponding elements in
        /// the current matrix.
        /// </summary>
        /// <remarks>The arctangent is computed element-wise. The result is expressed in radians. The
        /// behavior for elements outside the valid domain of the arctangent function depends on the implementation of
        /// <typeparamref name="T"/>.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> containing the arctangent of each element in the current matrix. The returned
        /// matrix has the same dimensions as the original.</returns>
        public IMatrix2D<T> Log10();

        /// <summary>
        /// Returns a new matrix with the elements in inverted order along both rows and columns.
        /// </summary>
        /// <remarks>Use this method to obtain a matrix that is a mirror image of the original, flipped both horizontally
        /// and vertically. This can be useful for image processing, data analysis, or algorithms that require reversed data
        /// orientation.</remarks>
        /// <returns>An <see cref="IMatrix2D{T}"/> representing the inverted matrix. The original matrix is not modified.</returns>
        public IMatrix2D<T> Invert();

        /// <summary>
        /// Compute a matrix determinant
        /// </summary>
        /// <returns>Determinant value of the matrix</returns>
        public T Determinant();

        /// <summary>
        /// Get the condition number of the matrix
        /// </summary>
        /// <returns>condition number</returns>
        public T Condition();

        /// <summary>
        /// Return the trace, which is the sum of the diagonal elements of the matrix
        /// </summary>
        /// <returns>The trace value</returns>
        public T Trace();

        /// <summary>
        /// Return a new matrix with the sign of each element
        /// </summary>
        /// <returns>An <see cref="IMatrix2D{T}"/> representing the sign of each element of the original matrix. The original matrix is not modified.</returns>
        public IMatrix2D<T> Sign();

        /// <summary>
        /// Return a new matrix with the square root of each element
        /// </summary>
        /// <returns>An <see cref="IMatrix2D{T}"/> representing the square root of each element of the original matrix. The original matrix is not modified.</returns>
        public IMatrix2D<T> Sqrt();

        /// <summary>
        /// Return a new matrix whose elements are the angle (in radians) between the corresponding elements of this matrix and another matrix.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>An <see cref="IMatrix2D{T}"/> with calculated angled with the given matrix. The original matrix is not modified.</returns>
        public IMatrix2D<T> Atan2(IMatrix2D<T> other);

        /// <summary>
        /// Returns a new matrix containing the minimum value from each corresponding position in the current matrix.
        /// </summary>
        /// <returns>>An <see cref="IMatrix2D{T}"/> with the minimum value. The original matrix is not modified.</returns>
        public IMatrix2D<T> Minimum();

        /// <summary>
        /// Returns a new matrix containing the maximum value from each corresponding position in the current matrix.
        /// </summary>
        /// <returns>An <see cref="IMatrix2D{T}"/> with the maximum value. The original matrix is not modified.</returns>
        public IMatrix2D<T> Maximum();

        /// <summary>
        /// Returns a new matrix containing the minimum absolute value from each corresponding position in the current matrix.
        /// </summary>
        /// <returns>An <see cref="IMatrix2D{T}"/> with the minimum absolute value. The original matrix is not modified.</returns>
        public IMatrix2D<T> AbsoluteMinimum();

        /// <summary>
        /// Returns a new matrix containing the maximum absolute value from each corresponding position in the current matrix.
        /// </summary>
        /// <returns>An <see cref="IMatrix2D{T}"/> with the maximum absolute value. The original matrix is not modified.</returns>
        public IMatrix2D<T> AbsoluteMaximum();

        /// <summary>
        /// Returns this matrix as a 3D matrix by adding a third dimension with size 1.
        /// </summary>
        /// <returns>An <see cref="IMatrix3D{T}"/> with a third dimension with size 1. The original matrix is not modified.</returns>
        public IMatrix3D<T> AsMatrix3D();

    }
}
