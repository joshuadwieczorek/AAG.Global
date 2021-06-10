using System.IO;

namespace AAG.Global.ExtensionMethods
{
    public static class SystemIOMethods
    {
        /// <summary>
        /// Check if a directory exists from a string value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDirectory(this string value)
            => Directory.Exists(value);


        /// <summary>
        /// Check if a file path exists from a string value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool FileExists(this string value)
            => File.Exists(value);
    }
}