using System;

namespace NeoSharp.Core.Network.Rpc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcMethodAttribute : Attribute
    {
        public string Name { get; set; }
    }
}