﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Client
{
    public class ConsoleReader : IConsoleReader
    {
        private readonly IConsoleWriter _consoleWriter;

        private const string _readPrompt = "neo#> ";

        public ConsoleReader(IConsoleWriter consoleWriterInit)
        {
            _consoleWriter = consoleWriterInit;
        }

        public string ReadFromConsole()
        {
            _consoleWriter.Write(_readPrompt);
            return Console.ReadLine();
        }
    }
}
