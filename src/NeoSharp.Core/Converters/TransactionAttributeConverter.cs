using System;
using System.IO;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.SerializationHooks;
using NeoSharp.Core.Exceptions;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Converters
{
    public class TransactionAttributeConverter : IBinaryCustomSerializable
    {
        /// <summary>
        /// Deserialize logic
        /// </summary>
        /// <param name="binaryDeserializer">Deserializer</param>
        /// <param name="reader">Reader</param>
        /// <param name="type">Type</param>
        /// <param name="settings">Settigns</param>
        /// <returns>Return object</returns>
        public object Deserialize(IBinarySerializer binaryDeserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            byte[] data;
            var usage = (TransactionAttributeUsage)reader.ReadByte();

            if (usage == TransactionAttributeUsage.ContractHash || usage == TransactionAttributeUsage.Vote ||
               (usage >= TransactionAttributeUsage.Hash1 && usage <= TransactionAttributeUsage.Hash15))
            {
                data = reader.ReadBytes(32);
            }
            else if (usage == TransactionAttributeUsage.ECDH02 || usage == TransactionAttributeUsage.ECDH03)
            {
                data = new[] { (byte)usage }.Concat(reader.ReadBytes(32)).ToArray();
            }
            else if (usage == TransactionAttributeUsage.Script)
            {
                data = reader.ReadBytes(20);
            }
            else if (usage == TransactionAttributeUsage.DescriptionUrl)
            {
                data = reader.ReadBytes(reader.ReadByte());
            }
            else if (usage == TransactionAttributeUsage.Description || usage >= TransactionAttributeUsage.Remark)
            {
                data = reader.ReadVarBytes(ushort.MaxValue);
            }
            else
            {
                throw new InvalidTransactionException();
            }

            return new TransactionAttribute()
            {
                Usage = usage,
                Data = data
            };
        }

        /// <summary>
        /// Serialize logic
        /// </summary>
        /// <param name="binarySerializer">Serializer</param>
        /// <param name="writer">Writer</param>
        /// <param name="value">Value</param>
        /// <param name="settings">Settigns</param>
        public int Serialize(IBinarySerializer binarySerializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            var v = (TransactionAttribute)value;

            writer.Write((byte)v.Usage);

            int l = 1;
            if (v.Usage == TransactionAttributeUsage.DescriptionUrl)
            {
                writer.Write((byte)v.Data.Length);
                writer.Write(v.Data);
                l += v.Data.Length + 1;
            }
            else if (v.Usage == TransactionAttributeUsage.Description || v.Usage >= TransactionAttributeUsage.Remark)
            {
                l += writer.WriteVarBytes(v.Data);
            }
            else if (v.Usage == TransactionAttributeUsage.Script)
            {
                writer.Write(v.Data, 0, 20);
                l += 20;
            }
            else if (v.Usage == TransactionAttributeUsage.ECDH02 || v.Usage == TransactionAttributeUsage.ECDH03)
            {
                writer.Write(v.Data, 1, 32);
                l += 32;
            }
            else if (v.Usage == TransactionAttributeUsage.ContractHash || v.Usage == TransactionAttributeUsage.Vote ||
               (v.Usage >= TransactionAttributeUsage.Hash1 && v.Usage <= TransactionAttributeUsage.Hash15))
            {
                writer.Write(v.Data, 0, 32);
                l += 32;
            }
            else
            {
                writer.Write(v.Data);
                l += v.Data.Length;
            }

            return l;
        }
    }
}