using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using NServiceKit.DesignPatterns.Serialization;

#if !SILVERLIGHT && !MONOTOUCH && !XBOX
using System.IO.Compression;
#endif

namespace NServiceKit.ServiceModel.Serialization
{
    /// <summary>A data contract serializer.</summary>
    public class DataContractSerializer : IStringSerializer 
    {
        private static readonly Encoding Encoding = Encoding.UTF8;// new UTF8Encoding(true);
        /// <summary>The instance.</summary>
        public static DataContractSerializer Instance = new DataContractSerializer();

        /// <summary>Parses the given from.</summary>
        ///
        /// <exception cref="SerializationException">Thrown when a Serialization error condition occurs.</exception>
        ///
        /// <typeparam name="XmlDto">Type of the XML dto.</typeparam>
        /// <param name="from">     Source for the.</param>
        /// <param name="indentXml">true to indent XML.</param>
        ///
        /// <returns>A string.</returns>
        public string Parse<XmlDto>(XmlDto from, bool indentXml)
        {
            try
            {
                if (Equals(@from, default(XmlDto))) return null;
                using (var ms = new MemoryStream())
                {
                    var serializer = new System.Runtime.Serialization.DataContractSerializer(from.GetType());
#if !SILVERLIGHT && !MONOTOUCH && !XBOX
                    using (var xw = new XmlTextWriter(ms, Encoding)) 
                    {
                        if (indentXml)
                        {
                            xw.Formatting = Formatting.Indented;	
                        }

                        serializer.WriteObject(xw, from);
                        xw.Flush();
#else
                        serializer.WriteObject(ms, from);
#endif

                        ms.Seek(0, SeekOrigin.Begin);
                        using (var reader = new StreamReader(ms))
                        {
                            return reader.ReadToEnd();
                        }

#if !SILVERLIGHT && !MONOTOUCH && !XBOX
                    }
#endif
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException(string.Format("Error serializing object of type {0}", from.GetType().FullName), ex);
            }
        }

        /// <summary>Parses the given from.</summary>
        ///
        /// <typeparam name="XmlDto">Type of the XML dto.</typeparam>
        /// <param name="from">Source for the.</param>
        ///
        /// <returns>A string.</returns>
        public string Parse<XmlDto>(XmlDto from)
        {
            return Parse(from, false);
        }

        /// <summary>Serialize to stream.</summary>
        ///
        /// <param name="obj">   The object.</param>
        /// <param name="stream">The stream.</param>
        public void SerializeToStream(object obj, Stream stream)
        {
#if !SILVERLIGHT && !MONOTOUCH && !XBOX
            using (var xw = new XmlTextWriter(stream, Encoding))
            {
                var serializer = new System.Runtime.Serialization.DataContractSerializer(obj.GetType());
                serializer.WriteObject(xw, obj);
            }
#else
            var serializer = new System.Runtime.Serialization.DataContractSerializer(obj.GetType());
            serializer.WriteObject(stream, obj);
#endif
        }

#if !SILVERLIGHT && !MONOTOUCH && !XBOX

        /// <summary>Compress to stream.</summary>
        ///
        /// <typeparam name="XmlDto">Type of the XML dto.</typeparam>
        /// <param name="from">  Source for the.</param>
        /// <param name="stream">The stream.</param>
        public void CompressToStream<XmlDto>(XmlDto from, Stream stream)
        {
            using (var deflateStream = new DeflateStream(stream, CompressionMode.Compress))
            using (var xw = new XmlTextWriter(deflateStream, Encoding))
            {
                var serializer = new System.Runtime.Serialization.DataContractSerializer(from.GetType());
                serializer.WriteObject(xw, from);
                xw.Flush();
            }
        }

        /// <summary>Compress the given from.</summary>
        ///
        /// <typeparam name="XmlDto">Type of the XML dto.</typeparam>
        /// <param name="from">Source for the.</param>
        ///
        /// <returns>A byte[].</returns>
        public byte[] Compress<XmlDto>(XmlDto from)
        {
            using (var ms = new MemoryStream())
            {
                CompressToStream(from, ms);
                
                return ms.ToArray();
            }
        }
#endif

    }
}
