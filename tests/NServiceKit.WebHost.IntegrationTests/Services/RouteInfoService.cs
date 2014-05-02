using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using DeliveryService.Model.Types;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;

namespace DeliveryService.Model.Operations
{
    /// <summary>Information about the route.</summary>
	[Description("POST the route information based on the Application Token associated to a route and Associate ID")]
	[Route("/RouteInfo", "POST")]
	[Route("/RouteInfo/{AppToken}")]
	[Route("/RouteInfo/{AppToken}/{HasProduct}")]
	[DataContract]
	public class RouteInfo
	{
        /// <summary>Gets or sets the application token.</summary>
        ///
        /// <value>The application token.</value>
		[DataMember]
		public string AppToken { get; set; }

        /// <summary>Gets or sets the has product.</summary>
        ///
        /// <value>The has product.</value>
		[DataMember]
		public bool? HasProduct { get; set; }
	}

    /// <summary>A route information response.</summary>
	[DataContract]
	public class RouteInfoResponse : IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the DeliveryService.Model.Operations.RouteInfoResponse class.</summary>
		public RouteInfoResponse()
		{
			this.ResponseStatus = new ResponseStatus();
			this.Customers = new List<Customer>();
			this.Outcomes = new List<Outcome>();
			this.DD = new Dictionary<string, Dictionary<string, string>>();
			this.Tweak = new Dictionary<string, int>();
		}

        /// <summary>Gets or sets the customers.</summary>
        ///
        /// <value>The customers.</value>
		[DataMember]
		public List<Customer> Customers { get; set; }

        /// <summary>Gets or sets the outcomes.</summary>
        ///
        /// <value>The outcomes.</value>
		[DataMember]
		public List<Outcome> Outcomes { get; set; }

        /// <summary>Gets or sets the dd.</summary>
        ///
        /// <value>The dd.</value>
		[DataMember]
		public Dictionary<string, Dictionary<string, string>> DD { get; set; }

        /// <summary>Gets or sets the tweak.</summary>
        ///
        /// <value>The tweak.</value>
		[DataMember]
		public Dictionary<string, int> Tweak { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A route information service.</summary>
	public class RouteInfoService : Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(RouteInfo request)
		{
			throw new NotImplementedException();
		}
	}
}

namespace DeliveryService.Model.Types
{
    /// <summary>An outcome.</summary>
	[DataContract]
	public class Outcome
	{
        /// <summary>Gets or sets the UID.</summary>
        ///
        /// <value>The UID.</value>
		[DataMember]
		public string UID { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		[DataMember]
		public string Name { get; set; }

        /// <summary>Gets or sets the reasons.</summary>
        ///
        /// <value>The reasons.</value>
		[DataMember]
		public List<OutcomeReason> Reasons { get; set; }
	}


    /// <summary>An outcome reason.</summary>
	[DataContract]
	public class OutcomeReason
	{
        /// <summary>Gets or sets the UID.</summary>
        ///
        /// <value>The UID.</value>
		[DataMember]
		public string UID { get; set; }

        /// <summary>Gets or sets the message.</summary>
        ///
        /// <value>The message.</value>
		[DataMember]
		public string Message { get; set; }

	}
}


namespace DeliveryService.Model.Types
{
    /// <summary>A customer.</summary>
	[DataContract]
	public class Customer
	{
        /// <summary>Gets or sets the UID.</summary>
        ///
        /// <value>The UID.</value>
		[DataMember]
		public string UID { get; set; }

        /// <summary>Gets or sets the route position.</summary>
        ///
        /// <value>The route position.</value>
		[DataMember]
		public int RoutePos { get; set; }

        /// <summary>Gets or sets the invoice.</summary>
        ///
        /// <value>The invoice.</value>
		[DataMember]
		public string Invoice { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
		[DataMember]
		public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
		[DataMember]
		public string LastName { get; set; }

        /// <summary>Gets or sets the address.</summary>
        ///
        /// <value>The address.</value>
		[DataMember]
		public string Address { get; set; }

        /// <summary>Gets or sets the city.</summary>
        ///
        /// <value>The city.</value>
		[DataMember]
		public string City { get; set; }

        /// <summary>Gets or sets the state.</summary>
        ///
        /// <value>The state.</value>
		[DataMember]
		public string State { get; set; }

        /// <summary>Gets or sets the zip code.</summary>
        ///
        /// <value>The zip code.</value>
		[DataMember]
		public string ZipCode { get; set; }

        /// <summary>Gets or sets the hm phone.</summary>
        ///
        /// <value>The hm phone.</value>
		[DataMember]
		public string HmPhone { get; set; }

        /// <summary>Gets or sets the wk phone.</summary>
        ///
        /// <value>The wk phone.</value>
		[DataMember]
		public string WkPhone { get; set; }

        /// <summary>Gets or sets the cl phone.</summary>
        ///
        /// <value>The cl phone.</value>
		[DataMember]
		public string ClPhone { get; set; }

        /// <summary>Gets or sets the arrival eta.</summary>
        ///
        /// <value>The arrival eta.</value>
		[DataMember]
		public string ArrivalETA { get; set; }

        /// <summary>Gets or sets the completion code.</summary>
        ///
        /// <value>The completion code.</value>
		[DataMember]
		public string CompletionCode { get; set; }

        /// <summary>Gets or sets the confirmation code.</summary>
        ///
        /// <value>The confirmation code.</value>
		[DataMember]
		public string ConfirmationCode { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is posted.</summary>
        ///
        /// <value>true if this object is posted, false if not.</value>
		[DataMember]
		public bool IsPosted { get; set; }

        /// <summary>Gets or sets the lat.</summary>
        ///
        /// <value>The lat.</value>
		[DataMember]
		public string Lat { get; set; }

        /// <summary>Gets or sets the long.</summary>
        ///
        /// <value>The long.</value>
		[DataMember]
		public string Long { get; set; }

	}
}