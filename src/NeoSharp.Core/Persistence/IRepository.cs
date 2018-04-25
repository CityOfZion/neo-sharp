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
        IEnumerable<byte[]> GetKeys(DataEntry entry, byte[] startKey);
        /// <summary>
        /// Get Key values
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <param name="startKey">Start Key</param>
        IEnumerable<KeyValuePair<byte[], byte[]>> GetKeyValues(DataEntry entry, byte[] startKey);

        #endregion

        #region Core Repository

        /// <summary>
        /// Create snapshot
        /// </summary>
        ISnapshot CreateSnapshot();
        /// <summary>
        /// Prepare entry for this repository
        /// </summary>
        /// <param name="entry">Entry</param>
        void Prepare(DataEntry entry);
        /// <summary>
        /// Get Value
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>Return true if is finded</returns>
        bool TryGetValue(DataEntry entry, byte[] key, out byte[] value);
        /// <summary>
        /// Remove Key
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <param name="key">Key</param>
        /// <returns>Return true if is deleted</returns>
        bool RemoveKey(DataEntry entry, byte[] key);
        /// <summary>
        /// Set Value
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        void SetValue(DataEntry entry, byte[] key, byte[] value);

        #endregion
    }
}