using System;

namespace AAG.Global.ExtensionMethods
{
    public static class DateTimeMethods
    {
        /// <summary>
        /// Convert string to nullable date time.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime? ToNDateTime(
              this string val
            , DateTime? defaultValue = null)
        {
            if (val is null)
                return null;

            if (DateTime.TryParse(val, out DateTime outDate))
                return outDate;

            return defaultValue;
        }


        /// <summary>
        /// Convert string to date time or DBNull.Value.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object ToNDBDateTime(this string val)
        {
            if (val is null)
                return DBNull.Value;

            if (DateTime.TryParse(val, out DateTime outDate))
                return outDate;

            return DBNull.Value;
        }

        /// <summary>
        /// Get first day of last month.
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfLastMonth(this DateTime today)
        {
            var lastMonth = today.AddMonths(-1);
            return new DateTime(lastMonth.Year, lastMonth.Month, 1);
        }


        /// <summary>
        /// Get last day of last month.
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static DateTime LastDayOfLastMonth(this DateTime today)
            => today.FirstDayOfLastMonth().AddMonths(1).AddMilliseconds(-1);


        /// <summary>
        /// Get yesterday.
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public static DateTime Yesterday(this DateTime today)
            => today.AddDays(-1);
    }
}