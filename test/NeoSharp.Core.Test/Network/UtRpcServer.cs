using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Network.Rpc;
using NeoSharp.Core.Network.Security;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Network
{
    [TestClass]
    public class UtRpcServer : TestBase
    {
        [TestMethod]
        public void bindAndCall()
        {
            var loggerMock = AutoMockContainer.GetMock<ILogger<RpcServer>>();
            var aclLoaderMock = AutoMockContainer.GetMock<INetworkAclLoader>();
            RpcServer rpcServer = new RpcServer(GetRpcConfig(), loggerMock.Object, aclLoaderMock.Object);
            
            // binding
            
            rpcServer.BindOperation(null, "checkDifferent", new Func<int, string, bool>((a, b) =>
            {
                int.TryParse(b, out var bInt);
                return !bInt.Equals(a);
            }));

            rpcServer.BindOperation("math", "checkEquals", new Func<int, string, bool>((a, b) =>
            {
                int.TryParse(b, out var bInt);
                return bInt.Equals(a);
            }));

            rpcServer.BindController<MathController>();
            rpcServer.BindController(typeof(Foo));
            
            // inject

            rpcServer.InjectSpecialParameter(context => new RandomObjectForTest() { RandomProp = "Hello" });
            
            // calling

            var resp1 = (bool) rpcServer.CallOperation(null, null, "checkDifferent", 2, "2");
            Assert.IsFalse(resp1);

            var resp2 = (bool) rpcServer.CallOperation(null, "math", "checkEquals", new Dictionary<string, object>()
            {
                {"a", 2 }, 
                {"b", "2"}
            });
            Assert.IsTrue(resp2);

            var resp3 = (int) rpcServer.CallOperation(null, "math", "sum", 2, 3);
            Assert.AreEqual(resp3, 5);

            var resp4 = (int) rpcServer.CallOperation(null, "math", "sub", 3, 1);
            Assert.AreEqual(resp4, 2);

            var resp5 = (string) rpcServer.CallOperation(null, "Foo", "Bar");
            Assert.AreEqual(resp5, "foo bar :)");
            
            var resp6 = (string) rpcServer.CallOperation(null, "Foo", "PrettyMessage", "how are you?");
            Assert.AreEqual(resp6, "Hello, how are you?");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void unbind()
        {
            var loggerMock = AutoMockContainer.GetMock<ILogger<RpcServer>>();
            var aclLoaderMock = AutoMockContainer.GetMock<INetworkAclLoader>();
            RpcServer rpcServer = new RpcServer(GetRpcConfig(), loggerMock.Object, aclLoaderMock.Object);
            
            // binding
            
            rpcServer.BindOperation(null, "checkDifferent", new Func<int, string, bool>((a, b) =>
            {
                int.TryParse(b, out var bInt);
                return !bInt.Equals(a);
            }));
            rpcServer.BindController<Foo>();
            
            // unbinding
            
            rpcServer.UnbindController("Foo");
            
            // calling

            var resp = (bool) rpcServer.CallOperation(null, null, "checkDifferent", 2, "2");
            Assert.IsFalse(resp);
            
            rpcServer.CallOperation(null, "Foo", "Bar");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void unbindAll()
        {
            var loggerMock = AutoMockContainer.GetMock<ILogger<RpcServer>>();
            var aclLoaderMock = AutoMockContainer.GetMock<INetworkAclLoader>();
            RpcServer rpcServer = new RpcServer(GetRpcConfig(), loggerMock.Object, aclLoaderMock.Object);
            
            // binding
            
            rpcServer.BindOperation(null, "checkDifferent", new Func<int, string, bool>((a, b) =>
            {
                int.TryParse(b, out var bInt);
                return !bInt.Equals(a);
            }));
            
            // unbinding
            
            rpcServer.UnbindAllOperations();
            
            // calling

            rpcServer.CallOperation(null, null, "checkDifferent", 2, "2");
        }

        private static RpcConfig GetRpcConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);

            var configuration = builder.Build();

            return new RpcConfig(configuration);
        }
    }
    
    [RpcController(Name = "math")]
    public class MathController
    {
        [RpcMethod]
        public int sum(int a, int b)
        {
            return a + b;
        }
    
        [RpcMethod(Name = "sub")]
        public int Subtract(int a, int b)
        {
            return a - b;
        }
    }

    public class Foo
    {
        [RpcMethod]
        public string Bar()
        {
            return "foo bar :)";
        }
    
        [RpcMethod]
        public string PrettyMessage(RandomObjectForTest mo, string message)
        {
            return mo.RandomProp + ", " + message;
        }
    }

    public class RandomObjectForTest
    {
        public string RandomProp { get; set; }
    }
}
