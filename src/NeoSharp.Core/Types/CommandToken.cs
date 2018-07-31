using System;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Types
{
    public class CommandToken : IEquatable<CommandToken>
    {
        /// <summary>
        /// Value
        /// </summary>
        public readonly string Value;
        /// <summary>
        /// Is Quoted
        /// </summary>
        public readonly bool Quoted;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="quoted">Quoted</param>
        public CommandToken(string value, bool quoted)
        {
            Value = value;
            Quoted = quoted;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value (calculate quoted)</param>
        public CommandToken(string value)
        {
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
            return obj != null && obj.Value == Value && obj.Quoted == Quoted;
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return Quoted ? "\"" + Value.Replace("\"", "\\\"") + "\"" : Value;
        }
    }
}