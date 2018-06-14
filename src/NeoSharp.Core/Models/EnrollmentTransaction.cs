using System.IO;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Models
{
    [BinaryTypeSerializer(typeof(TransactionSerializer))]
    public class EnrollmentTransaction : Transaction
    {
        /// <summary>
        /// PublicKey
        /// </summary>
        //public ECPoint PublicKey;

        //private UInt160 _script_hash = null;
        //private UInt160 ScriptHash
        //{
        //    get
        //    {
        //        if (_script_hash == null)
        //        {
        //            _script_hash = Contract.CreateSignatureRedeemScript(PublicKey).ToScriptHash();
        //        }
        //        return _script_hash;
        //    }
        //}

        /// <summary>
        /// Constructor
        /// </summary>
        public EnrollmentTransaction() : base(TransactionType.EnrollmentTransaction) { }

        protected override void DeserializeExclusiveData(IBinaryDeserializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            //PublicKey = ECPoint.DeserializeFrom(reader, ECCurve.Secp256r1);
        }

        protected override int SerializeExclusiveData(IBinarySerializer serializer, BinaryWriter writer, BinarySerializerSettings settings = null)
        {
            //writer.Write(PublicKey);
            return 0;
        }
    }
}