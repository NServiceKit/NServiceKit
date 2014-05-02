using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceModel.Serialization;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests.ServiceClient.Web
{
    /// <summary>A HTML service client.</summary>
    public class HtmlServiceClient: ServiceClientBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.ServiceClient.Web.HtmlServiceClient class.</summary>
        public HtmlServiceClient()
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.ServiceClient.Web.HtmlServiceClient class.</summary>
        ///
        /// <param name="baseUri">URI of the base.</param>
        public HtmlServiceClient(string baseUri)
            // Can't call SetBaseUri as that appends the format specific suffixes.
            :base(baseUri, baseUri)
        {
        }

        /// <summary>Gets the format to use.</summary>
        ///
        /// <value>The format.</value>
        public override string Format
        {
            // Don't return a format as we are not using a NServiceKit format specific endpoint, but 
            // rather the general purpose endpoint (just like a html <form> POST would use).
            get { return null; }
        }

        /// <summary>Gets the accept.</summary>
        ///
        /// <value>The accept.</value>
        public override string Accept
        {
            get { return Common.Web.ContentType.Html; }
        }

        /// <summary>Gets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
        public override string ContentType
        {
            // Only used by the base class when POST-ing.
            get { return Common.Web.ContentType.FormUrlEncoded; }
        }

        /// <summary>Serialize to stream.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="request">       The request.</param>
        /// <param name="stream">        The stream.</param>
        public override void SerializeToStream(IRequestContext requestContext, object request, Stream stream)
        {
            var queryString = QueryStringSerializer.SerializeToString(request);
            stream.Write(queryString);
        }

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="stream">The stream.</param>
        ///
        /// <returns>A T.</returns>
        public override T DeserializeFromStream<T>(Stream stream)
        {
            return (T) DeserializeDtoFromHtml(typeof (T), stream);
        }

        /// <summary>Gets the stream deserializer.</summary>
        ///
        /// <value>The stream deserializer.</value>
        public override StreamDeserializerDelegate StreamDeserializer
        {
            get { return DeserializeDtoFromHtml; }
        }

        private object DeserializeDtoFromHtml(Type type, Stream fromStream)
        {
            // TODO: No tests currently use the response, but this could be something that will come in handy later.
            // It isn't trivial though, will have to parse the HTML content.
            return Activator.CreateInstance(type);
        }
    }
}
