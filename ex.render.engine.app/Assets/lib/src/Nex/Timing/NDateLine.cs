using System;
using System.Linq;

using Nex.Core;

namespace Nex.Timing
{
    public class NDateLine:NBaseLine<IDate>
    {
        protected override void OnItemAdded(IDate item)
        {
            this.OrderBy(a => a.Date);
        }
        public override void Run()
        {
            if (Clock != null)
            {
                Clock.UpdateTime();
                DateTime a = Clock.DateTimePlayerAtLastUpdate;
                DateTime b = Clock.DateTimePlayer;

                foreach (var item in this)
                {
                    var date = item.Date;
                    if (b < date)
                        continue;
                    else if (a < date && date <= b)
                    {
                        NotifyMessage(new NMessage(this, NMessages.DateEnterMessage, item, a, b));
                        if (item is NObservable)
                            (item as NObservable).NotifyMessage(new NMessage<NDateEventArgs>(item, NMessages.DateEnterMessage, new NDateEventArgs( item, a, b)));
                    }
                    else if (item is ITimeInterval)
                    {
                        ITimeInterval durationItem = item as ITimeInterval;
                        DateTime startDate = date;
                        DateTime endDate = date + durationItem.TimeInterval;

                        if (startDate < a && b < endDate)
                        {
                            NotifyMessage(new NMessage(this, NMessages.DateOverMessage, item, a, b));
                            if (item is NObservable)
                                (item as NObservable).NotifyMessage(new NMessage<NDateEventArgs>(item, NMessages.DateOverMessage, new NDateEventArgs(item, a, b)));
                        }
                        else if (a < endDate && endDate < b)
                        {
                            NotifyMessage(new NMessage(this, NMessages.DateLeaveMessage, item, a, b));
                            if (item is NObservable)
                                (item as NObservable).NotifyMessage(new NMessage<NDateEventArgs>(item, NMessages.DateLeaveMessage, new NDateEventArgs(item, a, b)));
                        }
                    }
                }
            }
        }
    }
}
