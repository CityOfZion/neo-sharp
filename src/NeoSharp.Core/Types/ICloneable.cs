using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Core.Types
{
    public interface ICloneable<T>
    {
        T Clone();
        void FromReplica(T replica);
    }
}
