using System;
using Funq;
using NServiceKit.FluentValidation;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Mvc
{
    /// <summary>A funq validator factory.</summary>
	public class FunqValidatorFactory : ValidatorFactoryBase
	{
		private readonly ContainerResolveCache funqBuilder;

        /// <summary>Initializes a new instance of the NServiceKit.Mvc.FunqValidatorFactory class.</summary>
        ///
        /// <param name="container">The container.</param>
		public FunqValidatorFactory(Container container=null)
		{
			this.funqBuilder = new ContainerResolveCache(container ?? AppHostBase.Instance.Container);
		}

        /// <summary>Creates an instance.</summary>
        ///
        /// <param name="validatorType">Type of the validator.</param>
        ///
        /// <returns>The new instance.</returns>
		public override IValidator CreateInstance(Type validatorType)
		{
			return funqBuilder.CreateInstance(validatorType, true) as IValidator;
		}
	}
}