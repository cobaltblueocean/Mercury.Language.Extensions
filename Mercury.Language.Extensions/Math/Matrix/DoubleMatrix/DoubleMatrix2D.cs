/// <summary>
/// COPYRIGHT
/// See the COPYRIGHT.txt file
/// </summary>
/// <see cref="COPYRIGHT.txt"/>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercury.Language.Math.Matrix;
using Mercury.Language.Utility;

namespace Mercury.Language.Math.Matrix.DoubleMatrix
{
    /// <summary>
    /// Represents a two-dimensional matrix of double-precision floating-point values, providing operations and
    /// accessors for 2D numerical data.
    /// </summary>
    /// <remarks>Use this class to store and manipulate two-dimensional arrays of double values, such as
    /// those found in scientific computing, image processing, or data analysis. The matrix supports a
    /// variety of mathematical operations and element-wise access. All sub-arrays in the input data must have
    /// consistent dimensions to ensure correct behavior.</remarks>
    public class DoubleMatrix2D : AbstractMatrix2D<Double>
    {
        private double[][] _data;
        private Boolean _isIdentity;
        private int _numOfElement;
        private int _columnCount;
        private int _rowCount;

        public override double this[int indexX, int indexY] => _data[indexX][indexY];

        /// <inheritdoc />
        public override double[][] Data => _data;

        /// <inheritdoc />
        public override bool Identity => _isIdentity;

        /// <inheritdoc />
        public override int NumberOfElements => _numOfElement;

        public override int ColumnCount => _columnCount;

        public override int RowCount => _rowCount;

        /// <summary>
        /// Initializes a new instance of the DoubleMatrix2D class using the specified two-dimensional array of double
        /// values.
        /// </summary>
        /// <param name="data">A jagged array of double values representing the elements of the matrix. Each inner array corresponds to a
        /// row in the matrix. Cannot be null.</param>
        public DoubleMatrix2D(Double[][] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            _data = data;
            _isIdentity = IsIdentity();
            _numOfElement = _data.Sum(x => x.Length);
            _columnCount = _data.Length > 0 ? _data[0].Length : 0;
            _rowCount = _data.Length;

            if (_data.Any(x => x.Length != _columnCount))
            {
                throw new ArgumentException(LocalizedResources.Instance().ALL_ROWS_MUST_HAVE_THE_SAME_NUMBER_OF_COLUMNS);
            }
        }

        #region Data Operation

        /// <inheritdoc />
        public override double GetEntry(params int[] indices)
        {
            if (indices == null || indices.Length != 2)
            {
                throw new ArgumentException(LocalizedResources.Instance().INVALID_NUMBER_OF_INDICES_2D);
            }

            int indexX = indices[0];
            int indexY = indices[1];

            return _data[indexX][indexY];
        }

        /// <inheritdoc />
        public override void SetValue(int indexX, int indexY, double value)
        {
            _data[indexX][indexY] = value;
        }
        #endregion

        #region Calculation Methods

