#if !SILVERLIGHT && !MONOTOUCH && !XBOX
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using NServiceKit.DesignPatterns.Serialization;

namespace NServiceKit.ServiceModel.Serialization
{
    /// <summary>An XML serializable deserializer.</summary>
    public class XmlSerializableDeserializer : IStringDeserializer
    {
        /// <summary>The instance.</summary>
        public static XmlSerializableDeserializer Instance = new XmlSerializableDeserializer();

        /// <summary>Parses the given from.</summary>
        ///
        /// <typeparam name="To">Type of to.</typeparam>
        /// <param name="xml">The XML.</param>
        ///
        /// <returns>To.</returns>
        public To Parse<To>(string xml)
        {
            var type = typeof(To);
            return (To)Parse(xml, type);
        }

        /// <summary>Parses.</summary>
        ///
        /// <exception cref="SerializationException">Thrown when a Serialization error condition occurs.</exception>
        ///
        /// <param name="xml"> The XML.</param>
        /// <param name="type">The type.</param>
        ///
        /// <returns>An object.</returns>
        public object Parse(string xml, Type type)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(xml);
                using (var reader = XmlDictionaryReader.CreateTextReader(bytes, new XmlDictionaryReaderQuotas()))
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(type);
                    return serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException(string.Format("Error serializing object of type {0}", type.FullName), ex);
            }
        }

        /// <summary>Parses the given from.</summary>
        ///
        /// <exception cref="SerializationException">Thrown when a Serialization error condition occurs.</exception>
        ///
        /// <typeparam name="To">Type of to.</typeparam>
        /// <param name="from">Source for the.</param>
        ///
        /// <returns>To.</returns>
        public To Parse<To>(TextReader from)
        {
            var type = typeof(To);
            try
            {
                using (from)
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(type);
                    return (To)serializer.Deserialize(from);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException(string.Format("Error serializing object of type {0}", type.FullName), ex);
            }
        }

        /// <summary>Parses the given from.</summary>
        ///
        /// <exception cref="SerializationException">Thrown when a Serialization error condition occurs.</exception>
        ///
        /// <typeparam name="To">Type of to.</typeparam>
        /// <param name="from">Source for the.</param>
        ///
        /// <returns>To.</returns>
        public To Parse<To>(Stream from)
        {
            var type = typeof(To);
            try
            {
                using (var reader = XmlDictionaryReader.CreateTextReader(from, new XmlDictionaryReaderQuotas()))
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(type);
                    return (To)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException(string.Format("Error serializing object of type {0}", type.FullName), ex);
            }
        }
    }
}
#endif
