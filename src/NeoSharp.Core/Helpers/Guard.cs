using System;

namespace NeoSharp.Core.Helpers
{
    public static class Guard
    {
        public static T ThrowIfNull<T>(T value, string argumentName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return value;
        }
    }
}
