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

/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreementsd  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the Licensed  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercury.Language.Exceptions;
using Mercury.Language;
using Mercury.Language.Math.Random;
using Mercury.Language.Math.Analysis.Function;
using Mercury.Language.Log;

namespace Mercury.Language.Math.Optimization
{
    /// <summary>
    /// Special implementation of the {@link IDifferentiableMultivariateVectorialOptimizer} interface adding
    /// multi-start features to an existing optimizer.
    /// <p>
    /// This class wraps a classical optimizer to use it several times in
    /// turn with different starting points in order to avoid being trapped
    /// into a local extremum when looking for a global one.
    /// </p>
    /// @version $Revision: 1073158 $ $Date: 2011-02-21 22:46:52 +0100 (lund 21 févrd 2011) $
    /// @since 2.0
    /// </summary>
    public class MultiStartDifferentiableMultivariateVectorialOptimizer : IDifferentiableMultivariateVectorialOptimizer
    {

        /// <summary>IUnderlying classical optimizerd */
        private IDifferentiableMultivariateVectorialOptimizer optimizer;

        /// <summary>Maximal number of iterations allowedd */
        private int _maxIterations;

        /// <summary>Number of iterations already performed for all startsd */
        private int _totalIterations;

        /// <summary>Maximal number of evaluations allowedd */
        private int _maxEvaluations;

        /// <summary>Number of evaluations already performed for all startsd */
        private int _totalEvaluations;

        /// <summary>Number of jacobian evaluations already performed for all startsd */
        private int _totalJacobianEvaluations;

        /// <summary>Number of starts to god */
        private int _starts;

        /// <summary>Random generator for multi-startd */
        private IRandomVectorGenerator _generator;

        /// <summary>Found optimad */
        private VectorialPointValuePair[] _optima;

        /// <summary>
        /// Create a multi-start optimizer from a single-start optimizer
        /// </summary>
        /// <param Name="optimizer">single-start optimizer to wrap</param>
        /// <param Name="starts">number of starts to perform (including the</param>
        /// first one), multi-start is disabled if value is less than or
        /// equal to 1
        /// <param Name="generator">random vector generator to use for restarts</param>
        public MultiStartDifferentiableMultivariateVectorialOptimizer(IDifferentiableMultivariateVectorialOptimizer optimizer, int starts, IRandomVectorGenerator generator)
        {
            this.optimizer = optimizer;
            this._totalIterations = 0;
            this._totalEvaluations = 0;
            this._totalJacobianEvaluations = 0;
            this._starts = starts;
            this._generator = generator;
            this._optima = null;
            MaxIterations = Int32.MaxValue;
            MaxEvaluations = Int32.MaxValue;
        }

        /// <summary>Get all the optima found during the last call to {@link
        /// #Optimize(IDifferentiableMultivariateVectorialFunction,
        /// double[], double[], double[]) optimize}.
        /// <p>The optimizer stores all the optima found during a set of
        /// restartsd The {@link #Optimize(IDifferentiableMultivariateVectorialFunction,
        /// double[], double[], double[]) optimize} method returns the
        /// best point onlyd This method returns all the points found at the
        /// end of each starts, including the best one already returned by the {@link
        /// #Optimize(IDifferentiableMultivariateVectorialFunction, double[],
        /// double[], double[]) optimize} method.
        /// </p>
        /// <p>
        /// The returned array as one element for each start as specified
        /// in the constructord It is ordered with the results from the
        /// runs that did converge first, sorted from best to worst
        /// objective value (i.e in ascending order if minimizing and in
        /// descending order if maximizing), followed by and null elements
        /// corresponding to the runs that did not converged This means all
        /// elements will be null if the {@link #Optimize(IDifferentiableMultivariateVectorialFunction,
        /// double[], double[], double[]) optimize} method did throw a {@link
        /// org.apache.commons.System.Math.NonConvergenceException NonConvergenceException}).
        /// This also means that if the first element is non null, it is the best
        /// point found across all starts.</p>
        /// </summary>
        /// <returns>array containing the optima</returns>
        /// <exception cref="InvalidOperationException">if {@link #Optimize(IDifferentiableMultivariateVectorialFunction, </exception>
        /// double[], double[], double[]) optimize} has not been called
        public VectorialPointValuePair[] GetOptima()
        {
            if (_optima == null)
            {
                throw new InvalidOperationException(LocalizedResources.Instance().NO_OPTIMUM_COMPUTED_YET);
            }
            return _optima.CloneExact();
        }

