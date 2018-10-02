using System;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Types
{
    public class CommandToken : IEquatable<CommandToken>
    {
        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Is Quoted
        /// </summary>
        public bool Quoted { get; }

        /// <summary>
        /// Start index
        /// </summary>
        public int StartIndex { get; }

        /// <summary>
        /// Real length
        /// </summary>
        public int RealLength { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="quoted">Quoted</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="realLength">Real length</param>
        public CommandToken(string value, bool quoted, int startIndex = -1, int realLength = -1)
        {
            Value = value;
            Quoted = quoted;

            StartIndex = startIndex;
            RealLength = realLength;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value (calculate quoted)</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="realLength">Real length</param>
        public CommandToken(string value, int startIndex = -1, int realLength = -1)
        {
            StartIndex = startIndex;
            RealLength = realLength;

            value = value.Trim();
            Value = value.Trim().TrimMatchingQuotes('\"');
            Quoted = Value != value;
            Value = Value.Replace("\\\"", "\"");
        }

        /// <summary>
        /// Return true if is equal
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Return true if is equal</returns>
        public bool Equals(CommandToken obj)
        {
            return obj != null &&
                obj.Value == Value &&
                obj.Quoted == Quoted &&
                StartIndex == obj.StartIndex &&
                RealLength == obj.RealLength;
        }

        /// <summary>
        /// Return true if is equal
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Return true if is equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is CommandToken c)
            {
                return Equals(c);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return Quoted ? "\"" + Value.Replace("\"", "\\\"") + "\"" : Value;
        }
    }
}