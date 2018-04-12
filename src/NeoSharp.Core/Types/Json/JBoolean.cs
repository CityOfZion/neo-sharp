using System;
using System.IO;

namespace NeoSharp.Core.Types.Json
{
    public class JBoolean : JObject
    {
        public bool Value { get; private set; }

        public JBoolean(bool value = false)
        {
            Value = value;
        }

        public override bool AsBoolean()
        {
            return Value;
        }

        public override string AsString()
        {
            return Value.ToString().ToLower();
        }

        public override bool CanConvertTo(Type type)
        {
            if (type == typeof(bool))
                return true;
            if (type == typeof(string))
                return true;
            return false;
        }

        internal new static JBoolean Parse(TextReader reader)
        {
            SkipSpace(reader);
            var firstChar = (char)reader.Read();
            if (firstChar == 't')
            {
                var c2 = reader.Read();
                var c3 = reader.Read();
                var c4 = reader.Read();
                if (c2 == 'r' && c3 == 'u' && c4 == 'e')
                {
                    return new JBoolean(true);
                }
            }
            else if (firstChar == 'f')
            {
                var c2 = reader.Read();
                var c3 = reader.Read();
                var c4 = reader.Read();
                var c5 = reader.Read();
                if (c2 == 'a' && c3 == 'l' && c4 == 's' && c5 == 'e')
                {
                    return new JBoolean();
                }
            }
            throw new FormatException();
        }

        public override string ToString()
        {
            return Value.ToString().ToLower();
        }
    }
}
