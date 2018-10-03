using System;
using System.IO;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.SmartContract;
using NeoSharp.Types;

namespace NeoSharp.Core.Models
{
    [Obsolete]
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class PublishTransaction : Transaction
    {
        public byte[] Script;
        public ContractParameterType[] ParameterList;
        public ContractParameterType ReturnType;
        public bool NeedStorage;
        public string Name;
        public string CodeVersion;
        public string Author;
        public string Email;
        public string Description;

        private UInt160 _scriptHash;

        public UInt160 ScriptHash
        {
            get
            {
                if (_scriptHash == null)
                {
                    _scriptHash = Script.ToScriptHash();
                }
                return _scriptHash;
            }
        }

        /// <inheritdoc />
        public PublishTransaction() : base(TransactionType.PublishTransaction) { }

        #region Exclusive serialization

        protected override void DeserializeExclusiveData(IBinarySerializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            Script = reader.ReadVarBytes();
            ParameterList = reader.ReadVarBytes().Select(p => (ContractParameterType)p).ToArray();
            ReturnType = (ContractParameterType)reader.ReadByte();

            if (Version >= 1)
                NeedStorage = reader.ReadBoolean();
            else
                NeedStorage = false;

            Name = reader.ReadVarString(252);
            CodeVersion = reader.ReadVarString(252);
            Author = reader.ReadVarString(252);
            Email = reader.ReadVarString(252);
            Description = reader.ReadVarString(65536);
        }

        protected override int SerializeExclusiveData(IBinarySerializer serializer, BinaryWriter writer, BinarySerializerSettings settings = null)
        {
            var l = writer.WriteVarBytes(Script);
            l += writer.WriteVarBytes(ParameterList.Cast<byte>().ToArray());
            writer.Write((byte)ReturnType); l++;

            if (Version >= 1)
            {
                writer.Write(NeedStorage);
                l++;
            }

            l += writer.WriteVarString(Name);
            l += writer.WriteVarString(CodeVersion);
            l += writer.WriteVarString(Author);
            l += writer.WriteVarString(Email);
            l += writer.WriteVarString(Description);

            return l;
        }

        #endregion
    }
}