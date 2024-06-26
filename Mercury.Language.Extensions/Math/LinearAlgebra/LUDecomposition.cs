﻿// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by Apache Software Foundation (ASF).
//
// Licensed to the Apache Software Foundation (ASF) under one or more
// contributor license agreementsd  See the NOTICE file distributed with
// this work for additional information regarding copyright ownership.
// The ASF licenses this file to You under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with
// the Licensed  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;
using Mercury.Language.Exceptions;
using Mercury.Language;
using Mercury.Language.Math.Matrix;
using Mercury.Language.Math.Analysis;
using Mercury.Language.Math.Analysis.Solver;
using Mercury.Language.Log;

namespace Mercury.Language.Math.LinearAlgebra
{
    /// <summary>
    ///   LU decomposition of a multidimensional rectangular matrix.
    /// </summary>
    ///
    /// <remarks>
    ///   <para>
    ///     For an m-by-n matrix <c>A</c> with <c>m >= n</c>, the LU decomposition is an m-by-n
    ///     unit lower triangular matrix <c>L</c>, an n-by-n upper triangular matrix <c>U</c>,
    ///     and a permutation vector <c>piv</c> of Length m so that <c>A(piv) = L*U</c>.
    ///     If m &lt; n, then <c>L</c> is m-by-m and <c>U</c> is m-by-n.</para>
    ///   <para>
    ///     The LU decomposition with pivoting always exists, even if the matrix is
    ///     singular, so the constructor will never faild  The primary use of the
    ///     LU decomposition is in the solution of square systems of simultaneous
    ///     linear equationsd This will fail if <see cref="Nonsingular"/> returns
    ///     <see langword="false"/>.</para>
    ///   <para>
    ///     If you need to compute a LU decomposition for matrices with data types other than
    ///     double, see <see cref="LuDecompositionF"/>, <see cref="LuDecompositionD"/>d If you
    ///     need to compute a LU decomposition for a jagged matrix, see <see cref="JaggedLuDecomposition"/>,
    ///     <see cref="JaggedLuDecompositionF"/>, and <see cref="JaggedLuDecompositionD"/>.</para>
    /// </remarks>
    ///
    /// <example>
    ///   <code source="Unit Tests\Accord.Tests.Math\Decompositions\LuDecompositionTest.cs" region="doc_ctor" />
    /// </example>
    ///
    /// <seealso cref="CholeskyDecomposition"/>
    /// <seealso cref="EigenvalueDecomposition"/>
    /// <seealso cref="SingularValueDecomposition"/>
    /// <seealso cref="JaggedEigenvalueDecomposition"/>
    /// <seealso cref="JaggedSingularValueDecomposition"/>
    /// 
    public sealed class LUDecomposition : ILUDecomposition<Double>
    {
        private int rows;
        private int cols;
        private Double[,] lu;
        private int pivotSign;
        private int[] pivotVector;


        // cache for lazy evaluation
        private Double? determinant;
        private double? lndeterminant;
        private bool singular;
        private Double[,] lowerTriangularFactor;
        private Double[,] upperTriangularFactor;

        // Default bound to determine effective singularity in LU decomposition 
        private static double DEFAULT_TOO_SMALL = 10E-12;

        // Parity of the permutation associated with the LU decomposition 
        private Boolean even;

        // Cached value of L. 
        private Matrix<Double> cachedL;

        // Cached value of U. 
        private Matrix<Double> cachedU;

        // Cached value of P. 
        private Matrix<Double> cachedP;

        /// <summary>
        ///   Constructs a new LU decomposition.
        /// </summary>    
        /// <param name="value">The matrix A to be decomposed.</param>
        /// 
        public LUDecomposition(Double[,] value)
            : this(value, DEFAULT_TOO_SMALL)
        {
        }

        /// <summary>
        ///   Constructs a new LU decomposition.
        /// </summary>    
        /// <param name="value">The matrix A to be decomposed.</param>
        /// <param name="singularityThreshold">THreshold value to check the singularity
        /// 
        public LUDecomposition(Double[,] value, double singularityThreshold)
            : this(value, singularityThreshold, false)
        {
        }

