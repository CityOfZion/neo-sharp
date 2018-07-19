using NeoSharp.Application.Attributes;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Caching;
using NeoSharp.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace NeoSharp.Application.Client
{
    public class ConsoleWriter : IConsoleWriter
    {
        #region Variables

        /// <summary>
        /// Prompt
        /// </summary>
        private const string ReadPrompt = "neo#> ";

        public const string DoubleFixedPoint = "0.###################################################################################################################################################################################################################################################################################################################################################";

        private object _lockObject = new object();

        static readonly ReflectionCache<ConsoleOutputStyle, ConsoleOutputStyleAttribute> _cache =
            ReflectionCache<ConsoleOutputStyle, ConsoleOutputStyleAttribute>.CreateFromEnum();

        #endregion

        /// <summary>
        /// Get current cursor positon
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void GetCursorPosition(out int x, out int y)
        {
            x = Console.CursorLeft;
            y = Console.CursorTop;
        }
        /// <summary>
        /// Set cursor position
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void SetCursorPosition(int x, int y)
        {
            Console.CursorLeft = x;
            Console.CursorTop = y;
        }
        /// <summary>
        /// Apply style
        /// </summary>
        /// <param name="style">Style</param>
        public void ApplyStyle(ConsoleOutputStyle style)
        {
            _cache[style]?.Apply();
        }
        /// <summary>
        /// Beep
        /// </summary>
        public void Beep()
        {
            Console.Beep();
        }
        /// <inheritdoc />
        public void Clear()
        {
            Console.Clear();
        }
        /// <inheritdoc />
        public void WritePrompt()
        {
            Write(ReadPrompt, ConsoleOutputStyle.Prompt);
        }
        /// <summary>
        /// Write output into console
        /// </summary>
        /// <param name="output">Output</param>
        /// <param name="style">Style</param>
        public void Write(string output, ConsoleOutputStyle style = ConsoleOutputStyle.Output)
        {
            lock (_lockObject)
            {
                ApplyStyle(style);
                Console.Write(output);
            }
        }
        /// <summary>
        /// Write line into console
        /// </summary>
        /// <param name="line">Line</param>
        /// <param name="style">Style</param>
        public void WriteLine(string line, ConsoleOutputStyle style = ConsoleOutputStyle.Output)
        {
            lock (_lockObject)
            {
                ApplyStyle(style);
                Console.WriteLine(line);
            }
        }
        /// <summary>
        /// Create percent writer
        /// </summary>
        /// <param name="maxValue">Maximum value</param>
        /// <returns>Return Console percent writer</returns>
        public ConsolePercentWriter CreatePercent(long maxValue = 100)
        {
            return new ConsolePercentWriter(this, 0, maxValue);
        }

        /// <summary>
        /// Write object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="output">Output</param>
        /// <param name="style">Style</param>
        public void WriteObject(object obj, PromptOutputStyle output, ConsoleOutputStyle style = ConsoleOutputStyle.Output)
        {
            if (obj == null)
            {
                WriteLine("NULL", ConsoleOutputStyle.DarkRed);
                return;
            }

            switch (output)
            {
                case PromptOutputStyle.json:
                    {
                        using (TextReader tx = new StringReader(obj is JObject ? obj.ToString() : JsonConvert.SerializeObject(obj)))
                        using (JsonTextReader reader = new JsonTextReader(tx))
                        {
                            var indent = "";
                            var last = JsonToken.None;

                            while (reader.Read())
                            {
                                var first = last == JsonToken.None;

                                switch (reader.TokenType)
                                {
                                    case JsonToken.StartArray:
                                        {
                                            var app = first ? indent : Environment.NewLine + indent;

                                            Write(app + "[", ConsoleOutputStyle.DarkGray);
                                            indent += " ";
                                            break;
                                        }
                                    case JsonToken.StartConstructor:
                                    case JsonToken.StartObject:
                                        {
                                            var app = first ? indent : Environment.NewLine + indent;

                                            Write(app + "{", ConsoleOutputStyle.DarkGray);
                                            indent += " ";
                                            break;
                                        }
                                    case JsonToken.EndArray:
                                        {
                                            indent = indent.Remove(indent.Length - 1, 1);
                                            var app = first ? indent : Environment.NewLine + indent;

                                            Write(app + "]", ConsoleOutputStyle.DarkGray);
                                            break;
                                        }
                                    case JsonToken.EndConstructor:
                                    case JsonToken.EndObject:
                                        {
                                            indent = indent.Remove(indent.Length - 1, 1);
                                            var app = first ? indent : Environment.NewLine + indent;

                                            Write(app + "}", ConsoleOutputStyle.DarkGray);
                                            break;
                                        }
                                    case JsonToken.PropertyName:
                                        {
                                            var needComma = NeedJsonComma(last);
                                            var app = first ? indent : Environment.NewLine + indent;
                                            if (needComma) app = " ," + app;

                                            Write(app + ScapeJsonString(reader.Value) + ":", ConsoleOutputStyle.Gray);
                                            break;
                                        }
                                    case JsonToken.String:
                                    case JsonToken.Comment:
                                    case JsonToken.Date:
                                        {
                                            var needComma = NeedJsonComma(last);
                                            var app = first || last == JsonToken.PropertyName ? indent : Environment.NewLine + indent;
                                            if (needComma) app = " ," + app;

                                            Write(app + ScapeJsonString(reader.Value), ConsoleOutputStyle.White);
                                            break;
                                        }
                                    case JsonToken.Null:
                                        {
                                            var needComma = NeedJsonComma(last);
                                            var app = first || last == JsonToken.PropertyName ? indent : Environment.NewLine + indent;
                                            if (needComma) app = " ," + app;

                                            Write(app + "NULL", ConsoleOutputStyle.DarkRed);
                                            break;
                                        }
                                    case JsonToken.Float:
                                        {
                                            var needComma = NeedJsonComma(last);
                                            var app = first || last == JsonToken.PropertyName ? indent : Environment.NewLine + indent;
                                            if (needComma) app = " ," + app;

                                            Write(app + Convert.ToDecimal(reader.Value).ToString(DoubleFixedPoint), ConsoleOutputStyle.White);
                                            break;
                                        }
                                    case JsonToken.Bytes:
                                        {
                                            var needComma = NeedJsonComma(last);
                                            var app = first || last == JsonToken.PropertyName ? indent : Environment.NewLine + indent;
                                            if (needComma) app = " ," + app;

                                            Write(app + ((byte[])reader.Value).ToHexString(true), ConsoleOutputStyle.White);
                                            break;
                                        }
                                    case JsonToken.Boolean:
                                    case JsonToken.Integer:
                                    case JsonToken.Raw:
                                    case JsonToken.Undefined:
                                        {
                                            var needComma = NeedJsonComma(last);
                                            var app = first || last == JsonToken.PropertyName ? indent : Environment.NewLine + indent;
                                            if (needComma) app = " ," + app;

                                            Write(app + ScapeJsonString(reader.Value), ConsoleOutputStyle.White);
                                            break;
                                        }
                                }

                                last = reader.TokenType;
                            }

                            WriteLine("", style);
                        }
                        break;
                    }
                case PromptOutputStyle.raw:
                    {
                        if (obj is byte[] data)
                        {
                            WriteLine(data.ToHexString(true), style);
                        }
                        else
                        {
                            WriteLine(BinarySerializer.Default.Serialize(obj).ToHexString(true));
                        }
                        break;
                    }
            }
        }

        private string ScapeJsonString(object value)
        {
            return JsonConvert.ToString(value.ToString());
        }

        private bool NeedJsonComma(JsonToken last)
        {
            return
                last != JsonToken.PropertyName && last != JsonToken.None && last != JsonToken.Undefined &&
                last != JsonToken.StartArray && last != JsonToken.StartConstructor && last != JsonToken.StartObject &&
                last != JsonToken.EndArray && last != JsonToken.EndConstructor && last != JsonToken.EndObject
                ;
        }
    }
}
