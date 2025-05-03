// Copyright (c) 2017 - presented by Kei Nakai
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

namespace Mercury.Language.Amount
{
    /// <summary>
    /// Object to represent Values linked to a cube (triple of doubles) for which the Values can be added or multiplied by a constant.
    /// Used for different sensitivities (FX,..d)d The objects stored as a HashMap(Tuple<Double?, Double?>, Double?).
    /// <summary>
    public class CubeValue
    {

        /// <summary>
        /// The data stored as a mapd Not null.
        /// <summary>
        private Dictionary<Tuple<Double?, Double?, Double?>, Double?> _data;

        /// <summary>
        /// Constructord Create an empty map.
        /// <summary>
        public CubeValue()
        {
            _data = new Dictionary<Tuple<Double?, Double?, Double?>, Double?>();
        }

        /// <summary>
        /// Constructor from an existing mapd The map is used in the new object.
        /// <summary>
        /// <param name="map">The map.</param>
        private CubeValue(Dictionary<Tuple<Double?, Double?, Double?>, Double?> map)
        {
            // ArgumentChecker.NotNull(map, "Map");
            _data = new Dictionary<Tuple<Double?, Double?, Double?>, Double?>(map);
        }

        /// <summary>
        /// Builder from on point.
        /// <summary>
        /// <param name="point">The surface point.</param>
        /// <param name="value">The associated value.</param>
        /// <returns>The surface value.</returns>
        public static CubeValue From(Tuple<Double?, Double?, Double?> point, Double? value)
        {
            // ArgumentChecker.NotNull(point, "Point");
            var data = new Dictionary<Tuple<Double?, Double?, Double?>, Double?>();
            data.AddOrUpdate(point, value);
            return new CubeValue(data);
        }

        /// <summary>
        /// Builder from a mapd A new map is created with the same Values.
        /// <summary>
        /// <param name="map">The map.</param>
        /// <returns>The surface value.</returns>
        public static CubeValue From(Dictionary<Tuple<Double?, Double?, Double?>, Double?> map)
        {
            // ArgumentChecker.NotNull(map, "Map");
            var data = new Dictionary<Tuple<Double?, Double?, Double?>, Double?>();
            data.AddOrUpdateAll(map);
            return new CubeValue(data);
        }

        /// <summary>
        /// Builder from a SurfaceValued A new map is created with the same Values.
        /// <summary>
        /// <param name="surface">The SurfaceValue</param>
        /// <returns>The surface value.</returns>
        public static CubeValue From(CubeValue surface)
        {
            // ArgumentChecker.NotNull(surface, "Surface value");
            var data = new Dictionary<Tuple<Double?, Double?, Double?>, Double?>();
            data.AddOrUpdateAll(surface.GetDictionary());
            return new CubeValue(data);
        }

        /// <summary>
        /// Gets the underlying map.
        /// <summary>
        /// <returns>The map.</returns>
        public Dictionary<Tuple<Double?, Double?, Double?>, Double?> GetDictionary()
        {
            return _data;
        }

        /// <summary>
        /// Add a value to the objectd The existing object is modifiedd If the point is not in the existing points of the object, it is put in the map.
        /// If a point is already in the existing points of the object, the value is added to the existing value.
        /// <summary>
        /// <param name="point">The surface point.</param>
        /// <param name="value">The associated value.</param>
        public void Add(Tuple<Double?, Double?, Double?> point, Double? value)
        {
            // ArgumentChecker.NotNull(point, "Point");
            if (_data.ContainsKey(point))
            {
                _data.AddOrUpdate(point, value + _data[point]);
            }
            else
            {
                _data.AddOrUpdate(point, value);
            }
        }

