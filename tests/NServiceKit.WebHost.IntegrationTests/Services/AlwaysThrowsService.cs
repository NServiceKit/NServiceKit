using System;
using System.Runtime.Serialization;
using NServiceKit.Common.Web;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>The always throws.</summary>
	[DataContract]
	public class AlwaysThrows
	{
        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
	    [DataMember]
	    public int? StatusCode { get; set; }

        /// <summary>Gets or sets the value.</summary>
        ///
        /// <value>The value.</value>
		[DataMember]
		public string Value { get; set; }
	}

    /// <summary>The always throws response.</summary>
	[DataContract]
	public class AlwaysThrowsResponse
		: IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.AlwaysThrowsResponse class.</summary>
		public AlwaysThrowsResponse()
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

    /// <summary>The always throws service.</summary>
	public class AlwaysThrowsService 
		: ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="HttpError">              Thrown when a HTTP error error condition occurs.</exception>
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(AlwaysThrows request)
		{
            if (request.StatusCode.HasValue)
            {
                throw new HttpError(
                    request.StatusCode.Value,
                    typeof(NotImplementedException).Name,
                    request.Value);
            }

			throw new NotImplementedException(GetErrorMessage(request.Value));
		}

        /// <summary>Gets error message.</summary>
        ///
        /// <param name="value">The value.</param>
        ///
        /// <returns>The error message.</returns>
		public static string GetErrorMessage(string value)
		{
			return value + " is not implemented";
		}
	}
}