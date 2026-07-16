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
//     http://www.apache.org/licenses/LICENSE-2.0
//     
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Mercury.Language.Exceptions;

namespace System
{

    /// <summary>
    /// MatrixUtility Description
    /// </summary>
    public static class MatrixExceptionFactory
    {

        #region Throw DimensionMismatchException
        public static DimensionMismatchException DimensionsDontMatch<E>() where E : Exception
        {
            return new DimensionMismatchException();
        }

        public static DimensionMismatchException DimensionsDontMatch<E>(int wrong, int expected) where E : Exception
        {
            return new DimensionMismatchException(wrong, expected);
        }

        public static DimensionMismatchException DimensionsDontMatch<E>(int leftRow, int rightRow, int leftCol, int rightCol) where E : Exception
        {
            return new DimensionMismatchException("Left Row: " + leftRow + "\r\n" + "Right Row: " + rightRow + "\r\n" + "Left Col: " + leftCol + "\r\n" + "Right Col: " + rightCol);
        }

        public static DimensionMismatchException DimensionsDontMatch<E>(String message) where E : Exception
        {
            return new DimensionMismatchException(message);
        }

        public static DimensionMismatchException DimensionsDontMatch<E>(Object target, Object source, String message) where E : Exception
        {
            return new DimensionMismatchException("Target: " + target.GetType().FullName + "\r\n" + "Source: " + source.ToString() + "\r\n" + "Message: " + message);
        }

        public static DimensionMismatchException DimensionsDontMatch<E>(Object target, Object source1, Object source2) where E : Exception
        {
            return new DimensionMismatchException("Target: " + target.GetType().FullName + "\r\n" + "Source1 " + source1.ToString() + "\r\n" + "Source2: " + source2);
        }

        public static DimensionMismatchException DimensionsDontMatch<E>(String message, System.Exception cause) where E : Exception
        {
            return new DimensionMismatchException(message, cause);
        }

        public static DimensionMismatchException DimensionsDontMatch<E>(Vector<decimal> input, Vector<decimal> factor) where E : Exception
        {
            return new DimensionMismatchException(input.Count, factor.Count);
        }

        public static DimensionMismatchException DimensionsDontMatch<E>(Vector<decimal> input, Matrix<decimal> factor) where E : Exception
        {
            return new DimensionMismatchException(input.Count, factor.RowCount);
        }

        public static DimensionMismatchException DimensionsDontMatch<E>(Matrix<decimal> input, Vector<decimal> factor) where E : Exception
        {
            return new DimensionMismatchException(input.ColumnCount, factor.Count);
        }

        public static DimensionMismatchException DimensionsDontMatch<E>(Matrix<decimal> input) where E : Exception
        {
            return new DimensionMismatchException(input.ColumnCount, input.RowCount);
        }

        public static DimensionMismatchException DimensionsDontMatch<E>(Matrix<decimal> input, Matrix<decimal> factor) where E : Exception
        {
            return new DimensionMismatchException(input.RowCount, factor.RowCount);
        }
        #endregion

    }
}
