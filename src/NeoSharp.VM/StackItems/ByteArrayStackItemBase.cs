using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using NeoSharp.VM.Extensions;
using Newtonsoft.Json;

namespace NeoSharp.VM
{
    public abstract class ByteArrayStackItemBase : StackItem<byte[]>, IEquatable<ByteArrayStackItemBase>
    {
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

        /// <inheritdoc />
        protected ByteArrayStackItemBase(byte[] value) : base(value, EStackItemType.ByteArray) { }

        /// <inheritdoc />
        public bool Equals(ByteArrayStackItemBase other) => other != null && other.Value.SequenceEqual(Value);

        /// <inheritdoc />
        public override bool Equals(StackItemBase other)
        {
            if (other is ByteArrayStackItemBase b)
            {
                return b.Value.SequenceEqual(Value);
            }

            return false;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Value == null) return "NULL";

            return Value.IsASCIIPrintable() ? $"\'{Encoding.ASCII.GetString(Value)}\'" : Value.ToHexString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ByteArrayStackItemBase);
        }
    }
}