using System;
using NServiceKit.FluentValidation;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceInterface
{
    /// <summary>
    /// Enable the Registration feature and configure the RegistrationService.
    /// </summary>
    public class RegistrationFeature : IPlugin
    {
        /// <summary>Gets or sets the full pathname of at rest file.</summary>
        ///
        /// <value>The full pathname of at rest file.</value>
        public string AtRestPath { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.RegistrationFeature class.</summary>
        public RegistrationFeature()
        {
            this.AtRestPath = "/register";
        }

        /// <summary>Registers this object.</summary>
        ///
        /// <param name="appHost">The application host.</param>
        public void Register(IAppHost appHost)
        {
            appHost.RegisterService<RegistrationService>(AtRestPath);
            appHost.RegisterAs<RegistrationValidator, IValidator<Registration>>();
        }
    }
}