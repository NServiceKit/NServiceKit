using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Text;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
	[TestFixture]
	public class FilterTests
	{
		[Test]
		public void Can_call_service_returning_string()
		{
			var response = Config.NServiceKitBaseUri.CombineWith("hello2/world")
				.GetJsonFromUrl();

			Assert.That(response, Is.EqualTo("world"));
		}
	}
}