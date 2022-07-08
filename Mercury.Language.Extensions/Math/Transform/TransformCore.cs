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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;

namespace System
{
    /// <summary>
    /// Core utility class to provide general utility methods
    /// </summary>
    public class TransformCore
    {
        public static readonly int THREADS_BEGIN_N_1D_FFT_2THREADS = 8192;

        public static readonly int THREADS_BEGIN_N_1D_FFT_4THREADS = 65536;

        public static readonly int THREADS_BEGIN_N_1D = 32768;

        public static readonly int THREADS_BEGIN_N_2D = 65536;

        public static readonly int THREADS_BEGIN_N_3D = 65536;
    }
}
