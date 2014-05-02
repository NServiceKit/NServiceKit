using System;
using System.Data;
using NServiceKit.CacheAccess;
using NServiceKit.Configuration;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost.Tests.Support;
using NServiceKit.ServiceHost.Tests.UseCase.Operations;

namespace NServiceKit.ServiceHost.Tests.UseCase.Services
{
    /// <summary>A get customer service.</summary>
	public class GetCustomerService : ServiceInterface.Service
	{
		private static readonly string CacheKey = typeof (GetCustomer).Name;

		private readonly IDbConnection db;
		private readonly CustomerUseCaseConfig config;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.UseCase.Services.GetCustomerService class.</summary>
        ///
        /// <param name="dbConn">The database connection.</param>
        /// <param name="config">The configuration.</param>
		public GetCustomerService(IDbConnection dbConn, CustomerUseCaseConfig config)
		{
			this.db = dbConn;
			this.config = config;
		}

        /// <summary>Gets or sets the cache client.</summary>
        ///
        /// <value>The cache client.</value>
		public ICacheClient CacheClient { get; set; }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A GetCustomerResponse.</returns>
		public GetCustomerResponse Any(GetCustomer request)
		{
			if (config.UseCache)
			{
				var inCache = this.CacheClient.Get<GetCustomerResponse>(CacheKey);
				if (inCache != null) return inCache;
			}

			var response = new GetCustomerResponse {
				Customer = db.GetById<Customer>(request.CustomerId)
			};

			if (config.UseCache) 
				this.CacheClient.Set(CacheKey, response);

			return response;
		}
	}
}