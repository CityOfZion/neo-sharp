using System.Reflection;

namespace NeoSharp.Core.Network.Rpc
{
    internal class RcpTargetAndMethod
    {
        public object Target { get; set; }
        
        public MethodInfo Method { get; set; }
    }
}
