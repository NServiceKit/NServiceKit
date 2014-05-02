using System;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests.ServiceClient.Web
{
    /// <summary>An URL extensions tests.</summary>
    [TestFixture]
    public class UrlExtensionsTests
    {
        /// <summary>Format variable date time offset value is URL encoded.</summary>
        [Test]
        public void FormatVariable_DateTimeOffsetValue_ValueIsUrlEncoded()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            var formattedVariable = RestRoute.FormatVariable(dateTimeOffset);
            var jsv = dateTimeOffset.ToJsv();
            Assert.AreEqual(Uri.EscapeDataString(jsv), formattedVariable);
        }

        /// <summary>Format query parameter value date time offset value is URL encoded.</summary>
        [Test]
        public void FormatQueryParameterValue_DateTimeOffsetValue_ValueIsUrlEncoded()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            var formattedVariable = RestRoute.FormatQueryParameterValue(dateTimeOffset);
            var jsv = dateTimeOffset.ToJsv();
            Assert.AreEqual(Uri.EscapeDataString(jsv), formattedVariable);
        }
    }
}
