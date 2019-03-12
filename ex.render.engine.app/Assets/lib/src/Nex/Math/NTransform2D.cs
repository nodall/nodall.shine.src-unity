using System;
using System.Globalization;
using Nex.Core;

namespace Nex.Math
{
    public class NTransform2D: NObservable
    {
        #region [ CONST ]
        public static NPropertyMessageDescriptor<NVector2D> TranslationProperty = new NPropertyMessageDescriptor<NVector2D>("Translation");
        public static NPropertyMessageDescriptor<NVector2D> ScaleProperty = new NPropertyMessageDescriptor<NVector2D>( "Scale");
        public static NPropertyMessageDescriptor<double> RotationProperty = new NPropertyMessageDescriptor<double>("Rotation");
        #endregion

        #region [ FIELDS ]
        private NVector2D _translation;
        private NVector2D _scale;
        private double _rotation;
        #endregion

        #region [ PROPERTIES ]
        public NVector2D Translation
        {
            get { return _translation; }
            set
            {
                var oldValue = _translation;
                _translation = value;
                NotifyMessage(new NPropertyMessage<NVector2D>(this, TranslationProperty, oldValue, value));
            }
        }
        public NVector2D Scale
        {
            get { return _scale; }
            set
            {
                var oldValue = _scale;
                _scale = value;
                NotifyMessage(new NPropertyMessage<NVector2D>(this, ScaleProperty, oldValue, value));
            }
        }
        public double Rotation
        {
            get { return _rotation; }
            set
            {
                var oldValue = _rotation;
                _rotation = value;
                NotifyMessage(new NPropertyMessage<double>(this, RotationProperty, oldValue, value));
            }
        }
        #endregion

        #region [ CONSTRUCTOR ]
        public NTransform2D(object parent)
            : base(parent)
        {
            Inicialize();
        }
        public NTransform2D(object parent, NVector2D translation, NVector2D scale, double rotation)
            :base(parent)
        {
            Inicialize(translation, scale, rotation);
        }
        public NTransform2D(object parent, NTransform2D transform)
            :base(parent)
        {
            Inicialize(transform);
        }
        public NTransform2D()
        {
            Inicialize();
        }
        public NTransform2D(NVector2D translation, NVector2D scale, double rotation)
        {
            Inicialize(translation, scale, rotation);
        }
        public NTransform2D(NTransform2D transform)
        {
            Inicialize(transform);
        }
        void Inicialize()
        {
            _translation = new NVector2D();
            _scale = new NVector2D(1, 1);
            _rotation = 0;
        }
        void Inicialize(NVector2D translation, NVector2D scale, double rotation)
        {
            _translation = translation;
            _scale = scale;
            _rotation = rotation;
        }
        void Inicialize(NTransform2D transform)
        {
            _translation = transform.Translation;
            _scale = transform.Scale;
            _rotation = transform.Rotation;
        }
        #endregion

        #region [ PUBLIC METHODS ]
        public void CopyFrom(NTransform2D transform)
        {
            _translation = transform.Translation;
            _scale = transform.Scale;
            _rotation = transform.Rotation;
        }
        #endregion

        #region [ ToString METHODS ]
        public override string ToString()
        {
            return String.Format("{0}|{1}|{2}", 
                _translation.ToString(), 
                _scale.ToString(), 
                Rotation.ToString(CultureInfo.InvariantCulture));
        }
        #endregion

        #region [ STATIC METHODS ]
        static public NTransform2D Parse(string value)
        {
            string[] strList = value.Split('|');
            if (strList.Length != 3)
                throw new FormatException("Format must be 'trans | scale | rot' in value=" + value);

            NTransform2D trans = new NTransform2D(
                NVector2D.Parse(strList[0]),
                NVector2D.Parse(strList[1]),
                Double.Parse(strList[2], NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture));

            return trans;
        }
        #endregion
    }
}
