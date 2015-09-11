using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceModel.Serialization;
using NServiceKit.Text;

namespace NServiceKit.Common.Web
{
    /// <summary>A HTTP response filter.</summary>
    public class HttpResponseFilter : IContentTypeFilter
    {
        private static readonly UTF8Encoding UTF8EncodingWithoutBom = new UTF8Encoding(false);

        /// <summary>The instance.</summary>
        public static HttpResponseFilter Instance = new HttpResponseFilter();

        /// <summary>The content type serializers.</summary>
        public Dictionary<string, StreamSerializerDelegate> ContentTypeSerializers
            = new Dictionary<string, StreamSerializerDelegate>();

        /// <summary>The content type response serializers.</summary>
        public Dictionary<string, ResponseSerializerDelegate> ContentTypeResponseSerializers
            = new Dictionary<string, ResponseSerializerDelegate>();

        /// <summary>The content type deserializers.</summary>
        public Dictionary<string, StreamDeserializerDelegate> ContentTypeDeserializers
            = new Dictionary<string, StreamDeserializerDelegate>();

        /// <summary>Initializes a new instance of the NServiceKit.Common.Web.HttpResponseFilter class.</summary>
        public HttpResponseFilter()
        {
            this.ContentTypeFormats = new Dictionary<string, string>();
        }

        /// <summary>Clears the custom filters.</summary>
        public void ClearCustomFilters()
        {
            this.ContentTypeFormats = new Dictionary<string, string>();
            this.ContentTypeSerializers = new Dictionary<string, StreamSerializerDelegate>();
            this.ContentTypeDeserializers = new Dictionary<string, StreamDeserializerDelegate>();
        }

        /// <summary>Gets or sets the content type formats.</summary>
        ///
        /// <value>The content type formats.</value>
        public Dictionary<string, string> ContentTypeFormats { get; set; }

        /// <summary>Gets format content type.</summary>
        ///
        /// <param name="format">Describes the format to use.</param>
        ///
        /// <returns>The format content type.</returns>
        public string GetFormatContentType(string format)
        {
            //Ensure format suffix is in lower case:
            //XML, Json etc. should be acceptible
            format = format.ToLowerInvariant();

            //built-in formats
            if (format == "json")
                return ContentType.Json;
            if (format == "xml")
                return ContentType.Xml;
            if (format == "jsv")
                return ContentType.Jsv;

            string registeredFormats;
            ContentTypeFormats.TryGetValue(format, out registeredFormats);

            return registeredFormats;
        }

        /// <summary>Registers this object.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="contentType">       Type of the content.</param>
        /// <param name="streamSerializer">  The stream serializer.</param>
        /// <param name="streamDeserializer">The stream deserializer.</param>
        public void Register(string contentType, StreamSerializerDelegate streamSerializer, StreamDeserializerDelegate streamDeserializer)
        {
            if (contentType.IsNullOrEmpty())
                throw new ArgumentNullException("contentType");

            var parts = contentType.Split('/');
            var format = parts[parts.Length - 1];
            this.ContentTypeFormats[format] = contentType;

            SetContentTypeSerializer(contentType, streamSerializer);
            SetContentTypeDeserializer(contentType, streamDeserializer);
        }

        /// <summary>Registers this object.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        ///
        /// <param name="contentType">       Type of the content.</param>
        /// <param name="responseSerializer">The response serializer.</param>
        /// <param name="streamDeserializer">The stream deserializer.</param>
        public void Register(string contentType, ResponseSerializerDelegate responseSerializer,
                             StreamDeserializerDelegate streamDeserializer)
        {
            if (contentType.IsNullOrEmpty())
                throw new ArgumentNullException("contentType");

            var parts = contentType.Split('/');
            var format = parts[parts.Length - 1];
            this.ContentTypeFormats[format] = contentType;

            this.ContentTypeResponseSerializers[contentType] = responseSerializer;
            SetContentTypeDeserializer(contentType, streamDeserializer);
        }

        /// <summary>Sets content type serializer.</summary>
        ///
        /// <param name="contentType">     Type of the content.</param>
        /// <param name="streamSerializer">The stream serializer.</param>
        public void SetContentTypeSerializer(string contentType, StreamSerializerDelegate streamSerializer)
        {
            this.ContentTypeSerializers[contentType] = streamSerializer;
        }

        /// <summary>Sets content type deserializer.</summary>
        ///
        /// <param name="contentType">       Type of the content.</param>
        /// <param name="streamDeserializer">The stream deserializer.</param>
        public void SetContentTypeDeserializer(string contentType, StreamDeserializerDelegate streamDeserializer)
        {
            this.ContentTypeDeserializers[contentType] = streamDeserializer;
        }

