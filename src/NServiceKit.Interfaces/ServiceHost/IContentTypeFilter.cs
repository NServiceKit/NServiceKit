using System.Collections.Generic;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for content type filter.</summary>
	public interface IContentTypeFilter
		: IContentTypeWriter, IContentTypeReader
	{
        /// <summary>Gets format content type.</summary>
        ///
        /// <param name="format">Describes the format to use.</param>
        ///
        /// <returns>The format content type.</returns>
        string GetFormatContentType(string format);

        /// <summary>Gets the content type formats.</summary>
        ///
        /// <value>The content type formats.</value>
		Dictionary<string, string> ContentTypeFormats { get; }

        /// <summary>Registers this object.</summary>
        ///
        /// <param name="contentType">       Type of the content.</param>
        /// <param name="streamSerializer">  The stream serializer.</param>
        /// <param name="streamDeserializer">The stream deserializer.</param>
		void Register(string contentType,
			StreamSerializerDelegate streamSerializer, StreamDeserializerDelegate streamDeserializer);

        /// <summary>Registers this object.</summary>
        ///
        /// <param name="contentType">       Type of the content.</param>
        /// <param name="responseSerializer">The response serializer.</param>
        /// <param name="streamDeserializer">The stream deserializer.</param>
		void Register(string contentType,
			ResponseSerializerDelegate responseSerializer, StreamDeserializerDelegate streamDeserializer);

        /// <summary>Clears the custom filters.</summary>
		void ClearCustomFilters();
	}

}