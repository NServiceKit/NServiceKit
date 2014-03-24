using System;
using Proxy = NServiceKit.Common.ByteArrayExtensions;

namespace NServiceKit.Common.Extensions
{
    [Obsolete("Use NServiceKit.Common.ByteArrayExtensions")]
    public static class ByteArrayExtensions
    {
        public static bool AreEqual(this byte[] b1, byte[] b2)
        {
            return Proxy.AreEqual(b1, b2);
        }
    }
}