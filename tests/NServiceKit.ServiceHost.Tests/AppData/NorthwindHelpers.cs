using System.Collections.Generic;
using System.Linq;
using NServiceKit.Common;

namespace NServiceKit.ServiceHost.Tests.AppData
{
    /// <summary>A northwind helpers.</summary>
	public class NorthwindHelpers
	{
        /// <summary>Order total.</summary>
        ///
        /// <param name="orderDetails">The order details.</param>
        ///
        /// <returns>A string.</returns>
		public string OrderTotal(List<OrderDetail> orderDetails)
		{
			var total = 0m;
			if (!orderDetails.IsEmpty())
				total += orderDetails.Sum(item => item.Quantity * item.UnitPrice);

			return FormatHelpers.Instance.Money(total);
		}

        /// <summary>Customer order total.</summary>
        ///
        /// <param name="customerOrders">The customer orders.</param>
        ///
        /// <returns>A string.</returns>
		public string CustomerOrderTotal(List<CustomerOrder> customerOrders)
		{
			var total = customerOrders
				.Sum(x => 
					x.OrderDetails.Sum(item => item.Quantity*item.UnitPrice));
			
			return FormatHelpers.Instance.Money(total);
		}
	}
}