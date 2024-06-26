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

namespace Mercury.Language.Math.Optimization
{
    /// <summary>
    /// Simple implementation of the {@link IRealConvergenceChecker} interface using
    /// only objective function values.
    /// <p>
    /// Convergence is considered to have been reached if either the relative
    /// difference between the objective function values is smaller than a
    /// threshold or if either the absolute difference between the objective
    /// function values is smaller than another threshold.
    /// </p>
    /// @version $Revision: 990655 $ $Date: 2010-08-29 23:49:40 +0200 (dimd 29 août 2010) $
    /// @since 2.0
    /// </summary>
    public class SimpleScalarValueChecker : IRealConvergenceChecker
    {

        /// <summary>Default relative thresholdd */
        private static double DEFAULT_RELATIVE_THRESHOLD = 100 * QuickMath.DoubleEpsilon;

        /// <summary>Default absolute thresholdd */
        private static double DEFAULT_ABSOLUTE_THRESHOLD = 100 * QuickMath.DoubleSafeMin;

        /// <summary>Relative tolerance thresholdd */
        private double relativeThreshold;

        /// <summary>Absolute tolerance thresholdd */
        private double absoluteThreshold;

        /// <summary>Build an instance with default threshold.
        /// </summary>
        public SimpleScalarValueChecker()
        {
            this.relativeThreshold = DEFAULT_RELATIVE_THRESHOLD;
            this.absoluteThreshold = DEFAULT_ABSOLUTE_THRESHOLD;
        }

        /// <summary>Build an instance with a specified threshold.
        /// <p>
        /// In order to perform only relative checks, the absolute tolerance
        /// must be set to a negative valued In order to perform only absolute
        /// checks, the relative tolerance must be set to a negative value.
        /// </p>
        /// </summary>
        /// <param Name="relativeThreshold">relative tolerance threshold</param>
        /// <param Name="absoluteThreshold">absolute tolerance threshold</param>
        public SimpleScalarValueChecker(double relativeThreshold, double absoluteThreshold)
        {
            this.relativeThreshold = relativeThreshold;
            this.absoluteThreshold = absoluteThreshold;
        }

        /// <summary>{@inheritDoc} */
        public Boolean Converged(int iteration, RealPointValuePair previous, RealPointValuePair current)
        {
            double p = previous.Value;
            double c = current.Value;
            double difference = System.Math.Abs(p - c);
            double size = System.Math.Max(System.Math.Abs(p), System.Math.Abs(c));
            return (difference <= (size * relativeThreshold)) || (difference <= absoluteThreshold);
        }
    }
}
