using NeoSharp.BinarySerialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Contract
    {
        public string Hash
        {
            get
            {
                return Code.Hash;
            }
        }
        public string Script
        {
            get
            {
                return Code.Script;
            }
        }
        public string[] Parameters
        {
            get
            {
                return Code.Parameters;
            }
        }
        public string ReturnType
        {
            get
            {
                return Code.ReturnType;
            }
        }

        [BinaryProperty(1)]
        [JsonProperty("code")]
        public Code Code { get; set; }

        [BinaryProperty(2)]
        [JsonProperty("needstorage")]
        public string NeedStorage;

        [BinaryProperty(3)]
        [JsonProperty("name")]
        public string Name;

        [BinaryProperty(4)]
        [JsonProperty("version")]
        public string Version;

        [BinaryProperty(5)]
        [JsonProperty("author")]
        public string Author;

        [BinaryProperty(6)]
        [JsonProperty("email")]
        public string Email;

        [BinaryProperty(7)]
        [JsonProperty("description")]
        public string Description;
    }
}