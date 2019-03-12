using System;

using Nex.Core;

namespace Nex.Types
{
    public abstract class RangeBase:NObservable
    {
        #region [ Value Property ]
        public static NPropertyMessageDescriptor<IComparable> ValueProperty = new NPropertyMessageDescriptor<IComparable>( "Value");
        IComparable _value;
        public IComparable Value
        {
            get { return _value;}
            set
            {
                var newValue = value;
                if (HasRange)
                {
                    if (_min != null && _value.CompareTo(_min) < 0)
                        newValue = _min;
                    if (_max != null && _value.CompareTo(_max) > 0)
                        newValue = _max;
                }
                _value = newValue;
                NotifyProperty<IComparable>(this, ref _value, value, ValueProperty);
            }
        }
        #endregion
        #region [ Min Property ]
        public static NPropertyMessageDescriptor<IComparable> MinProperty = new NPropertyMessageDescriptor<IComparable>("Min");
        IComparable _min;
        public IComparable Min
        {
            get { return _min; }
            set
            {

                NotifyProperty<IComparable>(this, ref _min, value, MinProperty);

                if (_max != null && _max.CompareTo(_min) < 0)
                    Max = _min;
                if (_value != null && _value.CompareTo(_min) < 0 && HasRange)
                    Value = _min;
            }
        }
        #endregion
        #region [ Max Property ]
        public static NPropertyMessageDescriptor<IComparable> MaxProperty = new NPropertyMessageDescriptor<IComparable>( "Max");
        IComparable _max;
        public IComparable Max
        {
            get { return _max; }
            set
            {
                NotifyProperty<IComparable>(this, ref _max, value, MaxProperty);

                if (_min != null && _min.CompareTo(_max) > 0)
                    Min = _max;
                if (_value != null && _value.CompareTo(_max) > 0 && HasRange)
                    Value = _max;
            }
        }
        #endregion
        #region [ HasRange Property ]
        public static NPropertyMessageDescriptor<bool> HasRangeProperty = new NPropertyMessageDescriptor<bool>("HasRange");
        bool _hasRange;
        public bool HasRange
        {
            get { return _hasRange; }
            set
            {
                NotifyProperty<bool>(this, ref _hasRange, value, HasRangeProperty);
                if (HasRange)
                {
                    if (_value != null && _value.CompareTo(_min) < 0)
                        Value = _min;
                    if (_value != null && _value.CompareTo(_max) > 0)
                        Value = _max;
                }
            }
        }
        #endregion



    }
    public class Range<T> : RangeBase 
        where T : struct, IComparable
    {
        public new T Value
        {
            get { return (T)base.Value; }
            set { base.Value = value; }
        }
        public new T Min
        {
            get { return (T)base.Min; }
            set { base.Min = value; }
        }
        public new T Max
        {
            get { return (T)base.Max; }
            set { base.Max = value; }
        }

        public Range()
        {
            Value = default(T);
            Min = default(T);
            Max = default(T);
        }

        #region[ IClonable ]
        public object Clone()
        {
            var clone = new Range<T>();
            clone.Min = Min;
            clone.Max = Max;
            clone.Value = Value;
            clone.HasRange = HasRange;
            return clone;
        }
        #endregion
    }

           
}
