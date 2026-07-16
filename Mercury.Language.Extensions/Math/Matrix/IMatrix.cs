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
    /// Interface representing a matrix that can have an arbitrary number of dimensions and that contains an arbitrary type of data. 
    /// </summary>
    /// <typeparam name="T">Type of elements</typeparam>
    public interface IMatrix<T> : IMatrix where T : INumber<T>
    {
        /// <summary>
        /// Get the number of elements in this matrix
        /// </summary>
        /// <returns>The number of elements</returns>
        public int NumberOfElements { get; }

        /// <summary>
        /// Gets the entry specified by the indices. For example, for a 3-D matrix, the indices matrix must have three elements.
        /// </summary>
        /// <param name="indices">The indices, not null. The number of indices must match the dimension of the matrix</param>
        /// <returns>The entry</returns>
        public T GetEntry(params int[] indices);
    }

    /// <summary>
    /// General interface that representing a matrix
    /// </summary>
    public interface IMatrix
    {

    }
}
