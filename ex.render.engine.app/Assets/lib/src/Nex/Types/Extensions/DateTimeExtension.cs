using System;
using System.Collections.Generic;

namespace Nex.Types
{
    public static class DateTimeExtension
    {
        public const string StringFormatWhithMilliseconds = @"hh\:mm\:ss\.fff";
        public const string StringFormat = @"hh\:mm\:ss";

        public static DateTime BeginDayTime(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, 0);
        }
        public static DateTime EndDayTime(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59, 999); 
        }

        public static DateTime GetNextDate(this DateTime value,DayOfWeek dayOfWeek)
        {
            DateTime date = value.Date;
            while (date.DayOfWeek != dayOfWeek)
                date = date.AddDays(+1);

            return date;
        }
        public static DateTime GetPreviewsDate(this DateTime value, DayOfWeek dayOfWeek)
        {
            DateTime date = value.Date;
            while (date.DayOfWeek != dayOfWeek)
                date = date.AddDays(-1);

            return date;
        }
        public static bool IsSameDay(this DateTime value, DateTime date)
        {
            return value.Date == new DateTime(date.Year, date.Month, date.Day);    
        }
        public static bool IsSameWeek(this DateTime value, DateTime date)
        {
            DateTime start = value.GetPreviewsDate(DayOfWeek.Monday).Date;
            DateTime end = value.GetNextDate(DayOfWeek.Sunday).Date + new TimeSpan(0, 23, 59, 59, 999);
            return start <= date && date <= end;
            
        }
        public static bool IsSameMonth(this DateTime value, DateTime date)
        {
            return value.Year == date.Year && value.Month == date.Month;
        }
        public static DateTime GetFirstDateOfMonth(this DateTime value)
        {
            return new DateTime(value.Date.Year, value.Date.Month, 1);
        }
        public static DateTime GetLastDateOfMonth(this DateTime value)
        {
            DateTime date = value.Date;
            while (date.Month == value.Month)
                date = date.AddDays(+1);

            return date + TimeSpan.FromDays(1) - TimeSpan.FromSeconds(1);
        }
        public static DateTime[] GetWeeksInMonth (this DateTime value)
        {
            List<DateTime> list = new List<DateTime>();
            DateTime date = value.GetFirstDateOfMonth().GetPreviewsDate(DayOfWeek.Monday);

            do
            {
                list.Add(date);
                date = date.AddDays(7);
            }
            while (date.Month == value.Month);

            return list.ToArray();
            
        }
    }
}
