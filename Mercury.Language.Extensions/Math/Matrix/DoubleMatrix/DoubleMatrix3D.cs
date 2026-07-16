using Mercury.Language.Math.Matrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Language.Math.Matrix.DoubleMatrix
{
    /// <summary>
    /// Represents a three-dimensional matrix of double-precision floating-point values, providing operations and
    /// accessors for 3D numerical data.
    /// </summary>
    /// <remarks>Use this class to store and manipulate three-dimensional arrays of double values, such as
    /// those found in scientific computing, image processing, or volumetric data analysis. The matrix supports a
    /// variety of mathematical operations and element-wise access. All sub-arrays in the input data must have
    /// consistent dimensions to ensure correct behavior.</remarks>
    public class DoubleMatrix3D : AbstractMatrix3D<Double>
    {
        private double[][][] _data;
        private Boolean _isIdentity;
        private int _numOfElement;
        private int _rank;
        private int _columnCount;
        private int _rowCount;
        private int _sliceCount;

        /// <summary>
        /// Initializes a new instance of the DoubleMatrix3D class using the specified three-dimensional array of double
        /// values.
        /// </summary>
        /// <param name="data">A three-dimensional array of double values that defines the elements of the matrix. The array must not be
        /// null and should have consistent dimensions for all sub-arrays.</param>
        public DoubleMatrix3D(double[][][] data)
        {
            _data = data;
            _numOfElement = data.Length;
            _isIdentity = false;
            _rowCount = data.Length;
            _columnCount = _data[0].Length;
            _sliceCount = _data[0][0].Length;
        }
        /// <inheritdoc />
        public override double this[int indexX, int indexY, int indexZ]
        {
            get { return _data[indexX][indexY][indexZ]; }
        }

        /// <inheritdoc />
        public override int NumberOfElements => _numOfElement;

        /// <inheritdoc />
        public override double[][][] Data => _data;

        /// <inheritdoc />
        public override bool Identity => _isIdentity;

        /// <inheritdoc />
        public override int Rank
        {
            get { return _data.Rank; }
        }

        /// <inheritdoc />
        public override int ColumnCount => _columnCount;

        /// <inheritdoc />
        public override int RowCount => _rowCount;

        /// <inheritdoc />
        public override int SliceCount => _sliceCount;

        #region Data Operation

        /// <inheritdoc />
        public override void SetValue(int indexX, int indexY, int indexZ, double value)
        {
            _data[indexX][indexY][indexZ] = value;
        }

        /// <inheritdoc />
        public override double GetEntry(params int[] indices)
        {
            if (indices.Length != 3) {
                throw new ArgumentException(LocalizedResources.Instance().INVALID_NUMBER_OF_INDICES_3D);
            }
            return _data[indices[0]][indices[1]][indices[2]];
        }
        #endregion

        #region Calculation Methods

        /// <inheritdoc />
        public override IMatrix3D<double> Add(IMatrix3D<double> value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            if (_rowCount != value.RowCount || _columnCount != value.ColumnCount || _sliceCount != value.SliceCount) {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_DIMENSIONS_MUST_AGREE);
            }

            // Add each element in parallel
            AutoParallel.AutoParallelFor(0, RowCount, (i) =>
            {
                AutoParallel.AutoParallelFor(0, ColumnCount, (j) =>
                {
                    AutoParallel.AutoParallelFor(0, SliceCount, (k) =>
                    {
                        resultData[i][j][k] = _data[i][j][k] + value.Data[i][j][k];
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Devide(double value)
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // devide each element in parallel
            AutoParallel.AutoParallelFor(0, RowCount, (i) =>
            {
                AutoParallel.AutoParallelFor(0, ColumnCount, (j) =>
                {
                    AutoParallel.AutoParallelFor(0, SliceCount, (k) =>
                    {
                        resultData[i][j][k] = _data[i][j][k] / value;
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Devide(IMatrix3D<double> value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            if (_rowCount != value.RowCount || _columnCount != value.ColumnCount || _sliceCount != value.SliceCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_DIMENSIONS_MUST_AGREE);
            }

            // Divide each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                var devideRow = value.Data[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    var devideSlice = devideRow[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = slice[k] / devideSlice[k];
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> DotProduct(double[] b)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IMatrix3D<double> DotProduct(IMatrix3D<double> value)
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Dot product each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                var devideRow = value.Data[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    var devideSlice = devideRow[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = slice[k] * devideSlice[k];
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Modulus(double divisor)
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Modulus each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = slice[k] %= divisor;
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Multiply(double value)
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }


            // multiply each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = slice[k] * value;
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Multiply(IMatrix3D<double> value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            if (_rowCount != value.RowCount || _columnCount != value.ColumnCount || _sliceCount != value.SliceCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_DIMENSIONS_MUST_AGREE);
            }

            // Multiply each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                var multiplyRow = value.Data[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    var multiplySlice = multiplyRow[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = slice[k] * multiplySlice[k];
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Power(double exponent)
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // raise each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Pow(slice[k], exponent);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Remainder(double divisor)
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // get remainder of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = slice[k] % divisor;
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Remeinder(IMatrix3D<double> divisor)
        {
            if (divisor == null) throw new ArgumentNullException(nameof(divisor));

            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            if (_rowCount != divisor.RowCount || _columnCount != divisor.ColumnCount || _sliceCount != divisor.SliceCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_DIMENSIONS_MUST_AGREE);
            }

            // Get remainder of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                var devideRow = divisor.Data[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    var devideSlice = devideRow[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = slice[k] % devideSlice[k];
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Subtract(IMatrix3D<double> value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            if (_rowCount != value.RowCount || _columnCount != value.ColumnCount || _sliceCount != value.SliceCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_DIMENSIONS_MUST_AGREE);
            }

            // Subtract each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                var valueRow = value.Data[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    var valueSlice = valueRow[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = slice[k] - valueSlice[k];
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }
        #endregion


        #region trigonometric functions
        /// <inheritdoc />
        public override IMatrix3D<double> Acos()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get acos of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Acos(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Asin()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get asin of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Asin(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Atan()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get atan of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Atan(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Atan2(IMatrix3D<double> other)
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get atan2 of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                var otherRow = other.Data[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    var otherSlice = otherRow[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Atan2(slice[k], otherSlice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Cos()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get cos of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Cos(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Cosh()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get cosh of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Cosh(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Sin()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get sin of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Sin(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Sinh()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get sinh of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Sinh(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Tan()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get tan of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Tan(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Tanh()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get tanh of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Tanh(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }
        #endregion

        #region Matrix operations

        /// <inheritdoc />
        public override IMatrix3D<double> Abs()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get absolute value of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Abs(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> AbsoluteMaximum()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Compute absolute maximum safely (handle any empty rows)
            double absMax = _data
                .Where(r => r != null && r.Length > 0)
                .Select(r => r.Select(e => e.Select(t => System.Math.Abs(t)).Max()).Max())
                .DefaultIfEmpty(0.0)
                .Max();

            // Fill each row with the absMax value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = absMax;
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> AbsoluteMinimum()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Compute absolute maximum safely (handle any empty rows)
            double absMin = _data
                .Where(r => r != null && r.Length > 0)
                .Select(r => r.Select(e => e.Select(t => System.Math.Abs(t)).Min()).Min())
                .DefaultIfEmpty(0.0)
                .Min();

            // Fill each row with the absMin value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = absMin;
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Ceiling()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get ceiling value of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Ceiling(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override double Condition()
        {
            // Take the first 2D slice of the 3D matrix for condition number calculation
            // Note: This is a simplification. In practice, you might want to compute the condition number for each 2D slice or define it differently for 3D matrices.
            // Convert the first 2D slice to a format compatible with the SVD implementation
            // See: https://www.alglib.net/matrixops/rcond.php
            // See: https://people.math.sc.edu/Burkardt/f_src/condition/condition.html
            var svd = new Cern.Colt.Matrix.LinearAlgebra.SingularValueDecomposition((Cern.Colt.Matrix.Implementation.DenseDoubleMatrix2D)Cern.Colt.Matrix.DoubleFactory2D.Dense.Make(this.Data[0]));
            return svd.Cond();
        }

        /// <inheritdoc />
        public override double Determinant()
        {
            // Take the first 2D slice of the 3D matrix for determinant calculation
            // Note: This is a simplification. In practice, you might want to compute the determinant for each 2D slice or define it differently for 3D matrices.
            // Convert the first 2D slice to a format compatible with the determinant implementation
            return Cern.Colt.Matrix.LinearAlgebra.Algebra.Det((Cern.Colt.Matrix.Implementation.DenseDoubleMatrix2D)Cern.Colt.Matrix.DoubleFactory2D.Dense.Make(this.Data[0]));
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Exp()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get exponential value of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Exp(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Floor()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get floor value of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Floor(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override double Invert()
        {
            // For 3D, use the first 2D slice as the representative slice (consistent with Determinant/Condition).
            // Return the multiplicative inverse of that slice's determinant (i.e. determinant of the inverse).
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return 0.0;
            }

            double det = Determinant();

            if (System.Math.Abs(det) <= double.Epsilon)
            {
                throw new InvalidOperationException(LocalizedResources.Instance().CANNOT_DIVIDE_BY_ZERO);
            }

            return 1.0 / det;
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Log()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get Log value of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Log(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Log10()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get Log10 value of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Log10(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Maximum()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Compute maximum safely (handle any empty rows)
            double max = _data
                .Where(r => r != null && r.Length > 0)
                .Select(r => r.Select(e => e.Max()).Max())
                .DefaultIfEmpty(0.0)
                .Max();

            // Fill each row with the max value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = max;
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Minimum()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Compute minimum safely (handle any empty rows)
            double min = _data
                .Where(r => r != null && r.Length > 0)
                .Select(r => r.Select(e => e.Min()).Min())
                .DefaultIfEmpty(0.0)
                .Min();

            // Fill each row with the min value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = min;
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Negate()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get ceiling value of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = -slice[k];
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override double Norm()
        {
            double sumOfSquares = 0;

            // Get ceiling value of each element in parallel
            AutoParallel.AutoParallelFor(0, _data.Length, (i) =>
            {
                var row = _data[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        sumOfSquares += System.Math.Pow(slice[k], 2);
                    });
                });
            });

            return System.Math.Sqrt(sumOfSquares);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Prepend(IMatrix2D<double> value)
        {
            // Create result jagged array
            double[][][] resultData = ArrayUtility.CreateJaggedArray<double>(RowCount + 1, ColumnCount, SliceCount);

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            if (value.Data.Length != RowCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().MATRIX_DIMENSIONS_MUST_AGREE);
            }

            // Get ceiling value of each element in parallel
            AutoParallel.AutoParallelFor(0, RowCount, (i) => {
                AutoParallel.AutoParallelFor(0, ColumnCount, (j) => {
                    AutoParallel.AutoParallelFor(0, SliceCount, (k) => {
                        resultData[i + 1][j][k] = _data[i][j][k];
                    });
                });
            });

            // Get ceiling value of each element in parallel
            AutoParallel.AutoParallelFor(0, ColumnCount, (j) => {
                AutoParallel.AutoParallelFor(0, SliceCount, (k) => {
                    resultData[0][j][k] = value.Data[j][k];
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Reverse()
        {
            // Create result jagged array
            double[][][] resultData = _data.Copy();
            Array.Reverse(resultData);


            // Get reversed value of each element in parallel
            AutoParallel.AutoParallelFor(0, RowCount, (i) => {
                AutoParallel.AutoParallelFor(0, ColumnCount, (j) => {
                    Array.Reverse(resultData[i][j]);
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Rotate()
        {
            // Rotate around Z axis: identical behaviour to Rotate() (rotate each X-Y slice 90° clockwise)
            return RotateZ();
        }

        /// <inheritdoc />
        public override IMatrix3D<double> RotateX()
        {
            // Rotate each Y-Z plane (for every X) by 90 degrees clockwise.
            // Result dims: RowCount (X) unchanged, ColumnCount' = SliceCount, SliceCount' = ColumnCount
            double[][][] resultData = ArrayUtility.CreateJaggedArray<double>(RowCount, SliceCount, ColumnCount);

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // For each x-plane, perform a 2D rotate on the matrix _data[x][y][z] (y,z)
            AutoParallel.AutoParallelFor(0, RowCount, (x) =>
            {
                var newPlane = resultData[x];   // dimensions: [SliceCount][ColumnCount]
                var oldPlane = _data[x];       // dimensions: [ColumnCount][SliceCount]

                AutoParallel.AutoParallelFor(0, newPlane.Length, (i) =>
                {
                    var newRow = newPlane[i];
                    AutoParallel.AutoParallelFor(0, newRow.Length, (j) =>
                    {
                        // Mapping for 90° clockwise on (y,z) plane:
                        // new[y'] (i) corresponds to old z index; new z' (j) corresponds to old y index.
                        // Using the same mapping pattern as 2D Rotate:
                        // new[i][j] = old[oldRows - 1 - j][i]
                        newRow[j] = oldPlane[ColumnCount - 1 - j][i];
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> RotateX(double radians, bool clockwise = true, double precision = 1E-10)
        {
            // Rotate each Y-Z plane (for every X) by 'radians' about the X axis.
            // Uses inverse mapping + nearest-neighbour sampling.
            double angle = clockwise ? -radians : radians;
            double cos = System.Math.Cos(angle);
            double sin = System.Math.Sin(angle);

            int rx = RowCount;
            int ry = ColumnCount;
            int rz = SliceCount;

            double cy = (ry - 1) / 2.0;
            double cz = (rz - 1) / 2.0;

            double[][][] resultData = ArrayUtility.CreateJaggedArray<double>(rx, ry, rz);

            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // For each x-plane, map destination (y,z) back to source using rotation -angle.
            AutoParallel.AutoParallelFor(0, rx, (x) =>
            {
                var newPlane = resultData[x];
                // iterate over destination rows in Y
                for (int y = 0; y < ry; y++)
                {
                    for (int z = 0; z < rz; z++)
                    {
                        // coords relative to center
                        double dy = y - cy;
                        double dz = z - cz;

                        // inverse rotation (src = R(-angle) * dest). Because we used angle (clockwise?), compute using cos,sin as:
                        // srcY =  cos*dy + sin*dz
                        // srcZ = -sin*dy + cos*dz
                        double srcYf = cos * dy + sin * dz + cy;
                        double srcZf = -sin * dy + cos * dz + cz;

                        int srcY = (int)System.Math.Round(srcYf);
                        int srcZ = (int)System.Math.Round(srcZf);

                        if (srcY >= 0 && srcY < ry && srcZ >= 0 && srcZ < rz)
                        {
                            newPlane[y][z] = _data[x][srcY][srcZ];
                        }
                        else
                        {
                            // out of bounds: leave default 0.0
                            newPlane[y][z] = 0.0;
                        }
                    }
                }
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> RotateY()
        {
            // Rotate each X-Z plane (for every Y) by 90 degrees clockwise.
            // Result dims: RowCount' = SliceCount, ColumnCount (Y) unchanged, SliceCount' = RowCount
            double[][][] resultData = ArrayUtility.CreateJaggedArray<double>(SliceCount, ColumnCount, RowCount);

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // For each y-plane, perform a 2D rotate on the matrix _data[.,y,.] (x,z)
            AutoParallel.AutoParallelFor(0, ColumnCount, (y) =>
            {
                // oldPlane[x][z] = _data[x][y][z]  (dimensions: RowCount x SliceCount)
                // newPlane[i][j] (i -> new X' index 0..SliceCount-1, j -> new Z' index 0..RowCount-1)
                // mapping for 90° clockwise: new[i][j] = old[oldRows - 1 - j][i]
                for (int i = 0; i < SliceCount; i++)
                {
                    var newRow = resultData[i];
                    for (int j = 0; j < RowCount; j++)
                    {
                        newRow[y][j] = _data[RowCount - 1 - j][y][i];
                    }
                }
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> RotateY(double radians, bool clockwise = true, double precision = 1E-10)
        {
            // Rotate each X-Z plane (for every Y) by 'radians' about the Y axis.
            // Uses inverse mapping + nearest-neighbour sampling.
            double angle = clockwise ? -radians : radians;
            double cos = System.Math.Cos(angle);
            double sin = System.Math.Sin(angle);

            int rx = RowCount;
            int ry = ColumnCount;
            int rz = SliceCount;

            double cx = (rx - 1) / 2.0;
            double cz = (rz - 1) / 2.0;

            // result dims: same shape (we preserve original dimensions)
            double[][][] resultData = ArrayUtility.CreateJaggedArray<double>(rx, ry, rz);

            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // For each y-plane, map destination (x,z) back to source using rotation -angle.
            AutoParallel.AutoParallelFor(0, ry, (y) =>
            {
                for (int x = 0; x < rx; x++)
                {
                    for (int z = 0; z < rz; z++)
                    {
                        double dx = x - cx;
                        double dz = z - cz;

                        // inverse rotation about Y: srcX = cos*dx + sin*dz ; srcZ = -sin*dx + cos*dz
                        double srcXf = cos * dx + sin * dz + cx;
                        double srcZf = -sin * dx + cos * dz + cz;

                        int srcX = (int)System.Math.Round(srcXf);
                        int srcZ = (int)System.Math.Round(srcZf);

                        if (srcX >= 0 && srcX < rx && srcZ >= 0 && srcZ < rz)
                        {
                            resultData[x][y][z] = _data[srcX][y][srcZ];
                        }
                        else
                        {
                            resultData[x][y][z] = 0.0;
                        }
                    }
                }
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> RotateZ()
        {
            // Rotate each 2D slice 90 degrees clockwise (same behaviour as DoubleMatrix2D.Rotate)
            // Result dimensions: RowCount' = ColumnCount, ColumnCount' = RowCount, SliceCount unchanged
            double[][][] resultData = ArrayUtility.CreateJaggedArray<double>(ColumnCount, RowCount, SliceCount);

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // For each slice z, perform a 2D rotate on the matrix _data[.,.,z]
            AutoParallel.AutoParallelFor(0, SliceCount, (z) =>
            {
                // For each new row index (which corresponds to old column index)
                AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
                {
                    var newRow = resultData[i];
                    // For each new column index (which corresponds to old row index)
                    AutoParallel.AutoParallelFor(0, newRow.Length, (j) =>
                    {
                        // Mapping for 90° clockwise: new[i][j] = old[oldRows - 1 - j][i]
                        newRow[j][z] = _data[RowCount - 1 - j][i][z];
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> RotateZ(double radians, bool clockwise = true, double precision = 1E-10)
        {
            // Rotate each X-Y plane (for every Z) by 'radians' about the Z axis.
            // Uses inverse mapping + nearest-neighbour sampling.
            double angle = clockwise ? -radians : radians;
            double cos = System.Math.Cos(angle);
            double sin = System.Math.Sin(angle);

            int rx = RowCount;
            int ry = ColumnCount;
            int rz = SliceCount;

            double cx = (rx - 1) / 2.0;
            double cy = (ry - 1) / 2.0;

            double[][][] resultData = ArrayUtility.CreateJaggedArray<double>(rx, ry, rz);

            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // For each slice z, map destination (x,y) back to source using rotation -angle.
            AutoParallel.AutoParallelFor(0, rz, (z) =>
            {
                for (int x = 0; x < rx; x++)
                {
                    for (int y = 0; y < ry; y++)
                    {
                        double dx = x - cx;
                        double dy = y - cy;

                        // inverse rotation: srcX = cos*dx + sin*dy ; srcY = -sin*dx + cos*dy
                        double srcXf = cos * dx + sin * dy + cx;
                        double srcYf = -sin * dx + cos * dy + cy;

                        int srcX = (int)System.Math.Round(srcXf);
                        int srcY = (int)System.Math.Round(srcYf);

                        if (srcX >= 0 && srcX < rx && srcY >= 0 && srcY < ry)
                        {
                            resultData[x][y][z] = _data[srcX][srcY][z];
                        }
                        else
                        {
                            resultData[x][y][z] = 0.0;
                        }
                    }
                }
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Round()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get rounded value of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Round(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Sign()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get sign value of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Sign(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Sqrt()
        {
            // Create result jagged array
            double[][][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            // Get square root value of each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) => {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) => {
                        slice[k] = System.Math.Sqrt(slice[k]);
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }

        /// <inheritdoc />
        public override double Trace()
        {
            // Define trace for 3D as sum of elements _data[i][i][i] up to the minimal dimension
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return 0.0;
            }

            int minDim = System.Math.Min(RowCount, System.Math.Min(ColumnCount, SliceCount));
            double sum = 0.0;
            for (int i = 0; i < minDim; i++)
            {
                sum += _data[i][i][i];
            }
            return sum;
        }

        /// <inheritdoc />
        public override IMatrix3D<double> Transpose()
        {
            // Transpose swaps X and Y axes: result dimensions ColumnCount x RowCount x SliceCount
            double[][][] resultData = ArrayUtility.CreateJaggedArray<double>(ColumnCount, RowCount, SliceCount);

            // If there are no elements, return zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix3D(resultData);
            }

            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    var slice = row[j];
                    AutoParallel.AutoParallelFor(0, slice.Length, (k) =>
                    {
                        // result[i][j][k] = original[j][i][k]
                        slice[k] = _data[j][i][k];
                    });
                });
            });

            return new DoubleMatrix3D(resultData);
        }
        #endregion

        #region Private Helper Methods

        private Double[][][] copyLocalData()
        {
            double[][][] resultData = ArrayUtility.CreateJaggedArray<double>(RowCount, ColumnCount, SliceCount);
            AutoParallel.AutoParallelFor(0, _data.Length, (i) =>
            {
                AutoParallel.AutoParallelFor(0, _data[i].Length, (j) =>
                {
                    AutoParallel.AutoParallelFor(0, _data[i][j].Length, (k) =>
                    {
                        resultData[i][j][k] = _data[i][j][k];
                    });
                });
            });

            return resultData;
        }

        #endregion
    }
}
