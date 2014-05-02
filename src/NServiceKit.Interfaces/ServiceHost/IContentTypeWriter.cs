using System.IO;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for content type writer.</summary>
	public interface IContentTypeWriter
	{
        /// <summary>Serialize to bytes.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="response">      The response.</param>
        ///
        /// <returns>A byte[].</returns>
        byte[] SerializeToBytes(IRequestContext requestContext, object response);

        /// <summary>Serialize to string.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="response">      The response.</param>
        ///
        /// <returns>A string.</returns>
        string SerializeToString(IRequestContext requestContext, object response);

        /// <summary>Serialize to stream.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="response">      The response.</param>
        /// <param name="toStream">      to stream.</param>
        void SerializeToStream(IRequestContext requestContext, object response, Stream toStream);

        /// <summary>Serialize to response.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="response">      The response.</param>
        /// <param name="httpRes">       The HTTP resource.</param>
		void SerializeToResponse(IRequestContext requestContext, object response, IHttpResponse httpRes);

        /// <summary>Gets response serializer.</summary>
        ///
        /// <param name="contentType">Type of the content.</param>
        ///
        /// <returns>The response serializer.</returns>
		ResponseSerializerDelegate GetResponseSerializer(string contentType);
	}

    /// <summary>Text serializer delegate.</summary>
    ///
    /// <param name="dto">The dto.</param>
    ///
    /// <returns>A string.</returns>
	public delegate string TextSerializerDelegate(object dto);

    /// <summary>Stream serializer delegate.</summary>
    ///
    /// <param name="requestContext">Context for the request.</param>
    /// <param name="dto">           The dto.</param>
    /// <param name="outputStream">  Stream to write data to.</param>
	public delegate void StreamSerializerDelegate(IRequestContext requestContext, object dto, Stream outputStream);

    /// <summary>Response serializer delegate.</summary>
    ///
    /// <param name="requestContext">Context for the request.</param>
    /// <param name="dto">           The dto.</param>
    /// <param name="httpRes">       The HTTP resource.</param>
	public delegate void ResponseSerializerDelegate(IRequestContext requestContext, object dto, IHttpResponse httpRes);
}