        /// <summary>Serialize to bytes.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="response">      The response.</param>
        ///
        /// <returns>A byte[].</returns>
        public byte[] SerializeToBytes(IRequestContext requestContext, object response)
        {
            var contentType = requestContext.ResponseContentType;

            StreamSerializerDelegate responseStreamWriter;
            if (this.ContentTypeSerializers.TryGetValue(contentType, out responseStreamWriter) ||
                this.ContentTypeSerializers.TryGetValue(ContentType.GetRealContentType(contentType), out responseStreamWriter))
            {
                using (var ms = new MemoryStream())
                {
                    responseStreamWriter(requestContext, response, ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }

            ResponseSerializerDelegate responseWriter;
            if (this.ContentTypeResponseSerializers.TryGetValue(contentType, out responseWriter) ||
                this.ContentTypeResponseSerializers.TryGetValue(ContentType.GetRealContentType(contentType), out responseWriter))
            {
                using (var ms = new MemoryStream())
                {
                    var httpRes = new HttpResponseStreamWrapper(ms);
                    responseWriter(requestContext, response, httpRes);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }

            var contentTypeAttr = ContentType.GetEndpointAttributes(contentType);
            switch (contentTypeAttr)
            {
                case EndpointAttributes.Xml:
                    return XmlSerializer.SerializeToString(response).ToUtf8Bytes();

                case EndpointAttributes.Json:
                    return JsonDataContractSerializer.Instance.SerializeToString(response).ToUtf8Bytes();

                case EndpointAttributes.Jsv:
                    return TypeSerializer.SerializeToString(response).ToUtf8Bytes();
            }

            throw new NotSupportedException("ContentType not supported: " + contentType);
        }

        /// <summary>Serialize to string.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="response">      The response.</param>
        ///
        /// <returns>A string.</returns>
        public string SerializeToString(IRequestContext requestContext, object response)
        {
            var contentType = requestContext.ResponseContentType;

            StreamSerializerDelegate responseStreamWriter;
            if (this.ContentTypeSerializers.TryGetValue(contentType, out responseStreamWriter) ||
                this.ContentTypeSerializers.TryGetValue(ContentType.GetRealContentType(contentType), out responseStreamWriter))
            {
                using (var ms = new MemoryStream())
                {
                    responseStreamWriter(requestContext, response, ms);

                    ms.Position = 0;
                    var result = new StreamReader(ms, UTF8EncodingWithoutBom).ReadToEnd();
                    return result;
                }
            }

            ResponseSerializerDelegate responseWriter;
            if (this.ContentTypeResponseSerializers.TryGetValue(contentType, out responseWriter) ||
                this.ContentTypeResponseSerializers.TryGetValue(ContentType.GetRealContentType(contentType), out responseWriter))
            {
                using (var ms = new MemoryStream())
                {

                    var httpRes = new HttpResponseStreamWrapper(ms) {
                        KeepOpen = true, //Don't let view engines close the OutputStream
                    };
                    responseWriter(requestContext, response, httpRes);

                    var bytes = ms.ToArray();
                    var result = bytes.FromUtf8Bytes();

                    httpRes.ForceClose(); //Manually close the OutputStream

                    return result;
                }
            }


            var contentTypeAttr = ContentType.GetEndpointAttributes(contentType);
            switch (contentTypeAttr)
            {
                case EndpointAttributes.Xml:
                    return XmlSerializer.SerializeToString(response);

                case EndpointAttributes.Json:
                    return JsonDataContractSerializer.Instance.SerializeToString(response);

                case EndpointAttributes.Jsv:
                    return TypeSerializer.SerializeToString(response);
            }

            throw new NotSupportedException("ContentType not supported: " + contentType);
        }

        /// <summary>Serialize to stream.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="response">      The response.</param>
        /// <param name="responseStream">The response stream.</param>
        public void SerializeToStream(IRequestContext requestContext, object response, Stream responseStream)
        {
            var contentType = requestContext.ResponseContentType;
            var serializer = GetResponseSerializer(contentType);
            if (serializer == null)
                throw new NotSupportedException("ContentType not supported: " + contentType);

            var httpRes = new HttpResponseStreamWrapper(responseStream);
            serializer(requestContext, response, httpRes);
        }

        /// <summary>Serialize to response.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="response">      The response.</param>
        /// <param name="httpResponse">  The HTTP response.</param>
        public void SerializeToResponse(IRequestContext requestContext, object response, IHttpResponse httpResponse)
        {
            var contentType = requestContext.ResponseContentType;
            var serializer = GetResponseSerializer(contentType);
            if (serializer == null)
                throw new NotSupportedException("ContentType not supported: " + contentType);

            serializer(requestContext, response, httpResponse);
        }

        /// <summary>Gets response serializer.</summary>
        ///
        /// <param name="contentType">Type of the content.</param>
        ///
        /// <returns>The response serializer.</returns>
        public ResponseSerializerDelegate GetResponseSerializer(string contentType)
        {
            ResponseSerializerDelegate responseWriter;
            if (this.ContentTypeResponseSerializers.TryGetValue(contentType, out responseWriter) ||
                this.ContentTypeResponseSerializers.TryGetValue(ContentType.GetRealContentType(contentType), out responseWriter))
            {
                return responseWriter;
            }

            var serializer = GetStreamSerializer(contentType);
            if (serializer == null) return null;

            return (httpReq, dto, httpRes) => serializer(httpReq, dto, httpRes.OutputStream);
        }

        /// <summary>Gets stream serializer.</summary>
        ///
        /// <param name="contentType">Type of the content.</param>
        ///
        /// <returns>The stream serializer.</returns>
        public StreamSerializerDelegate GetStreamSerializer(string contentType)
        {
            StreamSerializerDelegate responseWriter;
            if (this.ContentTypeSerializers.TryGetValue(contentType, out responseWriter) ||
                this.ContentTypeSerializers.TryGetValue(ContentType.GetRealContentType(contentType), out responseWriter))
            {
                return responseWriter;
            }

            var contentTypeAttr = ContentType.GetEndpointAttributes(contentType);
            switch (contentTypeAttr)
            {
                case EndpointAttributes.Xml:
                    return (r, o, s) => XmlSerializer.SerializeToStream(o, s);

                case EndpointAttributes.Json:
                    return (r, o, s) => JsonDataContractSerializer.Instance.SerializeToStream(o, s);

                case EndpointAttributes.Jsv:
                    return (r, o, s) => TypeSerializer.SerializeToStream(o, s);
            }

            return null;
        }

        /// <summary>Deserialize from string.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="contentType">Type of the content.</param>
        /// <param name="type">       The type.</param>
        /// <param name="request">    The request.</param>
        ///
        /// <returns>An object.</returns>
        public object DeserializeFromString(string contentType, Type type, string request)
        {
            var contentTypeAttr = ContentType.GetEndpointAttributes(contentType);
            switch (contentTypeAttr)
            {
                case EndpointAttributes.Xml:
                    return XmlSerializer.DeserializeFromString(request, type);

                case EndpointAttributes.Json:
                    return JsonDataContractDeserializer.Instance.DeserializeFromString(request, type);

                case EndpointAttributes.Jsv:
                    return TypeSerializer.DeserializeFromString(request, type);

                default:
                    throw new NotSupportedException("ContentType not supported: " + contentType);
            }
        }

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="contentType">Type of the content.</param>
        /// <param name="type">       The type.</param>
        /// <param name="fromStream"> from stream.</param>
        ///
        /// <returns>An object.</returns>
        public object DeserializeFromStream(string contentType, Type type, Stream fromStream)
        {
            var deserializer = GetStreamDeserializer(contentType);
            if (deserializer == null)
                throw new NotSupportedException("ContentType not supported: " + contentType);

            return deserializer(type, fromStream);
        }

        /// <summary>Gets stream deserializer.</summary>
        ///
        /// <param name="contentType">Type of the content.</param>
        ///
        /// <returns>The stream deserializer.</returns>
        public StreamDeserializerDelegate GetStreamDeserializer(string contentType)
        {
            StreamDeserializerDelegate streamReader;
            var realContentType = contentType.Split(';')[0].Trim();
            if (this.ContentTypeDeserializers.TryGetValue(realContentType, out streamReader))
            {
                return streamReader;
            }

            var contentTypeAttr = ContentType.GetEndpointAttributes(contentType);
            switch (contentTypeAttr)
            {
                case EndpointAttributes.Xml:
                    return XmlSerializer.DeserializeFromStream;

                case EndpointAttributes.Json:
                    return JsonDataContractDeserializer.Instance.DeserializeFromStream;

                case EndpointAttributes.Jsv:
                    return TypeSerializer.DeserializeFromStream;
            }

            return null;
        }
    }
}