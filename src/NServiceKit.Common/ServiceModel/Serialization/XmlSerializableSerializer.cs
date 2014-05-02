#if !SILVERLIGHT && !MONOTOUCH && !XBOX
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using NServiceKit.DesignPatterns.Serialization;
using NServiceKit.ServiceModel.Support;

namespace NServiceKit.ServiceModel.Serialization
{
    /// <summary>An XML serializable serializer.</summary>
    public class XmlSerializableSerializer : IStringSerializer 
    {
        /// <summary>The instance.</summary>
        public static XmlSerializableSerializer Instance = new XmlSerializableSerializer();

        /// <summary>Parses the given from.</summary>
        ///
        /// <exception cref="SerializationException">Thrown when a Serialization error condition occurs.</exception>
        ///
        /// <typeparam name="XmlDto">Type of the XML dto.</typeparam>
        /// <param name="from">Source for the.</param>
        ///
        /// <returns>A string.</returns>
        public string Parse<XmlDto>(XmlDto from)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (XmlWriter xw = new XmlTextWriter(ms, Encoding.UTF8))
                    {
                        var ser = new XmlSerializerWrapper(from.GetType());
                        ser.WriteObject(xw, from);
                        xw.Flush();
                        ms.Seek(0, SeekOrigin.Begin);
                        using (var reader = new StreamReader(ms))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException(string.Format("Error serializing object of type {0}", from.GetType().FullName), ex);
            }
        }
    }
}
#endif