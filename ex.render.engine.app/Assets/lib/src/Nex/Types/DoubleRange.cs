using Nex.Core;

namespace Nex.Types
{
    public class DoubleRange:NObservable
    {
        #region [ Value Property ]
        public static NPropertyMessageDescriptor<double> ValueProperty = new NPropertyMessageDescriptor<double>("Value");
        double _value;
        public double Value
        {
            get { return _value;}
            set
            {
                var newValue = value;
                if (HasRange)
                {
                    if (_value <_min)
                        newValue = _min;
                    if (_value > _max)
                        newValue = _max;
                }
                _value = newValue;
                NotifyProperty<double>(this, ref _value, value, ValueProperty);
            }
        }
        #endregion
        #region [ Min Property ]
        public static NPropertyMessageDescriptor<double> MinProperty = new NPropertyMessageDescriptor<double>("Min");
        double _min;
        public double Min
        {
            get { return _min; }
            set
            {
                
                NotifyProperty<double>(this, ref _min, value, MinProperty);

                if (_max < _min)
                    Max = _min;
                if (_value < _min && HasRange)
                    Value = _min;
            }
        }
        #endregion
        #region [ Max Property ]
        public static NPropertyMessageDescriptor<double> MaxProperty = new NPropertyMessageDescriptor<double>("Max");
        double _max;
        public double Max
        {
            get { return _max; }
            set
            {
                NotifyProperty<double>(this, ref _max, value, MaxProperty);

                if (_min > _max)
                    Min = _max;
                if (_value > _max && HasRange)
                    Value = _max;
            }
        }
        #endregion
        #region [ HasRange Property ]
        public static NPropertyMessageDescriptor<bool> HasRangeProperty = new NPropertyMessageDescriptor<bool>( "HasRange");
        bool _hasRange;
        public bool HasRange
        {
            get { return _hasRange; }
            set
            {
                NotifyProperty<bool>(this, ref _hasRange, value, HasRangeProperty);
                if (HasRange)
                {
                    if (_value < _min)
                        Value = _min;
                    if (_value > _max)
                        Value = _max;
                }
            }
        }
        #endregion
    }
   
}
