using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using NeoSharp.VM.Extensions;
using Newtonsoft.Json;

namespace NeoSharp.VM
{
    public abstract class IByteArrayStackItem : IStackItem<byte[]>, IEquatable<IByteArrayStackItem>
    {
        public override bool CanConvertToByteArray => true;

        public override byte[] ToByteArray() => Value;

        /// <summary>
        /// Ascii representation
        /// </summary>
        [DefaultValue(null), JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ValueString => Value != null && Value.IsASCIIPrintable() ? Encoding.ASCII.GetString(Value) : null;

        /// <summary>
        /// Ascii representation
        /// </summary>
        public BigInteger ValueInteger => Value != null ? new BigInteger(Value) : BigInteger.Zero;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value</param>
        protected IByteArrayStackItem(byte[] value) : base(value, EStackItemType.ByteArray) { }

        /// <summary>
        /// Is Equal
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if is equal</returns>
        public bool Equals(IByteArrayStackItem other) => other != null && other.Value.SequenceEqual(Value);

        /// <summary>
        /// Is Equal
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if is equal</returns>
        public override bool Equals(IStackItem other)
        {
            if (other is IByteArrayStackItem b)
            {
                return b.Value.SequenceEqual(Value);
            }

            return false;
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            if (Value == null) return "NULL";

            return Value.IsASCIIPrintable() ? "'" + Encoding.ASCII.GetString(Value) + "'" : Value.ToHexString();
        }
    }
}