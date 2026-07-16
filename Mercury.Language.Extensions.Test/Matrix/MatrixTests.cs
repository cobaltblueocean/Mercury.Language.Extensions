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
using Mercury.Language.Math.Matrix.DoubleMatrix;

namespace Mercury.Language.Extensions.Test.Matrix
{
    public class DoubleMatrixTests
    {
        [Test]
        public void DoubleMatrix1D_BasicOperations()
        {
            var data = new double[] { 1.0, 2.0, 3.0 };
            var m = new DoubleMatrix1D(data);

            Assert2.AreEqual(3, m.NumberOfElements);
            Assert2.AreEqual(1.0, m[0]);
            Assert2.AreEqual(2.0, m[1]);

            var multiplied = m.Multiply(2.0);
            Assert2.AreEqual(2.0, multiplied[0]);
            Assert2.AreEqual(4.0, multiplied[1]);

            var reversed = m.Reverse();
            Assert2.AreEqual(3.0, reversed[0]);
            Assert2.AreEqual(1.0, reversed[2]);
        }

        [Test]
        public void DoubleMatrix2D_BasicOperations()
        {
            double[][] data = new double[][] {
                new double[] { 1.0, 2.0 },
                new double[] { 3.0, 4.0 }
            };

            var m = new DoubleMatrix2D(data);

            Assert2.AreEqual(2, m.RowCount);
            Assert2.AreEqual(2, m.ColumnCount);
            Assert2.AreEqual(2.0, m[0,1]);

            var scaled = m.Multiply(3.0);
            Assert2.AreEqual(3.0, scaled[0,0]);
            Assert2.AreEqual(6.0, scaled[0,1]);

            var as3d = m.AsMatrix3D();
            // original element should be at z=0 in converted 3D
            Assert2.AreEqual(1.0, as3d[0,0,0]);
            Assert2.AreEqual(4.0, as3d[1,1,0]);
        }

        [Test]
        public void DoubleMatrix3D_BasicOperations()
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
            Assert2.AreEqual(1.0, m[0,0,0]);

            // SetValue and GetEntry
            m.SetValue(0, 0, 0, 9.0);
            Assert2.AreEqual(9.0, m.GetEntry(0, 0, 0));

            // Add should double values when adding the matrix to itself
            var added = m.Add(m);
            Assert2.AreEqual(18.0, added[0,0,0]);
            Assert2.AreEqual(4.0, added[0,0,1]);
        }
    }
}
