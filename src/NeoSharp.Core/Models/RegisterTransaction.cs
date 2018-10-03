using System;
using System.IO;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;
using NeoSharp.Core.Cryptography;
using NeoSharp.Types;

namespace NeoSharp.Core.Models
{
    [Obsolete]
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class RegisterTransaction : Transaction
    {
        /// <summary>
        /// Asset Type
        /// </summary>
        public AssetType AssetType;
        /// <summary>
        /// Name
        /// </summary>
        public string Name;
        /// <summary>
        /// The total number of issues, a total of two modes:
        ///   1. Limited Mode: When Amount is positive, the maximum total amount of the current asset is Amount, and cannot be modified (Equities may support expansion or additional issuance in the future, and will consider the company’s signature or a certain proportion of shareholders Signature recognition).
        ///   2. Unlimited mode: When Amount is equal to -1, the current asset can be issued by the creator indefinitely. This model has the greatest degree of freedom, but it has the lowest credibility and is not recommended for use.
        /// </summary>
        public Fixed8 Amount;
        /// <summary>
        /// Precision
        /// </summary>
        public byte Precision;
        /// <summary>
        /// Publisher's public key
        /// </summary>
        public ECPoint Owner;
        /// <summary>
        /// Asset Manager Contract Hash Value
        /// </summary>
        public UInt160 Admin;

        /// <summary>
        /// Constructor
        /// </summary>
        public RegisterTransaction() : base(TransactionType.RegisterTransaction) { }

        protected override void DeserializeExclusiveData(IBinarySerializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            AssetType = (AssetType)reader.ReadByte();
            Name = reader.ReadVarString(1024);
            Amount = deserializer.Deserialize<Fixed8>(reader, settings);
            Precision = reader.ReadByte();
            Owner = deserializer.Deserialize<ECPoint>(reader, settings);
            Admin = deserializer.Deserialize<UInt160>(reader, settings);
        }

        protected override int SerializeExclusiveData(IBinarySerializer serializer, BinaryWriter writer, BinarySerializerSettings settings = null)
        {
            var l = 1;
            writer.Write((byte)AssetType);
            l += writer.WriteVarString(Name);
            writer.Write(Amount.Value); 
            l += Fixed8.Size;
            writer.Write(Precision); l++;
            l += serializer.Serialize(Owner, writer, settings);
            l += serializer.Serialize(Admin, writer, settings);

            return l;
        }
    }
}