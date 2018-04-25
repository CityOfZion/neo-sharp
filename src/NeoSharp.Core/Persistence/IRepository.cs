using System.Collections.Generic;

namespace NeoSharp.Core.Persistence
{
    public interface IRepository
    {
        #region IEnumerable

        /// <summary>
        /// Get Keys
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <param name="startKey">Start Key</param>
        IEnumerable<byte[]> GetKeys(DataEntryPrefix entry, byte[] startKey);
        /// <summary>
        /// Get Key values
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <param name="startKey">Start Key</param>
        IEnumerable<KeyValuePair<byte[], byte[]>> GetKeyValues(DataEntryPrefix entry, byte[] startKey);

        #endregion

        #region Transactions
        
        /// <summary>
        /// Start transaction
        /// </summary>
        void StartTransaction();
        /// <summary>
        /// Commit
        /// </summary>
        void Commit();
        /// <summary>
        /// Rollback
        /// </summary>
        void Rollback();

        #endregion

        #region Core Repository

        /// <summary>
        /// Get Value
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>Return true if is finded</returns>
        bool TryGetValue(DataEntryPrefix entry, byte[] key, out byte[] value);
        /// <summary>
        /// Remove Key
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <param name="key">Key</param>
        /// <returns>Return true if is deleted</returns>
        bool RemoveKey(DataEntryPrefix entry, byte[] key);
        /// <summary>
        /// Set Value
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        void SetValue(DataEntryPrefix entry, byte[] key, byte[] value);

        #endregion
    }
}