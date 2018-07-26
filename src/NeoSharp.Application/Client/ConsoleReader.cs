using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace NeoSharp.Application.Client
{
    public class ConsoleReader : IConsoleReader
    {
        #region Constants

        /// <summary>
        /// Max history size
        /// </summary>
        private const int MaxHistorySize = 32;

        private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        #endregion

        #region Public Fields

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

        private readonly IConsoleWriter _consoleWriter;
        private readonly List<string> _manualInputs;
        private readonly List<string> _history;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="consoleWriterInit">Console writer</param>
        public ConsoleReader(IConsoleWriter consoleWriterInit)
        {
            _consoleWriter = consoleWriterInit;
            _manualInputs = new List<string>();
            _history = new List<string>();
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
        /// Read password
        /// </summary>
        /// <param name="promptLabel">Prompt label</param>
        /// <returns>Reteurn Secure string password</returns>
        public SecureString ReadPassword(bool promptLabel = true)
        {
            if (promptLabel)
            {
                _consoleWriter.WriteLine("Password: ");
            }

            State = ConsoleReaderState.ReadingPassword;

            Console.ForegroundColor = ConsoleColor.Cyan;

            var pwd = new SecureString();

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

            State = ConsoleReaderState.None;

            return pwd;
        }

        /// <inheritdoc />
        public string ReadFromConsole(IAutoCompleteHandler autocomplete = null)
        {
            State = ConsoleReaderState.Reading;

            // Write prompt
            _consoleWriter.WritePrompt();

            // If have something loaded
            if (_manualInputs.Count > 0)
            {
                State = ConsoleReaderState.ReadingDirty;

                // Get first loaded command
                var input = _manualInputs[0];
                _manualInputs.RemoveAt(0);

                if (!string.IsNullOrEmpty(input))
                {
                    // Print it
                    _consoleWriter.WriteLine(input, ConsoleOutputStyle.Input);

                    // Use it
                    State = ConsoleReaderState.None;

                    return input;
                }
            }

            // Read from console
            string ret;
            var cursor = 0;
            var historyIndex = 0;
            var insertMode = true;
            var txt = new StringBuilder();

            if (IsWindows)
            {
                // In Mac don't work

                Console.CursorSize = !insertMode ? 100 : 25;
            }

            _consoleWriter.GetCursorPosition(out var startX, out var startY);

            var i = new ConsoleKeyInfo();

            do
            {
                i = Console.ReadKey(true);

                switch (i.Key)
                {
                    // Accept
                    case ConsoleKey.Enter:
                        {
                            _consoleWriter.WriteLine("", ConsoleOutputStyle.Input);
                            break;
                        }
                    // Remove
                    case ConsoleKey.Delete:
                        {
                            if (cursor >= txt.Length)
                                break;

                            txt.Remove(cursor, 1);

                            if (txt.Length - cursor != 0)
                            {
                                _consoleWriter.SetCursorPosition(startX + cursor, startY);
                                _consoleWriter.Write(txt.ToString().Substring(cursor) + " \b", ConsoleOutputStyle.Input);
                                _consoleWriter.SetCursorPosition(startX + cursor, startY);
                            }
                            else
                            {
                                _consoleWriter.Write(" \b", ConsoleOutputStyle.Input);
                            }

                            break;
                        }
                    case ConsoleKey.Backspace:
                        {
                            if (cursor > 0)
                            {
                                txt.Remove(cursor - 1, 1);
                                cursor--;
                            }
                            else if (cursor == 0)
                                break;

                            int l = txt.Length - cursor;
                            if (l > 0)
                            {
                                _consoleWriter.Write("".PadLeft(l, ' '), ConsoleOutputStyle.Input);
                                _consoleWriter.SetCursorPosition(startX + cursor, startY);
                                _consoleWriter.Write(txt.ToString().Substring(cursor), ConsoleOutputStyle.Input);
                                _consoleWriter.SetCursorPosition(startX + cursor, startY);
                            }
                            else
                            {
                                _consoleWriter.Write("\b \b", ConsoleOutputStyle.Input);
                            }

                            break;
                        }
                    // Move
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.RightArrow:
                        {
                            if (i.Key == ConsoleKey.LeftArrow)
                                cursor = Math.Max(0, cursor - 1);
                            else
                                cursor = Math.Min(txt.Length, cursor + 1);

                            _consoleWriter.SetCursorPosition(startX + cursor, startY);
                            break;
                        }
                    case ConsoleKey.Home:
                    case ConsoleKey.End:
                        {
                            if (i.Key != ConsoleKey.End)
                                cursor = 0;
                            else
                                cursor = txt.Length;

                            _consoleWriter.SetCursorPosition(startX + cursor, startY);
                            break;
                        }
                    // History
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.PageUp:
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.PageDown:
                        {
                            string strH = "";
                            if (_history.Count > 0)
                            {
                                historyIndex = (historyIndex + (i.Key == ConsoleKey.DownArrow || i.Key == ConsoleKey.PageDown ? 1 : -1));

                                if (historyIndex < 0) historyIndex = _history.Count - 1;
                                else if (historyIndex > _history.Count - 1) historyIndex = 0;

                                strH = _history[historyIndex];

                                txt.Clear();
                                txt.Append(strH);
                                cursor = txt.Length;
                            }

                            CleanFromThisPoint(startX, startY);
                            _consoleWriter.Write(strH, ConsoleOutputStyle.Input);

                            break;
                        }
                    // Autocomplete
                    case ConsoleKey.Tab:
                        {
                            string cmd = txt.ToString().ToLowerInvariant();
                            string[] matches = autocomplete?.Keys.Where(key => key.StartsWith(cmd)).OrderBy(u => u).ToArray();

                            if (matches == null || matches.Length <= 0)
                            {
                                // No match

                                break;
                            }

                            int max = 0;

                            if (matches.Length == 1)
                            {
                                // 1 found

                                txt.Clear();
                                txt.Append(matches[0] + " ");
                                cursor = txt.Length;
                                max = matches[0].Length;
                            }
                            else
                            {
                                // Search match

                                cmd = matches[0];
                                for (int x = 1, m = cmd.Length; x <= m; x++)
                                {
                                    var ok = true;
                                    var split = cmd.Substring(0, x);

                                    foreach (string s in matches)
                                    {
                                        if (s.StartsWith(split)) continue;

                                        ok = false;
                                        break;
                                    }

                                    if (ok) max = x;
                                    else break;
                                }

                                // Take coincidences

                                txt.Clear();
                                txt.Append(matches[0].Substring(0, max));
                                cursor = txt.Length;
                            }

                            // Print found

                            _consoleWriter.WriteLine("", ConsoleOutputStyle.Input);

                            foreach (var var in matches)
                            {
                                _consoleWriter.Write(var.Substring(0, max), ConsoleOutputStyle.AutocompleteMatch);
                                _consoleWriter.WriteLine(var.Substring(max), ConsoleOutputStyle.Autocomplete);
                            }

                            // Prompt

                            _consoleWriter.WritePrompt();
                            _consoleWriter.GetCursorPosition(out startX, out startY);

                            _consoleWriter.Write(txt.ToString(), ConsoleOutputStyle.Input);
                            _consoleWriter.SetCursorPosition(startX + cursor, startY);

                            break;
                        }
                    // Special
                    case ConsoleKey.Insert:
                        {
                            insertMode = !insertMode;
                            Console.CursorSize = !insertMode ? 100 : 25;
                            break;
                        }
                    // Write
                    default:
                        {
                            txt.Insert(cursor, i.KeyChar);
                            cursor++;

                            if (!insertMode)
                            {
                                _consoleWriter.Write(i.KeyChar.ToString(), ConsoleOutputStyle.Input);

                                if (cursor < txt.Length)
                                    txt.Remove(cursor, 1);
                            }
                            else
                            {
                                _consoleWriter.Write(txt.ToString().Substring(cursor - 1), ConsoleOutputStyle.Input);

                                if (cursor != txt.Length)
                                {
                                    _consoleWriter.SetCursorPosition(startX + cursor, startY);
                                }
                            }

                            break;
                        }
                }

                State = txt.Length > 0 ? ConsoleReaderState.ReadingDirty : ConsoleReaderState.Reading;
            }
            while (i.Key != ConsoleKey.Enter);

            ret = txt.ToString();

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

        private void CleanFromThisPoint(int startX, int startY)
        {
            _consoleWriter.GetCursorPosition(out int endX, out int endY);

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
                    l += (Math.Max(0, endY - startY - 1)) * Console.WindowWidth;
                }
                else
                {
                    l = 0;
                }
            }

            _consoleWriter.SetCursorPosition(startX, startY);

            // Clean

            if (l > 0)
            {
                _consoleWriter.Write("".PadLeft(l, ' '), ConsoleOutputStyle.Input);
                _consoleWriter.SetCursorPosition(startX, startY);
            }
        }
    }
}
