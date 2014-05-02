using System;
using System.IO;
using System.Runtime.Serialization;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Utils;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A file upload.</summary>
	[DataContract]
	[Route("/fileuploads/{RelativePath*}")]
	[Route("/fileuploads", HttpMethods.Post)]
	public class FileUpload
	{
        /// <summary>Gets or sets the full pathname of the relative file.</summary>
        ///
        /// <value>The full pathname of the relative file.</value>
		[DataMember]
		public string RelativePath { get; set; }

        /// <summary>Gets or sets the name of the customer.</summary>
        ///
        /// <value>The name of the customer.</value>
        [DataMember]
        public string CustomerName { get; set; }

        /// <summary>Gets or sets the identifier of the customer.</summary>
        ///
        /// <value>The identifier of the customer.</value>
        [DataMember]
        public int CustomerId { get; set; }
	}

    /// <summary>A file upload response.</summary>
	[DataContract]
	public class FileUploadResponse : IHasResponseStatus
	{
        /// <summary>Gets or sets the filename of the file.</summary>
        ///
        /// <value>The name of the file.</value>
		[DataMember]
		public string FileName { get; set; }

        /// <summary>Gets or sets the length of the content.</summary>
        ///
        /// <value>The length of the content.</value>
		[DataMember]
		public long ContentLength { get; set; }

        /// <summary>Gets or sets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
		[DataMember]
		public string ContentType { get; set; }

        /// <summary>Gets or sets the contents.</summary>
        ///
        /// <value>The contents.</value>
		[DataMember]
		public string Contents { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }

        /// <summary>Gets or sets the name of the customer.</summary>
        ///
        /// <value>The name of the customer.</value>
        [DataMember]
        public string CustomerName { get; set; }

        /// <summary>Gets or sets the identifier of the customer.</summary>
        ///
        /// <value>The identifier of the customer.</value>
        [DataMember]
        public int CustomerId { get; set; }
	}

    /// <summary>A file upload service.</summary>
	public class FileUploadService : ServiceInterface.Service
	{
        /// <summary>Gets the given request.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(FileUpload request)
		{
			if (request.RelativePath.IsNullOrEmpty())
				throw new ArgumentNullException("RelativePath");

			var filePath = ("~/" + request.RelativePath).MapProjectPath();
			if (!File.Exists(filePath))
				throw new FileNotFoundException(request.RelativePath);

			var result = new HttpResult(new FileInfo(filePath));
			return result;
		}

        /// <summary>Post this message.</summary>
        ///
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(FileUpload request)
		{
			if (this.RequestContext.Files.Length == 0)
				throw new FileNotFoundException("UploadError", "No such file exists");

			if (request.RelativePath == "ThrowError")
				throw new NotSupportedException("ThrowError");

			var file = this.RequestContext.Files[0];
			return new FileUploadResponse
			{
				FileName = file.FileName,
				ContentLength = file.ContentLength,
				ContentType = file.ContentType,
				Contents = new StreamReader(file.InputStream).ReadToEnd(),
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName
			};
		}
	}
}