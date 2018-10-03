namespace NeoSharp.Core.Cryptography
{
    public class ScryptParameters
    {
        public static ScryptParameters Default { get; } = new ScryptParameters(16384, 8, 8);

        /// <summary>
        /// n is a parameter that defines the CPU/memory cost.
        /// Must be a value 2^N.
        /// </summary>
        public int N { get; private set; }

        /// <summary>
        /// r is a tuning parameter.
        /// </summary>
        public int R { get; private set; }

        /// <summary>
        /// p is a tuning parameter (parallelization parameter). A large value
        /// of p can increase computational cost of SCrypt without increasing
        /// the memory usage.
        /// </summary>
        public int P { get; private set; }

        public ScryptParameters(int n, int r, int p)
        {
            this.N = n;
            this.R = r;
            this.P = p;
        }
    }
}
