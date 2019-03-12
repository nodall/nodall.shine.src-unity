using System;
using System.Runtime.CompilerServices;

using Nex.Core;
using System.Collections.Generic;
using System.Collections;

namespace Nex.Math
{
    [Serializable]
    public abstract class NVector2DCollection:NObservable, IEnumerable<NVector2D>
    {
        #region [ MESSAGE ] 
        static public NPropertyMessageDescriptor<NVector2D> IndexerProperty = new NPropertyMessageDescriptor<NVector2D>("Indexer", new Type[] { typeof(int) });
        static public NMessageDescriptor BeginChangeMessage = new NMessageDescriptor();
        static public NMessageDescriptor EndChangeMessage = new NMessageDescriptor();
        #endregion

        #region [ FIELDS ]
        NVector2D[] _vectors;
        NRectangle2D _rectangle;
        bool _isChangeEnabled = false;
        #endregion

        #region [ PROPERTIES ]
        public int Length { get { return _vectors.Length; } }
        public NRectangle2D Rectangle { get { return _rectangle;}}
        public NVector2D[] Vectors { get { return _vectors; } }
        #endregion

        #region [ CONSTRUCTOR ]
        public NVector2DCollection(int size, NRectangle2D rectangle)
        {
            _rectangle = rectangle;
            _vectors = new NVector2D[size];
            Reset();
        }
        public NVector2DCollection(object parent, int size, NRectangle2D rectangle)
            :base(parent)
        {
            _rectangle = rectangle;
            _vectors = new NVector2D[size];
            Reset();
        }
        #endregion

        #region [ public METHODS ]
        public void BeginChange()
        {
            _isChangeEnabled = true;
            NotifyMessage(new NMessage(this, BeginChangeMessage));
        }
        public void EndChange()
        {
            _isChangeEnabled = false;
            NotifyMessage(new NMessage(this, EndChangeMessage));
        }

        #endregion

        #region [ INDEXER ]
        [IndexerName("Indexer")]
        public virtual NVector2D this[int index]
        {
            get
            {
                if (index < 0 || Length <= index)
                    throw new IndexOutOfRangeException(String.Format("index must be in range [0..{0}]", (Length - 1)));
                return _vectors[index];
            }
            set
            {
                if (index < 0 || Length <= index)
                    throw new IndexOutOfRangeException(String.Format("index must be in range [0..{0}]", (Length - 1)));

                var oldValue = _vectors[index];
                _vectors[index] = value;
                if (!_isChangeEnabled)
                    NotifyMessage(new NPropertyMessage<NVector2D>(this, IndexerProperty, oldValue, value, new object[] { index }));
            }
        }
        #endregion

        #region [ Abstract Methods ]
        public abstract void Reset();
        public abstract void Reset(int index);
        #endregion

        #region [ PUBLIC METHODS ]
        public void Rotate(double degrees)
        {
            NVector2D center = _rectangle.Center;

            for (int i = 0; i < 16; i++)
            {
                NVector2D point = this[i];
                double px = System.Math.Cos(Math.NMath.DegreeToRadian(degrees)) * (point.X - center.X) -
                            System.Math.Sin(Math.NMath.DegreeToRadian(degrees)) * (point.Y - center.Y) +
                            center.X;

                double py = System.Math.Sin(Math.NMath.DegreeToRadian(degrees)) * (point.X - center.X) -
                            System.Math.Cos(Math.NMath.DegreeToRadian(degrees)) * (point.Y - center.Y) +
                            center.Y;
                this[i] = new NVector2D(px, py);
            }
        }
        public void FlipH()
        {
            NVector2D center = _rectangle.Center;

            for (int i = 0; i < 16; i++)
            {
                double newX = -(this[i].X - center.X) + center.X;
                this[i] = new NVector2D(newX, this[i].Y);
            }
        }
        public void FlipV()
        {
            NVector2D center = _rectangle.Center;

            for (int i = 0; i < 16; i++)
            {
                double newY = -(this[i].Y - center.Y) + center.Y;
                this[i] = new NVector2D(this[i].X, newY);
            }
        }
        public void Offset(NVector2D offset)
        {
            for (int i = 0; i < 16; i++)
                this[i] = _vectors[i] + offset;
        }
        public void CopyFrom(NVector2DCollection collection)
        {
            CopyFrom(collection.Vectors);
            _rectangle = collection._rectangle;
        }
        public void CopyFrom(NVector2D[] vectors)
        {
            if (vectors.Length != Length)
                throw new Exception("Collections must have the same Length");

            for (int i = 0; i < Length; i++)
                this[i] = vectors[i];
        }
        #endregion

        #region [ ToString METHODS ]
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < Length; i++)
                str += this[i].ToString() + "|";

            if (str.EndsWith("|"))
                str = str.Remove(str.Length - 1, 1);

            return str;
        }

        #endregion

        #region [ IEnumerable ]
        public IEnumerator<NVector2D> GetEnumerator()
        {
            return new List<NVector2D>(_vectors).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _vectors.GetEnumerator();
        }
        #endregion
    }
}
