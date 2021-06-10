using System.IO;
using System.Xml.Serialization;
using AAG.Global.ExtensionMethods;

namespace AAG.Global.Serialization.Xml
{
    public partial class Deserialize
    {
        /// <summary>
        /// Deserialize xml file into object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T FromFile<T>(string filePath)
        {
            var fileInfo = filePath.ToFileInfo();
            if (!fileInfo.Exists) throw new FileNotFoundException(filePath);
            if (!fileInfo.IsFileLocked()) throw new FileLoadException($"'{filePath}' is locked by another process!");
            var xmlSerializer = new XmlSerializer(typeof(T));
            using var streamReader = new StreamReader(filePath);
            return (T)xmlSerializer.Deserialize(streamReader);
        }
    }
}