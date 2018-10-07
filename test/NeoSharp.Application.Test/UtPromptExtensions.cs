using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Application.Attributes;
using NeoSharp.Application.Client;
using NeoSharp.Application.Extensions;
using NeoSharp.Core.DI;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UtPromptExtensions : TestBase
    {
        class DummyPrompt
        {
            [PromptCommand("mul", Category = "Maths", Help = "a*b", Order = 0)]
            public int Mul(int a, int b)
            {
                return a * b;
            }

            [PromptCommand("sum", Category = "Maths", Help = "a+b", Order = 1)]
            public int Sum(int a, int b)
            {
                return a + b;
            }

            [PromptCommand("sum", Category = "Maths", Help = "a+b+c", Order = 0)]
            public int Sum(int a, int b, int c)
            {
                return a + b + c;
            }
        }

        [TestMethod]
        public void SearchCommands_Fail()
        {
            // quoted

            var command = "\"suma\" 1 2";
            var cmdArgs = new List<CommandToken>(command.SplitCommandLine());

            // cache

            var autoComplete = new AutoCommandComplete();
            var cache = new Dictionary<string[], PromptCommandAttribute>();
            cache.Cache(typeof(DummyPrompt), autoComplete);

            var cmds = cache.SearchCommands(cmdArgs).ToArray();

            Assert.AreEqual(0, cmds.Length);
        }

        [TestMethod]
        public void SearchCommands_Ok_ABC()
        {
            // init

            IContainer injector = null;
            var r1 = RandomInt();
            var r2 = RandomInt();
            var r3 = RandomInt();
            var command = $"\"sum\" {r1} {r2} {r3}";
            var cmdArgs = command.SplitCommandLine().ToArray();

            // cache

            var autoComplete = new AutoCommandComplete();
            var cache = new Dictionary<string[], PromptCommandAttribute>();
            cache.Cache(new DummyPrompt(), autoComplete);

            var cmds = cache.SearchCommands(cmdArgs).ToArray();

            Assert.AreEqual(2, cmds.Length);
            Assert.AreEqual("sum", cmds[0].Command);

            var cmd = cmds.SearchRightCommand(cmdArgs, injector, out var args);
            Assert.AreEqual("sum", cmd.Command);
            Assert.AreEqual(3, cmd.Parameters.Length);

            // execute

            var res = cmd.Method.Invoke(new DummyPrompt(), args);
            Assert.AreEqual(r1 + r2 + r3, res);
        }

        [TestMethod]
        public void SearchCommands_Ok_AB()
        {
            // init

            IContainer injector = null;
            var r1 = RandomInt();
            var r2 = RandomInt();
            var command = $"\"sum\" {r1} {r2}";
            var cmdArgs = command.SplitCommandLine().ToArray();

            // cache

            var autoComplete = new AutoCommandComplete();
            var cache = new Dictionary<string[], PromptCommandAttribute>();
            cache.Cache(new DummyPrompt(), autoComplete);

            var cmds = cache.SearchCommands(cmdArgs).ToArray();

            Assert.AreEqual(2, cmds.Length);
            Assert.AreEqual("sum", cmds[0].Command);

            var cmd = cmds.SearchRightCommand(cmdArgs, injector, out var args);
            Assert.AreEqual("sum", cmd.Command);
            Assert.AreEqual(2, cmd.Parameters.Length);

            Assert.IsTrue(cmdArgs[0].IsInside(0));
            Assert.IsTrue(cmdArgs[0].IsInside(1));
            Assert.IsTrue(cmdArgs[0].IsInside(2));
            Assert.IsTrue(cmdArgs[0].IsInside(3));
            Assert.IsTrue(cmdArgs[0].IsInside(4));
            Assert.IsTrue(cmdArgs[0].IsInside(5));
            Assert.IsFalse(cmdArgs[0].IsInside(6));

            // execute

            var res = cmd.Method.Invoke(new DummyPrompt(), args);
            Assert.AreEqual(r1 + r2, res);
        }

        [TestMethod]
        public void Cache_Extensions()
        {
            var cache = new Dictionary<string[], PromptCommandAttribute>();

            var autoComplete = new AutoCommandComplete();
            cache.Cache(new DummyPrompt(), autoComplete);

            Assert.AreEqual(3, cache.Count);
            Assert.AreEqual("mul,sum,sum", string.Join(",", cache.Keys.Select(u => string.Join(",", u))));

            var atr = cache.Values.FirstOrDefault();

            Assert.AreEqual("mul", atr.Command);
            Assert.AreEqual(1, atr.TokensCount);
            CollectionAssert.AreEqual(new string[] { "mul" }, atr.Commands);

            Assert.AreEqual("Maths", atr.Category);
            Assert.AreEqual("a*b", atr.Help);
            Assert.AreEqual(0, atr.Order);

            Assert.AreEqual(nameof(DummyPrompt.Mul), atr.Method.Name);

            CollectionAssert.AreEqual(new string[] { "mul", "sum" }, autoComplete.Commands.ToArray());
        }
    }
}