#if !SILVERLIGHT 
using System;
using System.IO;
using System.Net;
using System.Text;
using NServiceKit.Common;
using NServiceKit.Common.Web;
using NServiceKit.Logging;
using NServiceKit.Text;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>A web request extensions.</summary>
    public static class WebRequestExtensions
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (WebRequestExtensions));

        /// <summary>A WebResponse extension method that downloads the text described by webRes.</summary>
        ///
        /// <param name="webRes">The webRes to act on.</param>
        ///
        /// <returns>A string.</returns>
        public static string DownloadText(this WebResponse webRes)
        {
            using (var stream = webRes.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>A WebResponse extension method that downloads the binary described by webRes.</summary>
        ///
        /// <param name="webRes">The webRes to act on.</param>
        ///
        /// <returns>A byte[].</returns>
        public static byte[] DownloadBinary(this WebResponse webRes)
        {
            using (var stream = webRes.GetResponseStream())
            {
                return stream.ReadFully();
            }
        }

        /// <summary>A string extension method that gets error response.</summary>
        ///
        /// <param name="url">The URL to act on.</param>
        ///
        /// <returns>The error response.</returns>
        public static HttpWebResponse GetErrorResponse(this string url)
        {
            try
            {
                var webReq = WebRequest.Create(url);
                var webRes = webReq.GetResponse();
                var strRes = webRes.DownloadText();
                Console.WriteLine("Expected error, got: " + strRes);
                return null;
            }
            catch (WebException webEx)
            {
                return (HttpWebResponse)webEx.Response;
            }
        }

        /// <summary>A string extension method that posts a file to URL.</summary>
        ///
        /// <param name="url">               The URL to act on.</param>
        /// <param name="uploadFileInfo">    Information describing the upload file.</param>
        /// <param name="uploadFileMimeType">Type of the upload file mime.</param>
        /// <param name="acceptContentType"> Type of the accept content.</param>
        /// <param name="requestFilter">     A filter specifying the request.</param>
        ///
        /// <returns>A WebResponse.</returns>
        public static WebResponse PostFileToUrl(this string url,
            FileInfo uploadFileInfo, string uploadFileMimeType,
            string acceptContentType = null,
            Action<HttpWebRequest> requestFilter = null)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            using (var fileStream = uploadFileInfo.OpenRead())
            {
                var fileName = uploadFileInfo.Name;

                webReq.UploadFile(fileStream, fileName, uploadFileMimeType, acceptContentType: acceptContentType, requestFilter: requestFilter, method: "POST");
            }

            return webReq.GetResponse();
        }

        /// <summary>A string extension method that puts file to URL.</summary>
        ///
        /// <param name="url">               The URL to act on.</param>
        /// <param name="uploadFileInfo">    Information describing the upload file.</param>
        /// <param name="uploadFileMimeType">Type of the upload file mime.</param>
        /// <param name="acceptContentType"> Type of the accept content.</param>
        /// <param name="requestFilter">     A filter specifying the request.</param>
        ///
        /// <returns>A WebResponse.</returns>
        public static WebResponse PutFileToUrl(this string url,
            FileInfo uploadFileInfo, string uploadFileMimeType,
            string acceptContentType = null,
            Action<HttpWebRequest> requestFilter = null)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            using (var fileStream = uploadFileInfo.OpenRead())
            {
                var fileName = uploadFileInfo.Name;

                webReq.UploadFile(fileStream, fileName, uploadFileMimeType, acceptContentType: acceptContentType, requestFilter: requestFilter, method: "PUT");
            }

            return webReq.GetResponse();
        }

        /// <summary>A WebRequest extension method that uploads a file.</summary>
        ///
        /// <param name="webRequest">        The webRequest to act on.</param>
        /// <param name="uploadFileInfo">    Information describing the upload file.</param>
        /// <param name="uploadFileMimeType">Type of the upload file mime.</param>
        ///
        /// <returns>A WebResponse.</returns>
        public static WebResponse UploadFile(this WebRequest webRequest,
            FileInfo uploadFileInfo, string uploadFileMimeType)
        {
            using (var fileStream = uploadFileInfo.OpenRead())
            {
                var fileName = uploadFileInfo.Name;

                webRequest.UploadFile(fileStream, fileName, uploadFileMimeType);
            }

            return webRequest.GetResponse();
        }

        /// <summary>A WebRequest extension method that uploads a file.</summary>
        ///
        /// <param name="webRequest">       The webRequest to act on.</param>
        /// <param name="fileStream">       The file stream.</param>
        /// <param name="fileName">         Filename of the file.</param>
        /// <param name="mimeType">         Type of the mime.</param>
        /// <param name="acceptContentType">Type of the accept content.</param>
        /// <param name="requestFilter">    A filter specifying the request.</param>
        /// <param name="method">           The method.</param>
        public static void UploadFile(this WebRequest webRequest, Stream fileStream, string fileName, string mimeType,
            string acceptContentType = null, Action<HttpWebRequest> requestFilter = null, string method="POST")
        {
            var httpReq = (HttpWebRequest)webRequest;
            httpReq.UserAgent = Env.ServerUserAgent;
            httpReq.Method = method;
            httpReq.AllowAutoRedirect = false;
            httpReq.KeepAlive = false;

            if (acceptContentType != null)
                httpReq.Accept = acceptContentType;

            if (requestFilter != null)
                requestFilter(httpReq);

            var boundary = "----------------------------" + DateTime.UtcNow.Ticks.ToString("x");

            httpReq.ContentType = "multipart/form-data; boundary=" + boundary;

            var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var headerTemplate = "\r\n--" + boundary +
                                 "\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\nContent-Type: {1}\r\n\r\n";

            var header = string.Format(headerTemplate, fileName, mimeType);

            var headerbytes = System.Text.Encoding.ASCII.GetBytes(header);

            httpReq.ContentLength = fileStream.Length + headerbytes.Length + boundarybytes.Length;

            using (Stream outputStream = httpReq.GetRequestStream())
            {
                outputStream.Write(headerbytes, 0, headerbytes.Length);

                byte[] buffer = new byte[4096];
                int byteCount;

                while ((byteCount = fileStream.Read(buffer, 0, 4096)) > 0)
                {
                    outputStream.Write(buffer, 0, byteCount);
                }

                outputStream.Write(boundarybytes, 0, boundarybytes.Length);

                outputStream.Close();
            }
        }

        /// <summary>A WebRequest extension method that uploads a file.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="webRequest">The webRequest to act on.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="fileName">  Filename of the file.</param>
        public static void UploadFile(this WebRequest webRequest, Stream fileStream, string fileName)
        {
            fileName.ThrowIfNull("fileName");
            var mimeType = MimeTypes.GetMimeType(fileName);
            if (mimeType == null)
                throw new ArgumentException("Mime-type not found for file: " + fileName);

            UploadFile(webRequest, fileStream, fileName, mimeType);
        }
    }

}
#endif