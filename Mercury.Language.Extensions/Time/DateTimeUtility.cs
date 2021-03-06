// Copyright (c) 2017 - presented by Kei Nakai
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenGamma.Utility;
using Mercury.Language.Exception;
using NodaTime;
using NodaTime.Text;

namespace OpenGamma.Utility
{
    /// <summary>
    /// DateTimeUtility Description
    /// </summary>
    public static class DateTimeUtility
    {
        public static NodaTime.Period ParsePeriod(String tenorStr)
        {
            var pattern = DurationPattern.CreateWithInvariantCulture("H'h'm'm's's'");
            var duration = pattern.Parse(tenorStr).Value;

            return NodaTime.Period.FromTicks((long) duration.TotalTicks);
        }

        public static Boolean IsLeap(int year)
        {
            DateTimeZone timeZone = DateTimeZoneProviders.Bcl.GetSystemDefault();
            var date = new ZonedDateTime(new DateTime(year, 1, 1).ToInstant(), timeZone);
            return date.IsLeapYear();
        }

    }
}
