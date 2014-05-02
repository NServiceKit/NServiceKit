using System;
using System.IO;
using System.Runtime.Serialization;
using ProtoBuf;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.Plugins.ProtoBuf
{
    /// <summary>A prototype buffer service client.</summary>
	public class ProtoBufServiceClient : ServiceClientBase
	{
        /// <summary>Gets the format to use.</summary>
        ///
        /// <value>The format.</value>
        public override string Format
        {
            get { return "x-protobuf"; }
        }

        /// <summary>Initializes a new instance of the NServiceKit.Plugins.ProtoBuf.ProtoBufServiceClient class.</summary>
        ///
        /// <param name="baseUri">URI of the base.</param>
		public ProtoBufServiceClient(string baseUri)
		{
			SetBaseUri(baseUri);
		}

        /// <summary>Initializes a new instance of the NServiceKit.Plugins.ProtoBuf.ProtoBufServiceClient class.</summary>
        ///
        /// <param name="syncReplyBaseUri">  URI of the synchronise reply base.</param>
        /// <param name="asyncOneWayBaseUri">URI of the asynchronous one way base.</param>
		public ProtoBufServiceClient(string syncReplyBaseUri, string asyncOneWayBaseUri)
			: base(syncReplyBaseUri, asyncOneWayBaseUri) {}

        /// <summary>Serialize to stream.</summary>
        ///
        /// <exception cref="SerializationException">Thrown when a Serialization error condition occurs.</exception>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="request">       The request.</param>
        /// <param name="stream">        The stream.</param>
		public override void SerializeToStream(IRequestContext requestContext, object request, Stream stream)
		{
			try
			{
				Serializer.NonGeneric.Serialize(stream, request);
			}
			catch (Exception ex)
			{
				throw new SerializationException("ProtoBufServiceClient: Error serializing: " + ex.Message, ex);
			}
		}

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <exception cref="SerializationException">Thrown when a Serialization error condition occurs.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="stream">The stream.</param>
        ///
        /// <returns>A T.</returns>
		public override T DeserializeFromStream<T>(Stream stream)
		{
			try
			{
				return Serializer.Deserialize<T>(stream);
			}
			catch (Exception ex)
			{
				throw new SerializationException("ProtoBufServiceClient: Error deserializing: " + ex.Message, ex);
			}
		}

        /// <summary>Gets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
		public override string ContentType
		{
			get { return Common.Web.ContentType.ProtoBuf; }
		}

        /// <summary>Gets the stream deserializer.</summary>
        ///
        /// <value>The stream deserializer.</value>
		public override StreamDeserializerDelegate StreamDeserializer
		{
			get { return Deserialize; }
		}
		
		private static object Deserialize(Type type, Stream source)
		{
			try
			{
				return Serializer.NonGeneric.Deserialize(type, source);
			}
			catch (Exception ex)
			{
				throw new SerializationException("ProtoBufServiceClient: Error deserializing: " + ex.Message, ex);
			}
		}
	}
}
