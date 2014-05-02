using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using NUnit.Framework;
using NServiceKit.Messaging;
using NServiceKit.ServiceModel.Serialization;
using Message = System.ServiceModel.Channels.Message;
using DataContractSerializer = NServiceKit.ServiceModel.Serialization.DataContractSerializer;

namespace NServiceKit.WebHost.Endpoints.Tests
{
	[DataContract(Namespace = "http://schemas.NServiceKit.net/types")]
	public class Reverse
	{
        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>

        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
		[DataMember]
		public string Value { get; set; }
	}

    /// <summary>A message serialization tests.</summary>
	[TestFixture]
	public class MessageSerializationTests
	{
		static string xml = "<Reverse xmlns=\"http://schemas.NServiceKit.net/types\"><Value>test</Value></Reverse>";
		Reverse request = new Reverse { Value = "test" };
		string msgXml = "<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\"><s:Body>" + xml + "</s:Body></s:Envelope>";

        /// <summary>Can deserialize message from get body.</summary>
		[Test]
		public void Can_Deserialize_Message_from_GetBody()
		{
			var msg = Message.CreateMessage(MessageVersion.Default, "Reverse", request);
			//Console.WriteLine("BODY: " + msg.GetReaderAtBodyContents().ReadOuterXml());

			var fromRequest = msg.GetBody<Reverse>(new System.Runtime.Serialization.DataContractSerializer(typeof(Reverse)));
			Assert.That(fromRequest.Value, Is.EqualTo(request.Value));
		}

        /// <summary>Can deserialize message from get reader at body contents.</summary>
		[Test]
		public void Can_Deserialize_Message_from_GetReaderAtBodyContents()
		{
			var msg = Message.CreateMessage(MessageVersion.Default, "Reverse", request);
			using (var reader = msg.GetReaderAtBodyContents())
			{
				var requestXml = reader.ReadOuterXml();
				var fromRequest = (Reverse)DataContractDeserializer.Instance.Parse(requestXml, typeof(Reverse));
				Assert.That(fromRequest.Value, Is.EqualTo(request.Value));
			}
		}

		internal class SimpleBodyWriter : BodyWriter
		{
			private readonly string message;

            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.MessageSerializationTests.SimpleBodyWriter class.</summary>
            ///
            /// <param name="message">The message.</param>
			public SimpleBodyWriter(string message)
				: base(false)
			{
				this.message = message;
			}

            /// <summary>When implemented, provides an extensibility point when the body contents are written.</summary>
            ///
            /// <param name="writer">The <see cref="T:System.Xml.XmlDictionaryWriter" /> used to write out the message body.</param>
			protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
			{
				writer.WriteRaw(message);
			}
		}

        /// <summary>Can create entire message from XML.</summary>
		[Test]
		public void Can_create_entire_message_from_xml()
		{

			//var msg = Message.CreateMessage(MessageVersion.Default,
			//    "Reverse", new SimpleBodyWriter(msgXml));

			var doc = new XmlDocument();
			doc.LoadXml(msgXml);

			using (var xnr = new XmlNodeReader(doc))
			{
				var msg = Message.CreateMessage(xnr, msgXml.Length, MessageVersion.Soap12WSAddressingAugust2004);

				var xml = msg.GetReaderAtBodyContents().ReadOuterXml();
				Console.WriteLine("BODY: " + DataContractSerializer.Instance.Parse(request));
				Console.WriteLine("EXPECTED BODY: " + xml);

				var fromRequest = (Reverse)DataContractDeserializer.Instance.Parse(xml, typeof(Reverse));
				Assert.That(fromRequest.Value, Is.EqualTo(request.Value));
			}

			//var fromRequest = msg.GetBody<Request>(new DataContractSerializer(typeof(Request)));
		}

