using NServiceKit.ServiceHost.Tests.UseCase;

namespace NServiceKit.ServiceHost.Tests.Support
{
    /// <summary>A customer use case configuration.</summary>
	public class CustomerUseCaseConfig
	{
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.Support.CustomerUseCaseConfig class.</summary>
		public CustomerUseCaseConfig()
		{
			this.UseCache = CustomerUseCase.UseCache;
		}

        /// <summary>Gets or sets a value indicating whether this object use cache.</summary>
        ///
        /// <value>true if use cache, false if not.</value>
		public bool UseCache { get; set; }
	}
}