namespace NeoSharp.VM.Types
{
    public class InstructionLocation
    {
        /// <summary>
        /// Index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Offset
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// String reprsentation
        /// </summary>
        public override string ToString()
        {
            return Offset.ToString("x6");
        }
    }
}