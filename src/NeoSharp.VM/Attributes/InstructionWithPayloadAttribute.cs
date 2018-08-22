using System.IO;
using NeoSharp.VM.Types;

namespace NeoSharp.VM.Attributes
{
    public class InstructionWithPayloadAttribute : InstructionAttribute
    {
        public enum EQuantity
        {
            FixedQuantity,
            Byte,
            Int16,
            Int32,
            UInt16,
            UInt32,
            String,
        }

        /// <summary>
        /// Fixed Quantity
        /// </summary>
        public byte FixedQuantity { get; set; } = 0;

        /// <summary>
        /// Quantity
        /// </summary>
        public EQuantity DynamicQuantity { get; set; } = EQuantity.FixedQuantity;

        /// <summary>
        /// Constructor
        /// </summary>
        public InstructionWithPayloadAttribute() : base(typeof(InstructionWithPayload)) { }

        /// <summary>
        /// Fill instruction
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <param name="instruction">Instruction</param>
        public override bool Fill(BinaryReader reader, Instruction instruction)
        {
            if (!(instruction is InstructionWithPayload ip)) return false;

            switch (DynamicQuantity)
            {
                case EQuantity.FixedQuantity:
                    {
                        ip.PayloadSize = FixedQuantity;
                        break;
                    }
                case EQuantity.Byte: ip.PayloadSize = reader.ReadByte(); break;
                case EQuantity.Int16: ip.PayloadSize = reader.ReadInt16(); break;
                case EQuantity.UInt16: ip.PayloadSize = reader.ReadUInt16(); break;
                case EQuantity.Int32: ip.PayloadSize = reader.ReadInt32(); break;
                case EQuantity.UInt32: ip.PayloadSize = reader.ReadUInt32(); break;
                case EQuantity.String:
                    {
                        ip.PayloadSize = reader.ReadByte();
                        if (ip.PayloadSize >= 252) return false;
                        break;
                    }
            }

            ip.Payload = new byte[ip.PayloadSize];

            var read = 0;
            var index = 0;

            while (index < ip.PayloadSize)
            {
                read = reader.Read(ip.Payload, index, (int)(ip.PayloadSize - index));
                if (read <= 0) break;

                index += read;
            }

            return true;
        }
    }
}