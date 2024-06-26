﻿// Copyright (c) 2017 - presented by Kei Nakai
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
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Mercury.Language.Math.Analysis.Solver;
using Mercury.Test.Utility;

namespace Mercury.Language.Extensions.Test.Solver
{
    /// <summary>
    /// LaguerreSolverTest Description
    /// </summary>
    [Parallelizable(ParallelScope.ContextMask)]
    public class LaguerreSolverTest
    {
        private static LaguerreSolver ROOT_FINDER = new LaguerreSolver();

        [Test]
        public void Test()
        {
            Complex[] expected = new Complex[2] { new Complex(-3.0000000000000013, 0), new Complex(-3.9999999999999982, 0) };

            Complex[] roots = ROOT_FINDER.SolveAllComplex(new double[] { 12, 7, 1 }, 0);

            Assert2.AreEqual(expected.Length, roots.Length);
            Assert2.AreEqual(expected, roots);
        }
    }
}
