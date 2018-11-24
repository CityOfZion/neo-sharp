using Newtonsoft.Json;

namespace NeoSharp.Core.Extensions
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Convert object to json
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="indented">Indented</param>
        /// <returns>Return json</returns>
        public static string ToJson(this object obj, bool indented = false)
        {
            return JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None);
        }

        /// <summary>
        /// Convert json to object
        /// </summary>
        /// <param name="json">Json</param>
        /// <returns>Return object</returns>
        public static T JsonToObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}