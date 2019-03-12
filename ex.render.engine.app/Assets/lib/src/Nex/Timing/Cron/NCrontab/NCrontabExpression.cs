using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Nex.Core;
using Nex.Timing.Cron.NCrontab.Lib;

namespace Nex.Timing.Cron.NCrontab
{
    public class NCrontabExpression : CronExpression
    {
        #region [ FIELDS ]
        CrontabSchedule _schedule;
        #endregion

        #region [CONSTRUCTOR ]
        public NCrontabExpression(object parent)
            : base(parent)
        {
        }
        public NCrontabExpression(string expression, object parent)
            : base(expression, parent)
        {
        }
        #endregion

        #region [ ABSTRACT IMPLEMENTATION ]
        public override void FromCurrentTime()
        {
            DateTime date = DateTime.Now;
            String str = String.Format("{0} {1} {2} {3} {4}", date.Minute, date.Hour, date.Day, date.Month, "*");
            CronExpressionString = str;
        }
        public override void FromDateTime(DateTime date)
        {
            String str = String.Format("{0} {1} {2} {3} {4}", date.Minute, date.Hour, date.Day, date.Month, "*");
            CronExpressionString = str;
        }
        protected override void OnCronExpressionStringChange(string oldValue, string newValue)
        {
            _schedule = CrontabSchedule.Parse(newValue);
        }
        public override IEnumerable<DateTime> GetDatesInRange(DateTime baseTime, DateTime endTime)
        {
            return _schedule.GetNextOccurrences(baseTime, endTime);
        }
        #endregion

        #region [ PARSE ]
        public static NCrontabExpression Parse(string value)
        {
            return new NCrontabExpression(value, null);
        }
        #endregion
    }
}
