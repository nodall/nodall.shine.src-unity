using System;
using System.Runtime.CompilerServices;

using Nex.Core;

namespace Nex.Math
{
    public class NQuad2D : NVector2DCollection
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
        public NQuad2D(NRectangle2D rectangle = new NRectangle2D())
            : base(4, rectangle)
        {
        }
        public NQuad2D(object parent, NRectangle2D rectangle)
            : base(parent, 4, rectangle)
        {
        }
        public NQuad2D(NQuad2D quad)
            : base(quad.Length, quad.Rectangle)
        {
            CopyFrom(quad);
        }
        public NQuad2D(object parent, NQuad2D quad)
            : base(parent, quad.Length, quad.Rectangle)
        {
            CopyFrom(quad);
        }
        public NQuad2D(object parent, NVector2D p0, NVector2D p1, NVector2D p2, NVector2D p3, NRectangle2D rectangle = new NRectangle2D())
            : base(parent, 4, rectangle)
        {
            this[0] = p0;
            this[1] = p1;
            this[2] = p2;
            this[3] = p3;
        }
        public NQuad2D(NVector2D p0, NVector2D p1, NVector2D p2, NVector2D p3, NRectangle2D rectangle = new NRectangle2D())
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
        public NVector2D this[QuadVertex vertex]
        {
            get { return this[(int)vertex]; }
            set
            {
                var oldValue = this[(int)vertex];
                this[(int)vertex] = value;
                NotifyMessage(new NPropertyMessage<NVector2D>(this, IndexerProperty, oldValue, value, new object[] { (int)vertex }));
            }
        }
        #endregion

        #region [ Parse Method ]
        static public NQuad2D Parse(string value)
        {
            string[] strList = value.Split('|');
            if (strList.Length != 4)
                throw new FormatException("Format must be 'p[0] | ... | p[3]' in value=" + value);

            var quad = new NQuad2D(new NRectangle2D());
            for (int i = 0; i < 4; i++)
                quad[i] = NVector2D.Parse(strList[i]);

            return quad;
        }
        #endregion
    }
}
