using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using NServiceKit.Common.Web;
using NServiceKit.FluentValidation;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A customers.</summary>
	[Route("/customers")]
	[Route("/customers/{Id}")]
    public class Customers : IReturn<CustomersResponse>
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
		public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
		public string LastName { get; set; }

        /// <summary>Gets or sets the company.</summary>
        ///
        /// <value>The company.</value>
		public string Company { get; set; }

        /// <summary>Gets or sets the discount.</summary>
        ///
        /// <value>The discount.</value>
		public decimal Discount { get; set; }

        /// <summary>Gets or sets the address.</summary>
        ///
        /// <value>The address.</value>
		public string Address { get; set; }

        /// <summary>Gets or sets the postcode.</summary>
        ///
        /// <value>The postcode.</value>
		public string Postcode { get; set; }

        /// <summary>Gets or sets a value indicating whether this object has discount.</summary>
        ///
        /// <value>true if this object has discount, false if not.</value>
		public bool HasDiscount { get; set; }
	}

    /// <summary>The customers response.</summary>
    public class CustomersResponse
    {
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public Customers Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>The customers validator.</summary>
	public class CustomersValidator : AbstractValidator<Customers>
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.CustomersValidator class.</summary>
		public CustomersValidator()
		{
			RuleFor(x => x.Id).NotEqual(default(int));

			RuleSet(ApplyTo.Post | ApplyTo.Put, () => {
				RuleFor(x => x.LastName).NotEmpty().WithErrorCode("ShouldNotBeEmpty");
				RuleFor(x => x.FirstName).NotEmpty().WithMessage("Please specify a first name");
				RuleFor(x => x.Company).NotNull();
				RuleFor(x => x.Discount).NotEqual(0).When(x => x.HasDiscount);
				RuleFor(x => x.Address).Length(20, 250);
				RuleFor(x => x.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
			});
		}

		static readonly Regex UsPostCodeRegEx = new Regex(@"^\d{5}(-\d{4})?$", RegexOptions.Compiled);

		private bool BeAValidPostcode(string postcode)
		{
			return !string.IsNullOrEmpty(postcode) && UsPostCodeRegEx.IsMatch(postcode);
		}
	}

    /// <summary>A customer service.</summary>
    public class CustomerService : ServiceInterface.Service
	{
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(Customers request)
		{
			return new CustomersResponse { Result = request };
		}

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(Customers request)
		{
			return new CustomersResponse { Result = request };
		}

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Put(Customers request)
		{
			return new CustomersResponse { Result = request };
		}

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Delete(Customers request)
		{
			return new CustomersResponse { Result = request };
		}
	}

}