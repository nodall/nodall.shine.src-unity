using System;
using System.Collections.Generic;
using System.Linq;
using Nex.Core;

namespace Nex.Timing.Cron
{
    public class CronLine : NBaseLine<ICron>
    {
        #region [ MESSAGE ]
        public static NPropertyMessageDescriptor<TimeSpan> RunIntervalProperty = new NPropertyMessageDescriptor<TimeSpan>("RunInterval");
        #endregion

        #region [ FIELDS ]
        private TimeSpan _runInterval = TimeSpan.FromMinutes(2);
        private bool _isDirty = false;
        private DateTime _runMinDate = DateTime.MaxValue;
        private DateTime _runMaxDate = DateTime.MinValue;
        private TimeSpan _maxDuration = TimeSpan.Zero;

        private NDateContext[] _runBufferItems;
        #endregion

        #region [ PROPERTIES ]
        public TimeSpan RunInterval
        {
            get { return _runInterval; }
            set
            {
                var oldValue = _runInterval;
                _runInterval = value; 
                _isDirty = true;
                NotifyMessage(new NPropertyMessage<TimeSpan>(this, RunIntervalProperty, oldValue, value));
            }
        }
        
        #endregion

        #region [ CONSTRUCTOR ]
        public CronLine(object parent = null)
            : base(parent)
        {
        }
        #endregion

        #region [ STATIC METHODS ]
        public static NDateContext[] Expand(ICron item, DateTime start, DateTime end)
        {
            List<NDateContext> list = new List<NDateContext>();
            if (item is ITimeInterval)
                start = start - (item as ITimeInterval).TimeInterval.Add(TimeSpan.FromSeconds(1));

            IEnumerable<DateTime> dates = item.CronExpression.GetDatesInRange(start, end);

            foreach (var date in dates)
                list.Add(new NDateContext(date, item));
  
            return list.ToArray();
        }
        public static NDateContext[] ExpandOnlyOnePerDay(ICron item, DateTime start, DateTime end)
        {
            List<NDateContext> list = new List<NDateContext>();
            if (item is ITimeInterval)
                start = start - (item as ITimeInterval).TimeInterval.Add(TimeSpan.FromSeconds(1));

            int days = (end - start).Days;
            DateTime startDate = start;
            DateTime endDate = startDate;
            DateTime lastDate = start;
            while (startDate < end)
            {
                endDate = startDate.AddDays(1);

                var values = item.CronExpression.GetDatesInRange(startDate, endDate);
                if (values.Count() > 0)
                {
                    lastDate = values.First();
                    list.Add(new NDateContext(lastDate, item));
                }

                startDate = endDate;
            }
            if (lastDate.DayOfWeek != endDate.DayOfWeek)
            {
                var values = item.CronExpression.GetDatesInRange(endDate, end);
                if (values.Count() > 0)
                    list.Add(new NDateContext(values.First(), item));
            }


            return list.ToArray();
        }
        #endregion
     
        #region [MESSAGE_RECEIVED  ]
        void Message_Received(object sender, NMessage e)
        {
            if (e.Descriptor == CronExpression.CronExpressionStringProperty)
                _isDirty = true;            
        }
        #endregion

        #region [ PUBLIC METHODS ]
        public NDateContext[] Expand(DateTime start, DateTime end)
        {
            List<NDateContext> list = new List<NDateContext>();
            foreach (var item in this)
                list.AddRange(Expand(item, start, end));
            list.Sort((a, b) => a.Date.CompareTo(b.Date));
            return list.ToArray();
        }
        public NDateContext[] ExpandOnlyOnePerDay( DateTime start, DateTime end)
        {
            List<NDateContext> list = new List<NDateContext>();
            foreach (var item in this)
                list.AddRange(ExpandOnlyOnePerDay(item, start, end));
            list.Sort((a, b) => a.Date.CompareTo(b.Date));
            return list.ToArray();
        }
        public NDateContext GetFirstItemBeforeDate(DateTime date)
        {
            TimeSpan maxInterval = TimeSpan.FromDays(60);
            TimeSpan interval = TimeSpan.FromHours(5);
            DateTime currentDate = date;

            NDateContext foundItem = null;

            while (foundItem == null && currentDate > date - maxInterval )
            {
                currentDate = currentDate - interval;
                var list = Expand(currentDate, date);
                if (list.Count() > 0)
                    foundItem = list.Last();
            }

            return foundItem;

        }
        #endregion

        #region [ BaseLine Methods ]
        public override void Run()
        {
            if (Clock != null)
            {
                Clock.UpdateTime();
                DateTime a = Clock.DateTimePlayerAtLastUpdate;
                DateTime b = Clock.DateTimePlayer;

                if (_isDirty || _runBufferItems == null || _runMinDate > b || _runMaxDate < b)
                {
                    _runMinDate = b - RunInterval;
                    _runMaxDate = b + RunInterval;

                    _runBufferItems = Expand(_runMinDate, _runMaxDate);
                    _isDirty = false;
                }
                
                foreach (var dateObj in _runBufferItems)
                {
                    var date = dateObj.Date;
                    var value = dateObj.Value;
                    if (b < date)
                        continue;
                    else if (a < date && date <= b)
                    {
                        NotifyMessage(new NMessage(this, NMessages.DateEnterMessage, dateObj, a, b));
                        if (value is NObservable)
                            (value as NObservable).NotifyMessage(new NMessage(value, NMessages.DateEnterMessage, value, a, b));
                    }
                    else if (value is ITimeInterval)
                    {
                        ITimeInterval durationItem = value as ITimeInterval;
                        DateTime startDate = date;
                        DateTime endDate = date + durationItem.TimeInterval;

                        if (startDate< a && b < endDate)
                        {
                            NotifyMessage(new NMessage(this, NMessages.DateOverMessage, dateObj, a, b));
                            if (value is NObservable)
                                (value as NObservable).NotifyMessage(new NMessage(value, NMessages.DateOverMessage, value, a, b));
                        }
                        else if (a < endDate && endDate < b)
                        {
                            NotifyMessage(new NMessage(this, NMessages.DateLeaveMessage, dateObj, a, b));
                            if (value is NObservable)
                                (value as NObservable).NotifyMessage(new NMessage(value, NMessages.DateLeaveMessage, value, a, b));
                        }
                    }
                }
            }
        }
        protected override void OnItemAdded(ICron item)
        {
            _isDirty = true;
            item.CronExpression.AddEventHandler(CronExpression.CronExpressionStringProperty, Message_Received);
        }
        protected override void OnItemRemoved(ICron item)
        {
            _isDirty = true;
            item.CronExpression.RemoveEventHandler(CronExpression.CronExpressionStringProperty, Message_Received);
        }
        #endregion
    }
} 
