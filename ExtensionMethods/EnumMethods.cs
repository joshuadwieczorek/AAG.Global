using System;

namespace AAG.Global.ExtensionMethods
{
    public static class EnumMethods
    {
        /// <summary>
        /// Convert string to enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumString"></param>
        /// <returns></returns>
        public static T ToEnum<T>(
              this string enumString
            , T defaultValue)
        {
            if (!enumString.HasValue())
                return defaultValue;

            object enumValue;
            if (Enum.TryParse(typeof(T), enumString, true, out enumValue))
                return (T)enumValue;

            return defaultValue;
        }
    }
}