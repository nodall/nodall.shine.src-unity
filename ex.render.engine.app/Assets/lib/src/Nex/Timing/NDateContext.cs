using System;

namespace Nex.Timing
{
    public class NDateContext: IDate
    {
        #region [ FIELDS ]
        DateTime _date;
        object _value;
        #endregion

        #region [ PROPERTIES ]
        public DateTime Date { get { return _date; } set { _date = value; } }
        public object Value { get { return _value; } }
        #endregion

        #region [ CONSTRUCTOR ]
        public NDateContext(DateTime date, object value)
        {
            _date = date;
            _value = value;
        }
        #endregion
    }
}
