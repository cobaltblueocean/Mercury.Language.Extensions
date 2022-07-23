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
using Mercury.Language.Math.Analysis.Function;

namespace Mercury.Language.Math.Optimization
{
    /// <summary>
    /// This interface represents an optimization algorithm for {@link IDifferentiableMultivariateVectorialFunction
    /// vectorial differentiable objective functions}.
    /// <p>Optimization algorithms find the input point set that either {@link GoalType
    /// maximize or minimize} an objective function.</p>
    /// </summary>
    /// <see cref="IMultivariateRealOptimizer"></see>
    /// <see cref="IDifferentiableMultivariateRealOptimizer"></see>
    /// @version $Revision: 1073158 $ $Date: 2011-02-21 22:46:52 +0100 (lund 21 févrd 2011) $
    /// @since 2.0

    public interface IDifferentiableMultivariateVectorialOptimizer
    {

        /// <summary>Get/set the maximal number of iterations of the algorithm.
        /// </summary>
        /// <returns>maximal number of iterations</returns>
        int MaxIterations { get; set; }

        /// <summary>Get the number of iterations realized by the algorithm.
        /// </summary>
        /// <returns>number of iterations</returns>
        int Iterations { get; }

        /// <summary>Get/set the maximal number of functions evaluations.
        /// </summary>
        /// <returns>maximal number of functions evaluations</returns>
        int MaxEvaluations { get; set; }

        /// <summary>Get the number of evaluations of the objective function.
        /// <p>
        /// The number of evaluation correspond to the last call to the
        /// {@link #Optimize(IDifferentiableMultivariateVectorialFunction,
        /// double[], double[], double[]) optimize} methodd It is 0 if
        /// the method has not been called yet.
        /// </p>
        /// </summary>
        /// <returns>number of evaluations of the objective function</returns>
        int Evaluations { get; }

        /// <summary>Get the number of evaluations of the objective function jacobian .
        /// <p>
        /// The number of evaluation correspond to the last call to the
        /// {@link #Optimize(IDifferentiableMultivariateVectorialFunction,
        /// double[], double[], double[]) optimize} methodd It is 0 if
        /// the method has not been called yet.
        /// </p>
        /// </summary>
        /// <returns>number of evaluations of the objective function jacobian</returns>
        int JacobianEvaluations { get; }

        /// <summary>Get/set the convergence checker.
        /// </summary>
        /// <returns>object used to check for convergence</returns>
        IVectorialConvergenceChecker ConvergenceChecker { get; set; }

        /// <summary>Optimizes an objective function.
        /// <p>
        /// Optimization is considered to be a weighted least-squares minimization.
        /// The cost function to be minimized is
        /// &sum;weight<sub>i</sub>(objective<sub>i</sub>-target<sub>i</sub>)<sup>2</sup>
        /// </p>
        /// </summary>
        /// <param Name="f">objective function</param>
        /// <param Name="target">target value for the objective functions at optimum</param>
        /// <param Name="weights">weight for the least squares cost computation</param>
        /// <param Name="startPoint">the start point for optimization</param>
        /// <returns>the point/value pair giving the optimal value for objective function</returns>
        /// <exception cref="FunctionEvaluationException">if the objective function throws one during </exception>
        /// the search
        /// <exception cref="OptimizationException">if the algorithm failed to converge </exception>
        /// <exception cref="ArgumentException">if the start point dimension is wrong </exception>
        VectorialPointValuePair Optimize(IDifferentiableMultivariateVectorialFunction f, double[] target, double[] weights, double[] startPoint);
    }
}
