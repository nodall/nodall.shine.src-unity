using System;
using System.Globalization;

namespace Nex.Math
{
    [Serializable]
    public struct NVector2D
    {
        #region [ FIELDS ]
        private double _x;
        private double _y;
        #endregion

        #region [ PROPERTIES ]
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        #endregion

        #region [ CONSTRUCTOR ]
        public NVector2D(double x = 0, double y = 0)
        {
            _x = x;
            _y = y;
        }
        public NVector2D(NVector2D vector)
        {
            _x = vector.X;
            _y = vector.Y;
        }
        #endregion

        #region [ METHODS ]
        public NVector2D Normalize()
        {
            return new NVector2D(_x / Length, _y / Length);
        }
        public double SquareLength { get { return _x * _x + _y * _y; } }
        public double Length { get { return (float)System.Math.Sqrt(SquareLength); } }
        #endregion

        #region [ OVERRIDE METHOD ]
        public override string ToString()
        {
            return String.Format("{0};{1}", 
                _x.ToString("#.000000000", System.Globalization.CultureInfo.InvariantCulture),
                _y.ToString("#.000000000", System.Globalization.CultureInfo.InvariantCulture));
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return _x == ((NVector2D)obj).X && _y == ((NVector2D)obj).Y;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region [ VECTOR OPERATIONS ]
        public static NVector2D operator +(NVector2D a, NVector2D b)
        {
            return new NVector2D(a._x + b._x, a._y + b._y);
        }
        public static NVector2D operator -(NVector2D a, NVector2D b)
        {
            return new NVector2D(a._x - b._x, a._y - b._y);
        }
        public static double operator *(NVector2D a, NVector2D b)
        {
            return (a._x * b._x + a._y * b._y);
        }
        public static NVector2D operator -(NVector2D a)
        {
            return new NVector2D(-a._x, -a._y);
        }
        public static NVector2D operator +(NVector2D a)
        {
            return new NVector2D(+a._x, +a._y);
        }
        #endregion

        #region [ VECTOR COMPARE OPETARIONS ]
        public static bool operator ==(NVector2D a, NVector2D b)
        {
            return (a._x == b._x && a._y == b._y);
        }
        public static bool operator !=(NVector2D a, NVector2D b)
        {
            return (a._x != b._x || a._y != b._y);
        }
        #endregion

        #region [ SCALAR OPERATIONS ]
        public static NVector2D operator +(NVector2D a, double num)
        {
            return new NVector2D(a._x + num, a._y + num);
        }
        public static NVector2D operator +(double num, NVector2D a)
        {
            return new NVector2D(a._x + num, a._y + num);
        }
        public static NVector2D operator -(NVector2D a, double num)
        {
            return new NVector2D(a._x - num, a._y - num);
        }
        public static NVector2D operator -(double num, NVector2D a)
        {
            return new NVector2D(a._x - num, a._y - num);
        }
        public static NVector2D operator *(NVector2D a, double num)
        {
            return new NVector2D(a._x * num, a._y * num);
        }
        public static NVector2D operator *(double num, NVector2D a)
        {
            return new NVector2D(a._x * num, a._y * num);
        }
        public static NVector2D operator /(NVector2D a, double num)
        {
            return new NVector2D(a._x / num, a._y / num);
        }
        #endregion

        #region [ OTHER OPERATIONS ]
        public static double GetAngle(NVector2D a, NVector2D b)
        {
            return System.Math.Acos((a * b) / (a.Length * b.Length));
        }
        #endregion

        #region [ STATIC METHODS ]
        static public NVector2D Parse(string value)
        {
            string[] strList = value.Split(';');
            if (strList.Length != 2)
                throw new FormatException("Format must be x ; y in value=" + value);
            Double x, y;
            if (Double.TryParse(strList[0], NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out x) &&
                Double.TryParse(strList[1], NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out y))
                return new NVector2D(x, y);
            else
                throw new FormatException("Error parsing double in value=" + value);
        }
        #endregion
    }
}
