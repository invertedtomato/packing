using System;
using System.Globalization;

namespace InvertedTomato {
    public static class DateTimeExtensions {
        private static Calendar Calendar = CultureInfo.InvariantCulture.Calendar;

        /// <summary>
        /// Convert to Unix time stamp in a double.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double ToUnixTimeAsDouble(this DateTime target) {
            return (target - DateUtility.Epoch).TotalSeconds;
        }

        /// <summary>
        /// Convert to Unix time stamp in a UInt64.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static ulong ToUnixTimeAsUInt64(this DateTime target) {
            return (ulong)(target - DateUtility.Epoch).TotalSeconds;
        }

        public static ulong ToUnixTimeAsUInt32(this DateTime target) {
            return (uint)(target - DateUtility.Epoch).TotalSeconds;
        }

        /// <summary>
        /// Get the ISO8601 week-of-year.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int Iso8601WeekOfYear(this DateTime target) {
            // If its Monday, Tuesday or Wednesday, then it'll
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            var day = Calendar.GetDayOfWeek(target);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday) {
                target = target.AddDays(3);
            }

            // Return the week of our adjusted day
            return Calendar.GetWeekOfYear(target, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
