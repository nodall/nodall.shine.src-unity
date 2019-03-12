using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace nexcode.nwcore
{
    public class Quad2D : Vector2DCollection
    {
        #region [ ENUM ]
        public enum QuadVertex
        {
            LeftTop = 0,
            rightTop = 1,
            leftBottom = 2,
            rightBottom = 3
        }
        #endregion

        #region [ CONSTRUCTOR ]
        public Quad2D(Rectangle2D rectangle = new Rectangle2D())
            : base(4, rectangle)
        {
        }
        public Quad2D(Quad2D quad)
            : base(quad.Length, quad.Rectangle)
        {
            CopyFrom(quad);
        }
        public Quad2D(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Rectangle2D rectangle = new Rectangle2D())
            : base(4, rectangle)
        {
            this[0] = p0;
            this[1] = p1;
            this[2] = p2;
            this[3] = p3;
        }
        #endregion

        #region [ Abstracts ]
        public override void Reset()
        {
            this[0] = Rectangle.LeftTop;
            this[1] = Rectangle.RightTop;
            this[2] = Rectangle.LeftBottom;
            this[3] = Rectangle.RightBottom;
        }
        public override void Reset(int index)
        {
            switch (index)
            {
                case 0: this[0] = Rectangle.LeftTop; break;
                case 1: this[1] = Rectangle.RightTop; break;
                case 2: this[2] = Rectangle.LeftBottom; break;
                case 3: this[3] = Rectangle.RightBottom; break;
            }
        }
        #endregion

        #region [ Indexer ]
        [IndexerName("Indexer")]
        public Vector2 this[QuadVertex vertex]
        {
            get { return this[(int)vertex]; }
            set
            {
                var oldValue = this[(int)vertex];
                this[(int)vertex] = value;
            }
        }
        #endregion

        #region [ Parse Method ]
        static public Quad2D Parse(string value)
        {
            string[] strList = value.Split('|');
            if (strList.Length != 4)
                throw new FormatException("Format must be 'p[0] | ... | p[3]' in value=" + value);

            var quad = new Quad2D(new Rectangle2D());
            for (int i = 0; i < 4; i++)
                quad[i] = Vector2Utils.Parse(strList[i]);

            return quad;
        }
        #endregion
    }
}
