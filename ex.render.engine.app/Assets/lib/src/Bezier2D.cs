using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nexcode.nwcore
{
    public enum Bezier2DMode
    {
        Free,
        Quad,
        QuadPerspectiveCorrection,
        Undefined
    }

    [Serializable]
    public class Bezier2D : Vector2DCollection
    {
        public static int[] CornerQuadIndexes = new int[] { 0, 3, 12, 15 };


        #region [ FIELDS ]
        public Bezier2DMode mode = Bezier2DMode.Free;
        bool IsCornerIndex(int index) { return index == 0 || index == 3 || index == 15 || index == 12; }
        #endregion

        #region [ PROPERTIES ]
        [JsonIgnore]
        public Quad2D CornerQuad {
            get { return new Quad2D(this[0], this[3], this[12], this[15], Rectangle); }
            set
            {
                this[0] = value[0];
                this[3] = value[1];
                this[12] = value[2];
                this[15] = value[3];
            }
        }
        #endregion


        #region [ CONSTRUCTOR ]
        public Bezier2D()
            : base(16, new Rectangle2D())
        {
        }

        public Bezier2D(Bezier2D bezier)
            : base(16, bezier.Rectangle)
        {
            CopyFrom(bezier);
        }

        public Bezier2D(Rectangle2D rectangle)
            : base(16, rectangle)
        {
        }
        #endregion

        #region [ Abstract Methods ]
        public override void Reset()
        {
            var tmpMode = mode;
            mode = Bezier2DMode.Free;
            float dx = (Rectangle.right - Rectangle.left) / 3;
            float dy = (Rectangle.top - Rectangle.bottom) / 3;

            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                    this[x, y] = new Vector2(Rectangle.left + x * dx, Rectangle.top - dy * y);

            mode = tmpMode;

        }
        public override void Reset(int index)
        {
            if (mode == Bezier2DMode.Quad && !IsCornerIndex(index) ||
                mode == Bezier2DMode.QuadPerspectiveCorrection && !IsCornerIndex(index))
                return;

            float dx = (Rectangle.right - Rectangle.left) / 3;
            float dy = (Rectangle.top - Rectangle.bottom) / 3;

            int x = index % 4;
            int y = index / 4;

            this[x, y] = new Vector2(Rectangle.left + x * dx, Rectangle.top - dy * y);
        }
        #endregion

        #region [ COMPUTE POINT ]
        struct BezierCurve
        {
            Vector2 _p0, _p1, _p2, _p3;
            public BezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
            {
                _p0 = p0; _p1 = p1; _p2 = p2; _p3 = p3;
            }

            public Vector2 Compute(float t)
            {
                var c = (_p1 - _p0) * 3.0f;
                var b = ((_p2 - _p1) * 3.0f) - c;
                var a = _p3 - _p0 - c - b;

                float t2 = (t * t);

                return ((a * (t2 * t)) + (b * t2) + (c * t) + _p0);
            }
        };

        public Vector2 Compute(float tu, float tv)
        {
            var curves = new BezierCurve[] {
                new BezierCurve(this[0], this[1], this[2], this[3] ),
                new BezierCurve(this[4], this[5], this[6], this[7] ),
                new BezierCurve(this[8], this[9], this[10], this[11] ),
                new BezierCurve(this[12], this[13], this[14], this[15] )
            };

            Vector2[] p = new Vector2[4];

            for (int i = 0; i < 4; ++i)
            {
                p[i] = curves[i].Compute(tu);
            }

            var vCurve = new BezierCurve(p[0], p[1], p[2], p[3]);

            return vCurve.Compute(tv);
        }
        #endregion

        #region [ INDEX OPERATOR METHODS ]
        public Vector2 this[int x, int y]
        {
            get
            {
                if ((x < 0 && 3 < x) || (y < 0 && 3 < y))
                    throw new IndexOutOfRangeException("indexes must be in range [0..3]");

                int index = (y * 4) + x;

                return this[index];
            }
            set
            {
                if ((x < 0 && 3 < x) || (y < 0 && 3 < y))
                    throw new IndexOutOfRangeException("indexes must be in range [0..3]");

                int index = (y * 4) + x;

                var oldValue = this[index];
                this[index] = value;
            }
        }

        public override Vector2 this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                if (mode == Bezier2DMode.QuadPerspectiveCorrection)
                {
                    if (IsCornerIndex(index))
                    {
                        base[index] = value;
                        float dx = (Rectangle.right - Rectangle.left) / 3;
                        float dy = (Rectangle.top - Rectangle.bottom) / 3;
                        Quad2D quad = new Quad2D(this[0], this[3], this[15], this[12]);
                        for (int i = 0; i < Length; i++)
                        {
                            if (i != 0 && i != 3 && i != 15 && i != 12)
                            {
                                int x = i % 4;
                                int y = i / 4;
                                var originalValue = new Vector2(Rectangle.left + x * dx, Rectangle.top - dy * y);
                                base[i] = MathUtils.PerspectiveCorrection(quad, originalValue);
                            }
                        }
                    }
                }
                else if (mode == Bezier2DMode.Quad)
                {
                    if (IsCornerIndex(index))
                    {
                        base[index] = value;

                        base[1] = base[0] + (1.0f / 3.0f) * (base[3] - base[0]);
                        base[2] = base[0] + (2.0f / 3.0f) * (base[3] - base[0]);

                        base[13] = base[12] + (1.0f / 3.0f) * (base[15] - base[12]);
                        base[14] = base[12] + (2.0f / 3.0f) * (base[15] - base[12]);

                        base[4] = base[0] + (1.0f / 3.0f) * (base[12] - base[0]);
                        base[8] = base[0] + (2.0f / 3.0f) * (base[12] - base[0]);

                        base[07] = base[3] + (1.0f / 3.0f) * (base[15] - base[3]);
                        base[11] = base[3] + (2.0f / 3.0f) * (base[15] - base[3]);

                        base[5] = base[4] + (1.0f / 3.0f) * (base[7] - base[4]);
                        base[6] = base[4] + (2.0f / 3.0f) * (base[7] - base[4]);

                        base[09] = base[8] + (1.0f / 3.0f) * (base[11] - base[8]);
                        base[10] = base[8] + (2.0f / 3.0f) * (base[11] - base[8]);
                    }
                }
                else
                {
                    base[index] = value;
                }
            }
        }
        #endregion

        static public Bezier2D Parse(string value)
        {
            string[] strList = value.Split('|');
            if (strList.Length != 17)
                throw new FormatException("Format must be 'p[0] | ... | p[15] | Rectangle' " + value);

            var rectangle = Rectangle2D.Parse(strList[16]);
            Bezier2D bezier = new Bezier2D(rectangle);
            for (int index = 0; index < 16; index++)
                bezier[index] = Vector2Utils.Parse(strList[index]);

            return bezier;
        }
        public override string ToString()
        {
            return base.ToString() + "|" + Rectangle.ToString(); ;
        }
        public override bool Equals(object obj)
        {
            if (obj is Bezier2D)
            {
                for (int i = 0; i < 16; i++)
                    if (this[i] != (obj as Bezier2D)[i])
                        return false;

                return true;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void ResetToQuad(float x, float y, float width, float height)
        {
            this.mode = Bezier2DMode.Quad;
            this.CornerQuad = new Quad2D(new Vector2(x, y), new Vector2(x + width, y), new Vector2(x, y + height), new Vector2(x + width, y + height));

            this[1] = this[0] + (1.0f / 3.0f) * (this[3] - this[0]);
            this[2] = this[0] + (2.0f / 3.0f) * (this[3] - this[0]);

            this[13] = this[12] + (1.0f / 3.0f) * (this[15] - this[12]);
            this[14] = this[12] + (2.0f / 3.0f) * (this[15] - this[12]);

            this[4] = this[0] + (1.0f / 3.0f) * (this[12] - this[0]);
            this[8] = this[0] + (2.0f / 3.0f) * (this[12] - this[0]);

            this[07] = this[3] + (1.0f / 3.0f) * (this[15] - this[3]);
            this[11] = this[3] + (2.0f / 3.0f) * (this[15] - this[3]);

            this[5] = this[4] + (1.0f / 3.0f) * (this[7] - this[4]);
            this[6] = this[4] + (2.0f / 3.0f) * (this[7] - this[4]);

            this[09] = this[8] + (1.0f / 3.0f) * (this[11] - this[8]);
            this[10] = this[8] + (2.0f / 3.0f) * (this[11] - this[8]);

        }

    }
}