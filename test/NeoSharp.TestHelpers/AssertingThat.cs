using System.Diagnostics;

namespace NeoSharp.TestHelpers
{
    /// <summary>
    /// Provides an extensible way to verify conditions in unit tests. Extension methods over the specified type can be created to provide
    /// extra assertions.
    /// </summary>
    /// <typeparam name="T">The type of the instance to be verified.</typeparam>
    public class AssertingThat<T>
    {
        /// <summary>
        /// Gets the assertable to be verified.
        /// </summary>
        /// <value>The assertable.</value>
        public T Assertable { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertingThat{T}"/> class.
        /// </summary>
        /// <param name="assertable">The assertable instance.</param>
        [DebuggerStepThrough]
        public AssertingThat(T assertable)
        {
            this.Assertable = assertable;
        }
    }

    /// <summary>
    /// Provides an extensible way to verify conditions in unit tests. Extension methods over the specified type can be created to provide
    /// extra assertions.
    /// </summary>
    public class AssertingThat
    {
    }
}