using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using NServiceKit.Common.Extensions;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A HTTP error.</summary>
	[DataContract]
	[Route("/errors")]
	[Route("/errors/{Type}")]
	[Route("/errors/{Type}/{StatusCode}")]
	[Route("/errors/{Type}/{StatusCode}/{Message}")]
	public class HttpError
	{
        /// <summary>Gets or sets the type.</summary>
        ///
        /// <value>The type.</value>
		[DataMember]
		public string Type { get; set; }

        /// <summary>Gets or sets the message.</summary>
        ///
        /// <value>The message.</value>
		[DataMember]
		public string Message { get; set; }

        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
		[DataMember]
		public int? StatusCode { get; set; }
	}

    /// <summary>A HTTP error response.</summary>
	[DataContract]
	public class HttpErrorResponse
		: IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.HttpErrorResponse class.</summary>
		public HttpErrorResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A HTTP error service.</summary>
	public class HttpErrorService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        /// <exception cref="HttpError">            Thrown when a HTTP error error condition occurs.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
	    public object Any(HttpError request)
		{
			if (request.Type.IsNullOrEmpty())
				throw new ArgumentNullException("Type");

			var ex = new Exception(request.Message);
			switch (request.Type)
			{
				case "FileNotFoundException":
					ex = new FileNotFoundException(request.Message);
					break;
			}

			if (!request.StatusCode.HasValue)
				throw ex;

			var httpStatus = (HttpStatusCode)request.StatusCode.Value;
			throw new Common.Web.HttpError(httpStatus, ex);
		}
	}

}