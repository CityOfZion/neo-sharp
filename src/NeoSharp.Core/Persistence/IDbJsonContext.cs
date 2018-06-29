using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NeoSharp.Core.Persistence
{
    public interface IDbJsonContext
    {
        Task Create(string key, string content);

        Task Delete(string key);

        Task Update(string key, string content);

        Task<string> Get(string key);
    }
}
