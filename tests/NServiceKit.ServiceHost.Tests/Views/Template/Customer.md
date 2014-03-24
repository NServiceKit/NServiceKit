@model NServiceKit.ServiceHost.Tests.AppData.Customer
@template alt-template
@helper Fmt: NServiceKit.ServiceHost.Tests.AppData.FormatHelpers
@helper Nwnd: NServiceKit.ServiceHost.Tests.AppData.NorthwindHelpers

@var customer = Model

# @customer.ContactName Customer Details (@customer.City, @customer.Country)
### @customer.ContactTitle 

  - **Company Name:** @customer.CompanyName
  - **Address:** @customer.Address
  - **Email:** @customer.Email
