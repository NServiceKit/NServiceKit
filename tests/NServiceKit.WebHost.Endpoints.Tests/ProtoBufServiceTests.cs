using System;
using System.Runtime.Serialization;
using System.Text;
using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.Plugins.ProtoBuf;
using NServiceKit.ServiceClient.Web;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A prototype buffer email.</summary>
	[DataContract]
	public class ProtoBufEmail
	{
        /// <summary>Gets or sets to address.</summary>
        ///
        /// <value>to address.</value>
		[DataMember(Order = 1)]
		public string ToAddress { get; set; }

        /// <summary>Gets or sets from address.</summary>
        ///
        /// <value>from address.</value>
		[DataMember(Order = 2)]
		public string FromAddress { get; set; }

        /// <summary>Gets or sets the subject.</summary>
        ///
        /// <value>The subject.</value>
		[DataMember(Order = 3)]
		public string Subject { get; set; }

        /// <summary>Gets or sets the body.</summary>
        ///
        /// <value>The body.</value>
		[DataMember(Order = 4)]
		public string Body { get; set; }

        /// <summary>Gets or sets information describing the attachment.</summary>
        ///
        /// <value>Information describing the attachment.</value>
		[DataMember(Order = 5)]
		public byte[] AttachmentData { get; set; }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="other">The prototype buffer email to compare to this object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public bool Equals(ProtoBufEmail other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.ToAddress, ToAddress) 
				&& Equals(other.FromAddress, FromAddress) 
				&& Equals(other.Subject, Subject) 
				&& Equals(other.Body, Body)
				&& other.AttachmentData.EquivalentTo(AttachmentData);
		}

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        ///
        /// <param name="obj">The object to compare with the current object.</param>
        ///
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (ProtoBufEmail)) return false;
			return Equals((ProtoBufEmail) obj);
		}

        /// <summary>Serves as a hash function for a particular type.</summary>
        ///
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				int result = (ToAddress != null ? ToAddress.GetHashCode() : 0);
				result = (result*397) ^ (FromAddress != null ? FromAddress.GetHashCode() : 0);
				result = (result*397) ^ (Subject != null ? Subject.GetHashCode() : 0);
				result = (result*397) ^ (Body != null ? Body.GetHashCode() : 0);
				result = (result*397) ^ (AttachmentData != null ? AttachmentData.GetHashCode() : 0);
				return result;
			}
		}
	}

    /// <summary>A prototype buffer email response.</summary>
	[DataContract]
	public class ProtoBufEmailResponse
	{
        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember(Order = 1)]
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A prototype buffer email service.</summary>
	public class ProtoBufEmailService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(ProtoBufEmail request)
		{
			return request;
		}
	}


    /// <summary>A prototype buffer service tests.</summary>
	[TestFixture]
	public class ProtoBufServiceTests
	{
        /// <summary>The listening on.</summary>
		protected const string ListeningOn = "http://localhost:85/";

		ExampleAppHostHttpListener appHost;

        /// <summary>Executes the test fixture set up action.</summary>
		[TestFixtureSetUp]
		public void OnTestFixtureSetUp()
		{
			LogManager.LogFactory = new ConsoleLogFactory();

			appHost = new ExampleAppHostHttpListener();
			appHost.Plugins.Add(new ProtoBufFormat());
			appHost.Init();
			appHost.Start(ListeningOn);
		}

        /// <summary>Executes the test fixture tear down action.</summary>
		[TestFixtureTearDown]
		public void OnTestFixtureTearDown()
		{
			Dispose();
		}

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			if (appHost == null) return;
			appHost.Dispose();
			appHost = null;
		}

        /// <summary>Can send prototype buffer request.</summary>
		[Test]
		public void Can_Send_ProtoBuf_request()
		{
			var client = new ProtoBufServiceClient(ListeningOn);

			var request = new ProtoBufEmail {
				ToAddress = "to@email.com",
				FromAddress = "from@email.com",
				Subject = "Subject",
				Body = "Body",
				AttachmentData = Encoding.UTF8.GetBytes("AttachmentData"),
			};

			try
			{
				var response = client.Send<ProtoBufEmail>(request);

				Console.WriteLine(response.Dump());

				Assert.That(response.Equals(request));
			}
			catch (WebServiceException webEx)
			{
				Console.WriteLine(webEx.ResponseDto.Dump());
			}
		}

	}
}