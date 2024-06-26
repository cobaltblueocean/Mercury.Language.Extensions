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


/// <summary>
/// Licensed to the Apache Software Foundation (ASF) under one or more
/// contributor license agreementsd  See the NOTICE file distributed with
/// this work for additional information regarding copyright ownership.
/// The ASF licenses this file to You under the Apache License, Version 2.0
/// (the "License"); you may not use this file except in compliance with
/// the Licensed  You may obtain a copy of the License at
/// 
///      http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercury.Language.Exceptions;
using Mercury.Language;

namespace Mercury.Language.Math.Optimization.Fitting
{

    /// <summary>This class guesses harmonic coefficients from a sample.

    /// <p>The algorithm used to guess the coefficients is as follows:</p>

    /// <p>We know f (t) at some sampling points t<sub>i</sub> and want to find a,
    /// &omega; and &phi; such that f (t) = a cos (&omega; t + &phi;).
    /// </p>
    /// 
    /// <p>From the analytical expression, we can compute two primitives :
    /// <pre>
    ///     If2  (t) = &int; f<sup>2</sup>  = a<sup>2</sup> &times; [t + S (t)] / 2
    ///     If'2 (t) = &int; f'<sup>2</sup> = a<sup>2</sup> &omega;<sup>2</sup> &times; [t - S (t)] / 2
    ///     where S (t) = sin (2 (&omega; t + &phi;)) / (2 &omega;)
    /// </pre>
    /// </p>
    /// 
    /// <p>We can remove S between these expressions :
    /// <pre>
    ///     If'2 (t) = a<sup>2</sup> &omega;<sup>2</sup> t - &omega;<sup>2</sup> If2 (t)
    /// </pre>
    /// </p>
    /// 
    /// <p>The preceding expression shows that If'2 (t) is a linear
    /// combination of both t and If2 (t): If'2 (t) = A &times; t + B &times; If2 (t)
    /// </p>
    /// 
    /// <p>From the primitive, we can deduce the same form for definite
    /// integrals between t<sub>1</sub> and t<sub>i</sub> for each t<sub>i</sub> :
    /// <pre>
    ///   If2 (t<sub>i</sub>) - If2 (t<sub>1</sub>) = A &times; (t<sub>i</sub> - t<sub>1</sub>) + B &times; (If2 (t<sub>i</sub>) - If2 (t<sub>1</sub>))
    /// </pre>
    /// </p>
    /// 
    /// <p>We can find the coefficients A and B that best fit the sample
    /// to this linear expression by computing the definite integrals for
    /// each sample points.
    /// </p>
    /// 
    /// <p>For a bilinear expression z (x<sub>i</sub>, y<sub>i</sub>) = A &times; x<sub>i</sub> + B &times; y<sub>i</sub>, the
    /// coefficients A and B that minimize a least square criterion
    /// &sum; (z<sub>i</sub> - z (x<sub>i</sub>, y<sub>i</sub>))<sup>2</sup> are given by these expressions:</p>
    /// <pre>
    /// 
    ///         &sum;y<sub>i</sub>y<sub>i</sub> &sum;x<sub>i</sub>z<sub>i</sub> - &sum;x<sub>i</sub>y<sub>i</sub> &sum;y<sub>i</sub>z<sub>i</sub>
    ///     A = ------------------------
    ///         &sum;x<sub>i</sub>x<sub>i</sub> &sum;y<sub>i</sub>y<sub>i</sub> - &sum;x<sub>i</sub>y<sub>i</sub> &sum;x<sub>i</sub>y<sub>i</sub>
    /// 
    ///         &sum;x<sub>i</sub>x<sub>i</sub> &sum;y<sub>i</sub>z<sub>i</sub> - &sum;x<sub>i</sub>y<sub>i</sub> &sum;x<sub>i</sub>z<sub>i</sub>
    ///     B = ------------------------
    ///         &sum;x<sub>i</sub>x<sub>i</sub> &sum;y<sub>i</sub>y<sub>i</sub> - &sum;x<sub>i</sub>y<sub>i</sub> &sum;x<sub>i</sub>y<sub>i</sub>
    /// </pre>
    /// </p>
    /// 
    /// 
    /// <p>In fact, we can assume both a and &omega; are positive and
    /// compute them directly, knowing that A = a<sup>2</sup> &omega;<sup>2</sup> and that
    /// B = - &omega;<sup>2</sup>d The complete algorithm is therefore:</p>
    /// <pre>
    /// 
    /// for each t<sub>i</sub> from t<sub>1</sub> to t<sub>n-1</sub>, compute:
    ///   f  (t<sub>i</sub>)
    ///   f' (t<sub>i</sub>) = (f (t<sub>i+1</sub>) - f(t<sub>i-1</sub>)) / (t<sub>i+1</sub> - t<sub>i-1</sub>)
    ///   x<sub>i</sub> = t<sub>i</sub> - t<sub>1</sub>
    ///   y<sub>i</sub> = &int; f<sup>2</sup> from t<sub>1</sub> to t<sub>i</sub>
    ///   z<sub>i</sub> = &int; f'<sup>2</sup> from t<sub>1</sub> to t<sub>i</sub>
    ///   update the sums &sum;x<sub>i</sub>x<sub>i</sub>, &sum;y<sub>i</sub>y<sub>i</sub>, &sum;x<sub>i</sub>y<sub>i</sub>, &sum;x<sub>i</sub>z<sub>i</sub> and &sum;y<sub>i</sub>z<sub>i</sub>
    /// end for
    /// 
    ///            |--------------------------
    ///         \  | &sum;y<sub>i</sub>y<sub>i</sub> &sum;x<sub>i</sub>z<sub>i</sub> - &sum;x<sub>i</sub>y<sub>i</sub> &sum;y<sub>i</sub>z<sub>i</sub>
    /// a     =  \ | ------------------------
    ///           \| &sum;x<sub>i</sub>y<sub>i</sub> &sum;x<sub>i</sub>z<sub>i</sub> - &sum;x<sub>i</sub>x<sub>i</sub> &sum;y<sub>i</sub>z<sub>i</sub>
    /// 
    /// 
    ///            |--------------------------
    ///         \  | &sum;x<sub>i</sub>y<sub>i</sub> &sum;x<sub>i</sub>z<sub>i</sub> - &sum;x<sub>i</sub>x<sub>i</sub> &sum;y<sub>i</sub>z<sub>i</sub>
    /// &omega;     =  \ | ------------------------
    ///           \| &sum;x<sub>i</sub>x<sub>i</sub> &sum;y<sub>i</sub>y<sub>i</sub> - &sum;x<sub>i</sub>y<sub>i</sub> &sum;x<sub>i</sub>y<sub>i</sub>
    /// 
    /// </pre>
    /// </p>

