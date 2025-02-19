﻿// <copyright file="DenseEvd.cs" company="QuickMath.NET">
// QuickMath.NET Numerics, part of the QuickMath.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
//
// Copyright (c) 2009-2020 QuickMath.NET
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
using System.Linq;
using MathNet.Numerics.Providers.LinearAlgebra;
using Mercury.Language;

namespace MathNet.Numerics.LinearAlgebra.DecimalComplex.Factorization
{
    using DecimalComplex = System.Numerics.DecimalComplex;

    /// <summary>
    /// Eigenvalues and eigenvectors of a complex matrix.
    /// </summary>
    /// <remarks>
    /// If A is Hermitian, then A = V*D*V' where the eigenvalue matrix D is
    /// diagonal and the eigenvector matrix V is Hermitian.
    /// I.e. A = V*D*V' and V*VH=I.
    /// If A is not symmetric, then the eigenvalue matrix D is block diagonal
    /// with the real eigenvalues in 1-by-1 blocks and any complex eigenvalues,
    /// lambda + i*mu, in 2-by-2 blocks, [lambda, mu; -mu, lambda].  The
    /// columns of V represent the eigenvectors in the sense that A*V = V*D,
    /// i.e. A.Multiply(V) equals V.Multiply(D).  The matrix V may be badly
    /// conditioned, or even singular, so the validity of the equation
    /// A = V*D*Inverse(V) depends upon V.Condition().
    /// </remarks>
    internal sealed class DenseEvd : Evd
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DenseEvd"/> class. This object will compute the
        /// the eigenvalue decomposition when the constructor is called and cache it's decomposition.
        /// </summary>
        /// <param name="matrix">The matrix to factor.</param>
        /// <param name="symmetricity">If it is known whether the matrix is symmetric or not the routine can skip checking it itself.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="matrix"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If EVD algorithm failed to converge with matrix <paramref name="matrix"/>.</exception>
        public static DenseEvd Create(DenseMatrix matrix, Symmetricity symmetricity)
        {
            if (matrix.RowCount != matrix.ColumnCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_MUST_BE_SQUARE);
            }

            var order = matrix.RowCount;

            // Initialize matrices for eigenvalues and eigenvectors
            var eigenVectors = DenseMatrix.CreateIdentity(order);
            var blockDiagonal = new DenseMatrix(order);
            var eigenValues = new DenseVector(order);

            bool isSymmetric;
            switch (symmetricity)
            {
                case Symmetricity.Hermitian:
                    isSymmetric = true;
                    break;
                case Symmetricity.Asymmetric:
                    isSymmetric = false;
                    break;
                default:
                    isSymmetric = matrix.IsHermitian();
                    break;
            }

            LinearAlgebraControl2.Provider.EigenDecomp(isSymmetric, order, matrix.Values, eigenVectors.Values, eigenValues.Values.Select(x => x.ToComplex()).ToArray(), blockDiagonal.Values);

            return new DenseEvd(eigenVectors, eigenValues, blockDiagonal, isSymmetric);
        }

        DenseEvd(Matrix<DecimalComplex> eigenVectors, Vector<DecimalComplex> eigenValues, Matrix<DecimalComplex> blockDiagonal, bool isSymmetric)
            : base(eigenVectors, eigenValues.ToComplexVector(), blockDiagonal, isSymmetric)
        {
        }

        /// <summary>
        /// Solves a system of linear equations, <b>AX = B</b>, with A SVD factorized.
        /// </summary>
        /// <param name="input">The right hand side <see cref="Matrix{T}"/>, <b>B</b>.</param>
        /// <param name="result">The left hand side <see cref="Matrix{T}"/>, <b>X</b>.</param>
        public override void Solve(Matrix<DecimalComplex> input, Matrix<DecimalComplex> result)
        {
            // The solution X should have the same number of columns as B
            if (input.ColumnCount != result.ColumnCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_COLUMN_DIMENSIONS_MUST_AGREE);
            }

            // The dimension compatibility conditions for X = A\B require the two matrices A and B to have the same number of rows
            if (EigenValues.Count != input.RowCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_ROW_DIMENSIONS_MUST_AGREE);
            }

            // The solution X row dimension is equal to the column dimension of A
            if (EigenValues.Count != result.RowCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_COLUMN_DIMENSIONS_MUST_AGREE);
            }

            if (IsSymmetric)
            {
                var order = EigenValues.Count;
                var tmp = new DecimalComplex[order];

                for (var k = 0; k < order; k++)
                {
                    for (var j = 0; j < order; j++)
                    {
                        DecimalComplex value = 0.0M;
                        if (j < order)
                        {
                            for (var i = 0; i < order; i++)
                            {
                                value += ((DenseMatrix) EigenVectors).Values[(j*order) + i].Conjugate()*input.At(i, k);
                            }

                            value /= (decimal)EigenValues[j].Real;
                        }

                        tmp[j] = value;
                    }

                    for (var j = 0; j < order; j++)
                    {
                        DecimalComplex value = 0.0M;
                        for (var i = 0; i < order; i++)
                        {
                            value += ((DenseMatrix) EigenVectors).Values[(i*order) + j]*tmp[i];
                        }

                        result.At(j, k, value);
                    }
                }
            }
            else
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_MUST_BE_SYMMETRIC);
            }
        }

        /// <summary>
        /// Solves a system of linear equations, <b>Ax = b</b>, with A EVD factorized.
        /// </summary>
        /// <param name="input">The right hand side vector, <b>b</b>.</param>
        /// <param name="result">The left hand side <see cref="Matrix{T}"/>, <b>x</b>.</param>
        public override void Solve(Vector<DecimalComplex> input, Vector<DecimalComplex> result)
        {
            // Ax=b where A is an m x m matrix
            // Check that b is a column vector with m entries
            if (EigenValues.Count != input.Count)
            {
                throw new ArgumentException(LocalizedResources.Instance().ALL_VECTORS_MUST_HAVE_THE_SAME_DIMENSIONALITY);
            }

            // Check that x is a column vector with n entries
            if (EigenValues.Count != result.Count)
            {
                throw MatrixExceptionFactory.DimensionsDontMatch<ArgumentException>(EigenValues.Count, result.Count);
            }

            if (IsSymmetric)
            {
                // Symmetric case -> x = V * inv(λ) * VH * b;
                var order = EigenValues.Count;
                var tmp = new DecimalComplex[order];
                DecimalComplex value;

                for (var j = 0; j < order; j++)
                {
                    value = 0;
                    if (j < order)
                    {
                        for (var i = 0; i < order; i++)
                        {
                            value += ((DenseMatrix) EigenVectors).Values[(j*order) + i].Conjugate()*input[i];
                        }

                        value /= (decimal)EigenValues[j].Real;
                    }

                    tmp[j] = value;
                }

                for (var j = 0; j < order; j++)
                {
                    value = 0;
                    for (var i = 0; i < order; i++)
                    {
                        value += ((DenseMatrix) EigenVectors).Values[(i*order) + j]*tmp[i];
                    }

                    result[j] = value;
                }
            }
            else
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_MUST_BE_SYMMETRIC);
            }
        }
    }
}
