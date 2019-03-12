using System;
using System.Collections.Generic;

using Nex.Core;

namespace Nex.Timing.Cron
{
    public abstract class CronExpression:NObservable
    {
        #region [ MESSAGE ]
        public static NPropertyMessageDescriptor<string> CronExpressionStringProperty = new NPropertyMessageDescriptor<string>("CronExpressionString");
        #endregion

        #region [ FIELDS ]
        string _cronExpressionString;
        #endregion

        #region [ PROPERTY ]
        public string CronExpressionString
        {
            get { return _cronExpressionString; }
            set
            {
                var oldValue = _cronExpressionString;
                _cronExpressionString = value;
                OnCronExpressionStringChange(oldValue, value);
                NotifyMessage(new NPropertyMessage<string>(this, CronExpressionStringProperty, oldValue, value));
            }
        }
        #endregion

        #region [ CONSTRUCTOR ]
        public CronExpression(object parent = null)
            :base(parent)
        {
            FromCurrentTime();
        }
        public CronExpression(string cronExpressionString, object parent = null)
            :base(parent)
        { }
        #endregion

        #region [ ABSTRACT METHODS  ]
        public abstract IEnumerable<DateTime> GetDatesInRange(DateTime baseTime, DateTime endTime);
        public abstract void FromCurrentTime();
        public abstract void FromDateTime(DateTime date);
        protected abstract void OnCronExpressionStringChange(string oldValue, string newValue);
        #endregion

        #region [ ToString ]
        public override string ToString()
        {
            return _cronExpressionString;
        }
        #endregion
    }
}
