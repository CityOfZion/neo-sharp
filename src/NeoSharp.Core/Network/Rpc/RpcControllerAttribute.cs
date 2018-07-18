using System;

namespace NeoSharp.Core.Network.Rpc
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RpcControllerAttribute : Attribute
    {
        public string Name { get; set; }
    }
}