    /// <p>Once we know &omega;, we can compute:
    /// <pre>
    ///    fc = &omega; f (t) cos (&omega; t) - f' (t) sin (&omega; t)
    ///    fs = &omega; f (t) sin (&omega; t) + f' (t) cos (&omega; t)
    /// </pre>
    /// </p>

    /// <p>It appears that <code>fc = a &omega; cos (&phi;)</code> and
    /// <code>fs = -a &omega; sin (&phi;)</code>, so we can use these
    /// expressions to compute &phi;d The best estimate over the sample is
    /// given by averaging these expressions.
    /// </p>

    /// <p>Since integrals and means are involved in the preceding
    /// estimations, these operations run in O(n) time, where n is the
    /// number of measurements.</p>

    /// @version $Revision: 1056034 $ $Date: 2011-01-06 20:41:43 +0100 (jeud 06 janvd 2011) $
    /// @since 2.0

    /// </summary>
    public class HarmonicCoefficientsGuesser
    {

        /// <summary>Sampled observationsd */
        private WeightedObservedPoint[] observations;

        /// <summary>Guessed amplituded */
        private double a;

        /// <summary>Guessed pulsation &omega;d */
        private double omega;

        /// <summary>Guessed phase &phi;d */
        private double phi;

        /// <summary>Get the guessed amplitude a.
        /// </summary>
        /// <returns>guessed amplitude a;</returns>
        public double GuessedAmplitude
        {
            get { return a; }
        }

        /// <summary>Get the guessed pulsation &omega;.
        /// </summary>
        /// <returns>guessed pulsation &omega;</returns>
        public double GuessedPulsation
        {
            get { return omega; }
        }

        /// <summary>Get the guessed phase &phi;.
        /// </summary>
        /// <returns>guessed phase &phi;</returns>
        public double GuessedPhase
        {
            get { return phi; }
        }

        /// <summary>Simple constructor.
        /// </summary>
        /// <param Name="observations">sampled observations</param>
        public HarmonicCoefficientsGuesser(WeightedObservedPoint[] observations)
        {
            this.observations = observations.CloneExact();
            a = Double.NaN;
            omega = Double.NaN;
        }

