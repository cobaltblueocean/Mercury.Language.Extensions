using Mercury.Language.Math.Matrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mercury.Language.Math.Matrix.DoubleMatrix
{
    /// <summary>
    /// Represents a one-dimensional matrix of double-precision floating-point values and provides operations for
    /// mathematical and linear algebra computations on vectors.
    /// </summary>
    /// <remarks>Use this class to perform vector-based operations such as element-wise arithmetic,
    /// trigonometric functions, and various norms and projections. DoubleMatrix1D is suitable for numerical and
    /// scientific computing scenarios where efficient manipulation of 1D double arrays is required. Thread safety
    /// depends on the implementation and is not guaranteed unless otherwise specified.</remarks>
    public class DoubleMatrix1D : AbstractMatrix1D<Double>
    {
        private double[] _data;
        private Boolean _isIdentity;
        private int _numOfElement;

        public DoubleMatrix1D(double[] data) : base(data)
        {
            _data = data;
            _numOfElement = data.Length;
            _isIdentity = false;
        }

        /// <inheritdoc />
        public override double this[int index]
        {
            get { return _data[index]; }
        }

        /// <inheritdoc />
        public override double[] Data => _data;

        /// <inheritdoc />
        public override IMatrix1D<double> Abs()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Abs(x)).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> AbsoluteMaximum()
        {
            DoubleMatrix1D result;
            Double _absMax = 0;

            int n = _numOfElement;
            double[] resultData = new double[n];

            _absMax = _data.Select(x => System.Math.Abs(x)).Max();

            resultData.Fill(_absMax);
            result = new DoubleMatrix1D(resultData);
            return result;
        }

        /// <inheritdoc />
        public override IMatrix1D<double> AbsoluteMinimum()
        {
            DoubleMatrix1D result;
            Double _absMin = 0;

            int n = _numOfElement;
            double[] resultData = new double[n];

            _absMin = _data.Select(x => System.Math.Abs(x)).Min();

            resultData.Fill(_absMin);
            result = new DoubleMatrix1D(resultData);
            return result;
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Acos()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Acos(x)).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Add(IMatrix1D<double> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (NumberOfElements != value.NumberOfElements) throw new ArgumentException(LocalizedResources.Instance().LENGTH_DOES_NOT_MATCH);
            return new DoubleMatrix1D(_data.Select((x, i) => x + value[i]).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Asin()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Asin(x)).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix2D<double> AsMatrix2D()
        {
            DoubleMatrix2D result;
            int n = _numOfElement;
            double[][] resultData = ArrayUtility.CreateJaggedArray<double>(n, 1);
            AutoParallel.AutoParallelFor(0, n, (i) =>
            {
                resultData[i][0] = this[i];
            });
            result = new DoubleMatrix2D(resultData);
            return result;
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Atan()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Atan(x)).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Ceiling()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Ceiling(x)).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Cos()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Cos(x)).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Cosh()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Cosh(x)).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Devide(IMatrix1D<double> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (NumberOfElements != value.NumberOfElements) throw new ArgumentException(LocalizedResources.Instance().LENGTH_DOES_NOT_MATCH);
            if (value.Data.Any(x => x == 0))
            {
                // Handle division by zero based on requirements (throw exception, use infinity, etc.)
                throw new DivideByZeroException(LocalizedResources.Instance().CANNOT_DIVIDE_BY_ZERO);
            }
            return new DoubleMatrix1D(_data.Select((x, i) => x / value[i]).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Devide(double value)
        {
            DoubleMatrix1D result;
            double[] resultData = new double[NumberOfElements];
            if (value == 0)
            {
                // Handle division by zero based on requirements (throw exception, use infinity, etc.)
                throw new DivideByZeroException(LocalizedResources.Instance().CANNOT_DIVIDE_BY_ZERO);
            }

            return new DoubleMatrix1D(_data.Select(x => x / value).ToArray());
        }

        /// <inheritdoc />
        public override double DotProduct(IMatrix1D<double> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (NumberOfElements != value.NumberOfElements) throw new ArgumentException(LocalizedResources.Instance().LENGTH_DOES_NOT_MATCH);

            return _data.Select((x, i) => x * value[i]).Sum();
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Exp()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Exp(x)).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Floor()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Floor(x)).ToArray());
        }

        /// <inheritdoc />
        public override double InnerProduct(IMatrix1D<double> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (NumberOfElements != value.NumberOfElements) throw new ArgumentException(LocalizedResources.Instance().LENGTH_DOES_NOT_MATCH);

            return _data.Select((x, i) => x * value[i]).Sum();
        }

        /// <inheritdoc />
        public override double Distance(IMatrix1D<double> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (NumberOfElements != value.NumberOfElements) throw new ArgumentException(LocalizedResources.Instance().LENGTH_DOES_NOT_MATCH);

            return System.Math.Sqrt(_data.Select((x, i) => System.Math.Pow(x - value[i], 2)).Sum());
        }

        /// <inheritdoc />
        public override double L1Distance(IMatrix1D<double> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (NumberOfElements != value.NumberOfElements) throw new ArgumentException(LocalizedResources.Instance().LENGTH_DOES_NOT_MATCH);

            return _data.Select((x, i) => System.Math.Abs(x - value[i])).Sum();
        }

        /// <inheritdoc />
        public override double LInfDistance(IMatrix1D<double> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (NumberOfElements != value.NumberOfElements) throw new ArgumentException(LocalizedResources.Instance().LENGTH_DOES_NOT_MATCH);
            return _data.Select((x, i) => System.Math.Abs(x - value[i])).Max();
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Log()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Log(x)).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Maximum()
        {
            return new DoubleMatrix1D(_data.Max().ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Minimum()
        {
            return new DoubleMatrix1D(_data.Min().ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Multiply(IMatrix1D<double> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (NumberOfElements != value.NumberOfElements) throw new ArgumentException(LocalizedResources.Instance().LENGTH_DOES_NOT_MATCH);

            return new DoubleMatrix1D(_data.Select((x, i) => x * value[i]).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Multiply(double value)
        {
            return new DoubleMatrix1D(_data.Select(x => x * value).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Negate()
        {
            return new DoubleMatrix1D(_data.Select(x => -x).ToArray());
        }

        /// <inheritdoc />
        public override double Norm()
        {
            return Norm2();
        }

        /// <inheritdoc />
        public override double Norm1()
        {
            return _data.Select(x => System.Math.Abs(x)).Sum();
        }

        /// <inheritdoc />
        public override double Norm2()
        {
            return _data.Select(x => x * x).Sum();
        }

        /// <inheritdoc />
        public override double NormInfinity()
        {
            return _data.Select(x => System.Math.Abs(x)).Max();
        }

        /// <inheritdoc />
        public override IMatrix2D<double> OuterProduct(IMatrix1D<double> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (NumberOfElements != value.NumberOfElements) throw new ArgumentException(LocalizedResources.Instance().LENGTH_DOES_NOT_MATCH);
            DoubleMatrix2D result;
            int rLen = NumberOfElements;
            int cLen = value.NumberOfElements;

            // Specify generic type parameter explicitly to resolve CS0411.
            double[][] resultData = ArrayUtility.CreateJaggedArray<double>(rLen, cLen);

            AutoParallel.AutoParallelFor(0, rLen, (i) =>
            {
                AutoParallel.AutoParallelFor(0, cLen, (j) =>
                {
                    // Outer product is element-wise multiplication of pairwise elements.
                    resultData[i][j] = _data[i] * value[j];
                });
            });

            result = new DoubleMatrix2D(resultData);
            return result;
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Power(IMatrix1D<double> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (NumberOfElements != value.NumberOfElements) throw new ArgumentException(LocalizedResources.Instance().LENGTH_DOES_NOT_MATCH);

            return new DoubleMatrix1D(_data.Select((x, i) => System.Math.Pow(x, value[i])).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Projection(IMatrix1D<double> value)
        {
            var a = this.DotProduct(value);
            var b = value.DotProduct(value);

            return value.Multiply(a/b);
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Reverse()
        {
            return new DoubleMatrix1D(_data.Reverse().ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Round()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Round(x)).ToArray());
        }

        /// <inheritdoc />
        public override void SetValue(int index, double value)
        {
            _data[index] = value;
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Sin()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Sin(x)).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Sinh()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Sinh(x)).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Subtract(IMatrix1D<double> value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (NumberOfElements != value.NumberOfElements) throw new ArgumentException(LocalizedResources.Instance().LENGTH_DOES_NOT_MATCH);

            return new DoubleMatrix1D(_data.Select((x, i) => x - value[i]).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Tan()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Tan(x)).ToArray());
        }

        /// <inheritdoc />
        public override IMatrix1D<double> Tanh()
        {
            return new DoubleMatrix1D(_data.Select(x => System.Math.Tanh(x)).ToArray());
        }
    }
}