        /// <summary>
        /// {@inheritDoc} 
        ///</summary>
        public int MaxIterations
        {
            get { return _maxIterations; }
            set { this._maxIterations = value; }
        }

        /// <summary>
        /// {@inheritDoc} 
        /// </summary>
        public int Iterations
        {
            get { return _totalIterations; }
        }

        /// <summary>
        /// {@inheritDoc} 
        /// </summary>
        public int MaxEvaluations
        {
            get { return _maxEvaluations; }
            set { this._maxEvaluations = value; }
        }

        /// <summary>
        /// {@inheritDoc} 
        /// </summary>
        public int Evaluations
        {
            get { return _totalEvaluations; }
        }

        /// <summary>
        /// {@inheritDoc} 
        /// </summary>
        public int JacobianEvaluations
        {
            get { return _totalJacobianEvaluations; }
        }

        /// <summary>
        /// {@inheritDoc}
        /// </summary>
        public IVectorialConvergenceChecker ConvergenceChecker
        {
            get { return optimizer.ConvergenceChecker; }
            set { optimizer.ConvergenceChecker = value; }
        }

        /// <summary>
        /// {@inheritDoc} 
        /// </summary>
        public VectorialPointValuePair Optimize(IDifferentiableMultivariateVectorialFunction f, double[] target, double[] weights, double[] startPoint)
        {

            _optima = new VectorialPointValuePair[_starts];
            _totalIterations = 0;
            _totalEvaluations = 0;
            _totalJacobianEvaluations = 0;

            // multi-start loop
            for (int i = 0; i < _starts; ++i)
            {

                try
                {
                    optimizer.MaxIterations = (_maxIterations - _totalIterations);
                    optimizer.MaxEvaluations = (_maxEvaluations - _totalEvaluations);
                    _optima[i] = optimizer.Optimize(f, target, weights,
                                                   (i == 0) ? startPoint : _generator.NextVector());
                }
                catch (FunctionEvaluationException fee)
                {
                    Logger.Information(fee.Message);
                    _optima[i] = null;
                }
                catch (OptimizationException oe)
                {
                    Logger.Information(oe.Message);
                    _optima[i] = null;
                }

                _totalIterations += optimizer.Iterations;
                _totalEvaluations += optimizer.Evaluations;
                _totalJacobianEvaluations += optimizer.JacobianEvaluations;

            }

            // sort the optima from best to worst, followed by null elements
            Array.Sort(_optima, new VectorialPointValuePairComparator(target, weights));

            if (_optima[0] == null)
            {
                throw new OptimizationException(LocalizedResources.Instance().NO_CONVERGENCE_WITH_ANY_START_POINT, _starts);
            }

            // return the found point given the best objective function value
            return _optima[0];

        }

        private class VectorialPointValuePairComparator : IComparer<VectorialPointValuePair>
        {
            double[] _target;
            double[] _weights;

            public VectorialPointValuePairComparator(double[] target, double[] weights)
            {
                _target = target;
                _weights = weights;
            }

            public int Compare(VectorialPointValuePair o1, VectorialPointValuePair o2)
            {
                if (o1 == null)
                {
                    return (o2 == null) ? 0 : +1;
                }
                else if (o2 == null)
                {
                    return -1;
                }
                return weightedResidual(o1).CompareTo(weightedResidual(o2));
            }
            private double weightedResidual(VectorialPointValuePair pv)
            {
                double[] value = pv.Value;
                double sum = 0;
                for (int i = 0; i < value.Length; ++i)
                {
                    double ri = value[i] - _target[i];
                    sum += _weights[i] * ri * ri;
                }
                return sum;
            }
        }
    }
}
