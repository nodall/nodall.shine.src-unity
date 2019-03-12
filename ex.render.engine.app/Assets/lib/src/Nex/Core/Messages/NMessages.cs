using System;
using Nex.Logs;
using Nex.Timing;

namespace Nex.Core
{
    public class NMessages
    {
        public static NMessageDescriptor ExceptionMessage = new NMessageDescriptor(new NMessageArgument<Exception>("Value"));
        public static NMessageDescriptor<NLogEventArgs> LogMessage = new NMessageDescriptor<NLogEventArgs>();
        public static NPropertyMessageDescriptor<string> NameProperty = new NPropertyMessageDescriptor<string>("Name");

        #region [ Date MESSAGES ]
        public static NMessageDescriptor<NDateEventArgs> DateEnterMessage = new NMessageDescriptor<NDateEventArgs>();
        public static NMessageDescriptor<NDateEventArgs> DateOverMessage = new NMessageDescriptor<NDateEventArgs>();
        public static NMessageDescriptor<NDateEventArgs> DateLeaveMessage = new NMessageDescriptor<NDateEventArgs>();
        #endregion

        #region [ Time MESSAGES ]
        public static NMessageDescriptor<NTimeEventArgs> TimeEnterMessage = new NMessageDescriptor<NTimeEventArgs>();
        public static NMessageDescriptor<NTimeEventArgs> TimeOverMessage = new NMessageDescriptor<NTimeEventArgs>();
        public static NMessageDescriptor<NTimeEventArgs> TimeLeaveMessage = new NMessageDescriptor<NTimeEventArgs>();
        #endregion
    }
}
