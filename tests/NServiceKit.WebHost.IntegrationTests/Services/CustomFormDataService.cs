using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A custom form data.</summary>
	[Route("/customformdata")]
	[DataContract]
	public class CustomFormData { }

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

    /// <summary>A custom form data service.</summary>
	public class CustomFormDataService : ServiceInterface.Service
	{
		//Parsing: &first-name=tom&item-0=blah&item-1-delete=1
		public object Post(CustomFormData request)
		{
			var httpReq = base.RequestContext.Get<IHttpRequest>();

			return new CustomFormDataResponse
			{
				FirstName = httpReq.FormData["first-name"],
				Item0 = httpReq.FormData["item-0"],
				Item1Delete = httpReq.FormData["item-1-delete"]
			};
		}
	}
}