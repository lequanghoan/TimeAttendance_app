using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimeAttendance.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime ConvertDateFrom(DateTime? date)
        {
            if (date == null)
            {
                return new DateTime();
            }

            DateTime returnValue = DateTime.Parse(date.Value.ToShortDateString() + " 00:00:00");

            return returnValue;
        }

        public static DateTime ConvertDateTo(DateTime? date)
        {
            if (date == null)
            {
                return new DateTime();
            }

            DateTime returnValue = DateTime.Parse(date.Value.ToShortDateString() + " 23:59:59");

            return returnValue;
        }

        public static DateTime FirstDayOfWeek(DateTime dt)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
                diff += 7;
            return dt.AddDays(-diff).Date;
        }
    }
}
