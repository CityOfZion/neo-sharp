using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

namespace NeoSharp.Application.Client
{
    public class ConsoleReader : IConsoleReader
    {
        #region Constants

        /// <summary>
        /// Prompt
        /// </summary>
        private const string ReadPrompt = "neo#> ";
        /// <summary>
        /// Max history size
        /// </summary>
        private const int MaxHistorySize = 32;

        #endregion

        #region Variables

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
        /// <returns>Reteurn Secure string password</returns>
        public SecureString ReadPassword()
        {
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
            }

            return pwd;
        }
        /// <summary>
        /// Read string from console
        /// </summary>
        /// <param name="autocomplete">Autocomplete</param>
        /// <returns>Returns the readed string</returns>
        public string ReadFromConsole(IDictionary<string, List<ParameterInfo[]>> autocomplete = null)
        {
            // Write prompt

            _consoleWriter.Write(ReadPrompt, ConsoleOutputStyle.Prompt);

            // If have something loaded

            if (_manualInputs.Count > 0)
            {
                // Get first loaded command

                var input = _manualInputs[0];
                _manualInputs.RemoveAt(0);

                if (!string.IsNullOrEmpty(input))
                {
                    // Print it

                    _consoleWriter.WriteLine(input, ConsoleOutputStyle.Input);

                    // Use it

                    return input;
                }
            }

            // Read from console

            string ret;
            if (autocomplete != null && autocomplete.Count > 0)
            {
                int cursor = 0;
                int historyIndex = 0;
                bool insertMode = true;
                var txt = new StringBuilder();

                Console.CursorSize = !insertMode ? 100 : 25;
                _consoleWriter.GetCursorPosition(out int startX, out int startY);

                READ_LINE:

                var i = Console.ReadKey(true);

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
                                goto READ_LINE;

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

                            goto READ_LINE;
                        }
                    case ConsoleKey.Backspace:
                        {
                            if (cursor > 0)
                            {
                                txt.Remove(cursor - 1, 1);
                                cursor--;
                            }
                            else if (cursor == 0)
                                goto READ_LINE;

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

                            goto READ_LINE;
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
                            goto READ_LINE;
                        }
                    case ConsoleKey.Home:
                    case ConsoleKey.End:
                        {
                            if (i.Key != ConsoleKey.End)
                                cursor = 0;
                            else
                                cursor = txt.Length;

                            _consoleWriter.SetCursorPosition(startX + cursor, startY);
                            goto READ_LINE;
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

                            goto READ_LINE;
                        }
                    // Autocomplete
                    case ConsoleKey.Tab:
                        {
                            List<string> matches = new List<string>();
                            string cmd = txt.ToString().ToLowerInvariant();

                            foreach (KeyValuePair<string, List<ParameterInfo[]>> var in autocomplete)
                            {
                                if (!var.Key.StartsWith(cmd)) continue;

                                matches.Add(var.Key);
                            }

                            if (matches.Count > 0)
                            {
                                int max = 0;

                                if (matches.Count == 1)
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
                                    for (int x = 1, m = cmd.Length; x < m; x++)
                                    {
                                        bool ok = true;
                                        foreach (string s in matches)
                                        {
                                            if (s.StartsWith(cmd.Substring(0, x))) continue;

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

                                _consoleWriter.Write(ReadPrompt, ConsoleOutputStyle.Prompt);
                                _consoleWriter.GetCursorPosition(out startX, out startY);

                                _consoleWriter.Write(txt.ToString(), ConsoleOutputStyle.Input);
                                _consoleWriter.SetCursorPosition(startX + cursor, startY);
                            }
                            else
                            {
                                // No match

                                _consoleWriter.WriteLine("", ConsoleOutputStyle.Input);
                            }

                            goto READ_LINE;
                        }
                    // Special
                    case ConsoleKey.Insert:
                        {
                            insertMode = !insertMode;
                            Console.CursorSize = !insertMode ? 100 : 25;
                            goto READ_LINE;
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
                                _consoleWriter.SetCursorPosition(startX + cursor, startY);
                            }

                            goto READ_LINE;
                        }
                }

                ret = txt.ToString();
            }
            else
            {
                _consoleWriter.ApplyStyle(ConsoleOutputStyle.Input);
                ret = Console.ReadLine();
            }

            // Append to history

            if (_history.LastOrDefault() != ret)
            {
                if (_history.Count > MaxHistorySize)
                    _history.RemoveAt(0);

                _history.Add(ret);
            }

            // return text

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