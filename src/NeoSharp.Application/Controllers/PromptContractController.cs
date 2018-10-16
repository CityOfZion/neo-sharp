using System;
using System.Diagnostics;
using System.IO;
using NeoSharp.Application.Attributes;
using NeoSharp.Application.Client;
using NeoSharp.Application.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeoSharp.Application.Controllers
{
    public class PromptContractController : IPromptController
    {
        #region Private fields

        private readonly IConsoleHandler _consoleHandler;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="consoleHandler">Console handler</param>
        public PromptContractController(IConsoleHandler consoleHandler)
        {
            _consoleHandler = consoleHandler;
        }

        /*
        TODO #399: Implement contract import / invoke feature
        load_run {path/to/file.avm} (test {params} {returntype} {needs_storage} {needs_dynamic_invoke} {test_params})
        import contract {path/to/file.avm} {params} {returntype} {needs_storage} {needs_dynamic_invoke}
        import contract_addr {contract_hash} {pubkey}
        */

        /// <summary>
        /// Build contract
        /// </summary>
        /// <param name="inputPath">File Input</param>
        /// <param name="fileDest">File Dest</param>
        [PromptCommand("build", Help = "Build a contract", Category = "Contracts")]
        public void BuildCommand(FileInfo inputPath, FileInfo outputPath)
        {
            if (outputPath.Exists) throw (new InvalidParameterException("Output file already exists"));
            if (!inputPath.Exists) throw (new InvalidParameterException(inputPath.FullName));

            string[] dump;
            using (Process p = Process.Start(new ProcessStartInfo()
            {
                FileName = "neon",
                Arguments = "\"" + inputPath.FullName + "\"",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
            }))
            {
                if (!p.Start())
                {
                    throw (new Exception("Error starting neon. Make sure that neon is in your PATH variable"));
                }

                dump = p.StandardOutput.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                p.WaitForExit();
            }

            foreach (var line in dump)
            {
                _consoleHandler.WriteLine(line, ConsoleOutputStyle.Log);
            }

            // Looking for .abi.json

            string tempFile = Path.ChangeExtension(inputPath.FullName, ".abi.json");
            if (File.Exists(tempFile))
            {
                try
                {
                    JToken json = JToken.Parse(File.ReadAllText(tempFile));
                    _consoleHandler.WriteLine(json.ToString(Formatting.Indented), ConsoleOutputStyle.Information);
                }
                catch { }
            }

            // Looking for .avm

            tempFile = Path.ChangeExtension(inputPath.FullName, ".avm");
            if (File.Exists(tempFile))
            {
                var avm = File.ReadAllBytes(tempFile);
                if (avm != null) File.WriteAllBytes(outputPath.FullName, avm);
            }
            else
            {
                throw (new InvalidParameterException("Error compiling the contract"));
            }
        }
    }
}