using System;
using System.Runtime.Serialization;
using MsgPack.Serialization;
using System.IO;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.Plugins.MsgPack
{
    /// <summary>A message pack service client.</summary>
	public class MsgPackServiceClient : ServiceClientBase
	{
        /// <summary>Gets the format to use.</summary>
        ///
        /// <value>The format.</value>
        public override string Format
        {
            get { return "x-msgpack"; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.Plugins.MsgPack.MsgPackServiceClient class.</summary>
        ///
        /// <param name="baseUri">URI of the base.</param>
		public MsgPackServiceClient(string baseUri)
		{
			SetBaseUri(baseUri);
		}

        /// <summary>Initializes a new instance of the NServiceKit.Plugins.MsgPack.MsgPackServiceClient class.</summary>
        ///
        /// <param name="syncReplyBaseUri">  URI of the synchronise reply base.</param>
        /// <param name="asyncOneWayBaseUri">URI of the asynchronous one way base.</param>
        public MsgPackServiceClient(string syncReplyBaseUri, string asyncOneWayBaseUri)
			: base(syncReplyBaseUri, asyncOneWayBaseUri) {}

        /// <summary>Serialize to stream.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="request">       The request.</param>
        /// <param name="stream">        The stream.</param>
		public override void SerializeToStream(IRequestContext requestContext, object request, Stream stream)
		{
            if (request == null) return;
            try
            {
                MsgPackFormat.Serialize(requestContext, request, stream);
            }
            catch (Exception ex)
            {
                MsgPackFormat.HandleException(ex, request.GetType());
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
            try
            {
                var serializer = MessagePackSerializer.Create<T>();
                var obj = serializer.Unpack(stream);
                return obj;

            }
            catch (Exception ex)
            {
                return (T)MsgPackFormat.HandleException(ex, typeof(T));
            }
        }

        /// <summary>Gets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
		public override string ContentType
		{
			get { return Common.Web.ContentType.MsgPack; }
		}

        /// <summary>Gets the stream deserializer.</summary>
        ///
        /// <value>The stream deserializer.</value>
		public override StreamDeserializerDelegate StreamDeserializer
		{
            get { return MsgPackFormat.Deserialize; }
		}
	}
}