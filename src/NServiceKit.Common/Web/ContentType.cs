using System;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.Common.Web
{
    /// <summary>A content type.</summary>
    public static class ContentType
    {
        /// <summary>The UTF 8 suffix.</summary>
        public const string Utf8Suffix = "; charset=utf-8";

        /// <summary>Type of the header content.</summary>
        public const string HeaderContentType = "Content-Type";

        /// <summary>The form URL encoded.</summary>
        public const string FormUrlEncoded = "application/x-www-form-urlencoded";

        /// <summary>Information describing the multi part form.</summary>
        public const string MultiPartFormData = "multipart/form-data";

        /// <summary>The HTML.</summary>
        public const string Html = "text/html";

        /// <summary>The JSON report.</summary>
        public const string JsonReport = "text/jsonreport";

        /// <summary>The XML.</summary>
        public const string Xml = "application/xml";

        /// <summary>The XML text.</summary>
        public const string XmlText = "text/xml";

        /// <summary>The first SOAP 1.</summary>
        public const string Soap11 = " text/xml; charset=utf-8";

        /// <summary>The second SOAP 1.</summary>
        public const string Soap12 = " application/soap+xml";

        /// <summary>The JSON.</summary>
        public const string Json = "application/json";

        /// <summary>The JSON text.</summary>
        public const string JsonText = "text/json";

        /// <summary>The java script.</summary>
        public const string JavaScript = "application/javascript";

        /// <summary>The jsv.</summary>
        public const string Jsv = "application/jsv";

        /// <summary>The jsv text.</summary>
        public const string JsvText = "text/jsv";

        /// <summary>The CSV.</summary>
        public const string Csv = "text/csv";

        /// <summary>The yaml.</summary>
        public const string Yaml = "application/yaml";

        /// <summary>The yaml text.</summary>
        public const string YamlText = "text/yaml";

        /// <summary>The plain text.</summary>
        public const string PlainText = "text/plain";

        /// <summary>The markdown text.</summary>
        public const string MarkdownText = "text/markdown";

        /// <summary>Buffer for prototype data.</summary>
        public const string ProtoBuf = "application/x-protobuf";

        /// <summary>The message pack.</summary>
        public const string MsgPack = "application/x-msgpack";

        /// <summary>The bson.</summary>
        public const string Bson = "application/bson";

        /// <summary>The binary.</summary>
        public const string Binary = "application/octet-stream";

        /// <summary>Css.</summary>
        public const string css = "text/css";

        /// <summary>Gets endpoint attributes.</summary>
        ///
        /// <param name="contentType">The contentType to act on.</param>
        ///
        /// <returns>The endpoint attributes.</returns>
        public static EndpointAttributes GetEndpointAttributes(string contentType)
        {
            if (contentType == null)
                return EndpointAttributes.None;

            var realContentType = GetRealContentType(contentType);
            switch (realContentType)
            {
                case Json:
                case JsonText:
                    return EndpointAttributes.Json;

                case Xml:
                case XmlText:
                    return EndpointAttributes.Xml;

                case Html:
                    return EndpointAttributes.Html;

                case Jsv:
                case JsvText:
                    return EndpointAttributes.Jsv;

                case Yaml:
                case YamlText:
                    return EndpointAttributes.Yaml;

                case Csv:
                    return EndpointAttributes.Csv;

                case Soap11:
                    return EndpointAttributes.Soap11;

                case Soap12:
                    return EndpointAttributes.Soap12;

                case ProtoBuf:
                    return EndpointAttributes.ProtoBuf;

                case MsgPack:
                    return EndpointAttributes.MsgPack;

            }

            return EndpointAttributes.FormatOther;
        }

        /// <summary>Gets real content type.</summary>
        ///
        /// <param name="contentType">The contentType to act on.</param>
        ///
        /// <returns>The real content type.</returns>
        public static string GetRealContentType(string contentType)
        {
            return contentType == null
                       ? null
                       : contentType.Split(';')[0].Trim();
        }

        /// <summary>A string extension method that matches content type.</summary>
        ///
        /// <param name="contentType">       The contentType to act on.</param>
        /// <param name="matchesContentType">Type of the matches content.</param>
        ///
        /// <returns>true if matches content type, false if not.</returns>
        public static bool MatchesContentType(this string contentType, string matchesContentType)
        {
            return GetRealContentType(contentType) == GetRealContentType(matchesContentType);
        }

        /// <summary>A string extension method that query if 'contentType' is binary.</summary>
        ///
        /// <param name="contentType">The contentType to act on.</param>
        ///
        /// <returns>true if binary, false if not.</returns>
        public static bool IsBinary(this string contentType)
        {
            var realContentType = GetRealContentType(contentType);
            switch (realContentType)
            {
                case ProtoBuf:
                case MsgPack:
                case Binary:
                case Bson:
                    return true;
            }

            var primaryType = realContentType.SplitOnFirst('/')[0];
            switch (primaryType)
            {
                case "image":
                case "audio":
                case "video":
                    return true;
            }

            return false;
        }

        /// <summary>A string extension method that converts a contentType to a feature.</summary>
        ///
        /// <param name="contentType">The contentType to act on.</param>
        ///
        /// <returns>contentType as a Feature.</returns>
        public static Feature ToFeature(this string contentType)
        {
            if (contentType == null)
                return Feature.None;

            var realContentType = GetRealContentType(contentType);
            switch (realContentType)
            {
                case Json:
                case JsonText:
                    return Feature.Json;

                case Xml:
                case XmlText:
                    return Feature.Xml;

                case Html:
                    return Feature.Html;

                case Jsv:
                case JsvText:
                    return Feature.Jsv;

                case Csv:
                    return Feature.Csv;

                case Soap11:
                    return Feature.Soap11;

                case Soap12:
                    return Feature.Soap12;

                case ProtoBuf:
                    return Feature.ProtoBuf;

                case MsgPack:
                    return Feature.MsgPack;
            }

            return Feature.CustomFormat;
        }

        /// <summary>Gets content format.</summary>
        ///
        /// <param name="format">Describes the format to use.</param>
        ///
        /// <returns>The content format.</returns>
        public static string GetContentFormat(Format format)
        {
            var formatStr = format.ToString().ToLower();
            return format == Format.MsgPack || format == Format.ProtoBuf 
                ? "x-" + formatStr 
                : formatStr;
        }

        /// <summary>Gets content format.</summary>
        ///
        /// <param name="contentType">The contentType to act on.</param>
        ///
        /// <returns>The content format.</returns>
        public static string GetContentFormat(string contentType)
        {
            if (contentType == null) 
                return null;

            var parts = contentType.Split('/');
            return parts[parts.Length - 1];
        }

        /// <summary>A string extension method that converts a contentType to a content format.</summary>
        ///
        /// <param name="contentType">The contentType to act on.</param>
        ///
        /// <returns>contentType as a string.</returns>
        public static string ToContentFormat(this string contentType)
        {
            return GetContentFormat(contentType);
        }

        /// <summary>A Format extension method that converts the formats to a content type.</summary>
        ///
        /// <param name="formats">The formats to act on.</param>
        ///
        /// <returns>formats as a string.</returns>
        public static string ToContentType(this Format formats)
        {
            switch (formats)
            {
                case Format.Soap11:
                case Format.Soap12:
                case Format.Xml:
                    return Xml;

                case Format.Json:
                    return Json;
                    
                case Format.Jsv:
                    return JsvText;

                case Format.Csv:
                    return Csv;

                case Format.ProtoBuf:
                    return ProtoBuf;

                case Format.MsgPack:
                    return MsgPack;

                case Format.Html:
                    return Html;

                case Format.Yaml:
                    return Yaml;
                
                default:
                    return null;
            }
        }
    }

}