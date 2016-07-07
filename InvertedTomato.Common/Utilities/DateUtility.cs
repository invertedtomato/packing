using System;
using System.Globalization;

namespace InvertedTomato {
    public static class DateUtility {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Generate a DateTime from a given Unix time stamp.
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public static DateTime FromUnixTimestamp(double unixTime) {
            DateTime result;
            try {
                result = Epoch.AddSeconds(unixTime);
            } catch (ArgumentOutOfRangeException) {
                result = DateTime.MinValue;
            }

            return result;
        }

        public static DateTime? Max(params DateTime?[] ds) {
            DateTime? max = null;
            foreach (var d in ds) {
                max = Max(d, max);
            }
            return max;
        }

        private static DateTime? Max(DateTime? d1, DateTime? d2) {
            if (!d1.HasValue) {
                return d2;
            }
            if (!d2.HasValue) {
                return d1;
            }
            return d1 > d2 ? d1 : d2;
        }

        public static byte CurrentWeek { get { return (byte)DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(DateTime.UtcNow, DateTimeFormatInfo.CurrentInfo.CalendarWeekRule, DayOfWeek.Sunday); } }
        /// <summary>
        /// Get the start of the week
        /// </summary>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime StartOfWeek(DayOfWeek startOfWeek = DayOfWeek.Sunday) {
            var dt = DateTime.UtcNow;
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0) {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }
        // <summary>
        /// Get the start of the week
        /// </summary>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime EndOfWeek(DayOfWeek endOfWeek = DayOfWeek.Saturday) {
            var dt = DateTime.UtcNow;
            int diff = dt.DayOfWeek - endOfWeek;
            if (diff < 0) {
                diff += 7;
            }

            return dt.AddDays(diff + 2).Date - new TimeSpan(0, 0, 1);
        }
    }
}
