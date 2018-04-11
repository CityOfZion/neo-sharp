using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Core.Caching
{
    public enum TrackState : byte
    {
        None,
        Added,
        Changed,
        Deleted
    }
}
