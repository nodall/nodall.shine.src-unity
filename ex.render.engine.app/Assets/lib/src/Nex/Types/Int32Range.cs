
using Nex.Core;

namespace Nex.Types
{
    public class Int32Range:NObservable
    {
        #region [ Value Property ]
        public static NPropertyMessageDescriptor<int> ValueProperty = new NPropertyMessageDescriptor<int>( "Value");
        int _value;
        public int Value
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
                NotifyProperty<int>(this, ref _value, value, ValueProperty);
            }
        }
        #endregion
        #region [ Min Property ]
        public static NPropertyMessageDescriptor<int> MinProperty = new NPropertyMessageDescriptor<int>("Min");
        int _min;
        public int Min
        {
            get { return _min; }
            set
            {
                
                NotifyProperty<int>(this, ref _min, value, MinProperty);

                if (_max < _min)
                    Max = _min;
                if (_value < _min && HasRange)
                    Value = _min;
            }
        }
        #endregion
        #region [ Max Property ]
        public static NPropertyMessageDescriptor<int> MaxProperty = new NPropertyMessageDescriptor<int>("Max");
        int _max;
        public int Max
        {
            get { return _max; }
            set
            {
                NotifyProperty<int>(this, ref _max, value, MaxProperty);

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
