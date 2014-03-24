using NUnit.Framework;
using NServiceKit.Html;
using NServiceKit.Tests.Html.Support.Types;

namespace NServiceKit.Tests.Html
{
	[TestFixture]
	public class InputExtensionTests
	{
		HtmlHelper<Person> html;

		[SetUp]
		public void SetUp()
		{
			html = new HtmlHelper<Person>();
		}

		[Test]
		public void TextBoxFor_ValidModelValue_GeneratesBothNameAndIdAttributes()
		{
			MvcHtmlString result = html.TextBoxFor(p => p.First);

			Assert.AreEqual(@"<input id=""First"" name=""First"" type=""text"" value="""" />", result.ToHtmlString());
		}

		[Test]
		public void TextBoxFor_NestedProperty_GeneratesNameAttributeWithDotAndIDWithUnderscore()
		{
			MvcHtmlString result = html.TextBoxFor(p => p.Home.City);

			Assert.AreEqual(@"<input id=""Home_City"" name=""Home.City"" type=""text"" value="""" />", result.ToHtmlString());
		}
	}
}
