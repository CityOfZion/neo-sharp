using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Database
{
    public interface IRepository
    {
        object GetBlock();
        void WriteBlock(object block);
    }
}
