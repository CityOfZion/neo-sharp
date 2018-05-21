using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Types
{
    public class CommandToken
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
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return Quoted ? "\"" + Value.Replace("\"", "\\\"") + "\"" : Value;
        }
    }
}