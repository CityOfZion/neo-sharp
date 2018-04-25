namespace NeoSharp.Core.Persistence
{
    public class DataEntry
    {
        /// <summary>
        /// Type
        /// </summary>
        public readonly DataEntryPrefix Type;
        /// <summary>
        /// Snapshot
        /// </summary>
        public readonly ISnapshot Snapshot;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="snapshot">Snapshot</param>
        public DataEntry(DataEntryPrefix type, ISnapshot snapshot = null)
        {
            Type = type;
            Snapshot = snapshot;
        }
    }
}