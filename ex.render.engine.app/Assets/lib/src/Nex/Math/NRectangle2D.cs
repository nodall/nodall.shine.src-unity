using System;
using System.Globalization;


namespace Nex.Math
{
    public struct NRectangle2D
    {
        #region [ FIELDS ]
        private double _left;
        private double _top;
        private double _right;
        private double _bottom;
        #endregion

        #region [ CONSTRUCTOR ]
        public NRectangle2D(double left = 0, double top = 0, double right = 0, double bottom = 0)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }
        #endregion

        #region [ PUBLIC METHODS ]
        public void SetFromXYWH(double x = 0, double y = 0, double width = 0, double height = 0 )
        {
            _left = x;
            _top = y;
            _right = x + width;
            _bottom = y + height;
        }
        public void SetFromLTRB(double left = 0, double top = 0, double right = 0, double bottom = 0)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }
        #endregion

        #region [ PROPERTIES ]
        public bool IsZero { get { return Left == 0 && Top == 0 && Right == 0 && Bottom == 0; } }
        public double Height { get { return _top -  _bottom; } set { _bottom = _top + value; } }
        public double Width { get { return _right - _left; } set { _right = _left + value; } }
        public double X 
        { 
            get {return _left; } 
            set 
            {
                _right += value - _left;
                _left = value ;
            } 
        }
        public double Y
        {
            get { return _top; }
            set
            {
                _bottom += value - _top;
                _top = value;
            }
        }
        public double Left { get { return _left; } set { _left = value; } }
        public double Top { get { return _top; } set { _top = value; } }
        public double Bottom { get { return _bottom; } set { _bottom = value; } }
        public double Right { get { return _right; } set { _right = value; } }
        public NVector2D LeftTop { get { return new NVector2D(Left, Top); } }
        public NVector2D LeftBottom { get { return new NVector2D(Left, Bottom); } }
        public NVector2D RightTop { get { return new NVector2D(Right, Top); } }
        public NVector2D RightBottom { get { return new NVector2D(Right, Bottom); } }
        public NVector2D Center
        {
            get
            {
                double x = (Right - Left) / 2.0;
                if (Right < Left)
                    x = -x;

                double y = (Top - Bottom) / 2.0;
                if (Top < Bottom)
                    y = -y;

                return new NVector2D(x, y);
            }
        }
        
        #endregion

        #region [ ToString METHODS ]
        public override string ToString()
        {
            return String.Format("{0};{1};{2};{3}", 
                _left.ToString(CultureInfo.InvariantCulture), 
                _top.ToString(CultureInfo.InvariantCulture), 
                _right.ToString(CultureInfo.InvariantCulture), 
                _bottom.ToString(CultureInfo.InvariantCulture));
        }
        #endregion

        static public NRectangle2D Parse(string value)
        {
            string[] strList = value.Split(';');
            if (strList.Length != 4)
                throw new FormatException("Format must be 'left ; top ; right ; bottom' in value=" + value);
            Double l, t, r, b;
            if (Double.TryParse(strList[0], NumberStyles.Any, CultureInfo.InvariantCulture, out l) &&
                Double.TryParse(strList[1], NumberStyles.Any, CultureInfo.InvariantCulture, out t) &&
                Double.TryParse(strList[2], NumberStyles.Any, CultureInfo.InvariantCulture, out r) &&
                Double.TryParse(strList[3], NumberStyles.Any, CultureInfo.InvariantCulture, out b))
                return new NRectangle2D(l, t,r,b);
            else
                throw new FormatException("Error parsing double in value=" + value);
        }
        public static NRectangle2D FromXYWH(double x = 0, double y = 0, double width = 0, double height = 0 )
        {
            NRectangle2D rect = new NRectangle2D();
            rect.SetFromXYWH(x, y, width, height);
            return rect;
        }
        public static NRectangle2D FromLTRB(double left = 0, double top = 0, double right = 0, double bottom = 0)
        {
            return new NRectangle2D(left, top, right, bottom);
        }
    }
}
