using NServiceKit.ServiceHost.Tests.UseCase;

namespace NServiceKit.ServiceHost.Tests.Support
{
	public class CustomerUseCaseConfig
	{
		public CustomerUseCaseConfig()
		{
			this.UseCache = CustomerUseCase.UseCache;
		}

		public bool UseCache { get; set; }
	}
}