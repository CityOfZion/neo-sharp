using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace NeoSharp.VM.TestHelper.Extensions
{
    public static class ParserExtensions
    {
        private static readonly JsonSerializerSettings _settings;

        /// <summary>
        /// Static constructor
        /// </summary>
        static ParserExtensions()
        {
            _settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            _settings.Converters.Add(new StringEnumConverter
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            });
        }

        /// <summary>
        /// Deserialize string to unit test
        /// </summary>
        /// <param name="input">Json</param>
        /// <returns>Unit test</returns>
        public static VMUT JsonToVMUT(this string input)
        {
            return JsonConvert.DeserializeObject<VMUT>(input, _settings);
        }

        /// <summary>
        /// Serialize UT to json
        /// </summary>
        /// <param name="ut">Unit test</param>
        /// <returns>Json</returns>
        public static string ToJson(this VMUT ut)
        {
            return JsonConvert.SerializeObject(ut, _settings);
        }
    }
}