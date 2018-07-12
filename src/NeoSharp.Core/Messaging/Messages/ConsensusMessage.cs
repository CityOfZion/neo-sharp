using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Messaging.Messages
{
    public class ConsensusMessage : Message<ConsensusPayload>
    {
        #region Variables 

        UInt256 _hash;
        readonly ICrypto _crypto;
        readonly IBinarySerializer _serializer;

        #endregion

        /// <summary>
        /// Computed hash
        /// </summary>
        public UInt256 Hash
        {
            get
            {
                if (_hash == null)
                {
                    _hash = new UInt256(_crypto.Hash256(_serializer.Serialize(Payload.Unsigned)));
                }

                return _hash;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ConsensusMessage()
        {
            Command = MessageCommand.consensus;
            Payload = new ConsensusPayload();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializer">Serializer</param>
        /// <param name="crypto">Crypto</param>
        public ConsensusMessage(IBinarySerializer serializer, ICrypto crypto)
        {
            _crypto = crypto;
            _serializer = serializer;

            Command = MessageCommand.consensus;
            Payload = new ConsensusPayload();
        }

        /// <summary>
        /// Compute Hash
        /// </summary>
        public UInt256 ComputeHash()
        {
            _hash = null;
            return Hash;
        }
    }

    public class ConsensusPayloadUnsigned
    {
        [BinaryProperty(0)]
        public uint Version;

        [BinaryProperty(1)]
        public UInt256 PrevHash;

        [BinaryProperty(2)]
        public uint BlockIndex;

        [BinaryProperty(3)]
        public ushort ValidatorIndex;

        [BinaryProperty(4)]
        public uint Timestamp;

        [BinaryProperty(5)]
        public byte[] Data;
    }

    public class ConsensusPayload
    {
        // Serialize unsign

        [BinaryProperty(0)]
        public ConsensusPayloadUnsigned Unsigned;

        // Serialize sign

#pragma warning disable CS0414

        [BinaryProperty(1)]
        private readonly byte ScriptPreffix = 1;

#pragma warning restore CS0414

        [BinaryProperty(2)]
        public Witness Script;
    }
}