        /// <summary>
        ///   Constructs a new LU decomposition.
        /// </summary>    
        /// <param name="value">The matrix A to be decomposed.</param>
        /// <param name="transpose">True if the decomposition should be performed on
        /// the transpose of A rather than A itself, false otherwise. Default is false.</param>
        /// 
        public LUDecomposition(Double[,] value, bool transpose)
            : this(value, DEFAULT_TOO_SMALL, transpose, false)
        {
        }

        /// <summary>
        ///   Constructs a new LU decomposition.
        /// </summary>    
        /// <param name="value">The matrix A to be decomposed.</param>
        /// <param name="singularityThreshold">THreshold value to check the singularity
        /// <param name="transpose">True if the decomposition should be performed on
        /// the transpose of A rather than A itself, false otherwise. Default is false.</param>
        /// 
        public LUDecomposition(Double[,] value, double singularityThreshold, bool transpose)
            : this(value, singularityThreshold, transpose, false)
        {
        }

        /// <summary>
        ///   Constructs a new LU decomposition.
        /// </summary>    
        /// <param name="value">The matrix A to be decomposed.</param>
        /// <param name="singularityThreshold">THreshold value to check the singularity
        /// <param name="transpose">True if the decomposition should be performed on
        /// the transpose of A rather than A itself, false otherwise. Default is false.</param>
        /// <param name="inPlace">True if the decomposition should be performed over the
        /// <paramref name="value"/> matrix rather than on a copy of it. If true, the
        /// matrix will be destroyed during the decomposition. Default is false.</param>
        /// 
        public LUDecomposition(Double[,] value, double singularityThreshold, bool transpose, bool inPlace)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value", String.Format(LocalizedResources.Instance().MATRIX_CANNOT_BE_NULL, "value"));
            }

            if (transpose)
            {
                this.lu = value.Transpose(inPlace);
            }
            else
            {
                this.lu = inPlace ? value : (Double[,])value.Clone();
            }

            this.rows = lu.GetLength(0);
            this.cols = lu.GetLength(1);
            this.pivotSign = 1;

            int m = this.lu.GetMaxColumnLength();
            this.pivotVector = new int[m];
            cachedL = null;
            cachedU = null;
            cachedP = null;

            // Initialize permutation array and parity
            for (int row = 0; row < m; row++)
            {
                this.pivotVector[row] = row;
            }
            even = true;
            singular = false;

