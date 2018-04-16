using System;

namespace NeoSharp.Core.Persistence
{
    public interface ISnapshot : IDisposable
    {
        /// <summary>
        /// Commit
        /// </summary>
        void Commit();
        /// <summary>
        /// Rollback
        /// </summary>
        void Rollback();
    }
}