using Microsoft.Extensions.Logging;
using NeoSharp.Core.Network;

namespace NeoSharp.Application.Client
{
    public class Prompt : IPrompt
    {
        private readonly IConsoleReader _consoleReader;
        private readonly IConsoleWriter _consoleWriter;
        private readonly ILogger<Prompt> _logger;
        private readonly INetworkManager _networkManager;
        
        public Prompt(IConsoleReader consoleReaderInit, IConsoleWriter consoleWriterInit, ILogger<Prompt> logger, INetworkManager networkManagerInit)
        {
            _consoleReader = consoleReaderInit;
            _consoleWriter = consoleWriterInit;
            _logger = logger;
            _networkManager = networkManagerInit;
        }

        public void StartPrompt(string[] args)
        {
            _logger.LogInformation("Starting Prompt");
            _consoleWriter.WriteLine("Neo-Sharp");

            bool exit = false;
            while (!exit)
            {
                string cmd = _consoleReader.ReadFromConsole();
                if (string.IsNullOrWhiteSpace(cmd)) continue;

                switch (cmd)
                {
                    case "start":
                        startNetwork();
                        break;
                    case "stop":
                        stopNetwork();
                        break;
                    case "help":
                        help();
                        break;
                    case "exit":
                        stopNetwork();
                        exit = true;
                        break;
                }                
            }

            _consoleWriter.WriteLine("Exiting");
        }

        private void startNetwork()
        {
            _networkManager.StartNetwork();
        }

        private void stopNetwork()
        {
            _networkManager.StopNetwork();
        }

        private void help()
        {
            _consoleWriter.WriteLine("start");
            _consoleWriter.WriteLine("stop");
            _consoleWriter.WriteLine("help");
            _consoleWriter.WriteLine("exit");
        }
    }
}
