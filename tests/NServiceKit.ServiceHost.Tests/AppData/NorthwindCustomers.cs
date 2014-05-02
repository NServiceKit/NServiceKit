using System;
using System.Collections.Generic;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.ServiceHost.Tests.AppData
{
    /// <summary>A customers.</summary>
	public class Customers { }
	
    /// <summary>The customers response.</summary>
	public class CustomersResponse : IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.AppData.CustomersResponse class.</summary>
		public CustomersResponse()
		{
			this.ResponseStatus = new ResponseStatus();
			this.Customers = new List<Customer>();
		}

        /// <summary>Gets or sets the customers.</summary>
        ///
        /// <value>The customers.</value>
		public List<Customer> Customers { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		public ResponseStatus ResponseStatus { get; set; }
	}
	
    /// <summary>A customer details.</summary>
	public class CustomerDetails
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public string Id { get; set; }
	}
	
    /// <summary>A customer details response.</summary>
	public class CustomerDetailsResponse : IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.AppData.CustomerDetailsResponse class.</summary>
		public CustomerDetailsResponse()
		{
			this.ResponseStatus = new ResponseStatus();
			this.CustomerOrders = new List<CustomerOrder>();
		}

        /// <summary>Gets or sets the customer.</summary>
        ///
        /// <value>The customer.</value>
		public Customer Customer { get; set; }

        /// <summary>Gets or sets the customer orders.</summary>
        ///
        /// <value>The customer orders.</value>
		public List<CustomerOrder> CustomerOrders { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		public ResponseStatus ResponseStatus { get; set; }
	}
	
    /// <summary>An orders.</summary>
	public class Orders
	{
        /// <summary>Gets or sets the page.</summary>
        ///
        /// <value>The page.</value>
		public int? Page { get; set; }

        /// <summary>Gets or sets the identifier of the customer.</summary>
        ///
        /// <value>The identifier of the customer.</value>
		public string CustomerId { get; set; }
	}
	
    /// <summary>The orders response.</summary>
	public class OrdersResponse : IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.AppData.OrdersResponse class.</summary>
		public OrdersResponse()
		{
			this.ResponseStatus = new ResponseStatus();
			this.Results = new List<CustomerOrder>();
		}

        /// <summary>Gets or sets the results.</summary>
        ///
        /// <value>The results.</value>
		public List<CustomerOrder> Results { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		public ResponseStatus ResponseStatus { get; set; }
	}

    /// <summary>A category.</summary>
	public class Category
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the name of the category.</summary>
        ///
        /// <value>The name of the category.</value>
		public string CategoryName { get; set; }

        /// <summary>Gets or sets the description.</summary>
        ///
        /// <value>The description.</value>
		public string Description { get; set; }
	}

    /// <summary>A customer.</summary>
	public class Customer
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public string Id { get; set; }

        /// <summary>Gets or sets the name of the company.</summary>
        ///
        /// <value>The name of the company.</value>
		public string CompanyName { get; set; }

        /// <summary>Gets or sets the name of the contact.</summary>
        ///
        /// <value>The name of the contact.</value>
		public string ContactName { get; set; }

        /// <summary>Gets or sets the contact title.</summary>
        ///
        /// <value>The contact title.</value>
		public string ContactTitle { get; set; }

        /// <summary>Gets or sets the address.</summary>
        ///
        /// <value>The address.</value>
		public string Address { get; set; }

        /// <summary>Gets or sets the city.</summary>
        ///
        /// <value>The city.</value>
		public string City { get; set; }

        /// <summary>Gets or sets the region.</summary>
        ///
        /// <value>The region.</value>
		public string Region { get; set; }

        /// <summary>Gets or sets the postal code.</summary>
        ///
        /// <value>The postal code.</value>
		public string PostalCode { get; set; }

        /// <summary>Gets or sets the country.</summary>
        ///
        /// <value>The country.</value>
		public string Country { get; set; }

        /// <summary>Gets or sets the phone.</summary>
        ///
        /// <value>The phone.</value>
		public string Phone { get; set; }

        /// <summary>Gets or sets the fax.</summary>
        ///
        /// <value>The fax.</value>
		public string Fax { get; set; }

        /// <summary>Gets the email.</summary>
        ///
        /// <value>The email.</value>
		public string Email
		{
			get { return this.ContactName.Replace(" ", ".").ToLower() + "@gmail.com"; }
		}
	}

    /// <summary>A customer demo.</summary>
	public class CustomerCustomerDemo
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public string Id { get; set; }

        /// <summary>Gets or sets the identifier of the customer type.</summary>
        ///
        /// <value>The identifier of the customer type.</value>
		public string CustomerTypeId { get; set; }
	}

    /// <summary>A customer demographic.</summary>
	public class CustomerDemographic
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public string Id { get; set; }

        /// <summary>Gets or sets information describing the customer.</summary>
        ///
        /// <value>Information describing the customer.</value>
		public string CustomerDesc { get; set; }
	}
	
    /// <summary>A customer order.</summary>
	public class CustomerOrder
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.AppData.CustomerOrder class.</summary>
		public CustomerOrder()
		{
			this.OrderDetails = new List<OrderDetail>();
		}

        /// <summary>Gets or sets the order.</summary>
        ///
        /// <value>The order.</value>
		public Order Order { get; set; }

        /// <summary>Gets or sets the order details.</summary>
        ///
        /// <value>The order details.</value>
		public List<OrderDetail> OrderDetails { get; set; }
	}
	
    /// <summary>An employee.</summary>
	public class Employee
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
		public string LastName { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
		public string FirstName { get; set; }

        /// <summary>Gets or sets the title.</summary>
        ///
        /// <value>The title.</value>
		public string Title { get; set; }

        /// <summary>Gets or sets the title of courtesy.</summary>
        ///
        /// <value>The title of courtesy.</value>
		public string TitleOfCourtesy { get; set; }

        /// <summary>Gets or sets the birth date.</summary>
        ///
        /// <value>The birth date.</value>
		public DateTime? BirthDate { get; set; }

        /// <summary>Gets or sets the hire date.</summary>
        ///
        /// <value>The hire date.</value>
		public DateTime? HireDate { get; set; }

        /// <summary>Gets or sets the address.</summary>
        ///
        /// <value>The address.</value>
		public string Address { get; set; }

        /// <summary>Gets or sets the city.</summary>
        ///
        /// <value>The city.</value>
		public string City { get; set; }

        /// <summary>Gets or sets the region.</summary>
        ///
        /// <value>The region.</value>
		public string Region { get; set; }

        /// <summary>Gets or sets the postal code.</summary>
        ///
        /// <value>The postal code.</value>
		public string PostalCode { get; set; }

        /// <summary>Gets or sets the country.</summary>
        ///
        /// <value>The country.</value>
		public string Country { get; set; }

        /// <summary>Gets or sets the home phone.</summary>
        ///
        /// <value>The home phone.</value>
		public string HomePhone { get; set; }

        /// <summary>Gets or sets the extension.</summary>
        ///
        /// <value>The extension.</value>
		public string Extension { get; set; }

        /// <summary>Gets or sets the photo.</summary>
        ///
        /// <value>The photo.</value>
		public byte[] Photo { get; set; }

        /// <summary>Gets or sets the notes.</summary>
        ///
        /// <value>The notes.</value>
		public string Notes { get; set; }

        /// <summary>Gets or sets the reports to.</summary>
        ///
        /// <value>The reports to.</value>
		public int? ReportsTo { get; set; }

        /// <summary>Gets or sets the full pathname of the photo file.</summary>
        ///
        /// <value>The full pathname of the photo file.</value>
		public string PhotoPath { get; set; }
	}

    /// <summary>An employee territory.</summary>
	public class EmployeeTerritory
	{
        /// <summary>Gets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public string Id { get { return this.EmployeeId + "/" + this.TerritoryId; } }

        /// <summary>Gets or sets the identifier of the employee.</summary>
        ///
        /// <value>The identifier of the employee.</value>
		public int EmployeeId { get; set; }

        /// <summary>Gets or sets the identifier of the territory.</summary>
        ///
        /// <value>The identifier of the territory.</value>
		public string TerritoryId { get; set; }
	}
	
    /// <summary>An order.</summary>
	public class Order
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the customer.</summary>
        ///
        /// <value>The identifier of the customer.</value>
		public string CustomerId { get; set; }

        /// <summary>Gets or sets the identifier of the employee.</summary>
        ///
        /// <value>The identifier of the employee.</value>
		public int EmployeeId { get; set; }

        /// <summary>Gets or sets the order date.</summary>
        ///
        /// <value>The order date.</value>
		public DateTime? OrderDate { get; set; }

        /// <summary>Gets or sets the required date.</summary>
        ///
        /// <value>The required date.</value>
		public DateTime? RequiredDate { get; set; }

        /// <summary>Gets or sets the shipped date.</summary>
        ///
        /// <value>The shipped date.</value>
		public DateTime? ShippedDate { get; set; }

        /// <summary>Gets or sets the ship via.</summary>
        ///
        /// <value>The ship via.</value>
		public int? ShipVia { get; set; }

        /// <summary>Gets or sets the freight.</summary>
        ///
        /// <value>The freight.</value>
		public decimal Freight { get; set; }

        /// <summary>Gets or sets the name of the ship.</summary>
        ///
        /// <value>The name of the ship.</value>
		public string ShipName { get; set; }

        /// <summary>Gets or sets the ship address.</summary>
        ///
        /// <value>The ship address.</value>
		public string ShipAddress { get; set; }

        /// <summary>Gets or sets the ship city.</summary>
        ///
        /// <value>The ship city.</value>
		public string ShipCity { get; set; }

        /// <summary>Gets or sets the ship region.</summary>
        ///
        /// <value>The ship region.</value>
		public string ShipRegion { get; set; }

        /// <summary>Gets or sets the ship postal code.</summary>
        ///
        /// <value>The ship postal code.</value>
		public string ShipPostalCode { get; set; }

        /// <summary>Gets or sets the ship country.</summary>
        ///
        /// <value>The ship country.</value>
		public string ShipCountry { get; set; }
	}

    /// <summary>An order detail.</summary>
	public class OrderDetail
	{
        /// <summary>Gets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public string Id { get { return this.OrderId + "/" + this.ProductId; } }

        /// <summary>Gets or sets the identifier of the order.</summary>
        ///
        /// <value>The identifier of the order.</value>
		public int OrderId { get; set; }

        /// <summary>Gets or sets the identifier of the product.</summary>
        ///
        /// <value>The identifier of the product.</value>
		public int ProductId { get; set; }

        /// <summary>Gets or sets the unit price.</summary>
        ///
        /// <value>The unit price.</value>
		public decimal UnitPrice { get; set; }

        /// <summary>Gets or sets the quantity.</summary>
        ///
        /// <value>The quantity.</value>
		public short Quantity { get; set; }

        /// <summary>Gets or sets the discount.</summary>
        ///
        /// <value>The discount.</value>
		public double Discount { get; set; }
	}
	
    /// <summary>A product.</summary>
	public class Product
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the name of the product.</summary>
        ///
        /// <value>The name of the product.</value>
		public string ProductName { get; set; }

        /// <summary>Gets or sets the identifier of the supplier.</summary>
        ///
        /// <value>The identifier of the supplier.</value>
		public int SupplierId { get; set; }

        /// <summary>Gets or sets the identifier of the category.</summary>
        ///
        /// <value>The identifier of the category.</value>
		public int CategoryId { get; set; }

        /// <summary>Gets or sets the quantity per unit.</summary>
        ///
        /// <value>The quantity per unit.</value>
		public string QuantityPerUnit { get; set; }

        /// <summary>Gets or sets the unit price.</summary>
        ///
        /// <value>The unit price.</value>
		public decimal UnitPrice { get; set; }

        /// <summary>Gets or sets the units in stock.</summary>
        ///
        /// <value>The units in stock.</value>
		public short UnitsInStock { get; set; }

        /// <summary>Gets or sets the units on order.</summary>
        ///
        /// <value>The units on order.</value>
		public short UnitsOnOrder { get; set; }

        /// <summary>Gets or sets the reorder level.</summary>
        ///
        /// <value>The reorder level.</value>
		public short ReorderLevel { get; set; }

        /// <summary>Gets or sets a value indicating whether the discontinued.</summary>
        ///
        /// <value>true if discontinued, false if not.</value>
		public bool Discontinued { get; set; }
	}
	
    /// <summary>A region.</summary>
	public class Region
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets information describing the region.</summary>
        ///
        /// <value>Information describing the region.</value>
		public string RegionDescription { get; set; }
	}
	
    /// <summary>A shipper.</summary>
	public class Shipper
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the name of the company.</summary>
        ///
        /// <value>The name of the company.</value>
		public string CompanyName { get; set; }

        /// <summary>Gets or sets the phone.</summary>
        ///
        /// <value>The phone.</value>
		public string Phone { get; set; }
	}
	
    /// <summary>A supplier.</summary>
	public class Supplier
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the name of the company.</summary>
        ///
        /// <value>The name of the company.</value>
		public string CompanyName { get; set; }

        /// <summary>Gets or sets the name of the contact.</summary>
        ///
        /// <value>The name of the contact.</value>
		public string ContactName { get; set; }

        /// <summary>Gets or sets the contact title.</summary>
        ///
        /// <value>The contact title.</value>
		public string ContactTitle { get; set; }

        /// <summary>Gets or sets the address.</summary>
        ///
        /// <value>The address.</value>
		public string Address { get; set; }

        /// <summary>Gets or sets the city.</summary>
        ///
        /// <value>The city.</value>
		public string City { get; set; }

        /// <summary>Gets or sets the region.</summary>
        ///
        /// <value>The region.</value>
		public string Region { get; set; }

        /// <summary>Gets or sets the postal code.</summary>
        ///
        /// <value>The postal code.</value>
		public string PostalCode { get; set; }

        /// <summary>Gets or sets the country.</summary>
        ///
        /// <value>The country.</value>
		public string Country { get; set; }

        /// <summary>Gets or sets the phone.</summary>
        ///
        /// <value>The phone.</value>
		public string Phone { get; set; }

        /// <summary>Gets or sets the fax.</summary>
        ///
        /// <value>The fax.</value>
		public string Fax { get; set; }

        /// <summary>Gets or sets the home page.</summary>
        ///
        /// <value>The home page.</value>
		public string HomePage { get; set; }
	}
	
    /// <summary>A territory.</summary>
	public class Territory
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public string Id { get; set; }

        /// <summary>Gets or sets information describing the territory.</summary>
        ///
        /// <value>Information describing the territory.</value>
		public string TerritoryDescription { get; set; }

        /// <summary>Gets or sets the identifier of the region.</summary>
        ///
        /// <value>The identifier of the region.</value>
		public int RegionId { get; set; }
	}
}