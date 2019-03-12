using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace nexcode.nwcore
{
    public struct Rectangle2D
    {
        #region [ FIELDS ]
        public float left;
        public float top;
        public float right;
        public float bottom;
        #endregion

        #region [ CONSTRUCTOR ]
        public Rectangle2D(float left = 0, float top = 0, float right = 0, float bottom = 0)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
        #endregion

        #region [ PUBLIC METHODS ]
        public void SetFromXYWH(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            left = x;
            top = y;
            right = x + width;
            bottom = y + height;
        }
        public void SetFromLTRB(float left = 0, float top = 0, float right = 0, float bottom = 0)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
        #endregion

        #region [ PROPERTIES ]
        [JsonIgnore]
        public bool IsZero { get { return left == 0 && top == 0 && right == 0 && bottom == 0; } }
        [JsonIgnore]
        public float Height { get { return top - bottom; } set { bottom = top + value; } }
        [JsonIgnore]
        public float Width { get { return right - left; } set { right = left + value; } }
        [JsonIgnore]
        public float X
        {
            get { return left; }
            set
            {
                right += value - left;
                left = value;
            }
        }
        [JsonIgnore]
        public float Y
        {
            get { return top; }
            set
            {
                bottom += value - top;
                top = value;
            }
        }
        [JsonIgnore]
        public Vector2 LeftTop { get { return new Vector2(left, top); } }
        [JsonIgnore]
        public Vector2 LeftBottom { get { return new Vector2(left, bottom); } }
        [JsonIgnore]
        public Vector2 RightTop { get { return new Vector2(right, top); } }
        [JsonIgnore]
        public Vector2 RightBottom { get { return new Vector2(right, bottom); } }
        [JsonIgnore]
        public Vector2 Center
        {
            get
            {
                float x = (right - left) / 2f;
                if (right < left)
                    x = -x;

                float y = (top - bottom) / 2f;
                if (top < bottom)
                    y = -y;

                return new Vector2(x, y);
            }
        }

        #endregion

        #region [ ToString METHODS ]
        public override string ToString()
        {
            return String.Format("{0};{1};{2};{3}",
                left.ToString(CultureInfo.InvariantCulture),
                top.ToString(CultureInfo.InvariantCulture),
                right.ToString(CultureInfo.InvariantCulture),
                bottom.ToString(CultureInfo.InvariantCulture));
        }
        #endregion

        static public Rectangle2D Parse(string value)
        {
            string[] strList = value.Split(';');
            if (strList.Length != 4)
                throw new FormatException("Format must be 'left ; top ; right ; bottom' in value=" + value);
            float l, t, r, b;
            if (float.TryParse(strList[0], NumberStyles.Any, CultureInfo.InvariantCulture, out l) &&
                float.TryParse(strList[1], NumberStyles.Any, CultureInfo.InvariantCulture, out t) &&
                float.TryParse(strList[2], NumberStyles.Any, CultureInfo.InvariantCulture, out r) &&
                float.TryParse(strList[3], NumberStyles.Any, CultureInfo.InvariantCulture, out b))
                return new Rectangle2D(l, t, r, b);
            else
                throw new FormatException("Error parsing double in value=" + value);
        }
        public static Rectangle2D FromXYWH(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            Rectangle2D rect = new Rectangle2D();
            rect.SetFromXYWH(x, y, width, height);
            return rect;
        }
        public static Rectangle2D FromLTRB(float left = 0, float top = 0, float right = 0, float bottom = 0)
        {
            return new Rectangle2D(left, top, right, bottom);
        }
    }
}
