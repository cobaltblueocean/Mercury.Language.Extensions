﻿// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by Math.NET
//
// <copyright file="GramSchmidt.cs" company="Math.NET">
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
using MathNet.Numerics.LinearAlgebra.Factorization;
using Mercury.Language;

namespace MathNet.Numerics.LinearAlgebra.Decimal.Factorization
{
    /// <summary>
    /// <para>A class which encapsulates the functionality of the QR decomposition Modified Gram-Schmidt Orthogonalization.</para>
    /// <para>Any real square matrix A may be decomposed as A = QR where Q is an orthogonal mxn matrix and R is an nxn upper triangular matrix.</para>
    /// </summary>
    /// <remarks>
    /// The computation of the QR decomposition is done at construction time by modified Gram-Schmidt Orthogonalization.
    /// </remarks>
    public abstract class GramSchmidt : GramSchmidt<decimal>
    {
        protected GramSchmidt(Matrix<decimal> q, Matrix<decimal> rFull)
            : base(q, rFull)
    {
    }

    /// <summary>
    /// Gets the absolute determinant value of the matrix for which the QR matrix was computed.
    /// </summary>
    public override decimal Determinant
    {
        get
        {
            if (FullR.RowCount != FullR.ColumnCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_MUST_BE_SQUARE);
            }

            var det = 1.0M;
            for (var i = 0; i < FullR.ColumnCount; i++)
            {
                det *= FullR.At(i, i);
                if (QuickMath.Abs(FullR.At(i, i)).AlmostEqual(0.0M))
                {
                    return 0;
                }
            }

                return QuickMath.Abs(det); // Convert.ToSingle(QuickMath.Abs(det));
        }
    }

    /// <summary>
    /// Gets a value indicating whether the matrix is full rank or not.
    /// </summary>
    /// <value><c>true</c> if the matrix is full rank; otherwise <c>false</c>.</value>
    public override bool IsFullRank
    {
        get
        {
            for (var i = 0; i < FullR.ColumnCount; i++)
            {
                if (QuickMath.Abs(FullR.At(i, i)).AlmostEqual(0.0M))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
}
