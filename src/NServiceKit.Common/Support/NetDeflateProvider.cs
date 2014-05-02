#if !SILVERLIGHT && !MONOTOUCH && !XBOX
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using NServiceKit.CacheAccess;
using NServiceKit.Text;

namespace NServiceKit.Common.Support
{
    /// <summary>A net deflate provider.</summary>
    public class NetDeflateProvider : IDeflateProvider
    {
        /// <summary>Deflates.</summary>
        ///
        /// <param name="text">The text.</param>
        ///
        /// <returns>A byte[].</returns>
        public byte[] Deflate(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            using(var ms = new MemoryStream())
            using (var zipStream = new DeflateStream(ms, CompressionMode.Compress))
            {
                zipStream.Write(buffer, 0, buffer.Length);
                zipStream.Close();

                return ms.ToArray();
            }
        }

        /// <summary>Inflates the given gz buffer.</summary>
        ///
        /// <param name="gzBuffer">Buffer for gz data.</param>
        ///
        /// <returns>A string.</returns>
        public string Inflate(byte[] gzBuffer)
        {
            using (var compressedStream = new MemoryStream(gzBuffer))
            using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                var utf8Bytes = zipStream.ReadFully();
                return Encoding.UTF8.GetString(utf8Bytes);
            }
        }

    }
}
#endif