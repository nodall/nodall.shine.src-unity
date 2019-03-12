using System;
using System.Globalization;

namespace Nex.Math
{
    public struct NVector3D
    {
        #region [ FIELDS ]
        private double _x;
        private double _y;
        private double _z;
        #endregion

        #region [ PROPERTIES ]
        public  double X { get { return _x; } set { _x = value; } }
        public  double Y { get { return _y; } set { _y = value; } }
        public  double Z { get { return _z; } set { _z = value; } }
        #endregion

        #region [ CONSTRUCTOR ]
        public NVector3D(double x = 0, double y = 0, double z = 0)
        {
            _x = x;
            _y = y;
            _z = z;
        }
        public NVector3D(NVector3D vector)
        {
            _x = vector.X;
            _y = vector.Y ;
            _z = vector.Z; 
        }
        #endregion

        #region [ METHODS ]
        public NVector3D Normalize()
        {
            return new NVector3D(_x / Length, _y / Length, _z / Length);
        }
        public double SquareLength{get { return _x * _x + _y * _y + _z * _z;}}
        public double Length { get { return (double)System.Math.Sqrt(SquareLength); } }
        #endregion

        #region [ OVERRIDE METHODS ]
        public override string ToString()
        {
            return String.Format("{0};{1};{2}", 
                _x.ToString(CultureInfo.InvariantCulture), 
                _y.ToString(CultureInfo.InvariantCulture), 
                _z.ToString(CultureInfo.InvariantCulture));
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            return _x == ((NVector3D)obj).X && _y == ((NVector3D)obj).Y && _z == ((NVector3D)obj).Z;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region [ VECTOR OPERATIONS ]
        public static NVector3D operator +(NVector3D a, NVector3D b)
        {
            return new NVector3D(a._x + b._x, a._y + b._y, a._z + b._z);
        }
        public static NVector3D operator -(NVector3D a, NVector3D b)
        {
            return new NVector3D(a._x - b._x, a._y - b._y, a._z - b._z);
        }
        public static double operator *(NVector3D a, NVector3D b)
        {
            return (a._x * b._x + a._y * b._y + a._z + b._z);
        }
        public static NVector3D operator -(NVector3D a)
        {
            return new NVector3D(-a._x, -a._y, -a._z);
        }
        public static NVector3D operator +(NVector3D a)
        {
            return new NVector3D(+a._x, +a._y, +a._z);
        }
        #endregion

        #region [ SCALAR OPERATIONS ]
        public static NVector3D operator +(NVector3D a, double num)
        {
            return new NVector3D(a._x + num, a._y + num, a._z + num);
        }
        public static NVector3D operator +(double num, NVector3D a)
        {
            return new NVector3D(a._x + num, a._y + num, a._z + num);
        }
        public static NVector3D operator -(NVector3D a, double num)
        {
            return new NVector3D(a._x - num, a._y - num, a._z - num);
        }
        public static NVector3D operator -(double num, NVector3D a)
        {
            return new NVector3D(a._x - num, a._y - num, a._z - num);
        }
        public static NVector3D operator *(NVector3D a, double num)
        {
            return new NVector3D(a._x * num, a._y * num, a._z * num);
        }
        public static NVector3D operator *(double num, NVector3D a)
        {
            return new NVector3D(a._x * num, a._y * num, a._z * num);
        }
        public static NVector3D operator /(NVector3D a, double num)
        {
            return new NVector3D(a._x / num, a._y / num, a._z / num);
        }
        #endregion

        #region [ VECTOR COMPARE OPETARIONS ]
        public static bool operator ==(NVector3D a, NVector3D b)
        {
            return (a._x == b._x && a._y == b._y && a._z == b._z);
        }
        public static bool operator !=(NVector3D a, NVector3D b)
        {
            return (a._x != b._x || a._y != b._y || a._z != b._z);
        }
        #endregion

        #region [ OTHER OPERATIONS ]
        public static double GetAngle(NVector3D a, NVector3D b)
        {
            return (double)System.Math.Acos((a * b) / ( a.Length * b.Length));
        }
        public static NVector3D CrossProduct(NVector3D a, NVector3D b)
        {
            return new NVector3D((a._y * b._z) - (b._y * a._z),
                               (a._z * b._x) - (b._z * a._x),
                               (a._x * b._y) - (b._x * a._y));
        }
        #endregion

        #region [ STATIC METHODS ]
        static public NVector3D Parse(string value)
        {
            string[] strList = value.Split(';');
            if (strList.Length != 3)
                throw new FormatException("Format must be x ; y ; z in value=" + value);
            double x, y, z;
            if (Double.TryParse(strList[0], NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out x) &&
                Double.TryParse(strList[1], NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out y) &&
                Double.TryParse(strList[2], NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out z))
                return new NVector3D(x, y, z);
            else
                throw new FormatException("Error parsing double in value=" + value);
        }
        #endregion
    }
}
