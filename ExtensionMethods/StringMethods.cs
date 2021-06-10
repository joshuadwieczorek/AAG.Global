using System;
using System.Globalization;
using System.Text;

namespace AAG.Global.ExtensionMethods
{
    public static class StringMethods
    {
        /// <summary>
        /// Check if string is not empty and has a value.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool HasValue(this string val)
            => (!string.IsNullOrEmpty(val) && !string.IsNullOrWhiteSpace(val));


        /// <summary>
        /// Transform string to upper case.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string Upper(this string val)
        {
            if (val is null)
                return val;

            return val.ToUpperInvariant();
        }


        /// <summary>
        /// Transform string to lower case.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string Lower(this string val)
        {
            if (val is null)
                return val;

            return val.ToLowerInvariant();
        }


        /// <summary>
        /// Transform string to title case.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string TitleCase(
              this string val
            , string culture = "en-US")
        {
            if (val is null)
                return val;

            var textInfo = new CultureInfo(culture, false).TextInfo;
            return textInfo.ToTitleCase(val.Lower());
        }


        /// <summary>
        /// Return string or DBNull.value if string is empty or null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ToNValue(this string value)
        {
            if (!value.HasValue())
                return DBNull.Value;
            return value;
        }


        /// <summary>
        /// Return DBNull.value if object is empty or null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ToNValue(this object value)
        {
            if (value is null)
                return DBNull.Value;
            return value;
        }

        /// <summary>
        /// Transform string to capitalized string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String Capitalize(this String value, Char splitBy = ' ')
        {
            if (!String.IsNullOrEmpty(value))
            {
                StringBuilder sb = new StringBuilder();
                String[] parts = value.Split(splitBy);
                foreach (String part in parts)
                {
                    String val = part.Lower();
                    val = val.Length > 0 ? $"{val.Substring(0, 1).Upper()}{val[1..].Lower()}{splitBy}" : "";
                    sb.Append(val);
                }
                return sb.ToString().TrimEnd(splitBy);
            }

            return value;
        }
    }
}