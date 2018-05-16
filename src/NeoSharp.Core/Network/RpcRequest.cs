using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace NeoSharp.Core.Network
{
    public class RpcRequest
    {
        /// <summary>
        /// Version
        /// </summary>
        public string JsonRpc { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Method
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// Params
        /// </summary>
        public JArray Params { get; private set; }

        /// <summary>
        /// Set object from params
        /// </summary>
        /// <param name="par">Params</param>
        public void SetParams(string par)
        {
            try
            {
                Params = JArray.Parse(par);
            }
            catch
            {
                // Try in base64

                par = Encoding.UTF8.GetString(Convert.FromBase64String(par));
                Params = JArray.Parse(par);
            }
        }

        /// <summary>
        /// Is valid?
        /// </summary>
        public bool IsValid => !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Method) && Params != null && Params.Count > 0;
    }
}