using System;
using System.IO;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for content type reader.</summary>
	public interface IContentTypeReader
	{
        /// <summary>Deserialize from string.</summary>
        ///
        /// <param name="contentType">Type of the content.</param>
        /// <param name="type">       The type.</param>
        /// <param name="request">    The request.</param>
        ///
        /// <returns>An object.</returns>
		object DeserializeFromString(string contentType, Type type, string request);

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <param name="contentType">  Type of the content.</param>
        /// <param name="type">         The type.</param>
        /// <param name="requestStream">The request stream.</param>
        ///
        /// <returns>An object.</returns>
		object DeserializeFromStream(string contentType, Type type, Stream requestStream);

        /// <summary>Gets stream deserializer.</summary>
        ///
        /// <param name="contentType">Type of the content.</param>
        ///
        /// <returns>The stream deserializer.</returns>
		StreamDeserializerDelegate GetStreamDeserializer(string contentType);
	}

    /// <summary>Text deserializer delegate.</summary>
    ///
    /// <param name="type">The type.</param>
    /// <param name="dto"> The dto.</param>
    ///
    /// <returns>An object.</returns>
	public delegate object TextDeserializerDelegate(Type type, string dto);

    /// <summary>Stream deserializer delegate.</summary>
    ///
    /// <param name="type">      The type.</param>
    /// <param name="fromStream">from stream.</param>
    ///
    /// <returns>An object.</returns>
	public delegate object StreamDeserializerDelegate(Type type, Stream fromStream);

}