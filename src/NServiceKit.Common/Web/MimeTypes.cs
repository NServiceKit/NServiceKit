using System;
using System.Collections.Generic;

namespace NServiceKit.Common.Web
{
    /// <summary>A mime types.</summary>
    public static class MimeTypes
    {
        /// <summary>List of types of the extension mimes.</summary>
        public static Dictionary<string,string> ExtensionMimeTypes = new Dictionary<string, string>();

        /// <summary>The HTML.</summary>
        public const string Html = "text/html";
        /// <summary>The XML.</summary>
        public const string Xml = "text/xml";
        /// <summary>The JSON.</summary>
        public const string Json = "text/json";
        /// <summary>The jsv.</summary>
        public const string Jsv = "text/jsv";
        /// <summary>The CSV.</summary>
        public const string Csv = "text/csv";
        /// <summary>Buffer for prototype data.</summary>
        public const string ProtoBuf = "application/x-protobuf";

        /// <summary>The java script.</summary>
        public const string JavaScript = "text/javascript";

        /// <summary>Gets an extension.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="mimeType">Type of the mime.</param>
        ///
        /// <returns>The extension.</returns>
        public static string GetExtension(string mimeType)
        {
            switch (mimeType)
            {
                case ProtoBuf:
                    return ".pbuf";
            }

            var parts = mimeType.Split('/');
            if (parts.Length == 1) return "." + parts[0];
            if (parts.Length == 2) return "." + parts[1];

            throw new NotSupportedException("Unknown mimeType: " + mimeType);
        }

        /// <summary>Gets mime type.</summary>
        ///
        /// <param name="fileNameOrExt">Extent of the file name or.</param>
        ///
        /// <returns>The mime type.</returns>
        public static string GetMimeType(string fileNameOrExt)
        {
            fileNameOrExt.ThrowIfNullOrEmpty();
            var parts = fileNameOrExt.Split('.');
            var fileExt = parts[parts.Length - 1];

            string mimeType;
            if (ExtensionMimeTypes.TryGetValue(fileExt, out mimeType))
            {
                return mimeType;
            }

            switch (fileExt)
            {
                case "jpeg":
                case "gif":
                case "png":
                case "tiff":
                case "bmp":
                    return "image/" + fileExt;

                case "jpg":
                    return "image/jpeg";

                case "tif":
                    return "image/tiff";

                case "svg":
                    return "image/svg+xml";

                case "htm":
                case "html":
                case "shtml":
                    return "text/html";

                case "js":
                    return "text/javascript";

                case "csv":
                case "css":
                case "sgml":
                    return "text/" + fileExt;

                case "txt":
                    return "text/plain";

                case "wav":
                    return "audio/wav";

                case "mp3":
                    return "audio/mpeg3";

                case "mid":
                    return "audio/midi";

                case "qt":
                case "mov":
                    return "video/quicktime";

                case "mpg":
                    return "video/mpeg";

                case "avi":
                case "mp4":
                case "ogg":
                case "webm":
                    return "video/" + fileExt;

                case "rtf":
                    return "application/" + fileExt;

                case "xls":
                    return "application/x-excel";

                case "doc":
                    return "application/msword";

                case "ppt":
                    return "application/powerpoint";

                case "gz":
                case "tgz":
                    return "application/x-compressed";

                case "eot":
                    return "application/vnd.ms-fontobject";

                case "ttf":
                    return "application/octet-stream";

                case "woff":
                    return "application/font-woff";

                default:
                    return "application/" + fileExt;
            }
        }
    }
}