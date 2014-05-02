using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Tests.Support.Operations;

namespace NServiceKit.WebHost.Endpoints.Tests.Support
{
    /// <summary>A metadata test base.</summary>
	public abstract class MetadataTestBase
	{
        /// <summary>Gets or sets the metadata.</summary>
        ///
        /// <value>The metadata.</value>
        public ServiceMetadata Metadata { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.MetadataTestBase class.</summary>
		protected MetadataTestBase()
		{
            Metadata = new ServiceMetadata();
		    var dummyServiceType = GetType();
            Metadata.Add(dummyServiceType, typeof(GetCustomer), typeof(GetCustomerResponse));
            Metadata.Add(dummyServiceType, typeof(GetCustomers), typeof(GetCustomersResponse));
            Metadata.Add(dummyServiceType, typeof(StoreCustomer), null);
		}
	}
}