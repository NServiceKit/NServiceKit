using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Funq;
using NUnit.Framework;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Utils;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints.Extensions;
using NServiceKit.WebHost.Endpoints.Support.Mocks;
using NServiceKit.WebHost.Endpoints.Tests.Mocks;

namespace NServiceKit.WebHost.Endpoints.Tests
{
    /// <summary>A partial file.</summary>
    [Route("/partialfiles/{RelativePath*}")]
    public class PartialFile
    {
        /// <summary>Gets or sets the full pathname of the relative file.</summary>
        ///
        /// <value>The full pathname of the relative file.</value>
        public string RelativePath { get; set; }

        /// <summary>Gets or sets the type of the mime.</summary>
        ///
        /// <value>The type of the mime.</value>
        public string MimeType { get; set; }
    }

    /// <summary>A partial from memory.</summary>
    [Route("/partialfiles/memory")]
    public class PartialFromMemory { }

    /// <summary>A partial from text.</summary>
    [Route("/partialfiles/text")]
    public class PartialFromText { }

    /// <summary>A partial content service.</summary>
    public class PartialContentService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(PartialFile request)
        {
            if (request.RelativePath.IsNullOrEmpty())
                throw new ArgumentNullException("RelativePath");

            string filePath = "~/{0}".Fmt(request.RelativePath).MapProjectPath();
            if (!File.Exists(filePath))
                throw new FileNotFoundException(request.RelativePath);