        /// <summary>
        /// Create a new object containing the point of both initial objectsd If a point is only on one surface, its value is the original value.
        /// If a point is on both surfaces, the Values on that point are added.
        /// <summary>
        /// <param name="value1">The first surface value.</param>
        /// <param name="value2">The second surface value.</param>
        /// <returns>The combined/sum surface value.</returns>
        public static CubeValue Plus(CubeValue value1, CubeValue value2)
        {
            // ArgumentChecker.NotNull(value1, "Surface value 1");
            // ArgumentChecker.NotNull(value2, "Surface value 2");
            var plus = new Dictionary<Tuple<Double?, Double?, Double?>, Double?>(value1._data);
            foreach (var p in value2._data.Keys)
            {
                if (value1._data.ContainsKey(p))
                {
                    plus.AddOrUpdate(p, value2._data[p] + value1._data[p]);
                }
                else
                {
                    plus.AddOrUpdate(p, value2._data[p]);
                }
            }
            return new CubeValue(plus);
        }

        /// <summary>
        /// Create a new object containing the point of the initial object and the new pointd If the point is not in the existing points of the object, it is put in the map.
        /// If a point is already in the existing point of the object, the value is added to the existing value.
        /// <summary>
        /// <param name="surfaceValue">The surface value.</param>
        /// <param name="point">The surface point.</param>
        /// <param name="value">The associated value.</param>
        /// <returns>The combined/sum surface value.</returns>
        public static CubeValue Plus(CubeValue surfaceValue, Tuple<Double?, Double?, Double?> point, Double? value)
        {
            // ArgumentChecker.NotNull(surfaceValue, "Surface value");
            // ArgumentChecker.NotNull(point, "Point");
            var plus = new Dictionary<Tuple<Double?, Double?, Double?>, Double?>(surfaceValue._data);
            if (surfaceValue._data.ContainsKey(point))
            {
                plus.AddOrUpdate(point, value + surfaceValue._data[point]);
            }
            else
            {
                plus.AddOrUpdate(point, value);
            }
            return new CubeValue(plus);
        }

        /// <summary>
        /// Create a new object containing the point of the initial object with the all Values multiplied by a given factor.
        /// <summary>
        /// <param name="surfaceValue">The surface value.</param>
        /// <param name="factor">The multiplicative factor.</param>
        /// <returns>The multiplied surface.</returns>
        public static CubeValue MultiplyBy(CubeValue surfaceValue, double factor)
        {
            // ArgumentChecker.NotNull(surfaceValue, "Surface value");
            var multiplied = new Dictionary<Tuple<Double?, Double?, Double?>, Double?>();
            foreach (var p in surfaceValue._data.Keys)
            {
                multiplied.AddOrUpdate(p, surfaceValue._data[p] * factor);
            }
            return new CubeValue(multiplied);
        }

        public static Boolean CompareTo(CubeValue value1, CubeValue value2, double tolerance)
        {
            ArgumentChecker.NotNull(value1, "value1");
            ArgumentChecker.NotNull(value2, "value2");
            ArgumentChecker.NotNull(value1._data, "value1._data");
            ArgumentChecker.NotNull(value2._data, "value2._data");
            ArgumentChecker.NoNulls(value1._data, "value1._data");
            ArgumentChecker.NoNulls(value2._data, "value2._data");

            if ((value1 != null) && (value2 != null) && (value1._data != null) && (value2._data != null))
            {
                var set1 = value1._data.Keys.ToHashSet();
                var set2 = value2._data.Keys.ToHashSet();
                if (!set1.Equals(set2))
                {
                    return false;
                }
                foreach (var p in set1)
                {
                    if (value1._data[p].HasValue && value2._data[p].HasValue)
                    {
                        if ((value1._data[p] != null) && (value2._data[p] != null))
                        {
                            var d1 = value1._data[p].Value;
                            var d2 = value2._data[p].Value;
                            if (System.Math.Abs(d1 - d2) > tolerance)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Collapse the object to a single valued The points on which the amounts occur are ignored and the Values summed.
        /// <summary>
        /// <returns>The value.</returns>
        public double ToSingleValue()
        {
            ArgumentChecker.NotNull(_data, "_data");
            ArgumentChecker.NoNulls(_data, "_data");

            if (_data != null)
            {
                double amount = 0;
                foreach (var point in _data.Keys)
                {
                    amount += _data[point].Value;
                }
                return amount;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public override String ToString()
        {
            ArgumentChecker.NotNull(_data, "_data");
            ArgumentChecker.NoNulls(_data, "_data");

            return _data.ToString();
        }
    }
}
