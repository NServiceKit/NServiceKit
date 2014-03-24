using System;
using NUnit.Framework;
using NServiceKit.ServiceClient.Web;
using NServiceKit.Text;

namespace NServiceKit.Common.Tests.ServiceClient.Web
{
    [TestFixture]
    public class UrlExtensionsTests
    {
        [Test]
        public void FormatVariable_DateTimeOffsetValue_ValueIsUrlEncoded()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            var formattedVariable = RestRoute.FormatVariable(dateTimeOffset);
            var jsv = dateTimeOffset.ToJsv();
            Assert.AreEqual(Uri.EscapeDataString(jsv), formattedVariable);
        }

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
