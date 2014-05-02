using System;
using System.Data;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost.Tests.UseCase.Operations;

namespace NServiceKit.ServiceHost.Tests.UseCase.Services
{
    /// <summary>A store customers service.</summary>
	public class StoreCustomersService : ServiceInterface.Service
	{
		private readonly IDbConnection db;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.UseCase.Services.StoreCustomersService class.</summary>
        ///
        /// <param name="dbConn">The database connection.</param>
		public StoreCustomersService(IDbConnection dbConn)
		{
			this.db = dbConn;
			//Console.WriteLine("StoreCustomersService()");
		}

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(StoreCustomers request)
		{
			db.CreateTable<Customer>(false);
			foreach (var customer in request.Customers)
			{
				db.Insert(customer);
			}

			return null;
		}
	}

}