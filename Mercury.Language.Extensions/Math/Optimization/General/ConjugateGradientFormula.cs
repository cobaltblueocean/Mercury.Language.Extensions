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
//
//-------------------------------------------------------------------------------------
//
// Licensed to the Apache Software Foundation (ASF) under one or more
// contributor license agreementsd  See the NOTICE file distributed with
// this work for additional information regarding copyright ownership.
// The ASF licenses this file to You under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with
// the Licensed  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Language.Math.Optimization.General
{
    /// <summary>
    /// Available choices of update formulas for the &beta; parameter
    /// in {@link NonLinearConjugateGradientOptimizer}.
    /// <p>
    /// The &beta; parameter is used to compute the successive conjugate
    /// search directions. For non-linear conjugate gradients, there are
    /// two formulas to compute &beta;:
    /// <ul>
    ///   <li>Fletcher-Reeves formula</li>
    ///   <li>Polak-Ribi&egrave;re formula</li>
    /// </ul>
    /// On the one hand, the Fletcher-Reeves formula is guaranteed to converge
    /// if the start point is close enough of the optimum whether the
    /// Polak-Ribi&egrave;re formula may not converge in rare cases. On the
    /// other hand, the Polak-Ribi&egrave;re formula is often faster when it
    /// does converge. Polak-Ribi&egrave;re is often used.
    /// <p>
    /// </summary>
    /// <see cref="NonLinearConjugateGradientOptimizer"></see>
    /// @version $Revision: 758059 $ $Date: 2009-03-24 23:16:21 +0100 (mar. 24 mars 2009) $
    /// @since 2.0
    public enum ConjugateGradientFormula
    {
        /// <summary>
        /// Fletcher-Reeves formula.
        /// /// </summary>
        FLETCHER_REEVES,

        /// <summary>
        /// Polak-Ribi&egrave;re formula.
        /// /// </summary>
        POLAK_RIBIERE
    }
}
