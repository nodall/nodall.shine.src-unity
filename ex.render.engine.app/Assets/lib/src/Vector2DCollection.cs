using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;

namespace nexcode.nwcore
{
    [Serializable]
    public abstract class Vector2DCollection /*: IEnumerable<Vector2>*/
    {
        #region [ FIELDS ]
        public Vector2[] vectors;
        public Rectangle2D rectangle;
        bool _isChangeEnabled = false;
        #endregion

        #region [ PROPERTIES ]
        [JsonIgnore]
        public int Length { get { return vectors.Length; } }
        [JsonIgnore]
        public Rectangle2D Rectangle { get { return rectangle; } }
        [JsonIgnore]
        public Vector2[] Vectors { get { return vectors; } }
        #endregion

        #region [ CONSTRUCTOR ]
        public Vector2DCollection(int size, Rectangle2D rectangle)
        {
            this.rectangle = rectangle;
            vectors = new Vector2[size];
            Reset();
        }
        #endregion

        #region [ INDEXER ]
        [IndexerName("Indexer")]
        public virtual Vector2 this[int index]
        {
            get
            {
                if (index < 0 || Length <= index)
                    throw new IndexOutOfRangeException(String.Format("index must be in range [0..{0}]", (Length - 1)));
                return vectors[index];
            }
            set
            {
                if (index < 0 || Length <= index)
                    throw new IndexOutOfRangeException(String.Format("index must be in range [0..{0}]", (Length - 1)));

                var oldValue = vectors[index];
                vectors[index] = value;
            }
        }
        #endregion

        #region [ Abstract Methods ]
        public abstract void Reset();
        public abstract void Reset(int index);
        #endregion

        #region [ PUBLIC METHODS ]
        public void Rotate(float degrees)
        {
            Vector2 center = rectangle.Center;

            for (int i = 0; i < 16; i++)
            {
                Vector2 point = this[i];
                float px = Mathf.Cos(Mathf.Deg2Rad * degrees) * (point.x - center.x) -
                            Mathf.Sin(Mathf.Deg2Rad * degrees) * (point.y - center.y) +
                            center.x;

                float py = Mathf.Sin(Mathf.Deg2Rad * degrees) * (point.x - center.x) -
                            Mathf.Cos(Mathf.Deg2Rad * degrees) * (point.y - center.y) +
                            center.y;
                this[i] = new Vector2(px, py);
            }
        }
        public void FlipH()
        {
            Vector2 center = rectangle.Center;

            for (int i = 0; i < 16; i++)
            {
                float newX = -(this[i].x - center.x) + center.x;
                this[i] = new Vector2(newX, this[i].y);
            }
        }
        public void FlipV()
        {
            Vector2 center = rectangle.Center;

            for (int i = 0; i < 16; i++)
            {
                float newY = -(this[i].y - center.y) + center.y;
                this[i] = new Vector2(this[i].x, newY);
            }
        }
        public void Offset(Vector2 offset)
        {
            for (int i = 0; i < 16; i++)
                this[i] = vectors[i] + offset;
        }
        public void CopyFrom(Vector2DCollection collection)
        {
            CopyFrom(collection.Vectors);
            rectangle = collection.rectangle;
        }
        public void CopyFrom(Vector2[] vectors)
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
        public IEnumerator<Vector2> GetEnumerator()
        {
            return new List<Vector2>(vectors).GetEnumerator();
        }

        /*
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _vectors.GetEnumerator();
        }*/
        #endregion
    }
}
