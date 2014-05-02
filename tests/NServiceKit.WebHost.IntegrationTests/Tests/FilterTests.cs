using NUnit.Framework;
using NServiceKit.Common;
using NServiceKit.Text;

namespace NServiceKit.WebHost.IntegrationTests.Tests
{
    /// <summary>A filter tests.</summary>
	[TestFixture]
	public class FilterTests
	{
        /// <summary>Can call service returning string.</summary>
		[Test]
		public void Can_call_service_returning_string()
		{
			var response = Config.NServiceKitBaseUri.CombineWith("hello2/world")
				.GetJsonFromUrl();

			Assert.That(response, Is.EqualTo("world"));
		}
	}
}