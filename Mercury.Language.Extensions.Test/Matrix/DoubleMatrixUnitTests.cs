// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by OpenGamma Inc.
//
// Copyright (C) 2012 - present by OpenGamma Inc. and the OpenGamma group of companies
//
// Please see distribution for license.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Mercury.Language.Math.LinearAlgebra;
using Mercury.Test.Utility;
using NUnit.Framework.Legacy;
using Mercury.Language.Math.Matrix.DoubleMatrix;

namespace Mercury.Language.Extensions.Test.Matrix
{
    public class DoubleMatrixUnitTests
    {
        private const double Tolerance = 1e-9;

        [Test]
        public void DoubleMatrix1D_ArithmeticAndNorms()
        {
            var a = new DoubleMatrix1D(new double[] { 1.0, -2.0, 3.0 });
            var b = new DoubleMatrix1D(new double[] { 4.0, 5.0, -6.0 });

            var added = a.Add(b);
            Assert2.AreEqual(5.0, added[0], Tolerance);
            Assert2.AreEqual(3.0, added[1], Tolerance);
            Assert2.AreEqual(-3.0, added[2], Tolerance);

            var multipliedScalar = a.Multiply(2.0);
            Assert2.AreEqual(2.0, multipliedScalar[0], Tolerance);
            Assert2.AreEqual(-4.0, multipliedScalar[1], Tolerance);

            var multipliedElem = a.Multiply(b);
            Assert2.AreEqual(4.0, multipliedElem[0], Tolerance);
            Assert2.AreEqual(-10.0, multipliedElem[1], Tolerance);

            // Inner product: sum of element-wise product
            Assert2.AreEqual(1.0 * 4.0 + -2.0 * 5.0 + 3.0 * -6.0, a.InnerProduct(b), Tolerance);

            // Distances
            Assert2.AreEqual(System.Math.Sqrt((1 - 4) * (1 - 4) + (-2 - 5) * (-2 - 5) + (3 - -6) * (3 - -6)), a.Distance(b), Tolerance);
            Assert2.AreEqual(System.Math.Abs(1 - 4) + System.Math.Abs(-2 - 5) + System.Math.Abs(3 - -6), a.L1Distance(b), Tolerance);
            Assert2.AreEqual(System.Math.Max(System.Math.Abs(1 - 4), System.Math.Max(System.Math.Abs(-2 - 5), System.Math.Abs(3 - -6))), a.LInfDistance(b), Tolerance);

            // Norms
            Assert2.AreEqual(System.Math.Abs(1) + System.Math.Abs(-2) + System.Math.Abs(3), a.Norm1(), Tolerance);
            Assert2.AreEqual(1.0 * 1.0 + -2.0 * -2.0 + 3.0 * 3.0, a.Norm2(), Tolerance);
            Assert2.AreEqual(System.Math.Max(System.Math.Abs(1), System.Math.Max(System.Math.Abs(-2), System.Math.Abs(3))), a.NormInfinity(), Tolerance);

            // Outer product (with same length vector)
            var v1 = new DoubleMatrix1D(new double[] { 1.0, 2.0 });
            var v2 = new DoubleMatrix1D(new double[] { 3.0, 4.0 });
            var outer = v1.OuterProduct(v2);
            Assert2.AreEqual(3.0, outer[0, 0], Tolerance);
            Assert2.AreEqual(4.0, outer[0, 1], Tolerance);
            Assert2.AreEqual(6.0, outer[1, 0], Tolerance);
            Assert2.AreEqual(8.0, outer[1, 1], Tolerance);
        }

        [Test]
        public void DoubleMatrix2D_Operations()
        {
            double[][] dataA = new double[][] {
                new double[] { 1.0, 2.0 },
                new double[] { 3.0, 4.0 }
            };

            double[][] dataB = new double[][] {
                new double[] { 5.0, 6.0 },
                new double[] { 7.0, 8.0 }
            };

            var A = new DoubleMatrix2D(dataA);
            var B = new DoubleMatrix2D(dataB);

            var sum = A.Add(B);
            Assert2.AreEqual(6.0, sum[0, 0], Tolerance);
            Assert2.AreEqual(12.0, sum[1, 1], Tolerance);

            var diff = B.Subtract(A);
            Assert2.AreEqual(4.0, diff[0, 0], Tolerance);
            Assert2.AreEqual(4.0, diff[1, 1], Tolerance);

            var scaled = A.Multiply(2.0);
            Assert2.AreEqual(2.0, scaled[0, 0], Tolerance);
            Assert2.AreEqual(8.0, scaled[1, 1], Tolerance);

            var elemMul = A.Multiply(B);
            Assert2.AreEqual(19.0, elemMul[0, 0], Tolerance);
            Assert2.AreEqual(50.0, elemMul[1, 1], Tolerance);

            var transposed = A.Transpose();
            Assert2.AreEqual(1.0, transposed[0, 0], Tolerance);
            Assert2.AreEqual(2.0, transposed[1, 0], Tolerance);

            // Trace
            Assert2.AreEqual(1.0 + 4.0, A.Trace(), Tolerance);

            // Negate
            var neg = A.Negate();
            Assert2.AreEqual(-1.0, neg[0, 0], Tolerance);
            Assert2.AreEqual(-4.0, neg[1, 1], Tolerance);
        }

        [Test]
        public void DoubleMatrix3D_Operations()
        {
            double[][][] data = new double[][][] {
                new double[][] {
                    new double[] { 1.0, 2.0 },
                    new double[] { 3.0, 4.0 }
                },
                new double[][] {
                    new double[] { 5.0, 6.0 },
                    new double[] { 7.0, 8.0 }
                }
            };

            var m = new DoubleMatrix3D(data);

            Assert2.AreEqual(2, m.RowCount);
            Assert2.AreEqual(2, m.ColumnCount);
            Assert2.AreEqual(2, m.SliceCount);
            Assert2.AreEqual(1.0, m[0, 0, 0], Tolerance);

            // SetValue and GetEntry
            m.SetValue(0, 0, 0, 9.0);
            Assert2.AreEqual(9.0, m.GetEntry(0, 0, 0), Tolerance);

            // Add
            var added = m.Add(m);
            Assert2.AreEqual(18.0, added[0, 0, 0], Tolerance);
            Assert2.AreEqual(12.0, added[1, 0, 1], Tolerance);

            // Multiply scalar
            var scaled = m.Multiply(2.0);
            Assert2.AreEqual(18.0, scaled[0, 0, 0], Tolerance);

            // Negate
            var neg = m.Negate();
            Assert2.AreEqual(-9.0, neg[0, 0, 0], Tolerance);

            // Trace (sum of data[i][i][i])
            // After SetValue above data[0][0][0]=9, data[1][1][1]=8
            Assert2.AreEqual(9.0 + 8.0, m.Trace(), Tolerance);

            // Norm (Euclidean over all elements)
            double expectedNorm = 0.0;
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    for (int k = 0; k < data[i][j].Length; k++)
                    {
                        double val = m.Data[i][j][k];
                        expectedNorm += val * val;
                    }
                }
            }
            Assert2.AreEqual(System.Math.Sqrt(expectedNorm), m.Norm(), Tolerance);
        }
    }
}
