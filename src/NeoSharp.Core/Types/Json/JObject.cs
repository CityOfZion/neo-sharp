using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeoSharp.Core.Types.Json
{
    public class JObject
    {
        public static readonly JObject Null = null;
        private Dictionary<string, JObject> _properties = new Dictionary<string, JObject>();

        public JObject this[string name]
        {
            get
            {
                _properties.TryGetValue(name, out var value);
                return value;
            }
            set
            {
                _properties[name] = value;
            }
        }

        public IReadOnlyDictionary<string, JObject> Properties => _properties;

        public virtual bool AsBoolean()
        {
            throw new InvalidCastException();
        }

        public bool AsBooleanOrDefault(bool value = false)
        {
            if (!CanConvertTo(typeof(bool)))
                return value;
            return AsBoolean();
        }

        public virtual T AsEnum<T>(bool ignoreCase = false)
        {
            throw new InvalidCastException();
        }

        public T AsEnumOrDefault<T>(T value = default(T), bool ignoreCase = false)
        {
            if (!CanConvertTo(typeof(T)))
                return value;
            return AsEnum<T>(ignoreCase);
        }

        public virtual double AsNumber()
        {
            throw new InvalidCastException();
        }

        public double AsNumberOrDefault(double value = 0)
        {
            if (!CanConvertTo(typeof(double)))
                return value;
            return AsNumber();
        }

        public virtual string AsString()
        {
            throw new InvalidCastException();
        }

        public string AsStringOrDefault(string value = null)
        {
            if (!CanConvertTo(typeof(string)))
                return value;
            return AsString();
        }

        public virtual bool CanConvertTo(Type type)
        {
            return false;
        }

        public bool ContainsProperty(string key)
        {
            return _properties.ContainsKey(key);
        }

        public static JObject Parse(TextReader reader)
        {
            SkipSpace(reader);
            var firstChar = (char)reader.Peek();
            if (firstChar == '\"' || firstChar == '\'')
            {
                return JString.Parse(reader);
            }
            if (firstChar == '[')
            {
                return JArray.Parse(reader);
            }
            if ((firstChar >= '0' && firstChar <= '9') || firstChar == '-')
            {
                return JNumber.Parse(reader);
            }
            if (firstChar == 't' || firstChar == 'f')
            {
                return JBoolean.Parse(reader);
            }
            if (firstChar == 'n')
            {
                return ParseNull(reader);
            }
            if (reader.Read() != '{') throw new FormatException();
            SkipSpace(reader);
            var obj = new JObject();
            while (reader.Peek() != '}')
            {
                if (reader.Peek() == ',') reader.Read();
                SkipSpace(reader);
                var name = JString.Parse(reader).Value;
                SkipSpace(reader);
                if (reader.Read() != ':') throw new FormatException();
                var value = Parse(reader);
                obj._properties.Add(name, value);
                SkipSpace(reader);
            }
            reader.Read();
            return obj;
        }

        public static JObject Parse(string value)
        {
            using (var reader = new StringReader(value))
            {
                return Parse(reader);
            }
        }

        private static JObject ParseNull(TextReader reader)
        {
            var firstChar = (char)reader.Read();
            if (firstChar == 'n')
            {
                var c2 = reader.Read();
                var c3 = reader.Read();
                var c4 = reader.Read();
                if (c2 == 'u' && c3 == 'l' && c4 == 'l')
                {
                    return null;
                }
            }
            throw new FormatException();
        }

        protected static void SkipSpace(TextReader reader)
        {
            while (reader.Peek() == ' ' || reader.Peek() == '\t' || reader.Peek() == '\r' || reader.Peek() == '\n')
            {
                reader.Read();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('{');
            foreach (var pair in _properties)
            {
                sb.Append('"');
                sb.Append(pair.Key);
                sb.Append('"');
                sb.Append(':');
                if (pair.Value == null)
                {
                    sb.Append("null");
                }
                else
                {
                    sb.Append(pair.Value);
                }
                sb.Append(',');
            }
            if (_properties.Count == 0)
            {
                sb.Append('}');
            }
            else
            {
                sb[sb.Length - 1] = '}';
            }
            return sb.ToString();
        }

        public static implicit operator JObject(Enum value)
        {
            return new JString(value.ToString());
        }

        public static implicit operator JObject(JObject[] value)
        {
            return new JArray(value);
        }

        public static implicit operator JObject(bool value)
        {
            return new JBoolean(value);
        }

        public static implicit operator JObject(double value)
        {
            return new JNumber(value);
        }

        public static implicit operator JObject(string value)
        {
            return value == null ? null : new JString(value);
        }
    }
}
