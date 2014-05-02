using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
	//Request DTO
	[Route("/geoinfo")]
	[DataContract]
	public class GeoInfo
	{
        /// <summary>Gets or sets the application token.</summary>
        ///
        /// <value>The application token.</value>
		[DataMember]
		public string AppToken { get; set; }

        /// <summary>Gets or sets the identifier of the order.</summary>
        ///
        /// <value>The identifier of the order.</value>
		[DataMember]
		public int OrderId { get; set; }

        /// <summary>Gets or sets the geo code.</summary>
        ///
        /// <value>The geo code.</value>
		[DataMember]
		public GeoPoint GeoCode { get; set; }
	}

    /// <summary>A geo point.</summary>
	[Serializable]
	public class GeoPoint
	{
        /// <summary>Gets or sets the t.</summary>
        ///
        /// <value>The t.</value>
		public long t { get; set; }

        /// <summary>Gets or sets the latitude.</summary>
        ///
        /// <value>The latitude.</value>
		public decimal latitude { get; set; }

        /// <summary>Gets or sets the longitude.</summary>
        ///
        /// <value>The longitude.</value>
		public decimal longitude { get; set; }
	}

	//Response DTO
	public class GeoInfoResponse : IHasResponseStatus
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		public string Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		public ResponseStatus ResponseStatus { get; set; } //Where Exceptions get auto-serialized
	}

    /// <summary>A geo information service.</summary>
	public class GeoInfoService : ServiceInterface.Service
	{
        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(GeoInfo request)
		{
			return new GeoInfoResponse
			{
				Result = "Incoming Geopoint: Latitude="
					+ request.GeoCode.latitude.ToString()
					+ " Longitude="
					+ request.GeoCode.longitude.ToString()
			};
		}
	}
}