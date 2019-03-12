using System;
using System.Linq;

using Nex.Core;

namespace Nex.Timing
{
    public class NTimeline: NBaseLine<ITime>
    {
        #region [ VIRTUAL OVERRIDES ]
        protected override void OnItemAdded(ITime item)
        {
            this.OrderBy(a => a.Time);
        }
        #endregion

        #region [ Abstract OVERRIDES ]
        public override void Run()
        {
            if (Clock != null)
            {
                Clock.UpdateTime();

                TimeSpan a = Clock.TimePlayerAtLastUpdate;
                TimeSpan b = Clock.TimePlayer;

                foreach (var item in this)
                {
                    if (b < item.Time)
                        continue;
                    else if ( a < item.Time && item.Time <= b)
                    {
                        NotifyMessage( new NMessage(this, NMessages.TimeEnterMessage, item, a, b));
                        if (item is NObservable)
                            (item as NObservable).NotifyMessage(new NMessage<NTimeEventArgs>(item, NMessages.TimeEnterMessage, new NTimeEventArgs( item, a, b)));
                    }
                    else if (item is ITimeInterval)
                    {
                        ITimeInterval durationItem = item as ITimeInterval;
                        TimeSpan startTime = item.Time;
                        TimeSpan endTime = item.Time + durationItem.TimeInterval;
                        if (startTime < a && b < endTime)
                        {
                            NotifyMessage(new NMessage(this, NMessages.TimeOverMessage, item, a, b));
                            if (item is NObservable)
                                (item as NObservable).NotifyMessage(new NMessage<NTimeEventArgs>(item, NMessages.TimeOverMessage, new NTimeEventArgs(item, a, b)));
                        }
                        else if (a < endTime && endTime < b)
                        {
                            NotifyMessage(new NMessage(this, NMessages.TimeLeaveMessage, item, a, b));
                            if (item is NObservable)
                                (item as NObservable).NotifyMessage(new NMessage<NTimeEventArgs>(item, NMessages.TimeLeaveMessage, new NTimeEventArgs(item, a, b)));
                        }
                    }
                }
            }
            
        }
        #endregion     
    }
}
