using System;

namespace AAG.Global.ExtensionMethods
{
    public static class BooleanMethods
    {
        public static bool ToBool(this string value)
            => bool.TryParse(value, out var isBool) && isBool;


        public static object ToDbBool(this string value)
        {
            if (string.IsNullOrEmpty(value)) return DBNull.Value;
            return bool.TryParse(value, out var isBool) && isBool;
        }
    }
}