        /// <summary>What do the different SOAP payloads look like.</summary>
		[Test]
		public void What_do_the_different_soap_payloads_look_like()
		{
			var doc = new XmlDocument();
			doc.LoadXml(msgXml);

			//var action = "Request";
			string action = null;
			var soap12 = Message.CreateMessage(MessageVersion.Soap12, action, request);
			var soap12WSAddressing10 = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, action, request);
			var soap12WSAddressingAugust2004 = Message.CreateMessage(MessageVersion.Soap12WSAddressingAugust2004, action, request);

			Console.WriteLine("Soap12: " + GetMessageEnvelope(soap12));
			Console.WriteLine("Soap12WSAddressing10: " + GetMessageEnvelope(soap12WSAddressing10));
			Console.WriteLine("Soap12WSAddressingAugust2004: " + GetMessageEnvelope(soap12WSAddressingAugust2004));
		}

        /// <summary>Gets message envelope.</summary>
        ///
        /// <param name="msg">The message.</param>
        ///
        /// <returns>The message envelope.</returns>
		public string GetMessageEnvelope(Message msg)
		{
			var sb = new StringBuilder();
			using (var sw = XmlWriter.Create(new StringWriter(sb)))
			{
				msg.WriteMessage(sw);
				sw.Flush();
				return sb.ToString();
			}
		}

        /// <summary>Gets request message.</summary>
        ///
        /// <param name="requestXml">The request XML.</param>
        ///
        /// <returns>The request message.</returns>
		protected static Message GetRequestMessage(string requestXml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(requestXml);

			var msg = Message.CreateMessage(new XmlNodeReader(doc), int.MaxValue,
				MessageVersion.Soap11WSAddressingAugust2004);
			//var msg = Message.CreateMessage(MessageVersion.Soap12WSAddressingAugust2004, 
			//    "*", new XmlBodyWriter(requestXml));

			return msg;
		}

        /// <summary>Can create message from XML.</summary>
		[Test]
		public void Can_create_message_from_xml()
		{
			var requestXml =
				"<?xml version=\"1.0\" encoding=\"utf-8\"?>"
				+ "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\""
				+ " xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\""
				+ " xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body>"
				+ "<Reverse xmlns=\"http://schemas.NServiceKit.net/types\"><Value>Testing</Value></Reverse>"
				+ "</soap:Body></soap:Envelope>";

			var requestMsg = GetRequestMessage(requestXml);

			using (var reader = requestMsg.GetReaderAtBodyContents())
			{
				requestXml = reader.ReadOuterXml();
			}

			var requestType = typeof (Reverse);
			var request = (Reverse)DataContractDeserializer.Instance.Parse(requestXml, requestType);
			Assert.That(request.Value, Is.EqualTo("Testing"));
		}

        /// <summary>A dto body writer.</summary>
		public class DtoBodyWriter : BodyWriter
		{
			private readonly object dto;

            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.MessageSerializationTests.DtoBodyWriter class.</summary>
            ///
            /// <param name="dto">The dto.</param>
			public DtoBodyWriter(object dto)
				: base(true)
			{
				this.dto = dto;
			}

            /// <summary>When implemented, provides an extensibility point when the body contents are written.</summary>
            ///
            /// <param name="writer">The <see cref="T:System.Xml.XmlDictionaryWriter" /> used to write out the message body.</param>
			protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
			{
				var xml = DataContractSerializer.Instance.Parse(dto);
				writer.WriteString(xml);
			}
		}

        /// <summary>An XML body writer.</summary>
		public class XmlBodyWriter : BodyWriter
		{
			private readonly string xml;

            /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.MessageSerializationTests.XmlBodyWriter class.</summary>
            ///
            /// <param name="xml">The XML.</param>
			public XmlBodyWriter(string xml)
				: base(true)
			{
				this.xml = xml;
			}

            /// <summary>When implemented, provides an extensibility point when the body contents are written.</summary>
            ///
            /// <param name="writer">The <see cref="T:System.Xml.XmlDictionaryWriter" /> used to write out the message body.</param>
			protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
			{
				writer.WriteString(xml);
			}
		}
	}


}