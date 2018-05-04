using System;

namespace NeoSharp.Core.Messaging
{
    public interface ICarryPayload
    {
        Type PayloadType { get; }
        object Payload { get; set; }
    }
}