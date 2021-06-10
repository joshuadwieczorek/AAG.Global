using System;
using System.IO;

namespace AAG.Global.ExtensionMethods
{
    public static class FileMethods
    {
        /// <summary>
        /// Convert file path to file info object.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static FileInfo ToFileInfo(this string filePath)
            => new FileInfo(filePath);

        
        /// <summary>
        /// Check if file is readable and can be opened.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static bool IsFileLocked(this FileInfo fileInfo)
        {
            try
            {
                using var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                stream.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}