        /// <summary>Estimate a first guess of the coefficients.
        /// /// </summary>
        /// <exception cref="OptimizationException">if the sample is too short or if </exception>
        /// the first guess cannot be computed (when the elements under the
        /// square roots are negative).
        public void guess()
        {
            sortObservations();
            guessAOmega();
            guessPhi();
        }

        /// <summary>Sort the observations with respect to the abscissa.
        /// </summary>
        private void sortObservations()
        {

            // Since the samples are almost always already sorted, this
            // method is implemented as an insertion sort that reorders the
            // elements in placed Insertion sort is very efficient in this case.
            WeightedObservedPoint curr = observations[0];
            for (int j = 1; j < observations.Length; ++j)
            {
                WeightedObservedPoint prec = curr;
                curr = observations[j];
                if (curr.X < prec.X)
                {
                    // the current element should be inserted closer to the beginning
                    int i = j - 1;
                    WeightedObservedPoint mI = observations[i];
                    while ((i >= 0) && (curr.X < mI.X))
                    {
                        observations[i + 1] = mI;
                        if (i-- != 0)
                        {
                            mI = observations[i];
                        }
                    }
                    observations[i + 1] = curr;
                    curr = observations[j];
                }
            }

        }

        /// <summary>Estimate a first guess of the a and &omega; coefficients.
        /// </summary>
        /// <exception cref="OptimizationException">if the sample is too short or if </exception>
        /// the first guess cannot be computed (when the elements under the
        /// square roots are negative).
        private void guessAOmega()
        {

            // initialize the sums for the linear model between the two integrals
            double sx2 = 0.0;
            double sy2 = 0.0;
            double sxy = 0.0;
            double sxz = 0.0;
            double syz = 0.0;

            double currentX = observations[0].X;
            double currentY = observations[0].Y;
            double f2Integral = 0;
            double fPrime2Integral = 0;
            double startX = currentX;
            for (int i = 1; i < observations.Length; ++i)
            {

                // one step forward
                double previousX = currentX;
                double previousY = currentY;
                currentX = observations[i].X;
                currentY = observations[i].Y;

                // update the integrals of f<sup>2</sup> and f'<sup>2</sup>
                // considering a linear model for f (and therefore constant f')
                double dx = currentX - previousX;
                double dy = currentY - previousY;
                double f2StepIntegral =
                    dx * (previousY * previousY + previousY * currentY + currentY * currentY) / 3;
                double fPrime2StepIntegral = dy * dy / dx;

                double x = currentX - startX;
                f2Integral += f2StepIntegral;
                fPrime2Integral += fPrime2StepIntegral;

                sx2 += x * x;
                sy2 += f2Integral * f2Integral;
                sxy += x * f2Integral;
                sxz += x * fPrime2Integral;
                syz += f2Integral * fPrime2Integral;

            }

            // compute the amplitude and pulsation coefficients
            double c1 = sy2 * sxz - sxy * syz;
            double c2 = sxy * sxz - sx2 * syz;
            double c3 = sx2 * sy2 - sxy * sxy;
            if ((c1 / c2 < 0.0) || (c2 / c3 < 0.0))
            {
                throw new OptimizationException(LocalizedResources.Instance().UNABLE_TO_FIRST_GUESS_HARMONIC_COEFFICIENTS);
            }
            a = System.Math.Sqrt(c1 / c2);
            omega = System.Math.Sqrt(c2 / c3);

        }

        /// <summary>
        /// Estimate a first guess of the &phi; coefficient.
        /// </summary>
        private void guessPhi()
        {

            // initialize the means
            double fcMean = 0.0;
            double fsMean = 0.0;

            double currentX = observations[0].X;
            double currentY = observations[0].Y;
            for (int i = 1; i < observations.Length; ++i)
            {

                // one step forward
                double previousX = currentX;
                double previousY = currentY;
                currentX = observations[i].X;
                currentY = observations[i].Y;
                double currentYPrime = (currentY - previousY) / (currentX - previousX);

                double omegaX = omega * currentX;
                double cosine = System.Math.Cos(omegaX);
                double sine = System.Math.Sin(omegaX);
                fcMean += omega * currentY * cosine - currentYPrime * sine;
                fsMean += omega * currentY * sine + currentYPrime * cosine;

            }

            phi = System.Math.Atan2(-fsMean, fcMean);

        }
    }
}