            return new HttpResult(new FileInfo(filePath), request.MimeType);
        }

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(PartialFromMemory request)
        {
            var customText = "123456789012345678901234567890";
            var customTextBytes = customText.ToUtf8Bytes();
            var ms = new MemoryStream();
            ms.Write(customTextBytes, 0, customTextBytes.Length);

            var httpResult = new HttpResult(ms, "audio/mpeg");
            return httpResult;
        }

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(PartialFromText request)
        {
            const string customText = "123456789012345678901234567890";
            var httpResult = new HttpResult(customText, "text/plain");
            return httpResult;
        }
    }

    /// <summary>A partial content application host.</summary>
    public class PartialContentAppHost : AppHostHttpListenerBase
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.PartialContentAppHost class.</summary>
        public PartialContentAppHost() : base(typeof(PartialFile).Name, typeof(PartialFile).Assembly) { }

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
        public override void Configure(Container container) {}
    }

    /// <summary>A partial content result tests.</summary>
    [TestFixture]
    public class PartialContentResultTests
    {
        /// <summary>URI of the base.</summary>
        public const string BaseUri = Config.NServiceKitBaseUri;
        /// <summary>The listening on.</summary>
        public const string ListeningOn = Config.AbsoluteBaseUri;

        private PartialContentAppHost appHost;

        readonly FileInfo uploadedFile = new FileInfo("~/TestExistingDir/upload.html".MapProjectPath());
        readonly FileInfo uploadedTextFile = new FileInfo("~/TestExistingDir/textfile.txt".MapProjectPath());

        /// <summary>Text fixture set up.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        [TestFixtureSetUp]
        public void TextFixtureSetUp()
        {
            try
            {
                appHost = new PartialContentAppHost();
                appHost.Init();
                appHost.Start(ListeningOn);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (appHost != null) appHost.Dispose();
            appHost = null;
        }

        /// <summary>Can static file get 200 ok response for file with no range header.</summary>
        [Test]
        public void Can_StaticFile_GET_200_OK_response_for_file_with_no_range_header()
        {
            "File size {0}".Print(uploadedFile.Length);

            byte[] actualContents = "{0}/TestExistingDir/upload.html".Fmt(BaseUri).GetBytesFromUrl(
                responseFilter: httpRes => "Content-Length header {0}".Print(httpRes.Headers["Content-Length"]));

            "response size {0}".Fmt(actualContents.Length);

            Assert.That(actualContents.Length, Is.EqualTo(uploadedFile.Length));
        }

        /// <summary>Can get 200 ok response for file with no range header.</summary>
        [Test]
        public void Can_GET_200_OK_response_for_file_with_no_range_header()
        {
            "File size {0}".Print(uploadedFile.Length);

            byte[] actualContents = "{0}/partialfiles/TestExistingDir/upload.html".Fmt(BaseUri).GetBytesFromUrl(
                responseFilter: httpRes => "Content-Length header {0}".Print(httpRes.Headers["Content-Length"]));

            "response size {0}".Fmt(actualContents.Length);

            Assert.That(actualContents.Length, Is.EqualTo(uploadedFile.Length));
        }

        /// <summary>Can static file get 206 partial response for file with range header.</summary>
        [Test]
        public void Can_StaticFile_GET_206_Partial_response_for_file_with_range_header()
        {
            var actualContents = "{0}/TestExistingDir/upload.html".Fmt(BaseUri).GetStringFromUrl(
                requestFilter: httpReq => httpReq.AddRange(5, 11),
                responseFilter: httpRes =>
                {
                    "Content-Length header {0}".Print(httpRes.Headers["Content-Length"]);
                    Assert.That(httpRes.ContentType, Is.EqualTo(MimeTypes.GetMimeType(uploadedFile.Name)));
                });

            "Response length {0}".Print(actualContents.Length);
            Assert.That(actualContents, Is.EqualTo("DOCTYPE"));
        }

        /// <summary>Can get 206 partial response for file with range header.</summary>
        [Test]
        public void Can_GET_206_Partial_response_for_file_with_range_header()
        {
            var actualContents = "{0}/partialfiles/TestExistingDir/upload.html".Fmt(BaseUri).GetStringFromUrl(
                requestFilter: httpReq => httpReq.AddRange(5, 11),
                responseFilter: httpRes =>
                {
                    "Content-Length header {0}".Print(httpRes.Headers["Content-Length"]);
                    Assert.That(httpRes.ContentType, Is.EqualTo(MimeTypes.GetMimeType(uploadedFile.Name)));
                });

            "Response length {0}".Print(actualContents.Length);
            Assert.That(actualContents, Is.EqualTo("DOCTYPE"));
        }

        /// <summary>Can get 206 partial response for memory with range header.</summary>
        [Test]
        public void Can_GET_206_Partial_response_for_memory_with_range_header()
        {
            var actualContents = "{0}/partialfiles/memory?mimeType=audio/mpeg".Fmt(BaseUri).GetStringFromUrl(
                requestFilter: httpReq => httpReq.AddRange(5, 9),
                responseFilter: httpRes => "Content-Length header {0}".Print(httpRes.Headers["Content-Length"]));

            "Response Length {0}".Print(actualContents.Length);
            Assert.That(actualContents, Is.EqualTo("67890"));
        }

        /// <summary>Can get 206 partial response for text with range header.</summary>
        [Test]
        public void Can_GET_206_Partial_response_for_text_with_range_header()
        {
            var actualContents = "{0}/partialfiles/text".Fmt(BaseUri).GetStringFromUrl(
                requestFilter: httpReq => httpReq.AddRange(5, 9),
                responseFilter: httpRes => "Content-Length header {0}".Print(httpRes.Headers["Content-Length"]));

            "Response Length {0}".Print(actualContents.Length);
            Assert.That(actualContents, Is.EqualTo("67890"));
        }

        /// <summary>Can respond to non range requests with 200 ok response.</summary>
        [Test]
        public void Can_respond_to_non_range_requests_with_200_OK_response()
        {
            var mockRequest = new HttpRequestMock();
            var mockResponse = new HttpResponseMock();

            string customText = "1234567890";
            byte[] customTextBytes = customText.ToUtf8Bytes();
            var ms = new MemoryStream();
            ms.Write(customTextBytes, 0, customTextBytes.Length);

            var httpResult = new HttpResult(ms, "audio/mpeg");            

            bool reponseWasAutoHandled = mockResponse.WriteToResponse(mockRequest, httpResult);
            Assert.That(reponseWasAutoHandled, Is.True);

            string writtenString = mockResponse.GetOutputStreamAsString();
            Assert.That(writtenString, Is.EqualTo(customText));

            Assert.That(mockResponse.Headers["Content-Range"], Is.Null);
            Assert.That(mockResponse.Headers["Accept-Ranges"], Is.EqualTo("bytes"));
            Assert.That(mockResponse.StatusCode, Is.EqualTo(200));
        }

        /// <summary>Can seek from beginning to end.</summary>
        [Test]
        public void Can_seek_from_beginning_to_end()
        {
            var mockRequest = new HttpRequestMock();
            var mockResponse = new HttpResponseMock();

            mockRequest.Headers[HttpHeaders.Range] = "bytes=0";

            string customText = "1234567890";
            byte[] customTextBytes = customText.ToUtf8Bytes();
            var ms = new MemoryStream();
            ms.Write(customTextBytes, 0, customTextBytes.Length);

            var httpResult = new HttpResult(ms, "audio/mpeg");

            bool reponseWasAutoHandled = mockResponse.WriteToResponse(mockRequest, httpResult);
            Assert.That(reponseWasAutoHandled, Is.True);

            string writtenString = mockResponse.GetOutputStreamAsString();
            Assert.That(writtenString, Is.EqualTo(customText));

            Assert.That(mockResponse.Headers["Content-Range"], Is.EqualTo("bytes 0-9/10"));
            Assert.That(mockResponse.Headers["Content-Length"], Is.EqualTo(writtenString.Length.ToString()));
            Assert.That(mockResponse.Headers["Accept-Ranges"], Is.EqualTo("bytes"));
            Assert.That(mockResponse.StatusCode, Is.EqualTo(206));
        }

        /// <summary>Can seek from beginning to further than end.</summary>
        [Test]
        public void Can_seek_from_beginning_to_further_than_end()
        {
            // Not sure if this would ever occur in real streaming scenarios, but it does occur
            // when some crawlers use range headers to specify a max size to return.
            // e.g. Facebook crawler always sends range header of 'bytes=0-524287'.

            var mockRequest = new HttpRequestMock();
            var mockResponse = new HttpResponseMock();

            mockRequest.Headers[HttpHeaders.Range] = "bytes=0-524287";

            string customText = "1234567890";
            byte[] customTextBytes = customText.ToUtf8Bytes();
            var ms = new MemoryStream();
            ms.Write(customTextBytes, 0, customTextBytes.Length);

            var httpResult = new HttpResult(ms, "audio/mpeg");

            bool reponseWasAutoHandled = mockResponse.WriteToResponse(mockRequest, httpResult);
            Assert.That(reponseWasAutoHandled, Is.True);

            string writtenString = mockResponse.GetOutputStreamAsString();
            Assert.That(writtenString, Is.EqualTo(customText));

            Assert.That(mockResponse.Headers["Content-Range"], Is.EqualTo("bytes 0-9/10"));
            Assert.That(mockResponse.Headers["Content-Length"], Is.EqualTo(writtenString.Length.ToString()));
            Assert.That(mockResponse.Headers["Accept-Ranges"], Is.EqualTo("bytes"));
            Assert.That(mockResponse.StatusCode, Is.EqualTo(206));
        }

        /// <summary>Can seek from beginning to middle.</summary>
        [Test]
        public void Can_seek_from_beginning_to_middle()
        {
            var mockRequest = new HttpRequestMock();
            var mockResponse = new HttpResponseMock();

            mockRequest.Headers[HttpHeaders.Range] = "bytes=0-2";

            string customText = "1234567890";
            byte[] customTextBytes = customText.ToUtf8Bytes();
            var ms = new MemoryStream();
            ms.Write(customTextBytes, 0, customTextBytes.Length);


            var httpResult = new HttpResult(ms, "audio/mpeg");

            bool reponseWasAutoHandled = mockResponse.WriteToResponse(mockRequest, httpResult);
            Assert.That(reponseWasAutoHandled, Is.True);

            string writtenString = mockResponse.GetOutputStreamAsString();
            Assert.That(writtenString, Is.EqualTo("123"));

            Assert.That(mockResponse.Headers["Content-Range"], Is.EqualTo("bytes 0-2/10"));
            Assert.That(mockResponse.Headers["Content-Length"], Is.EqualTo(writtenString.Length.ToString()));
            Assert.That(mockResponse.Headers["Accept-Ranges"], Is.EqualTo("bytes"));
            Assert.That(mockResponse.StatusCode, Is.EqualTo(206));
        }

        /// <summary>Can seek from middle to end.</summary>
        [Test]
        public void Can_seek_from_middle_to_end()
        {
            var mockRequest = new HttpRequestMock();
            mockRequest.Headers.Add("Range", "bytes=4-");
            var mockResponse = new HttpResponseMock();

            string customText = "1234567890";
            byte[] customTextBytes = customText.ToUtf8Bytes();
            var ms = new MemoryStream();
            ms.Write(customTextBytes, 0, customTextBytes.Length);


            var httpResult = new HttpResult(ms, "audio/mpeg");

            bool reponseWasAutoHandled = mockResponse.WriteToResponse(mockRequest, httpResult);
            Assert.That(reponseWasAutoHandled, Is.True);

            string writtenString = mockResponse.GetOutputStreamAsString();
            Assert.That(writtenString, Is.EqualTo("567890"));

            Assert.That(mockResponse.Headers["Content-Range"], Is.EqualTo("bytes 4-9/10"));
            Assert.That(mockResponse.Headers["Content-Length"], Is.EqualTo(writtenString.Length.ToString()));
            Assert.That(mockResponse.Headers["Accept-Ranges"], Is.EqualTo("bytes"));
            Assert.That(mockResponse.StatusCode, Is.EqualTo(206));
        }

        /// <summary>Can seek from middle to middle.</summary>
        [Test]
        public void Can_seek_from_middle_to_middle()
        {
            var mockRequest = new HttpRequestMock();
            mockRequest.Headers.Add("Range", "bytes=3-5");
            var mockResponse = new HttpResponseMock();

            string customText = "1234567890";
            byte[] customTextBytes = customText.ToUtf8Bytes();
            var ms = new MemoryStream();
            ms.Write(customTextBytes, 0, customTextBytes.Length);


            var httpResult = new HttpResult(ms, "audio/mpeg");

            bool reponseWasAutoHandled = mockResponse.WriteToResponse(mockRequest, httpResult);
            Assert.That(reponseWasAutoHandled, Is.True);

            string writtenString = mockResponse.GetOutputStreamAsString();
            Assert.That(writtenString, Is.EqualTo("456"));

            Assert.That(mockResponse.Headers["Content-Range"], Is.EqualTo("bytes 3-5/10"));
            Assert.That(mockResponse.Headers["Content-Length"], Is.EqualTo(writtenString.Length.ToString()));
            Assert.That(mockResponse.Headers["Accept-Ranges"], Is.EqualTo("bytes"));
            Assert.That(mockResponse.StatusCode, Is.EqualTo(206));
        }

        /// <summary>Can use file stream.</summary>
        [Test]
        public void Can_use_fileStream()
        {
            byte[] fileBytes = uploadedTextFile.ReadFully();
            string fileText = Encoding.ASCII.GetString(fileBytes);

            "File content size {0}".Print(fileBytes.Length);
            "File content is {0}".Print(fileText);

            var mockRequest = new HttpRequestMock();
            var mockResponse = new HttpResponseMock();
            mockRequest.Headers.Add("Range", "bytes=6-8");

            var httpResult = new HttpResult(uploadedTextFile, "audio/mpeg");

            bool reponseWasAutoHandled = mockResponse.WriteToResponse(mockRequest, httpResult);
            Assert.That(reponseWasAutoHandled, Is.True);

            string writtenString = mockResponse.GetOutputStreamAsString();
            Assert.That(writtenString, Is.EqualTo(fileText.Substring(6, 3)));

            Assert.That(mockResponse.Headers["Content-Range"], Is.EqualTo("bytes 6-8/33"));
            Assert.That(mockResponse.Headers["Content-Length"], Is.EqualTo(writtenString.Length.ToString()));
            Assert.That(mockResponse.Headers["Accept-Ranges"], Is.EqualTo("bytes"));
            Assert.That(mockResponse.StatusCode, Is.EqualTo(206));
        }

        /// <summary>Executes for 30secs operation.</summary>
        [Test]
        [Explicit("Helps debugging when you need to find out WTF is going on")]
        public void Run_for_30secs()
        {
            Thread.Sleep(30000);
        }
    }
}