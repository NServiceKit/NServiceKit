using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using NServiceKit.DesignPatterns.Serialization;
using NServiceKit.Text;

namespace NServiceKit.ServiceModel.Serialization
{
    /// <summary>A JSON data contract deserializer.</summary>
    public class JsonDataContractDeserializer 
    {
        /// <summary>The instance.</summary>
        public static JsonDataContractDeserializer Instance = new JsonDataContractDeserializer();

        /// <summary>Gets or sets the text serializer.</summary>
        ///
        /// <value>The text serializer.</value>
        public ITextSerializer TextSerializer { get; set; }

        /// <summary>Gets or sets a value indicating whether this object use bcl.</summary>
        ///
        /// <value>true if use bcl, false if not.</value>
        public bool UseBcl { get; set; }

        /// <summary>Deserialize from string.</summary>
        ///
        /// <exception cref="SerializationException">Thrown when a Serialization error condition occurs.</exception>
        ///
        /// <param name="json">      The JSON.</param>
        /// <param name="returnType">Type of the return.</param>
        ///
        /// <returns>An object.</returns>
        public object DeserializeFromString(string json, Type returnType)
        {
            if (TextSerializer != null)
                return TextSerializer.DeserializeFromString(json, returnType);

#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
            if (!UseBcl)
                return JsonSerializer.DeserializeFromString(json, returnType);

            try
            {
                using (var ms = new MemoryStream())
                {
                    var bytes = Encoding.UTF8.GetBytes(json);
                    ms.Write(bytes, 0, bytes.Length);
                    ms.Position = 0;
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(returnType);
                    return serializer.ReadObject(ms);
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("JsonDataContractDeserializer: Error converting to type: " + ex.Message, ex);
            }
#else
                return JsonSerializer.DeserializeFromString(json, returnType);
#endif
        }

        /// <summary>Deserialize from string.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="json">The JSON.</param>
        ///
        /// <returns>A T.</returns>
        public T DeserializeFromString<T>(string json)
        {
            if (TextSerializer != null)
                return TextSerializer.DeserializeFromString<T>(json);

            if (UseBcl)
                return (T)DeserializeFromString(json, typeof(T));

            return JsonSerializer.DeserializeFromString<T>(json);
        }

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="stream">The stream.</param>
        ///
        /// <returns>A T.</returns>
        public T DeserializeFromStream<T>(Stream stream)
        {
            if (TextSerializer != null)
                return TextSerializer.DeserializeFromStream<T>(stream);

#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
            if (UseBcl)
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);				
            }
#endif
            return JsonSerializer.DeserializeFromStream<T>(stream);
        }

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <param name="type">  The type.</param>
        /// <param name="stream">The stream.</param>
        ///
        /// <returns>An object.</returns>
        public object DeserializeFromStream(Type type, Stream stream)
        {
            if (TextSerializer != null)
                return TextSerializer.DeserializeFromStream(type, stream);

#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
            if (UseBcl)
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(type);
                return serializer.ReadObject(stream);
            }
#endif

            return JsonSerializer.DeserializeFromStream(type, stream);
        }
    }
}
