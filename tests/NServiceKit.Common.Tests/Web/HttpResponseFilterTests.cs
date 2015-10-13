using NServiceKit.Common.Web;

using NUnit.Framework;

namespace NServiceKit.Common.Tests.Web
{
    /// <summary>
    ///     Unit tests for <see cref="HttpResponseFilter" />.
    /// </summary>
    [TestFixture]
    public class HttpResponseFilterTests
    {
        [Test]
        public void GetFormatContentType_WhenFormatIsLowercaseJSON_ShouldReturnJsonMediaType()
        {
            // Arrange
            // Act
            // Assert
            AssertThatGetFormatContentTypeReturnsMediaTypeForFormat("json", "application/json");
        }

        [Test]
        public void GetFormatContentType_WhenFormatIsUppercaseJSON_ShouldReturnJsonMediaType()
        {
            // Arrange
            // Act
            // Assert
            AssertThatGetFormatContentTypeReturnsMediaTypeForFormat("JSON", "application/json");
        }

        [Test]
        public void GetFormatContentType_WhenFormatIsMixedcaseJSON_ShouldReturnJsonMediaType()
        {
            // Arrange
            // Act
            // Assert
            AssertThatGetFormatContentTypeReturnsMediaTypeForFormat("JsOn", "application/json");
        }

        [Test]
        public void GetFormatContentType_WhenFormatIsLowercaseXML_ShouldReturnXMLMediaType()
        {
            // Arrange
            // Act
            // Assert
            AssertThatGetFormatContentTypeReturnsMediaTypeForFormat("xml", "application/xml");
        }

        [Test]
        public void GetFormatContentType_WhenFormatIsMixedcaseXML_ShouldReturnXMLMediaType()
        {
            // Arrange
            // Act
            // Assert
            AssertThatGetFormatContentTypeReturnsMediaTypeForFormat("XmL", "application/xml");
        }

        [Test]
        public void GetFormatContentType_WhenFormatIsUppercaseXML_ShouldReturnXMLMediaType()
        {
            // Arrange
            // Act
            // Assert
            AssertThatGetFormatContentTypeReturnsMediaTypeForFormat("XML", "application/xml");
        }

        [Test]
        public void GetFormatContentType_WhenFormatIsLowercaseJSV_ShouldReturnJSVMediaType()
        {
            // Arrange
            // Act
            // Assert
            AssertThatGetFormatContentTypeReturnsMediaTypeForFormat("jsv", "application/jsv");
        }

        [Test]
        public void GetFormatContentType_WhenFormatIsMixedcaseJSV_ShouldReturnJSVMediaType()
        {
            // Arrange
            // Act
            // Assert
            AssertThatGetFormatContentTypeReturnsMediaTypeForFormat("JsV", "application/jsv");
        }

        [Test]
        public void GetFormatContentType_WhenFormatIsUppercaseJSV_ShouldReturnJSVMediaType()
        {
            // Arrange
            // Act
            // Assert
            AssertThatGetFormatContentTypeReturnsMediaTypeForFormat("JSV", "application/jsv");
        }

        private void AssertThatGetFormatContentTypeReturnsMediaTypeForFormat(string format, string expectedMediaType)
        {
            // Arrange
            HttpResponseFilter responseFilter = new HttpResponseFilter();

            // Act
            string contentType = responseFilter.GetFormatContentType(format);

            // Assert
            Assert.That(contentType, Is.EqualTo(expectedMediaType));
        }
    }
}