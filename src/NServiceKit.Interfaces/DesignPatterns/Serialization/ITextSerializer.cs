using System;
using System.IO;

namespace NServiceKit.DesignPatterns.Serialization
{
    /// <summary>Interface for text serializer.</summary>
	public interface ITextSerializer
	{
        /// <summary>Deserialize from string.</summary>
        ///
        /// <param name="json">      The JSON.</param>
        /// <param name="returnType">Type of the return.</param>
        ///
        /// <returns>An object.</returns>
		object DeserializeFromString(string json, Type returnType);

        /// <summary>Deserialize from string.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="json">The JSON.</param>
        ///
        /// <returns>A T.</returns>
		T DeserializeFromString<T>(string json);

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="stream">The stream.</param>
        ///
        /// <returns>A T.</returns>
		T DeserializeFromStream<T>(Stream stream);

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <param name="type">  The type.</param>
        /// <param name="stream">The stream.</param>
        ///
        /// <returns>An object.</returns>
		object DeserializeFromStream(Type type, Stream stream);

        /// <summary>Serialize to string.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">The object.</param>
        ///
        /// <returns>A string.</returns>
		string SerializeToString<T>(T obj);

        /// <summary>Serialize to stream.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">   The object.</param>
        /// <param name="stream">The stream.</param>
		void SerializeToStream<T>(T obj, Stream stream);
	}
}