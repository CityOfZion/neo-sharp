using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Types;

namespace NeoSharp.Core.Messaging.Messages
{
    public class ConsensusMessage : Message<ConsensusPayload>
    {
        /// TODO: Create a ISigner<ConsensusMessage> for compute this hash

        /// <summary>
        /// Computed hash
        /// </summary>
        public UInt256 Hash { get; set; }

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
        /// <param name="payload">Payload</param>
        public ConsensusMessage(ConsensusPayload payload)
        {
            Command = MessageCommand.consensus;
            Payload = payload;
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

        [BinaryProperty(1, ValueHandlerLogic = ValueHandlerLogicType.MustBeEqual)]
        public readonly byte ScriptPrefix = 1;

#pragma warning restore CS0414

        [BinaryProperty(2)]
        public Witness Script;
    }
}