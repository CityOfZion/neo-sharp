using System.Diagnostics;

namespace NeoSharp.TestHelpers
{
    /// <summary>
    /// Provides an entry point to verify condition in unit tests.
    /// </summary>
    public static class Asserting
    {
        /// <summary>
        /// Specifies what's the instance to be verified.
        /// </summary>
        /// <typeparam name="T">The type of the instance to be verified.</typeparam>
        /// <param name="assertable">The instance to be verified.</param>
        /// <returns>A new instance of AssertingThat{T} providing the instance to be verified.</returns>
        [DebuggerStepThrough]
        public static AssertingThat<T> That<T>(T assertable)
        {
            return new AssertingThat<T>(assertable);
        }

        /// <summary>
        /// Specifies what's generic type to be verified.
        /// </summary>
        /// <typeparam name="T">The type to be verified.</typeparam>
        /// <returns>A new instance of AssertingThat{T} providing a default instance of the specified type to be verified.</returns>
        [DebuggerStepThrough]
        public static AssertingThat<T> That<T>()
        {
            return new AssertingThat<T>(default(T));
        }

        /// <summary>
        /// Provides an empty entry point to specify the type in further extension methods.
        /// </summary>
        public static AssertingThat That()
        {
            return new AssertingThat();
        }
    }
}