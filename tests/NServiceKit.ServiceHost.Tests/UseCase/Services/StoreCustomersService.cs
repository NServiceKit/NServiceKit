using System;
using System.Data;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost.Tests.UseCase.Operations;

namespace NServiceKit.ServiceHost.Tests.UseCase.Services
{
	public class StoreCustomersService
		: IService<StoreCustomers> 
	{
		private readonly IDbConnection db;

		public StoreCustomersService(IDbConnection dbConn)
		{
			this.db = dbConn;
			//Console.WriteLine("StoreCustomersService()");
		}

		public object Execute(StoreCustomers request)
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