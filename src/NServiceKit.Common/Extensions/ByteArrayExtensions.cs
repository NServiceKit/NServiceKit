using System;
using Proxy = NServiceKit.Common.ByteArrayExtensions;

namespace NServiceKit.Common.Extensions
{
    /// <summary>A byte array extensions.</summary>
    [Obsolete("Use NServiceKit.Common.ByteArrayExtensions")]
    public static class ByteArrayExtensions
    {
        /// <summary>A byte[] extension method that determine if we are equal.</summary>
        ///
        /// <param name="b1">The b1 to act on.</param>
        /// <param name="b2">The second byte[].</param>
        ///
        /// <returns>true if equal, false if not.</returns>
        public static bool AreEqual(this byte[] b1, byte[] b2)
        {
            return Proxy.AreEqual(b1, b2);
        }
    }
}