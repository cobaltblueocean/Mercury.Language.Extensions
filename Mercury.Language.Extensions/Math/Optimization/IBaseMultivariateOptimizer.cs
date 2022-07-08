﻿// Copyright (c) 2017 - presented by Kei Nakai
//
// Original project is developed and published by System.Math.Optimization Inc.
//
// Copyright (C) 2012 - present by System.Math.Optimization Inc. and the System.Math.Optimization group of companies
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
using Mercury.Language.Math.Analysis;

namespace Mercury.Language.Math.Optimization
{
    /// <summary>
    /// This interface is mainly intended to enforce the internal coherence of Math
    /// Users of the API are advised to base their code on the following interfaces:
    /// <see cref="IMultivariateOptimizer"/>
    /// <see cref="IMultivariateDifferentiableOptimizer"/> 
    /// </summary>
    /// <typeparam name="F"></typeparam>
    public interface IBaseMultivariateOptimizer<F>: IBaseOptimizer<Tuple<Double[], Double>> where F: IMultivariateRealFunction
    {
        /// <summary>
        /// Optimize an objective function.
        /// </summary>
        /// <param name="maxEval">Maximum number of function evaluations.</param>
        /// <param name="f">Objective function.</param>
        /// <param name="goalType">Type of optimization goal: either <see cref="GoalType.MAXIMIZE"/> or <see cref="GoalType.MINIMIZE"/>.</param>
        /// <param name="startPoint">Start point for optimization.</param>
        /// <returns>the point/value pair giving the optimal value for objective function</returns>
        Tuple<Double[], Double> Optimize(int maxEval, F f, GoalType goalType, double[] startPoint);
    }
}