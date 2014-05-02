using System;
using System.Collections.Generic;
using System.Linq;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceInterface
{
    /// <summary>
    /// Enable the authentication feature and configure the AuthService.
    /// </summary>
    public class AuthFeature : IPlugin
    {
        /// <summary>The add user identifier HTTP header.</summary>
        public static bool AddUserIdHttpHeader = true;

        private readonly Func<IAuthSession> sessionFactory;
        private readonly IAuthProvider[] authProviders;

        /// <summary>Gets or sets the service routes.</summary>
        ///
        /// <value>The service routes.</value>
        public Dictionary<Type, string[]> ServiceRoutes { get; set; }

        /// <summary>Gets or sets the register plugins.</summary>
        ///
        /// <value>The register plugins.</value>
        public List<IPlugin> RegisterPlugins { get; set; }

        /// <summary>Gets or sets the HTML redirect.</summary>
        ///
        /// <value>The HTML redirect.</value>
        public string HtmlRedirect { get; set; }

        /// <summary>Sets a value indicating whether the assign role services should be included.</summary>
        ///
        /// <value>true if include assign role services, false if not.</value>
        public bool IncludeAssignRoleServices
        {
            set
            {
                if (!value)
                {
                    (from registerService in ServiceRoutes
                     where registerService.Key == typeof(AssignRolesService)
                        || registerService.Key == typeof(UnAssignRolesService)
                     select registerService.Key).ToList()
                     .ForEach(x => ServiceRoutes.Remove(x));
                }
            }
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.AuthFeature class.</summary>
        ///
        /// <param name="sessionFactory">The session factory.</param>
        /// <param name="authProviders"> The authentication providers.</param>
        /// <param name="htmlRedirect">  The HTML redirect.</param>
        public AuthFeature(Func<IAuthSession> sessionFactory, IAuthProvider[] authProviders, string htmlRedirect = "~/login")
        {
            this.sessionFactory = sessionFactory;
            this.authProviders = authProviders;

            ServiceRoutes = new Dictionary<Type, string[]> {
                { typeof(AuthService), new[]{"/auth", "/auth/{provider}"} },
                { typeof(AssignRolesService), new[]{"/assignroles"} },
                { typeof(UnAssignRolesService), new[]{"/unassignroles"} },
            };

            RegisterPlugins = new List<IPlugin> {
                new SessionFeature()                          
            };

            this.HtmlRedirect = htmlRedirect;
        }

        /// <summary>Registers this object.</summary>
        ///
        /// <param name="appHost">The application host.</param>
        public void Register(IAppHost appHost)
        {
            AuthService.Init(sessionFactory, authProviders);
            AuthService.HtmlRedirect = HtmlRedirect;

            var unitTest = appHost == null;
            if (unitTest) return;

            foreach (var registerService in ServiceRoutes)
            {
                appHost.RegisterService(registerService.Key, registerService.Value);
            }

            RegisterPlugins.ForEach(x => appHost.LoadPlugin(x));
        }

        /// <summary>Gets default session expiry.</summary>
        ///
        /// <returns>The default session expiry.</returns>
        public static TimeSpan? GetDefaultSessionExpiry()
        {
            if (AuthService.AuthProviders == null)
                return SessionFeature.DefaultSessionExpiry;

            var authProvider = AuthService.AuthProviders.FirstOrDefault() as AuthProvider;
            return authProvider != null 
                ? authProvider.SessionExpiry
                : SessionFeature.DefaultSessionExpiry;
        }
    }
}