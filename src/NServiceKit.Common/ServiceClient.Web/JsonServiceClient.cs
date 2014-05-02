using System;
using System.IO;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceModel.Serialization;
using NServiceKit.Text;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>A JSON service client.</summary>
    public class JsonServiceClient
        : ServiceClientBase
    {
        /// <summary>Gets the format to use.</summary>
        ///
        /// <value>The format.</value>
        public override string Format
        {
            get { return "json"; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.JsonServiceClient class.</summary>
        public JsonServiceClient()
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.JsonServiceClient class.</summary>
        ///
        /// <param name="baseUri">URI of the base.</param>
        public JsonServiceClient(string baseUri) 
        {
            SetBaseUri(baseUri);
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.JsonServiceClient class.</summary>
        ///
        /// <param name="syncReplyBaseUri">  URI of the synchronise reply base.</param>
        /// <param name="asyncOneWayBaseUri">URI of the asynchronous one way base.</param>
        public JsonServiceClient(string syncReplyBaseUri, string asyncOneWayBaseUri) 
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
            JsonDataContractSerializer.Instance.SerializeToStream(request, stream);
        }

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="stream">The stream.</param>
        ///
        /// <returns>A T.</returns>
        public override T DeserializeFromStream<T>(Stream stream)
        {
            return JsonDataContractDeserializer.Instance.DeserializeFromStream<T>(stream);
        }

        /// <summary>Gets the stream deserializer.</summary>
        ///
        /// <value>The stream deserializer.</value>
        public override StreamDeserializerDelegate StreamDeserializer
        {
            get { return JsonSerializer.DeserializeFromStream; }
        }
    }
}