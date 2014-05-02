#if !SILVERLIGHT && !MONOTOUCH && !XBOX
using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace NServiceKit.ServiceModel.Support
{
    /// <summary>An XML serializer wrapper.</summary>
    public sealed class XmlSerializerWrapper : XmlObjectSerializer
    {
        System.Xml.Serialization.XmlSerializer serializer;
        string defaultNS;
        readonly Type objectType;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceModel.Support.XmlSerializerWrapper class.</summary>
        ///
        /// <param name="type">.</param>
        public XmlSerializerWrapper(Type type)
            : this(type, null, null)
        {

        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceModel.Support.XmlSerializerWrapper class.</summary>
        ///
        /// <param name="type">.</param>
        /// <param name="name">The name.</param>
        /// <param name="ns">  The ns.</param>
        public XmlSerializerWrapper(Type type, string name, string ns)
        {
            this.objectType = type;
            if (!String.IsNullOrEmpty(ns))
            {
                this.defaultNS = ns;
                this.serializer = new System.Xml.Serialization.XmlSerializer(type, ns);
            }
            else
            {
                this.defaultNS = GetNamespace(type);
                this.serializer = new System.Xml.Serialization.XmlSerializer(type);
            }
        }

        /// <summary>Gets a value that specifies whether the <see cref="T:System.Xml.XmlDictionaryReader" /> is positioned over an XML element that can be read.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="reader">An <see cref="T:System.Xml.XmlDictionaryReader" /> used to read the XML stream or document.</param>
        ///
        /// <returns>true if the reader can read the data; otherwise, false.</returns>
        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the XML stream or document with an <see cref="T:System.Xml.XmlDictionaryReader" /> and returns the deserialized object; it also enables you to specify whether the serializer can read the
        /// data before attempting to read it.
        /// </summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="reader">          An <see cref="T:System.Xml.XmlDictionaryReader" /> used to read the XML document.</param>
        /// <param name="verifyObjectName">true to check whether the enclosing XML element name and namespace correspond to the root name and root namespace; otherwise, false to skip the verification.
        /// </param>
        ///
        /// <returns>The deserialized object.</returns>
        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            throw new NotImplementedException();
        }

        /// <summary>Writes the end of the object data as a closing XML element to the XML document or stream with an <see cref="T:System.Xml.XmlDictionaryWriter" />.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="writer">An <see cref="T:System.Xml.XmlDictionaryWriter" /> used to write the XML document or stream.</param>
        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
            throw new NotImplementedException();
        }

        /// <summary>Writes only the content of the object to the XML document or stream using the specified <see cref="T:System.Xml.XmlDictionaryWriter" />.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="writer">An <see cref="T:System.Xml.XmlDictionaryWriter" /> used to write the XML document or stream.</param>
        /// <param name="graph"> The object that contains the content to write.</param>
        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            throw new NotImplementedException();
        }

        /// <summary>Writes the start of the object's data as an opening XML element using the specified <see cref="T:System.Xml.XmlDictionaryWriter" />.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="writer">An <see cref="T:System.Xml.XmlDictionaryWriter" /> used to write the XML document.</param>
        /// <param name="graph"> The object to serialize.</param>
        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
            throw new NotImplementedException();
        }

        /// <summary>Writes the complete content (start, content, and end) of the object to the XML document or stream with the specified <see cref="T:System.Xml.XmlDictionaryWriter" />.</summary>
        ///
        /// <param name="writer">An <see cref="T:System.Xml.XmlDictionaryWriter" /> used to write the content to the XML document or stream.</param>
        /// <param name="graph"> The object that contains the content to write.</param>
        public override void WriteObject(XmlDictionaryWriter writer, object graph)
        {
            this.serializer.Serialize(writer, graph);
        }

        /// <summary>Reads the XML document or stream with an <see cref="T:System.Xml.XmlDictionaryReader" /> and returns the deserialized object.</summary>
        ///
        /// <param name="reader">An <see cref="T:System.Xml.XmlDictionaryReader" /> used to read the XML document.</param>
        ///
        /// <returns>The deserialized object.</returns>
        public override object ReadObject(XmlDictionaryReader reader)
        {
            string readersNS;

            readersNS = (String.IsNullOrEmpty(reader.NamespaceURI)) ? "" : reader.NamespaceURI;
            if (String.Compare(this.defaultNS, readersNS) != 0)
            {
                this.serializer = new System.Xml.Serialization.XmlSerializer(this.objectType, readersNS);
                this.defaultNS = readersNS;
            }

            return (this.serializer.Deserialize(reader));
        }

        /// <summary>
        /// Gets the namespace from an attribute marked on the type's definition
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Namespace of type</returns>
        public static string GetNamespace(Type type)
        {
            Attribute[] attrs = (Attribute[])type.GetCustomAttributes(typeof(DataContractAttribute), true);
            if (attrs.Length > 0)
            {
                DataContractAttribute dcAttr = (DataContractAttribute)attrs[0];
                return dcAttr.Namespace;
            }
            attrs = (Attribute[])type.GetCustomAttributes(typeof(XmlRootAttribute), true);
            if (attrs.Length > 0)
            {
                XmlRootAttribute xmlAttr = (XmlRootAttribute)attrs[0];
                return xmlAttr.Namespace;
            }
            attrs = (Attribute[])type.GetCustomAttributes(typeof(XmlTypeAttribute), true);
            if (attrs.Length > 0)
            {
                XmlTypeAttribute xmlAttr = (XmlTypeAttribute)attrs[0];
                return xmlAttr.Namespace;
            }
            attrs = (Attribute[])type.GetCustomAttributes(typeof(XmlElementAttribute), true);
            if (attrs.Length > 0)
            {
                XmlElementAttribute xmlAttr = (XmlElementAttribute)attrs[0];
                return xmlAttr.Namespace;
            }
            return null;
        }
    }
}
#endif