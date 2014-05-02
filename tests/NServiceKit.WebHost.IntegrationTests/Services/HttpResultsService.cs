using System;
using System.Net;
using System.Runtime.Serialization;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A HTTP results.</summary>
    [Route("/httpresults")]
	[DataContract]
	public class HttpResults
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		[DataMember]
		public string Name { get; set; }
	}

    /// <summary>A HTTP results response.</summary>
	[DataContract]
	public class HttpResultsResponse
		: IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.HttpResultsResponse class.</summary>
        public HttpResultsResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A HTTP results service.</summary>
    public class HttpResultsService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="HttpError">Thrown when a HTTP error error condition occurs.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(HttpResults request)
		{
            if (request.Name == "Error")
                throw new HttpError(HttpStatusCode.NotFound, "Error NotFound");

            return new HttpResult(HttpStatusCode.NotFound, "Returned NotFound");
		}
	}
}