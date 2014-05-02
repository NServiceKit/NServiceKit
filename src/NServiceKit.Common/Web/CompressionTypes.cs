using System;

namespace NServiceKit.Common.Web
{
    /// <summary>A compression types.</summary>
    public static class CompressionTypes
    {
        /// <summary>List of types of all compressions.</summary>
        public static readonly string[] AllCompressionTypes = new[] { Deflate, GZip };

        /// <summary>The default.</summary>
        public const string Default = Deflate;
        /// <summary>The deflate.</summary>
        public const string Deflate = "deflate";
        /// <summary>The zip.</summary>
        public const string GZip = "gzip";

        /// <summary>Query if 'compressionType' is valid.</summary>
        ///
        /// <param name="compressionType">Type of the compression.</param>
        ///
        /// <returns>true if valid, false if not.</returns>
        public static bool IsValid(string compressionType)
        {
            return compressionType == Deflate || compressionType == GZip;
        }

        /// <summary>Assert is valid.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="compressionType">Type of the compression.</param>
        public static void AssertIsValid(string compressionType)
        {
            if (!IsValid(compressionType))
            {
                throw new NotSupportedException(compressionType
                    + " is not a supported compression type. Valid types: gzip, deflate.");
            }
        }

        /// <summary>Gets an extension.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="compressionType">Type of the compression.</param>
        ///
        /// <returns>The extension.</returns>
        public static string GetExtension(string compressionType)
        {
            switch (compressionType)
            {
                case Deflate:
                case GZip:
                    return "." + compressionType;
                default:
                    throw new NotSupportedException(
                        "Unknown compressionType: " + compressionType);
            }
        }
    }
}