using NServiceKit.WebHost.Endpoints.Tests.Support.Operations;
using NServiceKit.WebHost.Endpoints.Tests.Support.Types;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A get customer service.</summary>
	public class GetCustomerService
		: TestServiceBase<GetCustomer>
	{
        /// <summary>Runs the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		protected override object Run(GetCustomer request)
		{
			return new GetCustomerResponse
			{
				Customer = new Customer
				{
					Id = request.CustomerId
				}
			};
		}
	}

}