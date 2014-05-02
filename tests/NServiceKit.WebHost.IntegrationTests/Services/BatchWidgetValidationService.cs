using System;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A batch widget validation request.</summary>
	[DataContract]
	[Route("/BatchWidgetValidation")]
	public class BatchWidgetValidationRequest
	{
        /// <summary>Gets or sets the batch.</summary>
        ///
        /// <value>The batch.</value>
		[DataMember]
		public WidgetValidationRequest[] Batch { get; set; }
	}

    /// <summary>A batch widget validation response.</summary>
	[DataContract]
	public class BatchWidgetValidationResponse
	{
        /// <summary>Gets or sets the batch.</summary>
        ///
        /// <value>The batch.</value>
		[DataMember]
		public WidgetValidationResponse[] Batch { get; set; }
	}

    /// <summary>A batch widget validation request service.</summary>
	public class BatchWidgetValidationRequestService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(BatchWidgetValidationRequest request)
		{
			throw new NotImplementedException();
		}
	}

    /// <summary>A widget validation request.</summary>
	[DataContract]
	[Route("/WidgetValidation")]
	public class WidgetValidationRequest
	{
        /// <summary>Gets or sets the identifier that owns this item.</summary>
        ///
        /// <value>The identifier of the owner.</value>
		[DataMember]
		public int OwnerID { get; set; }

        /// <summary>Gets or sets the identifier of the seller.</summary>
        ///
        /// <value>The identifier of the seller.</value>
		[DataMember]
		public string SellerID { get; set; }

        /// <summary>Gets or sets the widget number.</summary>
        ///
        /// <value>The widget number.</value>
		[DataMember]
		public string WidgetNumber { get; set; }

        /// <summary>Gets or sets the quantity.</summary>
        ///
        /// <value>The quantity.</value>
		[DataMember]
		public string Quantity { get; set; }
	}

    /// <summary>A widget validation response.</summary>
	[DataContract]
	public class WidgetValidationResponse
	{
        /// <summary>Gets or sets the seller widget number.</summary>
        ///
        /// <value>The seller widget number.</value>
		[DataMember]
		public string SellerWidgetNumber { get; set; }

        /// <summary>Gets or sets the type of the match.</summary>
        ///
        /// <value>The type of the match.</value>
		[DataMember]
		public string MatchType { get; set; }

        /// <summary>Gets or sets the widget price.</summary>
        ///
        /// <value>The widget price.</value>
		[DataMember]
		public decimal WidgetPrice { get; set; }

        /// <summary>Gets or sets the name of the widget.</summary>
        ///
        /// <value>The name of the widget.</value>
		[DataMember]
		public string WidgetName { get; set; }
	}

    /// <summary>A widget validation request service.</summary>
	public class WidgetValidationRequestService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(WidgetValidationRequest request)
		{
			throw new NotImplementedException();
		}
	}
}