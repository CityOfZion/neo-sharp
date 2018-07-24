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
    }
}