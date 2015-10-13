using System;

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
        public void GetFormatContentType_WhenFormatIsMixedcaseJSON_ShouldReturnJsonMediaType()
        {
            // Arrange
            // Act
            // Assert
            AssertThatGetFormatContentTypeReturnsMediaTypeForFormat("JsOn", "application/json");
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

        [Test]
        public void GetFormatContentType_WhenFormatIsInLowercaseAndRegistered_ShouldReturnRegisteredMediaType()
        {
            // Arrange
            HttpResponseFilter responseFilter = new HttpResponseFilter();
            responseFilter.ContentTypeFormats.Add("registered_format", "registeredMediaType");

            // Act
            string contentType = responseFilter.GetFormatContentType("registered_format");

            // Assert
            Assert.That(contentType, Is.EqualTo("registeredMediaType"));
        }

        [Test]
        public void GetFormatContentType_WhenFormatIsInUppercaseAndRegistered_ShouldReturnRegisteredMediaType()
        {
            // Arrange
            HttpResponseFilter responseFilter = new HttpResponseFilter();
            responseFilter.ContentTypeFormats.Add("REGISTERED_FORMAT", "registeredMediaType");

            // Act
            string contentType = responseFilter.GetFormatContentType("REGISTERED_FORMAT");

            // Assert
            Assert.That(contentType, Is.EqualTo("registeredMediaType"));
        }

        [Test]
        public void GetFormatContentType_WhenFormatIsUnregistered_ShouldReturnNull()
        {
            // Arrange
            // Act
            // Assert
            AssertThatGetFormatContentTypeReturnsNullForFormat("unregisteredFormat");
        }

        [Test]
        public void GetFormatContentType_WhenFormatIsEmpty_ShouldReturnNull()
        {
            // Arrange
            string emptyFormat = string.Empty;

            // Act
            // Assert
            AssertThatGetFormatContentTypeReturnsNullForFormat(emptyFormat);
        }

        [Test]
        public void GetFormatContentType_WhenFormatIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            HttpResponseFilter responseFilter = new HttpResponseFilter();

            string nullFormat = null;

            // Act
            TestDelegate getFormatContentTypeAction = () => responseFilter.GetFormatContentType(nullFormat);

            // Assert
            Assert.Throws<ArgumentNullException>(getFormatContentTypeAction);
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

        private void AssertThatGetFormatContentTypeReturnsNullForFormat(string format)
        {
            // Arrange
            HttpResponseFilter responseFilter = new HttpResponseFilter();

            // Act
            string contentType = responseFilter.GetFormatContentType(format);

            // Assert
            Assert.That(contentType, Is.Null);
        }
    }
}