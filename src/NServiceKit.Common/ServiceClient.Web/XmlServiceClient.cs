using System.IO;
using System.Xml;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceModel.Serialization;
using NServiceKit.Text;
using System;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>An XML service client.</summary>
    public class XmlServiceClient
        : ServiceClientBase
    {
        /// <summary>Gets the format to use.</summary>
        ///
        /// <value>The format.</value>
        public override string Format
        {
            get { return "xml"; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.XmlServiceClient class.</summary>
        public XmlServiceClient()
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.XmlServiceClient class.</summary>
        ///
        /// <param name="baseUri">URI of the base.</param>
        public XmlServiceClient(string baseUri) 
        {
            SetBaseUri(baseUri);
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.XmlServiceClient class.</summary>
        ///
        /// <param name="syncReplyBaseUri">  URI of the synchronise reply base.</param>
        /// <param name="asyncOneWayBaseUri">URI of the asynchronous one way base.</param>
        public XmlServiceClient(string syncReplyBaseUri, string asyncOneWayBaseUri) 
            : base(syncReplyBaseUri, asyncOneWayBaseUri) {}

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
            if (request == null) return;
            DataContractSerializer.Instance.SerializeToStream(request, stream);
        }

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <exception cref="XmlException">Thrown when an XML error condition occurs.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="stream">The stream.</param>
        ///
        /// <returns>A T.</returns>
        public override T DeserializeFromStream<T>(Stream stream)
        {
            try
            {
                return DataContractDeserializer.Instance.DeserializeFromStream<T>(stream);
            }
            catch (XmlException ex)
            {
                if (ex.Message == "Unexpected end of file.") //Empty responses
                    return default(T);

                throw;
            }
        }

        /// <summary>Gets the stream deserializer.</summary>
        ///
        /// <value>The stream deserializer.</value>
        public override StreamDeserializerDelegate StreamDeserializer
        {
            get { return XmlSerializer.DeserializeFromStream; }
        }
    }
}