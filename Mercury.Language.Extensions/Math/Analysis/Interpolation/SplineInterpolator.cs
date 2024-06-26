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
using Mercury.Language.Exceptions;
using Mercury.Language;
using Mercury.Language.Math.Analysis.Function;
using Mercury.Language.Math.Analysis.Polynomial;

namespace Mercury.Language.Math.Analysis.Interpolation
{
    /// <summary>
    /// Computes a natural (also known as "free", "unclamped") cubic spline interpolation for the data set.
    /// <p>
    /// The {@link #Interpolate(double[], double[])} method returns a {@link PolynomialSplineFunction}
    /// consisting of n cubic polynomials, defined over the subintervals determined by the x values,
    /// x[0] < x[i] [] < x[n]d  The x values are referred to as "knot points."</p>
    /// <p>
    /// The value of the PolynomialSplineFunction at a point x that is greater than or equal to the smallest
    /// knot point and strictly less than the largest knot point is computed by finding the subinterval to which
    /// x belongs and computing the value of the corresponding polynomial at <code>x - x[i] </code> where
    /// <code>i</code> is the index of the subintervald  See {@link PolynomialSplineFunction} for more details.
    /// </p>
    /// <p>
    /// The interpolating polynomials satisfy: <ol>
    /// <li>The value of the PolynomialSplineFunction at each of the input x values equals the
    ///  corresponding y value.</li>
    /// <li>Adjacent polynomials are equal through two derivatives at the knot points (i.ed, adjacent polynomials
    ///  "match up" at the knot points, as do their first and second derivatives).</li>
    /// </ol></p>
    /// <p>
    /// The cubic spline interpolation algorithm implemented is as described in R.Ld Burden, J.Dd Faires,
    /// <u>Numerical Analysis</u>, 4th Edd, 1989, PWS-Kent, ISBN 0-53491-585-X, pp 126-131d
    /// </p>
    /// 
    /// @version $Revision: 983921 $ $Date: 2010-08-10 12:46:06 +0200 (mard 10 août 2010) $
    /// 
    /// </summary>
    public class SplineInterpolator : IUnivariateRealInterpolator
    {

        /// <summary>
        /// Computes an interpolating function for the data set.
        /// </summary>
        /// <param Name="x">the arguments for the interpolation points</param>
        /// <param Name="y">the values for the interpolation points</param>
        /// <returns>a function which interpolates the data set</returns>
        /// <exception cref="DimensionMismatchException">if {@code x} and {@code y} </exception>
        /// have different sizes.
        /// <exception cref="Mercury.Language.Exceptions.NonMonotonousSequenceException"></exception>
        /// if {@code x} is not sorted in strict increasing order.
        /// <exception cref="NumberIsTooSmallException">if the size of {@code x} is smaller </exception>
        /// than 3d
        public IUnivariateRealFunction Interpolate(double[] x, double[] y)
        {
            if (x.Length != y.Length)
            {
                throw new DimensionMismatchException(x.Length, y.Length);
            }

            if (x.Length < 3)
            {
                throw new NumberIsTooSmallException(LocalizedResources.Instance().NUMBER_OF_POINTS,
                                                    x.Length, 3, true);
            }

            // Number of intervalsd  The number of data points is n + 1d
            int n = x.Length - 1;

            QuickMath.CheckOrder(x);

            // Differences between knot points
            double[] h = new double[n];
            AutoParallel.AutoParallelFor(0, n, (i) =>
            {
                h[i] = x[i + 1] - x[i];
            });

            double[] mu = new double[n];
            double[] z = new double[n + 1];
            mu[0] = 0d;
            z[0] = 0d;
            double g = 0;
            for(int i = 1; i < n; i++)
            {
                g = 2d * (x[i + 1] - x[i - 1]) - h[i - 1] * mu[i - 1];
                mu[i] = h[i] / g;
                z[i] = (3d * (y[i + 1] * h[i - 1] - y[i] * (x[i + 1] - x[i - 1]) + y[i - 1] * h[i]) /
                        (h[i - 1] * h[i]) - h[i - 1] * z[i - 1]) / g;
            }

            // cubic spline coefficients --  b is linear, c quadratic, d is cubic (original y's are constants)
            double[] b = new double[n];
            double[] c = new double[n + 1];
            double[] d = new double[n];

            z[n] = 0d;
            c[n] = 0d;

            for (int j = n - 1; j >= 0; j--)
            {
                c[j] = z[j] - mu[j] * c[j + 1];
                b[j] = (y[j + 1] - y[j]) / h[j] - h[j] * (c[j + 1] + 2d * c[j]) / 3d;
                d[j] = (c[j + 1] - c[j]) / (3d * h[j]);
            }

            PolynomialFunction[] polynomials = new PolynomialFunction[n];
            double[] coefficients = new double[4];
            AutoParallel.AutoParallelFor(0, n, (i) =>
            {
                coefficients[0] = y[i];
                coefficients[1] = b[i];
                coefficients[2] = c[i];
                coefficients[3] = d[i];
                polynomials[i] = new PolynomialFunction(coefficients);
            });

            return new PolynomialSplineFunction(x, polynomials);
        }
    }
}
