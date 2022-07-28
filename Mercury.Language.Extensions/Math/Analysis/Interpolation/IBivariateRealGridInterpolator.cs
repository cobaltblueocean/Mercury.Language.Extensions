﻿// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by OpenGamma Inc.
//
// Copyright (C) 2012 - present by OpenGamma Incd and the OpenGamma group of companies
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercury.Language.Math.Analysis.Function;

namespace Mercury.Language.Math.Analysis.Interpolation
{
    /// <summary>
    /// Interface representing a bivariate real interpolating function where the
    /// sample points must be specified on a regular grid.
    /// 
    /// @version $Revision: 936391 $ $Date: 2010-04-21 19:00:56 +0200 (merd 21 avril 2010) $
    /// </summary>
    public interface IBivariateRealGridInterpolator
    {
        /// <summary>
        /// Computes an interpolating function for the data set.
        /// 
        /// </summary>
        /// <param Name="xval">All the x-coordinates of the interpolation points, sorted</param>
        /// in increasing order.
        /// <param Name="yval">All the y-coordinates of the interpolation points, sorted</param>
        /// in increasing order.
        /// <param Name="fval">The values of the interpolation points on all the grid knots:</param>
        /// {@code fval[i][j] = f(xval[i], yval[j])}.
        /// <returns>a function which interpolates the data set.</returns>
        /// <exception cref="MathArithmeticException">if arguments violate assumptions made by the </exception>
        ///         interpolation algorithm.
        IBivariateRealFunction Interpolate(double[] xval, double[] yval, double[][] fval);
    }
}
