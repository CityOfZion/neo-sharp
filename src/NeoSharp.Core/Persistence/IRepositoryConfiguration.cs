using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Core.Persistence
{
    public interface IRepositoryConfiguration
    {
        /// <summary>
        /// Connection string or filepath to connect to the database
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// Database identifier (for repositories that support multiple databases like redis)
        /// </summary>
        object DatabaseId { get; set; }
    }
}