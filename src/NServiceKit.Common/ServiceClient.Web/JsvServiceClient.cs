using System;
using System.IO;
using NServiceKit.ServiceHost;
using NServiceKit.Text;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>A jsv service client.</summary>
    public class JsvServiceClient
        : ServiceClientBase
    {
        /// <summary>Gets the format to use.</summary>
        ///
        /// <value>The format.</value>
        public override string Format
        {
            get { return "jsv"; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.JsvServiceClient class.</summary>
        public JsvServiceClient()
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.JsvServiceClient class.</summary>
        ///
        /// <param name="baseUri">URI of the base.</param>
        public JsvServiceClient(string baseUri) 
        {
            SetBaseUri(baseUri);
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.JsvServiceClient class.</summary>
        ///
        /// <param name="syncReplyBaseUri">  URI of the synchronise reply base.</param>
        /// <param name="asyncOneWayBaseUri">URI of the asynchronous one way base.</param>
        public JsvServiceClient(string syncReplyBaseUri, string asyncOneWayBaseUri) 
            : base(syncReplyBaseUri, asyncOneWayBaseUri)
        {
        }

        /// <summary>Gets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
        public override string ContentType
        {
            get { return String.Format("application/{0}", Format); }
        }

        /// <summary>Serialize to stream.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="request">       The request.</param>
        /// <param name="stream">        The stream.</param>
        public override void SerializeToStream(IRequestContext requestContext, object request, Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                TypeSerializer.SerializeToWriter(request, writer);
            }
        }

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="stream">The stream.</param>
        ///
        /// <returns>A T.</returns>
        public override T DeserializeFromStream<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return TypeSerializer.DeserializeFromReader<T>(reader);
            }
        }

        /// <summary>Gets the stream deserializer.</summary>
        ///
        /// <value>The stream deserializer.</value>
        public override StreamDeserializerDelegate StreamDeserializer
        {
            get { return TypeSerializer.DeserializeFromStream; }
        }
    }
}