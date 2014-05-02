using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using NServiceKit.DesignPatterns.Serialization;

namespace NServiceKit.ServiceModel.Serialization
{

    /// <summary>A data contract deserializer.</summary>
    public class DataContractDeserializer : IStringDeserializer
    {

        /// <summary>
        /// Default MaxStringContentLength is 8k, and throws an exception when reached
        /// </summary>
#if !SILVERLIGHT && !MONOTOUCH && !XBOX
        private readonly XmlDictionaryReaderQuotas quotas;
#endif

        /// <summary>The instance.</summary>
        public static DataContractDeserializer Instance 
            = new DataContractDeserializer(
#if !SILVERLIGHT && !MONOTOUCH && !XBOX
                new XmlDictionaryReaderQuotas { MaxStringContentLength = 1024 * 1024, }
#endif
                );

        /// <summary>Initializes a new instance of the NServiceKit.ServiceModel.Serialization.DataContractDeserializer class.</summary>
        ///
        /// <param name="quotas">Default MaxStringContentLength is 8k, and throws an exception when reached.</param>
        public DataContractDeserializer(
#if !SILVERLIGHT && !MONOTOUCH && !XBOX
            XmlDictionaryReaderQuotas quotas=null
#endif
            )
        {
#if !SILVERLIGHT && !MONOTOUCH && !XBOX
            this.quotas = quotas;
#endif
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

#if MONOTOUCH				
                using (var reader = XmlDictionaryReader.CreateTextReader(bytes, null))
#elif SILVERLIGHT && !WINDOWS_PHONE
                using (var reader = XmlDictionaryReader.CreateTextReader(bytes, XmlDictionaryReaderQuotas.Max))
#elif WINDOWS_PHONE
                using (var reader = XmlDictionaryReader.CreateBinaryReader(bytes, XmlDictionaryReaderQuotas.Max))
#else
                using (var reader = XmlDictionaryReader.CreateTextReader(bytes, this.quotas))
#endif
                {
                    var serializer = new System.Runtime.Serialization.DataContractSerializer(type);
                    return serializer.ReadObject(reader);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("DeserializeDataContract: Error converting type: " + ex.Message, ex);
            }
        }

        /// <summary>Parses.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="xml">The XML.</param>
        ///
        /// <returns>A T.</returns>
        public T Parse<T>(string xml)
        {
            var type = typeof(T);
            return (T)Parse(xml, type);
        }

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="stream">The stream.</param>
        ///
        /// <returns>A T.</returns>
        public T DeserializeFromStream<T>(Stream stream)
        {
            var serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }

    }


}