            // Loop over columns
            for (int col = 0; col < m; col++)
            {
                double sum = 0;

                // upper
                for (int row = 0; row < col; row++)
                {
                    double[] luRow = lu.GetRow(row);
                    sum = luRow[col];
                    for (int i = 0; i < row; i++)
                    {
                        sum -= luRow[i] * lu[i, col];
                    }
                    //luRow[col] = sum;
                    lu[row, col] = sum;
                }

                // lower
                int max = col; // permutation row
                double largest = Double.NegativeInfinity;
                for (int row = col; row < m; row++)
                {
                    double[] luRow = lu.GetRow(row);
                    sum = luRow[col];
                    for (int i = 0; i < col; i++)
                    {
                        sum -= luRow[i] * lu[i, col];
                    }
                    //luRow[col] = sum;
                    lu[row, col] = sum;

                    // maintain best permutation choice
                    if (QuickMath.Abs(sum) > largest)
                    {
                        largest = QuickMath.Abs(sum);
                        max = row;
                    }
                }

                // Singularity check
                if (QuickMath.Abs(lu[max, col]) < singularityThreshold)
                {
                    singular = true;
                    return;
                }

                // Pivot if necessary
                if (max != col)
                {
                    double tmp = 0;
                    double[] luMax = lu.GetRow(max);
                    double[] luCol = lu.GetRow(col);
                    for (int i = 0; i < m; i++)
                    {
                        tmp = luMax[i];
                        //lu.SetRow(max, luCol);  //luMax[i] = luCol[i];
                        lu[max, i] = luCol[i];
                        //luCol[i] = tmp;
                        lu[col, i] = tmp;
                    }

                    //lu.SetRow(max, luCol);
                    //lu.SetRow(col, luMax);

                    int temp = this.pivotVector[max];
                    this.pivotVector[max] = this.pivotVector[col];
                    this.pivotVector[col] = temp;
                    even = !even;
                }

                // Divide the lower elements by the "winning" diagonal elt.
                double luDiag = lu[col, col];
                for (int row = col + 1; row < m; row++)
                {
                    lu[row, col] /= luDiag;
                }
            }
        }

        /// <summary>
        /// Returns the matrix L of the decomposition.
        /// <p>L is an lower-triangular matrix</p>
        /// </summary>
        /// <returns>the L matrix (or null if decomposed matrix is singular)</returns>
        public Matrix<Double> GetL()
        {
            if ((cachedL == null) && !singular)
            {
                int m = pivotVector.Length;
                cachedL = Matrix.MathNetMatrixUtility.CreateMatrix(m, m);
                for (int i = 0; i < m; ++i)
                {
                    double[] luI = lu.GetRow(i);
                    for (int j = 0; j < i; ++j)
                    {
                        cachedL[i, j] = luI[j];
                    }
                    cachedL[i, i] = 1.0;
                }
            }
            return cachedL;
        }

        /// <summary>
        /// Returns the matrix U of the decomposition.
        /// <p>U is an upper-triangular matrix</p>
        /// </summary>
        /// <returns>the U matrix (or null if decomposed matrix is singular)</returns>
        public Matrix<Double> GetU()
        {
            if ((cachedU == null) && !singular)
            {
                int m = pivotVector.Length;
                cachedU = Matrix.MathNetMatrixUtility.CreateMatrix(m, m);
                for (int i = 0; i < m; ++i)
                {
                    double[] luI = lu.GetRow(i);
                    for (int j = i; j < m; ++j)
                    {
                        cachedU[i, j] = luI[j];
                    }
                }
            }
            return cachedU;
        }

        /// <summary>
        /// Returns the P rows permutation matrix.
        /// <p>P is a sparse matrix with exactly one element set to 1.0 in
        /// each row and each column, all other elements being set to 0.0.</p>
        /// <p>The positions of the 1 elements are given by the {@link #getPivot()
        /// pivot permutation vector}.</p>
        /// </summary>
        /// <returns>the P rows permutation matrix (or null if decomposed matrix is singular)</returns>
        /// <see cref="#getPivot()"></see>
        public Matrix<Double> GetP()
        {
            if ((cachedP == null) && !singular)
            {
                int m = pivotVector.Length;
                cachedP = Matrix.MathNetMatrixUtility.CreateMatrix(m, m);
                for (int i = 0; i < m; ++i)
                {
                    cachedP[i, pivotVector[i]] = 1.0;
                }
            }
            return cachedP;
        }

        /// <summary>
        /// Returns data
        /// </summary>
        public double[,] Data
        {
            get { return this.lu; }
        }

        /// <summary>
        ///   Returns if the matrix is non-singular (i.e. invertible).
        ///   Please see remarks for important information regarding
        ///   numerical stability when using this method.
        /// </summary>
        /// 
        /// <remarks>
        ///   Please keep in mind this is not one of the most reliable methods
        ///   for checking singularity of a matrix. For a more reliable method,
        ///   please use <see cref="Mercury.Language.Math.Matrix.MathNetMatrixUtility.IsSingular"/> or the 
        ///   <see cref="SingularValueDecomposition"/>.
        /// </remarks>
        public bool Nonsingular
        {
            get
            {
                //if (!nonsingular.HasValue)
                //{
                //    if (rows != cols)
                //        throw new InvalidOperationException(LocalizedResources.Instance().MATRIX_MUST_BE_SQUARE);

                //    bool nonSingular = true;
                //    for (int i = 0; i < rows && nonSingular; i++)
                //        if (lu[i, i] == 0) nonSingular = false;

                //    nonsingular = nonSingular;
                //}

                //return nonsingular.Value;
                return !singular;
            }
        }

        /// <summary>
        ///   Returns the determinant of the matrix.
        /// </summary>
        /// 
        public Double Determinant
        {
            get
            {
                if (!determinant.HasValue)
                {
                    if (rows != cols)
                        throw new InvalidOperationException(LocalizedResources.Instance().MATRIX_MUST_BE_SQUARE);

                    Double det = pivotSign;
                    for (int i = 0; i < rows; i++)
                        det *= lu[i, i];

                    determinant = det;
                }

                return determinant.Value;
            }
        }

        /// <summary>
        ///   Returns the log-determinant of the matrix.
        /// </summary>
        /// 
        public double LogDeterminant
        {
            get
            {
                if (!lndeterminant.HasValue)
                {
                    if (rows != cols)
                        throw new InvalidOperationException(LocalizedResources.Instance().MATRIX_MUST_BE_SQUARE);

                    double lndet = 0;
                    for (int i = 0; i < rows; i++)
                        lndet += System.Math.Log((double)System.Math.Abs(lu[i, i]));
                    lndeterminant = lndet;
                }

                return lndeterminant.Value;
            }
        }

        /// <summary>
        ///   Returns the lower triangular factor <c>L</c> with <c>A=LU</c>.
        /// </summary>
        /// 
        public Double[,] LowerTriangularFactor
        {
            get
            {
                if (lowerTriangularFactor == null)
                {
                    var L = new Double[rows, rows];

                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < rows; j++)
                        {
                            if (i > j)
                                L[i, j] = lu[i, j];
                            else if (i == j)
                                L[i, j] = 1;
                            else
                                L[i, j] = 0;
                        }
                    }

                    lowerTriangularFactor = L;
                }

                return lowerTriangularFactor;
            }
        }

        /// <summary>
        ///   Returns the lower triangular factor <c>L</c> with <c>A=LU</c>.
        /// </summary>
        /// 
        public Double[,] UpperTriangularFactor
        {
            get
            {
                if (upperTriangularFactor == null)
                {
                    var U = new Double[rows, cols];
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            if (i <= j)
                                U[i, j] = lu[i, j];
                            else
                                U[i, j] = 0;
                        }
                    }

                    upperTriangularFactor = U;
                }

                return upperTriangularFactor;
            }
        }

        /// <summary>
        ///   Returns the pivot permutation vector.
        /// </summary>
        /// 
        public int[] PivotPermutationVector
        {
            get { return this.pivotVector; }
        }

        /// <summary>
        ///   Solves a set of equation systems of type <c>A * X = I</c>.
        /// </summary>
        /// 
        public Double[,] Inverse()
        {
            if (!Nonsingular)
                throw new SingularMatrixException(LocalizedResources.Instance().MATRIX_IS_SINGULAR);


            int count = rows;

            // Copy right hand side with pivoting
            var X = new Double[rows, rows];
            for (int i = 0; i < rows; i++)
            {
                int k = pivotVector[i];
                X[i, k] = 1;
            }

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < rows; k++)
                for (int i = k + 1; i < rows; i++)
                    for (int j = 0; j < count; j++)
                        X[i, j] -= X[k, j] * lu[i, k];

            // Solve U*X = I;
            for (int k = rows - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                    X[k, j] /= lu[k, k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < count; j++)
                        X[i, j] -= X[k, j] * lu[i, k];
            }

            return X;
        }


        public double[,] Solve(double[,] value)
        {
            return GetSolver().Solve(value);
        }

        public double[] Solve(double[] value)
        {
            return GetSolver().Solve(value);
        }



        /// <summary>
        ///   Solves a set of equation systems of type <c>X * A = B</c>.
        /// </summary>
        /// <param name="value">Right hand side matrix with as many columns as <c>A</c> and any number of rows.</param>
        /// <returns>Matrix <c>X</c> so that <c>X * L * U = A</c>.</returns>
        /// 
        public Double[,] SolveTranspose(Double[,] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.GetLength(0) != rows)
                throw new DimensionMismatchException(LocalizedResources.Instance().MATRIX_SHOULD_HAVE_THE_SAME_NUMBER_OF_ROWS);

            if (!Nonsingular)
                throw new SingularMatrixException(LocalizedResources.Instance().MATRIX_IS_SINGULAR);


            // Copy right hand side with pivoting
            var X = value.Get(null, pivotVector);

            int count = X.GetLength(1);

            // Solve L*Y = B(piv,:)
            for (int k = 0; k < rows; k++)
                for (int i = k + 1; i < rows; i++)
                    for (int j = 0; j < count; j++)
                        X[j, i] -= X[j, k] * lu[i, k];

            // Solve U*X = Y;
            for (int k = rows - 1; k >= 0; k--)
            {
                for (int j = 0; j < count; j++)
                    X[j, k] /= lu[k, k];

                for (int i = 0; i < k; i++)
                    for (int j = 0; j < count; j++)
                        X[j, i] -= X[j, k] * lu[i, k];
            }

            return X;
        }

        /// <summary>
        ///   Reverses the decomposition, reconstructing the original matrix <c>X</c>.
        /// </summary>
        /// 
        public Double[,] Reverse()
        {
            return LowerTriangularFactor.Dot(UpperTriangularFactor)
                .Get(PivotPermutationVector.ArgSort(), null);
        }

        /// <summary>
        ///   Computes <c>(Xt * X)^1</c> (the inverse of the covariance matrix). This
        ///   matrix can be used to determine standard errors for the coefficients when
        ///   solving a linear set of equations through any of the <see cref="Solve(Double[,])"/>
        ///   methods.
        /// </summary>
        /// 
        public Double[,] GetInformationMatrix()
        {
            var X = Reverse();
            return X.TransposeAndDot(X).Inverse();
        }

        public IDecompositionSolver GetSolver()
        {
            return new Solver(lu.ToJagged(), pivotVector, !Nonsingular);
        }

        #region ICloneable Members

        private LUDecomposition()
        {
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            LUDecomposition lud = new LUDecomposition();
            lud.rows = this.rows;
            lud.cols = this.cols;
            lud.lu = (Double[,])this.lu.Clone();
            lud.pivotSign = this.pivotSign;
            lud.pivotVector = (int[])this.pivotVector;
            return lud;
        }
        #endregion

        private class Solver : IDecompositionSolver
        {

            /// <summary>Entries of LU decompositiond */
            private double[][] lu;

            /// <summary>Pivot permutation associated with LU decompositiond */
            private int[] pivot;

            /// <summary>Singularity indicatord */
            private Boolean singular;

            /// <summary>
            /// Build a solver from decomposed matrix.
            /// </summary>
            /// <param Name="lu">entries of LU decomposition</param>
            /// <param Name="pivot">pivot permutation associated with LU decomposition</param>
            /// <param Name="singular">singularity indicator</param>
            public Solver(double[][] lu, int[] pivot, Boolean singular)
            {
                this.lu = lu;
                this.pivot = pivot;
                this.singular = singular;
            }

            /// <summary>{@inheritDoc} */
            public Boolean IsNonSingular
            {
                get { return !singular; }
            }

            /// <summary>{@inheritDoc} */
            public double[] Solve(double[] b)
            {

                int m = pivot.Length;
                if (b.Length != m)
                {
                    throw new ArgumentException(String.Format(LocalizedResources.Instance().VECTOR_LENGTH_MISMATCH, b.Length, m));
                }
                if (singular)
                {
                    throw new SingularMatrixException();
                }

                double[] bp = new double[m];

                // Apply permutations to b
                for (int row = 0; row < m; row++)
                {
                    bp[row] = b[pivot[row]];
                }

                // Solve LY = b
                for (int col = 0; col < m; col++)
                {
                    double bpCol = bp[col];
                    for (int i = col + 1; i < m; i++)
                    {
                        bp[i] -= bpCol * lu[i][col];
                    }
                }

                // Solve UX = Y
                for (int col = m - 1; col >= 0; col--)
                {
                    bp[col] /= lu[col][col];
                    double bpCol = bp[col];
                    for (int i = 0; i < col; i++)
                    {
                        bp[i] -= bpCol * lu[i][col];
                    }
                }

                return bp;

            }

            /// <summary>{@inheritDoc} */
            public Vector<Double> Solve(Vector<Double> b)
            {
                try
                {
                    return Solve((DenseVector)b);
                }
                catch (InvalidCastException cce)
                {
                    Logger.Information(cce.Message);

                    int m = pivot.Length;
                    if (b.Count != m)
                    {
                        throw new ArgumentException(String.Format(LocalizedResources.Instance().VECTOR_LENGTH_MISMATCH, b.Count, m));
                    }
                    if (singular)
                    {
                        throw new SingularMatrixException();
                    }

                    double[] bp = new double[m];

                    // Apply permutations to b
                    for (int row = 0; row < m; row++)
                    {
                        bp[row] = b[pivot[row]];
                    }

                    // Solve LY = b
                    for (int col = 0; col < m; col++)
                    {
                        double bpCol = bp[col];
                        for (int i = col + 1; i < m; i++)
                        {
                            bp[i] -= bpCol * lu[i][col];
                        }
                    }

                    // Solve UX = Y
                    for (int col = m - 1; col >= 0; col--)
                    {
                        bp[col] /= lu[col][col];
                        double bpCol = bp[col];
                        for (int i = 0; i < col; i++)
                        {
                            bp[i] -= bpCol * lu[i][col];
                        }
                    }

                    return MathNetMatrixUtility.CreateRealVector(bp);
                }
            }

            /// <summary>Solve the linear equation A &times; X = B.
            /// <p>The A matrix is implicit hered It is </p>
            /// </summary>
            /// <param Name="b">right-hand side of the equation A &times; X = B</param>
            /// <returns>a vector X such that A &times; X = B</returns>
            /// <exception cref="ArgumentException">if matrices dimensions don't match </exception>
            /// <exception cref="InvalidMatrixException">if decomposed matrix is singular </exception>
            public DenseVector Solve(DenseVector b)
            {
                return (DenseVector)MathNetMatrixUtility.CreateRealVector(Solve(b.AsArrayEx()));
            }

            /// <summary>{@inheritDoc} */
            public Matrix<Double> Solve(Matrix<Double> b)
            {
                return MathNetMatrixUtility.CreateMatrix<Double>(Solve(b.AsArrayEx()));
            }

            public double[][] Solve(double[][] value)
            {

                int m = pivot.Length;
                if (value.Length != m)
                {
                    throw new ArgumentException(String.Format(LocalizedResources.Instance().DIMENSIONS_MISMATCH_2x2, value.Length, value.GetMaxColumnLength(), m, "n"));
                }
                if (singular)
                {
                    throw new SingularMatrixException();
                }

                int nColB = value.GetMaxColumnLength();

                // Apply permutations to b
                double[][] bp = ArrayUtility.CreateJaggedArray<double>(m, nColB);
                for (int row = 0; row < m; row++)
                {
                    double[] bpRow = bp[row];
                    int pRow = pivot[row];
                    for (int col = 0; col < nColB; col++)
                    {
                        bpRow[col] = value[pRow][col];
                    }
                }

                // Solve LY = b
                for (int col = 0; col < m; col++)
                {
                    double[] bpCol = bp[col];
                    for (int i = col + 1; i < m; i++)
                    {
                        double[] bpI = bp[i];
                        double luICol = lu[i][col];
                        for (int j = 0; j < nColB; j++)
                        {
                            bpI[j] -= bpCol[j] * luICol;
                        }
                    }
                }

                // Solve UX = Y
                for (int col = m - 1; col >= 0; col--)
                {
                    double[] bpCol = bp[col];
                    double luDiag = lu[col][col];
                    for (int j = 0; j < nColB; j++)
                    {
                        bpCol[j] /= luDiag;
                    }
                    for (int i = 0; i < col; i++)
                    {
                        double[] bpI = bp[i];
                        double luICol = lu[i][col];
                        for (int j = 0; j < nColB; j++)
                        {
                            bpI[j] -= bpCol[j] * luICol;
                        }
                    }
                }

                return bp;
            }

            public double[,] Solve(double[,] value)
            {
                return Solve(value.ToJagged()).ToMultidimensional();
            }

            /// <summary>{@inheritDoc} */
            public Matrix<Double> GetInverse()
            {
                return Solve(MathNetMatrixUtility.CreateRealIdentityMatrix(pivot.Length));
            }
        }
    }
}
