using System.Collections.Generic;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.Auth;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A user auths.</summary>
	[Route("/userauths")]
	public class UserAuths
	{
        /// <summary>Gets or sets the identifiers.</summary>
        ///
        /// <value>The identifiers.</value>
		public int[] Ids { get; set; }
	}

    /// <summary>A user auths response.</summary>
	public class UserAuthsResponse : IHasResponseStatus
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.IntegrationTests.Services.UserAuthsResponse class.</summary>
		public UserAuthsResponse()
		{
			this.Results = new List<UserAuth>();
			this.OAuthProviders = new List<UserOAuthProvider>();
		}

        /// <summary>Gets or sets the results.</summary>
        ///
        /// <value>The results.</value>
		public List<UserAuth> Results { get; set; }

        /// <summary>Gets or sets the authentication providers.</summary>
        ///
        /// <value>The o authentication providers.</value>
		public List<UserOAuthProvider> OAuthProviders { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
		public ResponseStatus ResponseStatus { get; set; }
	}

	//Implementation. Can be called via any endpoint or format, see: http://NServiceKit.net/NServiceKit.Hello/
	public class UserAuthsService : ServiceInterface.Service
	{
        /// <summary>Gets or sets the database factory.</summary>
        ///
        /// <value>The database factory.</value>
		public IDbConnectionFactory DbFactory { get; set; }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(UserAuths request)
		{
			return new UserAuthsResponse {
				Results = Db.Select<UserAuth>(),
				OAuthProviders = Db.Select<UserOAuthProvider>(),
			};
		}
	}
}