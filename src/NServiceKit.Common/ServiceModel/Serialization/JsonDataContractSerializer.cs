using System;
using System.IO;
using System.Runtime.Serialization;
using NServiceKit.DesignPatterns.Serialization;
using NServiceKit.Text;

namespace NServiceKit.ServiceModel.Serialization
{
    /// <summary>A JSON data contract serializer.</summary>
    public class JsonDataContractSerializer 
    {
        /// <summary>The instance.</summary>
        public static JsonDataContractSerializer Instance = new JsonDataContractSerializer();

        /// <summary>Gets or sets the text serializer.</summary>
        ///
        /// <value>The text serializer.</value>
        public ITextSerializer TextSerializer { get; set; }

        /// <summary>Use serializer.</summary>
        ///
        /// <param name="textSerializer">The text serializer.</param>
        public static void UseSerializer(ITextSerializer textSerializer)
        {
            Instance.TextSerializer = textSerializer;
            JsonDataContractDeserializer.Instance.TextSerializer = textSerializer;
        }

        /// <summary>Gets or sets a value indicating whether this object use bcl.</summary>
        ///
        /// <value>true if use bcl, false if not.</value>
        public bool UseBcl { get; set; }

        /// <summary>Serialize to string.</summary>
        ///
        /// <exception cref="SerializationException">Thrown when a Serialization error condition occurs.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">The object.</param>
        ///
        /// <returns>A string.</returns>
        public string SerializeToString<T>(T obj)
        {
            if (TextSerializer != null)
                return TextSerializer.SerializeToString(obj);

#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
            if (!UseBcl)
                return JsonSerializer.SerializeToString(obj);

            if (obj == null) return null;
            var type = obj.GetType();
            try
            {
                using (var ms = new MemoryStream())
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(type);
                    serializer.WriteObject(ms, obj);
                    ms.Position = 0;
                    using (var sr = new StreamReader(ms))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("JsonDataContractSerializer: Error converting type: " + ex.Message, ex);
            }
#else
                return JsonSerializer.SerializeToString(obj);
#endif
        }

        /// <summary>Serialize to stream.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="obj">   The object.</param>
        /// <param name="stream">The stream.</param>
        public void SerializeToStream<T>(T obj, Stream stream)
        {
            if (TextSerializer != null)
            {
                TextSerializer.SerializeToStream(obj, stream);
            }
#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
            else if (UseBcl)
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(stream, obj);
            }
#endif
            else
            {
                JsonSerializer.SerializeToStream(obj, stream);
            }
        }
    }
}
