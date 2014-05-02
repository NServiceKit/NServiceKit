using NUnit.Framework;
using NServiceKit.Html;
using NServiceKit.Tests.Html.Support.Types;

namespace NServiceKit.Tests.Html
{
    /// <summary>A label extensions tests.</summary>
	[TestFixture]
	public class LabelExtensionsTests
	{
		HtmlHelper<Person> html;

        /// <summary>Sets the up.</summary>
		[SetUp]
		public void SetUp()
		{
			html = new HtmlHelper<Person>();
		}

        /// <summary>Label for simple property for attribute is same as name.</summary>
		[Test]
		public void LabelFor_SimpleProperty_ForAttributeIsSameAsName()
		{
			MvcHtmlString result = html.LabelFor(p => p.First);

			Assert.AreEqual(@"<label for=""First"">First</label>", result.ToHtmlString());
		}

        /// <summary>Label for nested property for attribute references element identifier with underscores.</summary>
		[Test]
		public void LabelFor_NestedProperty_ForAttributeReferencesElementIDWithUnderscores()
		{
			MvcHtmlString result = html.LabelFor(p => p.Home.City);

			Assert.AreEqual(@"<label for=""Home_City"">City</label>", result.ToHtmlString());
		}
	}
}
