using NUnit.Framework;
using NServiceKit.Messaging;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests
{
    /// <summary>An increment.</summary>
	public class Incr
	{
        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
		public int Value { get; set; }
	}

    /// <summary>A test user session.</summary>
    public class TestUserSession : AuthUserSession
    {
    }

    /// <summary>A messaging tests.</summary>
	[TestFixture]
	public class MessagingTests
	{
        /// <summary>Can serialize i message into typed message.</summary>
		[Test]
		public void Can_serialize_IMessage_into_typed_Message()
		{
			var dto = new Incr { Value = 1 };
			IMessage iMsg = MessageFactory.Create(dto);
			var json = iMsg.ToJson();
			var typedMessage = json.FromJson<Message<Incr>>();

			Assert.That(typedMessage.GetBody().Value, Is.EqualTo(dto.Value));
		}

        /// <summary>Can serialize object i message into typed message.</summary>
		[Test]
		public void Can_serialize_object_IMessage_into_typed_Message()
		{
			var dto = new Incr { Value = 1 };
			var iMsg = MessageFactory.Create(dto);
			var json = ((object)iMsg).ToJson();
			var typedMessage = json.FromJson<Message<Incr>>();

			Assert.That(typedMessage.GetBody().Value, Is.EqualTo(dto.Value));
		}

        /// <summary>Can serialize i message to bytes into typed message.</summary>
		[Test]
		public void Can_serialize_IMessage_ToBytes_into_typed_Message()
		{
			var dto = new Incr { Value = 1 };
			var iMsg = MessageFactory.Create(dto);
			var bytes = iMsg.ToBytes();
			var typedMessage = bytes.ToMessage<Incr>();

			Assert.That(typedMessage.GetBody().Value, Is.EqualTo(dto.Value));
		}
        
        /// <summary>Can deserialize concrete type into i/o authentication session.</summary>
		[Test]
		public void Can_deserialize_concrete_type_into_IOAuthSession()
		{
            var json = "{\"__type\":\"NServiceKit.Common.Tests.TestUserSession, NServiceKit.Common.Tests\",\"ReferrerUrl\":\"http://localhost:4629/oauth\",\"Id\":\"0412cc4654484111b2e7162a24a83753\",\"RequestToken\":\"dw4U1RUBr8r5Bx1oBZfdmNiocsMrAtBmSoFHYCZrr4\",\"RequestTokenSecret\":\"HNvCiD1a61CrutnxZoiJXQlLKNN1GAtWn7pRuafYN0\",\"CreatedAt\":\"\\/Date(1320221243138+0000)\\/\",\"LastModified\":\"\\/Date(1320221243138+0000)\\/\",\"Items\":{}}";
			var fromJson = json.FromJson<IAuthSession>();
			Assert.That(fromJson, Is.Not.Null);
		}
	}
}