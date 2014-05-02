using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NServiceKit.CacheAccess;
using NServiceKit.Common.Support;
using NServiceKit.Common.Web;
using NServiceKit.Text;

namespace NServiceKit.Common
{
    /// <summary>A stream extensions.</summary>
    public static class StreamExtensions
    {
#if !SILVERLIGHT && !XBOX && !MONOTOUCH
        /// <summary>
        /// Compresses the specified text using the default compression method: Deflate
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="compressionType">Type of the compression.</param>
        /// <returns></returns>
        public static byte[] Compress(this string text, string compressionType)
        {
            if (compressionType == CompressionTypes.Deflate)
                return Deflate(text);

            if (compressionType == CompressionTypes.GZip)
                return GZip(text);

            throw new NotSupportedException(compressionType);
        }

        /// <summary>The deflate provider.</summary>
        public static IDeflateProvider DeflateProvider = new NetDeflateProvider();

        /// <summary>The zip provider.</summary>
        public static IGZipProvider GZipProvider = new NetGZipProvider();

        /// <summary>
        /// Decompresses the specified gz buffer using the default compression method: Inflate
        /// </summary>
        /// <param name="gzBuffer">The gz buffer.</param>
        /// <param name="compressionType">Type of the compression.</param>
        /// <returns></returns>
        public static string Decompress(this byte[] gzBuffer, string compressionType)
        {
            if (compressionType == CompressionTypes.Deflate)
                return Inflate(gzBuffer);

            if (compressionType == CompressionTypes.GZip)
                return GUnzip(gzBuffer);

            throw new NotSupportedException(compressionType);
        }

        /// <summary>A string extension method that deflates the given text.</summary>
        ///
        /// <param name="text">The text.</param>
        ///
        /// <returns>A byte[].</returns>
        public static byte[] Deflate(this string text)
        {
            return DeflateProvider.Deflate(text);
        }

        /// <summary>A byte[] extension method that inflates the given gz buffer.</summary>
        ///
        /// <param name="gzBuffer">The gz buffer.</param>
        ///
        /// <returns>A string.</returns>
        public static string Inflate(this byte[] gzBuffer)
        {
            return DeflateProvider.Inflate(gzBuffer);
        }

        /// <summary>A string extension method that zips the given text.</summary>
        ///
        /// <param name="text">The text.</param>
        ///
        /// <returns>A byte[].</returns>
        public static byte[] GZip(this string text)
        {
            return GZipProvider.GZip(text);
        }

        /// <summary>A byte[] extension method that unzips the given gz buffer.</summary>
        ///
        /// <param name="gzBuffer">The gz buffer.</param>
        ///
        /// <returns>A string.</returns>
        public static string GUnzip(this byte[] gzBuffer)
        {
            return GZipProvider.GUnzip(gzBuffer);
        }
#endif

        /// <summary>A Stream extension method that converts a stream to an UTF 8 string.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="stream">The stream to act on.</param>
        ///
        /// <returns>stream as a string.</returns>
        public static string ToUtf8String(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>A Stream extension method that converts a stream to the bytes.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="stream">The stream to act on.</param>
        ///
        /// <returns>stream as a byte[].</returns>
        public static byte[] ToBytes(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return stream.ReadFully();
        }

        /// <summary>A Stream extension method that writes.</summary>
        ///
        /// <param name="stream">The stream to act on.</param>
        /// <param name="text">  The text.</param>
        public static void Write(this Stream stream, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>A Stream extension method that closes the given stream.</summary>
        ///
        /// <param name="stream">The stream to act on.</param>
        public static void Close(this Stream stream)
        {
#if NETFX_CORE
            stream.Dispose();
#else
            stream.Close(); //For documentation purposes. In reality it won't call this Ext method.
#endif
        }
#if !SILVERLIGHT

        /// <summary>A byte[] extension method that converts the bytes to a md 5 hash.</summary>
        ///
        /// <param name="stream">The stream to act on.</param>
        ///
        /// <returns>bytes as a string.</returns>
        public static string ToMd5Hash(this Stream stream)
        {
            var hash = MD5.Create().ComputeHash(stream);
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>A byte[] extension method that converts the bytes to a md 5 hash.</summary>
        ///
        /// <param name="bytes">The bytes to act on.</param>
        ///
        /// <returns>bytes as a string.</returns>
        public static string ToMd5Hash(this byte[] bytes)
        {
            var hash = MD5.Create().ComputeHash(bytes);
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
#endif
    }
}
