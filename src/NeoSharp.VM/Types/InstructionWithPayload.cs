using System;

namespace NeoSharp.VM
{
    public class InstructionWithPayload : Instruction
    {
        /// <summary>
        /// Payload
        /// </summary>
        public long PayloadSize { get; set; }

        /// <summary>
        /// Payload
        /// </summary>
        public byte[] Payload { get; set; }

        /// <summary>
        /// Convert attribute to byte array
        /// </summary>
        public override byte[] ToByteArray()
        {
            var buffer = new byte[PayloadSize + 1];

            buffer[0] = (byte)OpCode;
            Array.Copy(Payload, 0, buffer, 1, PayloadSize);

            return buffer;
        }
    }
}