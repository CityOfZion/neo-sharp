using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using NeoSharp.Application.Attributes;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Caching;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using NeoSharp.Types.ExtensionMethods;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace NeoSharp.Application.Client
{
    public class ConsoleHandler : IConsoleHandler
    {
        class AutoCompleteCommand
        {
            public int ParameterStartIndex { get; set; } = -1;

            public bool IsExact { get; set; } = true;

            public string Method { get; set; }

            public IList<ParameterInfo[]> Methods { get; set; }
        }

        #region Constants

        /// <summary>
        /// Max history size
        /// </summary>
        private const int MaxHistorySize = 32;

        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        #endregion

        #region Public Fields

        public const string DoubleFixedPoint = "0.###################################################################################################################################################################################################################################################################################################################################################";

        /// <summary>
        /// State
        /// </summary>
        public ConsoleReaderState State { get; private set; } = ConsoleReaderState.None;

        /// <summary>
        /// Key available
        /// </summary>
        public bool KeyAvailable => Console.KeyAvailable;

        #endregion

        #region Private fields

        /// <summary>
        /// Prompt
        /// </summary>
        private const string ReadPrompt = "neo#> ";

        private readonly object _lockObject = new object();

        private static readonly ReflectionCache<ConsoleOutputStyle, ConsoleOutputStyleAttribute> Cache =
            ReflectionCache<ConsoleOutputStyle, ConsoleOutputStyleAttribute>.CreateFromEnum();

        private readonly List<string> _manualInputs;
        private readonly IList<string> _history;
        private readonly IDictionary<ConsoleKey, Action<ConsoleKey, ConsoleInputState>> _keyHandle;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ConsoleHandler()
        {
            _manualInputs = new List<string>();
            _history = new List<string>();

            _keyHandle = new Dictionary<ConsoleKey, Action<ConsoleKey, ConsoleInputState>>
            {
                [ConsoleKey.Enter] = ReadFromConsole_Enter,
                [ConsoleKey.Delete] = ReadFromConsole_Delete,
                [ConsoleKey.Backspace] = ReadFromConsole_Backspace,
                [ConsoleKey.LeftArrow] = ReadFromConsole_LeftRight,
                [ConsoleKey.RightArrow] = ReadFromConsole_LeftRight,
                [ConsoleKey.Home] = ReadFromConsole_HomeEnd,
                [ConsoleKey.End] = ReadFromConsole_HomeEnd,
                [ConsoleKey.UpArrow] = ReadFromConsole_UpDown,
                [ConsoleKey.DownArrow] = ReadFromConsole_UpDown,
                [ConsoleKey.PageDown] = ReadFromConsole_UpDown,
                [ConsoleKey.PageUp] = ReadFromConsole_UpDown,
                [ConsoleKey.Tab] = ReadFromConsole_Tab,
                [ConsoleKey.Insert] = ReadFromConsole_Insert,
            };
        }

        /// <summary>
        /// Append inputs
        /// </summary>
        /// <param name="inputs">Inputs</param>
        public void AppendInputs(params string[] inputs)
        {
            if (inputs == null) return;

            // Add non-empty entries to manual inputs

            _manualInputs.AddRange(inputs.Where(u => !string.IsNullOrEmpty(u)));
        }

        /// <summary>
        /// Try to read a manual input
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>Return true if is readed</returns>
        private bool TryReadManualInput(out string input)
        {
            if (_manualInputs.Count > 0)
            {
                input = _manualInputs[0];
                _manualInputs.RemoveAt(0);

                if (string.IsNullOrEmpty(input)) return TryReadManualInput(out input);

                return true;
            }

            input = null;
            return false;
        }

        /// <summary>
        /// Read password
        /// </summary>
        /// <param name="promptLabel">Prompt label</param>
        /// <returns>Reteurn Secure string password</returns>
        public SecureString ReadPassword(string promptLabel = "Password: ")
        {
            if (promptLabel != null)
            {
                WriteLine(promptLabel, ConsoleOutputStyle.Information);
            }

            State = ConsoleReaderState.ReadingPassword;

            var pwd = new SecureString();
            var colorBefore = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;

            // If have something loaded

            if (TryReadManualInput(out var manual))
            {
                State = ConsoleReaderState.ReadingDirty;

                // Print it
                Console.Write("*".PadLeft(manual.Length, '*'));
            }
            else
            {
                // Read from console

                while (true)
                {
                    var i = Console.ReadKey(true);

                    if (i.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    else if (i.Key == ConsoleKey.Backspace)
                    {
                        if (pwd.Length > 0)
                        {
                            pwd.RemoveAt(pwd.Length - 1);
                            Console.Write("\b \b");
                        }
                    }
                    else
                    {
                        pwd.AppendChar(i.KeyChar);
                        Console.Write("*");
                    }

                    State = pwd.Length > 0 ? ConsoleReaderState.ReadingPasswordDirty : ConsoleReaderState.ReadingPassword;
                }
            }

            Console.ForegroundColor = colorBefore;
            State = ConsoleReaderState.None;

            return pwd;
        }

        /// <inheritdoc />
        public string ReadFromConsole(IAutoCompleteHandler autocomplete = null)
        {
            State = ConsoleReaderState.Reading;

            // Write prompt

            WritePrompt();

            // If have something loaded

            if (TryReadManualInput(out var manual))
            {
                State = ConsoleReaderState.ReadingDirty;

                // Print it
                WriteLine(manual, ConsoleOutputStyle.Input);

                // Use it
                State = ConsoleReaderState.None;

                return manual;
            }

            // Read from console

            var state = new ConsoleInputState(this)
            {
                Autocomplete = autocomplete
            };

            if (IsWindows)
            {
                // In Mac don't work

                Console.CursorSize = !state.InsertMode ? 100 : 25;
            }

            ConsoleKeyInfo i;

            do
            {
                i = Console.ReadKey(true);

                if (_keyHandle.TryGetValue(i.Key, out var action))
                {
                    action(i.Key, state);
                }
                else
                {
                    ReadFromConsole_Default(i, state);
                }

                State = state.Txt.Length > 0 ? ConsoleReaderState.ReadingDirty : ConsoleReaderState.Reading;
            }
            while (i.Key != ConsoleKey.Enter);

            // Process return

            var ret = state.Txt.ToString();

            // Append to history

            if (_history.LastOrDefault() != ret)
            {
                if (_history.Count > MaxHistorySize)
                {
                    _history.RemoveAt(0);
                }

                _history.Add(ret);
            }

            // return text

            State = ConsoleReaderState.None;

            return ret;
        }

        #region Key logic

        private void ReadFromConsole_Default(ConsoleKeyInfo key, ConsoleInputState state)
        {
            state.Txt.Insert(state.Cursor, key.KeyChar);
            state.Cursor++;

            if (!state.InsertMode)
            {
                Write(key.KeyChar.ToString(), ConsoleOutputStyle.Input);

                if (state.Cursor < state.Txt.Length)
                    state.Txt.Remove(state.Cursor, 1);
            }
            else
            {
                Write(state.Txt.ToString().Substring(state.Cursor - 1), ConsoleOutputStyle.Input);

                if (state.Cursor != state.Txt.Length)
                {
                    SetCursorPosition(state.StartX + state.Cursor, state.StartY);
                }
            }
        }

        private AutoCompleteCommand[] SearchCommands(CommandToken[] tokens, IAutoCompleteHandler autocomplete)
        {
            if (autocomplete == null) return null;

            var ret = new Dictionary<string, AutoCompleteCommand>();

            foreach (var entry in autocomplete.Commands)
            {
                var entryTokens = entry.SplitCommandLine().ToArray();

                // check start with

                var exact = true;
                var parIndex = 0;
                var error = false;

                for (var max = Math.Min(tokens.Length, entryTokens.Length); parIndex < max; parIndex++)
                {
                    var a = entryTokens[parIndex].Value;
                    var b = tokens[parIndex].Value;

                    if (!string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (parIndex == max - 1)
                        {
                            if (!a.StartsWith(b, StringComparison.InvariantCultureIgnoreCase))
                            {
                                error = true;
                                break;
                            }

                            exact = false;
                        }
                        else
                        {
                            error = true;
                            break;
                        }
                    }
                }

                if (error) continue;

                if (!ret.TryGetValue(entry, out var value))
                {
                    value = new AutoCompleteCommand()
                    {
                        IsExact = exact,
                        ParameterStartIndex = parIndex,
                        Method = entry,
                        Methods = new List<ParameterInfo[]>()
                    };

                    ret.Add(entry, value);
                }

                // return the entry

                if (autocomplete.TryGetMethods(entry, out var avalue))
                {
                    foreach (var ls in avalue)
                    {
                        value.Methods.Add(ls.ToArray());
                    }
                }
            }

            // Return the find

            return ret.Count > 0 ? ret.Values.OrderBy(u => u.IsExact).ThenBy(u => u.Method).ToArray() : null;
        }

        private void ReadFromConsole_Tab(ConsoleKey key, ConsoleInputState state)
        {
            var cmdArgs = new List<CommandToken>(state.Txt.ToString().SplitCommandLine()).ToArray();
            var matches = SearchCommands(cmdArgs, state.Autocomplete);

            if (matches == null || matches.Length <= 0) return;

            string[] allowed;
            var isArgument = matches.Length == 1 && matches[0].IsExact;

            // Search match

            var argInjectionPoint = -1;
            var argInjectonPointLength = -1;

            if (isArgument)
            {
                var parIndex = 0;
                var selectedToken = cmdArgs.Where(u => u.IsInside(state.Cursor)).FirstOrDefault();

                if (selectedToken != null)
                {
                    parIndex = Array.IndexOf(cmdArgs, selectedToken) - matches[0].ParameterStartIndex;
                    argInjectionPoint = selectedToken.StartIndex;
                    argInjectonPointLength = selectedToken.RealLength;

                    if (selectedToken.Quoted)
                    {
                        argInjectionPoint++;
                        argInjectonPointLength -= 2;
                    }
                }
                else
                {
                    parIndex = -1;
                    argInjectionPoint = state.Txt.Length;
                    argInjectonPointLength = 0;
                }

                var parameter = GetParameterCandidate(matches[0].Methods, parIndex);

                allowed = state.Autocomplete.GetParameterValues(parameter, selectedToken?.Value)?.ToArray();

                // If any option have one space ...

                if (allowed != null && selectedToken != null && !selectedToken.Quoted && allowed.Any(u => u.Contains(" ")))
                {
                    // Check if we need to append the quotes (c:\\Program files)

                    state.Txt.Insert(selectedToken.StartIndex + selectedToken.RealLength, "\"");
                    state.Txt.Insert(selectedToken.StartIndex, "\"");

                    argInjectionPoint++;
                }
            }
            else
            {
                allowed = matches.Select(u => u.Method).ToArray();
            }

            var maxCommonLength = ComputeMaxString(allowed, out var cmd);

            if (string.IsNullOrEmpty(cmd)) return;

            // Take coincidences

            if (isArgument)
            {
                state.Txt.Remove(argInjectionPoint, argInjectonPointLength);
                state.Txt.Insert(argInjectionPoint, cmd.Substring(0, maxCommonLength));
                state.Cursor = argInjectionPoint + maxCommonLength;
            }
            else
            {
                // Is method

                state.Txt.Clear();
                state.Txt.Append(cmd.Substring(0, maxCommonLength));

                if (allowed.Length == 1)
                {
                    state.Txt.Append(' ');
                }

                state.Cursor = state.Txt.Length;
            }

            // Print found

            OutFoundStrs(allowed, maxCommonLength);

            // Prompt

            WritePrompt();
            state.GetCursorPosition();

            Write(state.Txt.ToString(), ConsoleOutputStyle.Input);
            SetCursorPosition(state.StartX + state.Cursor, state.StartY);
        }

        private ParameterInfo GetParameterCandidate(IList<ParameterInfo[]> methods, int parIndex)
        {
            ParameterInfo parameter = null;

            foreach (var candidate in methods)
            {
                var pi = parIndex < 0 ? candidate.Length - 1 : parIndex;

                if (pi < 0 || candidate.Length <= pi) continue;

                if (parameter == null)
                {
                    // Get this parameter

                    parameter = candidate[pi];
                }
                else
                {
                    // We don't know what overload should be used

                    if (parameter.ParameterType != candidate[pi].ParameterType)
                    {
                        return null;
                    }
                }
            }

            return parameter;
        }

        private void OutFoundStrs(string[] matches, int maxCommonLength)
        {
            WriteLine("", ConsoleOutputStyle.Input);

            // TODO: If there are more than 100 values we should ask

            if (maxCommonLength == 0)
            {
                // Prevent console blink

                var join = string.Join(Environment.NewLine, matches);

                WriteLine(join, ConsoleOutputStyle.Autocomplete);
            }
            else
            {
                foreach (var match in matches)
                {
                    Write(match.Substring(0, maxCommonLength), ConsoleOutputStyle.AutocompleteMatch);
                    WriteLine(match.Substring(maxCommonLength), ConsoleOutputStyle.Autocomplete);
                }
            }
        }

        private int ComputeMaxString(string[] allowedStrings, out string cmd)
        {
            if (allowedStrings == null || allowedStrings.Length == 0)
            {
                cmd = null;
                return 0;
            }

            cmd = allowedStrings.FirstOrDefault();

            if (string.IsNullOrEmpty(cmd))
            {
                return 0;
            }

            var max = 0;

            for (int x = 1, m = cmd.Length; x <= m; x++)
            {
                var ok = true;
                var split = cmd.Substring(0, x);

                foreach (var s in allowedStrings)
                {
                    if (s.StartsWith(split)) continue;

                    ok = false;
                    break;
                }

                if (ok) max = x;
                else break;
            }

            return max;
        }

        private void ReadFromConsole_Insert(ConsoleKey key, ConsoleInputState state)
        {
            state.InsertMode = !state.InsertMode;

            if (IsWindows)
            {
                Console.CursorSize = !state.InsertMode ? 100 : 25;
            }
        }

        private void ReadFromConsole_UpDown(ConsoleKey key, ConsoleInputState state)
        {
            var strH = "";

            if (_history.Count > 0)
            {
                state.HistoryIndex = state.HistoryIndex + (key == ConsoleKey.DownArrow || key == ConsoleKey.PageDown ? 1 : -1);

                if (state.HistoryIndex < 0) state.HistoryIndex = _history.Count - 1;
                else if (state.HistoryIndex > _history.Count - 1) state.HistoryIndex = 0;

                strH = _history[state.HistoryIndex];

                state.Txt.Clear();
                state.Txt.Append(strH);
                state.Cursor = state.Txt.Length;
            }

            CleanFromThisPoint(state.StartX, state.StartY);
            Write(strH, ConsoleOutputStyle.Input);
        }

        private void ReadFromConsole_HomeEnd(ConsoleKey key, ConsoleInputState state)
        {
            if (key != ConsoleKey.End)
                state.Cursor = 0;
            else
                state.Cursor = state.Txt.Length;

            SetCursorPosition(state.StartX + state.Cursor, state.StartY);
        }

        private void ReadFromConsole_LeftRight(ConsoleKey key, ConsoleInputState state)
        {
            if (key == ConsoleKey.LeftArrow)
            {
                state.Cursor = Math.Max(0, state.Cursor - 1);
            }
            else
            {
                state.Cursor = Math.Min(state.Txt.Length, state.Cursor + 1);
            }

            SetCursorPosition(state.StartX + state.Cursor, state.StartY);
        }

        private void ReadFromConsole_Backspace(ConsoleKey key, ConsoleInputState state)
        {
            if (state.Cursor > 0)
            {
                state.Txt.Remove(state.Cursor - 1, 1);
                state.Cursor--;
            }
            else if (state.Cursor == 0)
                return;

            var l = state.Txt.Length - state.Cursor;

            if (l > 0)
            {
                Write("".PadLeft(l, ' '), ConsoleOutputStyle.Input);
                SetCursorPosition(state.StartX + state.Cursor, state.StartY);
                Write(state.Txt.ToString().Substring(state.Cursor), ConsoleOutputStyle.Input);
                SetCursorPosition(state.StartX + state.Cursor, state.StartY);
            }
            else
            {
                Write("\b \b", ConsoleOutputStyle.Input);
            }
        }

        private void ReadFromConsole_Delete(ConsoleKey key, ConsoleInputState state)
        {
            if (state.Cursor >= state.Txt.Length)
            {
                return;
            }

            state.Txt.Remove(state.Cursor, 1);

            if (state.Txt.Length - state.Cursor != 0)
            {
                SetCursorPosition(state.StartX + state.Cursor, state.StartY);
                Write(state.Txt.ToString().Substring(state.Cursor) + " \b", ConsoleOutputStyle.Input);
                SetCursorPosition(state.StartX + state.Cursor, state.StartY);
            }
            else
            {
                Write(" \b", ConsoleOutputStyle.Input);
            }
        }

        private void ReadFromConsole_Enter(ConsoleKey key, ConsoleInputState state)
        {
            WriteLine("", ConsoleOutputStyle.Input);
        }

        #endregion

        private void CleanFromThisPoint(int startX, int startY)
        {
            GetCursorPosition(out int endX, out int endY);

            int l;

            if (startY == endY)
            {
                // Same row
                l = endX - startX;
            }
            else
            {
                if (startY < endY)
                {
                    // More than 1 row
                    l = endX + (Console.WindowWidth - startX);
                    l += Math.Max(0, endY - startY - 1) * Console.WindowWidth;
                }
                else
                {
                    l = 0;
                }
            }

            SetCursorPosition(startX, startY);

            // Clean

            if (l > 0)
            {
                Write("".PadLeft(l, ' '), ConsoleOutputStyle.Input);
                SetCursorPosition(startX, startY);
            }
        }

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
        public void ApplyStyle(ConsoleOutputStyle style) => Cache[style]?.Apply();

        /// <summary>
        /// Beep
        /// </summary>
        public void Beep() => Console.Beep();

        /// <inheritdoc />
        public void Clear() => Console.Clear();

        /// <inheritdoc />
        public void WritePrompt() => Write(ReadPrompt, ConsoleOutputStyle.Prompt);

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
        public ConsolePercentWriter CreatePercent(long maxValue = 100) => new ConsolePercentWriter(this, 0, maxValue);

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
                        var settings = new JsonSerializerSettings() { };

                        settings.Converters.Add(new StringEnumConverter());

                        using (TextReader tx = new StringReader(obj is JObject ? obj.ToString() : JsonConvert.SerializeObject(obj, settings)))
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

                                            if (last == JsonToken.StartArray)
                                            {
                                                Write(" ]", ConsoleOutputStyle.DarkGray);
                                            }
                                            else
                                            {
                                                var app = first ? indent : Environment.NewLine + indent;

                                                Write(app + "]", ConsoleOutputStyle.DarkGray);
                                            }
                                            break;
                                        }
                                    case JsonToken.EndConstructor:
                                    case JsonToken.EndObject:
                                        {
                                            indent = indent.Remove(indent.Length - 1, 1);

                                            if (last == JsonToken.StartConstructor || last == JsonToken.StartObject)
                                            {
                                                Write(" }", ConsoleOutputStyle.DarkGray);
                                            }
                                            else
                                            {
                                                var app = first ? indent : Environment.NewLine + indent;

                                                Write(app + "}", ConsoleOutputStyle.DarkGray);
                                            }
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
                                            var app = first || last == JsonToken.PropertyName ? "" : Environment.NewLine + indent;
                                            if (needComma) app = " ," + app;

                                            Write(app + ScapeJsonString(reader.Value), ConsoleOutputStyle.White);
                                            break;
                                        }
                                    case JsonToken.Null:
                                        {
                                            var needComma = NeedJsonComma(last);
                                            var app = first || last == JsonToken.PropertyName ? "" : Environment.NewLine + indent;
                                            if (needComma) app = " ," + app;

                                            Write(app + "NULL", ConsoleOutputStyle.DarkRed);
                                            break;
                                        }
                                    case JsonToken.Float:
                                        {
                                            var needComma = NeedJsonComma(last);
                                            var app = first || last == JsonToken.PropertyName ? "" : Environment.NewLine + indent;
                                            if (needComma) app = " ," + app;

                                            Write(app + Convert.ToDecimal(reader.Value).ToString(DoubleFixedPoint), ConsoleOutputStyle.White);
                                            break;
                                        }
                                    case JsonToken.Bytes:
                                        {
                                            var needComma = NeedJsonComma(last);
                                            var app = first || last == JsonToken.PropertyName ? "" : Environment.NewLine + indent;
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
                                            var app = first || last == JsonToken.PropertyName ? "" : Environment.NewLine + indent;
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

        /// <summary>
        /// Scape object for json format
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return scaped string</returns>
        private string ScapeJsonString(object value) => JsonConvert.ToString(value.ToString());

        /// <summary>
        /// Return if this token require a comma for serialize it
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Return true or false</returns>
        private bool NeedJsonComma(JsonToken token)
        {
            return
                token != JsonToken.PropertyName && token != JsonToken.None && token != JsonToken.Undefined &&
                token != JsonToken.StartArray && token != JsonToken.StartConstructor && token != JsonToken.StartObject &&
                token != JsonToken.EndArray && token != JsonToken.EndConstructor && token != JsonToken.EndObject;
        }
    }
}
