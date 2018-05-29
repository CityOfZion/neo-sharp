using NeoSharp.VM.Helpers;
using System;
using System.Linq;
using System.Text;

namespace NeoSharp.VM
{
    public abstract class IByteArrayStackItem : IStackItem<byte[]>, IEquatable<IByteArrayStackItem>
    {
        public override bool CanConvertToByteArray => true;

        public override byte[] ToByteArray()
        {
            return Value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="value">Value</param>
        public IByteArrayStackItem(IExecutionEngine engine, byte[] value) : base(engine, value, EStackItemType.ByteArray) { }

        /// <summary>
        /// Is Equal
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if is equal</returns>
        public bool Equals(IByteArrayStackItem other)
        {
            return other != null && other.Value.SequenceEqual(Value);
        }

        /// <summary>
        /// Is Equal
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if is equal</returns>
        public override bool Equals(IStackItem other)
        {
            if (other is IByteArrayStackItem b)
                return b.Value.SequenceEqual(Value);

            return false;
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            if (Value == null) return "NULL";

            // Check printable characters

            bool allOk = true;
            foreach (byte c in Value)
                if (c < 32 || c > 126)
                {
                    allOk = false;
                    break;
                }

            return allOk ? "'" + Encoding.ASCII.GetString(Value) + "'" : BitHelper.ToHexString(Value);
        }
    }
}