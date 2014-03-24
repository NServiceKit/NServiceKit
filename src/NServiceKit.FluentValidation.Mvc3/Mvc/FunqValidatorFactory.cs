using System;
using Funq;
using NServiceKit.FluentValidation;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Mvc
{
	public class FunqValidatorFactory : ValidatorFactoryBase
	{
		private readonly ContainerResolveCache funqBuilder;

		public FunqValidatorFactory(Container container=null)
		{
			this.funqBuilder = new ContainerResolveCache(container ?? AppHostBase.Instance.Container);
		}

		public override IValidator CreateInstance(Type validatorType)
		{
			return funqBuilder.CreateInstance(validatorType, true) as IValidator;
		}
	}
}