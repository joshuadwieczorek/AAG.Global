using System;

namespace AAG.Global.ExtensionMethods
{
    public static class NumericValueMethods
    {
        /// <summary>
        /// Convert string value to short or null value.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static short? ToNShort(
              this string val
            , short? defaultValue = null)
        {
            if (val is null)
                return null;

            if (short.TryParse(val, out short shortVal))
                return shortVal;

            return defaultValue;
        }


        /// <summary>
        /// Convert string value to short.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static short ToShort(
              this string val
            , short defaultValue = 0)
        {
            if (val is null)
                return defaultValue;

            if (short.TryParse(val, out short shortVal))
                return shortVal;

            return defaultValue;
        }


        /// <summary>
        /// Convert string value to int or null value.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int? ToNInt(
              this string val
            , int? defaultValue = null)
        {
            if (val is null)
                return null;

            if (int.TryParse(val, out int intVal))
                return intVal;

            return defaultValue;
        }


        /// <summary>
        /// Convert string value to int.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(
              this string val
            , int defaultValue = 0)
        {
            if (val is null)
                return defaultValue;

            if (int.TryParse(val, out int intVal))
                return intVal;

            return defaultValue;
        }


        /// <summary>
        /// Convert string value to long or null value.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long? ToNLong(
              this string val
            , long? defaultValue = null)
        {
            if (val is null)
                return null;

            if (long.TryParse(val, out long longVal))
                return longVal;

            return defaultValue;
        }


        /// <summary>
        /// Convert string value to long or null value.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToLong(
              this string val
            , long defaultValue = 0)
        {
            if (val is null)
                return defaultValue;

            if (long.TryParse(val, out long longVal))
                return longVal;

            return defaultValue;
        }


        /// <summary>
        /// Convert string value to short or DBNull.Value.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object ToNDBShort(this string val)
        {
            if (val is null)
                return DBNull.Value;

            if (short.TryParse(val, out short shortVal))
                return shortVal;

            return DBNull.Value;
        }


        /// <summary>
        /// Convert string value to int or DBNull.Value.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object ToNDBInt(this string val)
        {
            if (val is null)
                return DBNull.Value;

            if (int.TryParse(val, out int intVal))
                return intVal;

            return DBNull.Value;
        }


        /// <summary>
        /// Convert string value to long or DBNull.Value.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static object ToNDBLong(this string val)
        {
            if (val is null)
                return DBNull.Value;

            if (long.TryParse(val, out long longVal))
                return longVal;

            return DBNull.Value;
        }

        public static Object DBDouble(this string value)
        {
            if (!String.IsNullOrEmpty(value) && Double.TryParse(value, out Double val))
                return val;
            return DBNull.Value;
        }


        /// <summary>
        /// Convert string to decimal.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static decimal ToDecimal(
              this string val
            , decimal defaultVal = 0)
        {
            if (decimal.TryParse(val, out decimal goodVal))
                return goodVal;

            return defaultVal;
        }
    }
}