        /// <inheritdoc />
        public override IMatrix2D<double> Add(IMatrix2D<double> value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Add each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                var addRow = value.Data[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = row[j] + addRow[j];
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Devide(IMatrix2D<double> value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Add each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                var devideRow = value.Data[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    if (devideRow[j] == 0)
                    {
                        throw new DivideByZeroException(LocalizedResources.Instance().CANNOT_DIVIDE_BY_ZERO_AT_ELEMENT_2D);
                    }
                    row[j] = row[j] / devideRow[j];
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Modulus(double divisor)
        {
            if (divisor == 0) throw new DivideByZeroException(LocalizedResources.Instance().CANNOT_DIVIDE_BY_ZERO);

            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Add each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];

                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = row[j] %= divisor;
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Multiply(IMatrix2D<double> value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Add each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                AutoParallel.AutoParallelFor(0, resultData[i].Length, (j) =>
                {
                    resultData[i][j] = 0;
                    AutoParallel.AutoParallelFor(0, resultData[i].Length, (k) =>
                    {
                        resultData[i][j] += _data[i][k] * value.Data[k][j];
                    });
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Power(double exponent)
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Add each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Pow(row[j], exponent);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Remainder(double divisor)
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Add each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    if (divisor == 0)
                    {
                        throw new DivideByZeroException(LocalizedResources.Instance().CANNOT_DIVIDE_BY_ZERO_AT_ELEMENT_2D);
                    }
                    row[j] = row[j] % divisor;
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Remeinder(IMatrix2D<double> divisor)
        {
            if (_numOfElement != divisor.NumberOfElements || RowCount != divisor.RowCount || ColumnCount != divisor.ColumnCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().LENGTH_DOES_NOT_MATCH);
            }

            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Add each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                var divRow = divisor.Data[i];

                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    if (divRow[j] == 0)
                    {
                        throw new DivideByZeroException(LocalizedResources.Instance().CANNOT_DIVIDE_BY_ZERO_AT_ELEMENT_2D);
                    }
                    row[j] = row[j] % divRow[j];
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Subtract(IMatrix2D<double> value)
        {
            if (_numOfElement != value.NumberOfElements || RowCount != value.RowCount || ColumnCount != value.ColumnCount)
            {
                throw new ArgumentException(LocalizedResources.Instance().LENGTH_DOES_NOT_MATCH);
            }

            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Add each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                var subRow = value.Data[i];

                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = row[j] - subRow[j];
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Multiply(double value)
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Add each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];

                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = row[j] * value;
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> DotProduct(double[] value)
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Add each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];

                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = row[j] * value[j];
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> DotProduct(IMatrix2D<double> value)
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Add each element in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                var vRow = value.Data[i];

                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = row[j] * vRow[j];
                });
            });

            return new DoubleMatrix2D(resultData);
        }
        #endregion

        #region trigonometric functions
        /// <inheritdoc />
        public override IMatrix2D<double> Acos()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the acos value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Acos(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Asin()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the asin value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Asin(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Atan()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the asin value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Atan(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Atan2(IMatrix2D<double> other)
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the asin value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Atan2(row[j], other[i, j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Cos()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the asin value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Cos(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Cosh()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the asin value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Cosh(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Sin()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the asin value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Sin(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Sinh()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the sinh value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Sinh(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Tan()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the tan value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Tan(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Tanh()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the tanh value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Tanh(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }
        #endregion

        #region Matrix operations

        /// <inheritdoc />
        public override IMatrix2D<double> Abs()
        {
            ArgumentChecker.NotNull(_data);

            double[][] result = new double[_data.Length][];

            AutoParallel.AutoParallelFor(0, _data.Length, (i) =>
            {
                result[i] = new double[_data[i].Length];

                AutoParallel.AutoParallelFor(0, _data[i].Length, (j) =>
                {
                    result[i][j] = System.Math.Abs(_data[i][j]);
                });
            });

            return new DoubleMatrix2D(result);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> AbsoluteMaximum()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Compute absolute maximum safely (handle any empty rows)
            double absMax = _data
                .Where(r => r != null && r.Length > 0)
                .Select(r => r.Select(e => System.Math.Abs(e)).Max())
                .DefaultIfEmpty(0.0)
                .Max();

            // Fill each row with the absMax value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = absMax;
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> AbsoluteMinimum()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Compute absolute minimum safely (handle any empty rows)
            double absMin = _data
                .Where(r => r != null && r.Length > 0)
                .Select(r => r.Select(e => System.Math.Abs(e)).Min())
                .DefaultIfEmpty(0.0)
                .Min();

            // Fill each row with the absMin value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = absMin;
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix3D<double> AsMatrix3D()
        {
            DoubleMatrix3D result;
            int n = _numOfElement;
            double[][][] resultData = ArrayUtility.CreateJaggedArray<double>(RowCount, ColumnCount, 1);
            

            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var x = resultData[i];
                AutoParallel.AutoParallelFor(0, x.Length, (j) =>
                {
                    x[j][0] = this[i, j];
                });
            });
            result = new DoubleMatrix3D(resultData);
            return result;
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Ceiling()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the ceiling value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = double.Ceiling(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override double Condition()
        {
            var svd = new Cern.Colt.Matrix.LinearAlgebra.SingularValueDecomposition((Cern.Colt.Matrix.Implementation.DenseDoubleMatrix2D)Cern.Colt.Matrix.DoubleFactory2D.Dense.Make(this.Data));
            return svd.Cond();
        }

        /// <inheritdoc />
        public override double Determinant()
        {
            return Cern.Colt.Matrix.LinearAlgebra.Algebra.Det((Cern.Colt.Matrix.Implementation.DenseDoubleMatrix2D)Cern.Colt.Matrix.DoubleFactory2D.Dense.Make(this.Data));
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Floor()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the floor value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = double.Floor(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Invert()
        {
            var coltMatrix = (Cern.Colt.Matrix.Implementation.DenseDoubleMatrix2D)Cern.Colt.Matrix.DoubleFactory2D.Dense.Make(this.Data);
            var invertedColtMatrix = Cern.Colt.Matrix.LinearAlgebra.Algebra.Inverse(coltMatrix);
            double[][] resultData = copyLocalData();
            

            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = invertedColtMatrix[i, j];
                });
            });
            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Log()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the log value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Log(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Log10()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the log10 value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Log10(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Maximum()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Compute maximum safely (handle any empty rows)
            double max = _data
                .Where(r => r != null && r.Length > 0)
                .Select(r => r.Max())
                .DefaultIfEmpty(0.0)
                .Max();

            // Fill each row with the max value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = max;
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Minimum()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Compute minimum safely (handle any empty rows)
            double min = _data
                .Where(r => r != null && r.Length > 0)
                .Select(r => r.Min())
                .DefaultIfEmpty(0.0)
                .Min();

            // Fill each row with the min value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = min;
                });
            });

            return new DoubleMatrix2D(resultData);

        }

        /// <inheritdoc />
        public override IMatrix2D<double> Negate()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the negated value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = -_data[i][j];
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override double FrobeniusNorm()
        {
            var norm = 0.0;
            AutoParallel.AutoParallelFor(0, _data.Length, (i) =>
            {
                var row = _data[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    norm += row[j] * row[j];
                });
            });
            return System.Math.Sqrt(norm);
        }

        /// <inheritdoc />
        public override double L1Norm()
        {
            var norm = 0.0;
            AutoParallel.AutoParallelFor(0, _data.Length, (i) =>
            {
                var row = _data[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    norm += System.Math.Abs(row[j]);
                });
            });
            return System.Math.Sqrt(norm);
        }

        /// <inheritdoc />
        public override double InfinityNorm()
        {
            var norm = 0.0;
            AutoParallel.AutoParallelFor(0, _data.Length, (i) =>
            {
                var row = _data[i];
                var rowSum = 0.0;
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    rowSum += System.Math.Abs(row[j]);
                });
                norm = System.Math.Max(norm, rowSum);
            });
            return norm;
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Reverse()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }
            // Fill each row with the reversed value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = _data[_data.Length - 1 - i][_data[i].Length - 1 - j];
                });
            });
            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Rotate()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }
            // Fill each row with the rotated value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = _data[_data.Length - 1 - j][i];
                });
            });
            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Rotate(double radians, bool clockwise = true, double precision = 1E-10)
        {
            // Rotate the matrix by an arbitrary angle (radians) about its center.
            // Uses inverse mapping with nearest-neighbor sampling.
            double angle = clockwise ? -radians : radians;
            double cos = System.Math.Cos(angle);
            double sin = System.Math.Sin(angle);

            int rows = RowCount;
            int cols = ColumnCount;

            // center coordinates
            double cx = (rows - 1) / 2.0;
            double cy = (cols - 1) / 2.0;

            double[][] resultData = ArrayUtility.CreateJaggedArray<double>(rows, cols);

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            AutoParallel.AutoParallelFor(0, rows, (i) =>
            {
                var destRow = resultData[i];
                for (int j = 0; j < cols; j++)
                {
                    // destination coordinates relative to center
                    double dx = i - cx;
                    double dy = j - cy;

                    // inverse rotation: map destination back to source coordinates
                    // src = R(-angle) * dest  => using cos(angle) and sin(angle) where angle chosen above
                    double srcXf = cos * dx + sin * dy + cx;
                    double srcYf = -sin * dx + cos * dy + cy;

                    int srcX = (int)System.Math.Round(srcXf);
                    int srcY = (int)System.Math.Round(srcYf);

                    if (srcX >= 0 && srcX < rows && srcY >= 0 && srcY < cols)
                    {
                        destRow[j] = _data[srcX][srcY];
                    }
                    else
                    {
                        // Outside original bounds -> keep default 0.0
                        destRow[j] = 0.0;
                    }
                }
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Round()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();
            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }
            // Fill each row with the rounded value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Round(row[j]);
                });
            });
            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Sign()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the asin value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Sign(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Sqrt()
        {
            // Create result jagged array
            double[][] resultData = copyLocalData();

            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }

            // Fill each row with the sqrt value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = System.Math.Sqrt(row[j]);
                });
            });

            return new DoubleMatrix2D(resultData);
        }

        /// <inheritdoc />
        public override double Trace()
        {
            // If there are no elements, return 0
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return 0;
            }

            double trace = 0;
            int minDimension = System.Math.Min(RowCount, ColumnCount);
            for (int i = 0; i < minDimension; i++)
            {
                trace += _data[i][i];
            }
            return trace;
        }

        /// <inheritdoc />
        public override IMatrix2D<double> Transpose()
        {
            // Create result jagged array
            double[][] resultData = ArrayUtility.CreateJaggedArray<double>(ColumnCount, RowCount);
            // If there are no elements, return the zero-sized / zero-filled matrix
            if (_numOfElement == 0 || _data == null || _data.Length == 0)
            {
                return new DoubleMatrix2D(resultData);
            }
            // Fill each row with the transposed value in parallel
            AutoParallel.AutoParallelFor(0, resultData.Length, (i) =>
            {
                var row = resultData[i];
                AutoParallel.AutoParallelFor(0, row.Length, (j) =>
                {
                    row[j] = _data[j][i];
                });
            });
            return new DoubleMatrix2D(resultData);
        }
        #endregion

        #region Private Methods

        private Boolean IsIdentity()
        {
            Boolean result = true;

            AutoParallel.AutoParallelFor(0, _data.Length, (i) =>
            {
                AutoParallel.AutoParallelFor(0, _data[i].Length, (j) =>
                {
                    if (i == j)
                    {
                        if (_data[i][j] != 1)
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        if (_data[i][j] != 0)
                        {
                            result = false;
                        }
                    }
                });
            });

            return result;
        }

        private Double[][] copyLocalData()
        {
            double[][] resultData = ArrayUtility.CreateJaggedArray<double>(RowCount, ColumnCount);
            AutoParallel.AutoParallelFor(0, _data.Length, (i) =>
            {
                AutoParallel.AutoParallelFor(0, _data[i].Length, (j) =>
                {
                    resultData[i][j] = _data[i][j];
                });
            });

            return resultData;
        }
        #endregion
    }
}
