using Newtonsoft.Json;

namespace AAG.Global.ExtensionMethods
{
    public static class CloneObject
    {
        /// <summary>
        /// Clone object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}