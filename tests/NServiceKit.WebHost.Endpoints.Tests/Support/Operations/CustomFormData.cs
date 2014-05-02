using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Operations
{
    /// <summary>A custom form data.</summary>
	[Route("/customformdata")]
	[DataContract]
	public class CustomFormData {}

    /// <summary>A custom form data response.</summary>
	[DataContract]
	public class CustomFormDataResponse : IHasResponseStatus
	{
        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
		[DataMember]
		public string FirstName { get; set; }

        /// <summary>Gets or sets the item 0.</summary>
        ///
        /// <value>The item 0.</value>
		[DataMember]
		public string Item0 { get; set; }

        /// <summary>Gets or sets the item 1 delete.</summary>
        ///
        /// <value>The item 1 delete.</value>
		[DataMember]
		public string Item1Delete { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}
}
