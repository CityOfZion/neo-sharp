namespace NeoSharp.VM.Extensions
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Return true if is ASCCI string
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return true if is ASCCI string</returns>
        public static bool IsASCIIPrintable(this byte[] data)
        {
            if (data == null) return false;

            foreach (var c in data)
            {
                if (c < 32 || c > 126)
                {
                    return false;
                }
            }

            return true;
        }
    }
}