using System.Web.UI;
using NServiceKit.CacheAccess;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.IntegrationTests.Tests;

namespace NServiceKit.WebHost.IntegrationTests
{
    /// <summary>A custom user session.</summary>
	public class CustomUserSession : AuthUserSession
	{
        /// <summary>Gets or sets the custom propety.</summary>
        ///
        /// <value>The custom propety.</value>
		public string CustomPropety { get; set; }

        /// <summary>Executes the authenticated action.</summary>
        ///
        /// <param name="authService">The authentication service.</param>
        /// <param name="session">    The session.</param>
        /// <param name="tokens">     The tokens.</param>
        /// <param name="authInfo">   Information describing the authentication.</param>
		public override void OnAuthenticated(IServiceBase authService, IAuthSession session, IOAuthTokens tokens, System.Collections.Generic.Dictionary<string, string> authInfo)
		{
			base.OnAuthenticated(authService, session, tokens, authInfo);

			if (session.Email == AuthTestsBase.AdminEmail)
				session.Roles.Add(RoleNames.Admin);
		}
	}

    /// <summary>A page base.</summary>
	public class PageBase : Page
	{
        /// <summary>
        /// Typed UserSession
        /// </summary>
        private object userSession;

        /// <summary>Session as.</summary>
        ///
        /// <typeparam name="TUserSession">Type of the user session.</typeparam>
        ///
        /// <returns>A TUserSession.</returns>
        protected virtual TUserSession SessionAs<TUserSession>()
        {
            return (TUserSession)(userSession ?? (userSession = Cache.SessionAs<TUserSession>()));
        }

        /// <summary>Gets the user session.</summary>
        ///
        /// <value>The user session.</value>
        protected CustomUserSession UserSession
        {
            get
            {
                return SessionAs<CustomUserSession>();
            }
        }

        /// <summary>Gets the cache.</summary>
        ///
        /// <value>The cache.</value>
        public new ICacheClient Cache
        {
            get { return AppHostBase.Resolve<ICacheClient>(); }
        }

        private ISessionFactory sessionFactory;

        /// <summary>Gets the session factory.</summary>
        ///
        /// <value>The session factory.</value>
        public virtual ISessionFactory SessionFactory
        {
            get { return sessionFactory ?? (sessionFactory = AppHostBase.Resolve<ISessionFactory>()) ?? new SessionFactory(Cache); }
        }

        /// <summary>
        /// Dynamic Session Bag
        /// </summary>
        private ISession session;

        /// <summary>Gets the session.</summary>
        ///
        /// <value>The session.</value>
        public new ISession Session
        {
            get
            {
                return session ?? (session = SessionFactory.GetOrCreateSession());
            }
        }

        /// <summary>Clears the session.</summary>
        public void ClearSession()
        {
            userSession = null;
            this.Cache.Remove(SessionFeature.GetSessionKey());
        }
    }
}