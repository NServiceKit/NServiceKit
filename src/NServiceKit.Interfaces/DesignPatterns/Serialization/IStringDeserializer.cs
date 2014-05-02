using System;

namespace NServiceKit.DesignPatterns.Serialization
{
    /// <summary>Interface for string deserializer.</summary>
    public interface IStringDeserializer
    {
        /// <summary>Parses.</summary>
        ///
        /// <typeparam name="To">Type of to.</typeparam>
        /// <param name="serializedText">The serialized text.</param>
        ///
        /// <returns>To.</returns>
        To Parse<To>(string serializedText);

        /// <summary>Parses.</summary>
        ///
        /// <param name="serializedText">The serialized text.</param>
        /// <param name="type">          The type.</param>
        ///
        /// <returns>An object.</returns>
        object Parse(string serializedText, Type type);
    }
}