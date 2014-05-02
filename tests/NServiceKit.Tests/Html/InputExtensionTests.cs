using NUnit.Framework;
using NServiceKit.Html;
using NServiceKit.Tests.Html.Support.Types;

namespace NServiceKit.Tests.Html
{
    /// <summary>An input extension tests.</summary>
	[TestFixture]
	public class InputExtensionTests
	{
		HtmlHelper<Person> html;

        /// <summary>Sets the up.</summary>
		[SetUp]
		public void SetUp()
		{
			html = new HtmlHelper<Person>();
		}

        /// <summary>Text box for valid model value generates both name and identifier attributes.</summary>
		[Test]
		public void TextBoxFor_ValidModelValue_GeneratesBothNameAndIdAttributes()
		{
			MvcHtmlString result = html.TextBoxFor(p => p.First);

			Assert.AreEqual(@"<input id=""First"" name=""First"" type=""text"" value="""" />", result.ToHtmlString());
		}

        /// <summary>Text box for nested property generates name attribute with dot and identifier with underscore.</summary>
		[Test]
		public void TextBoxFor_NestedProperty_GeneratesNameAttributeWithDotAndIDWithUnderscore()
		{
			MvcHtmlString result = html.TextBoxFor(p => p.Home.City);

			Assert.AreEqual(@"<input id=""Home_City"" name=""Home.City"" type=""text"" value="""" />", result.ToHtmlString());
		}
	}
}
