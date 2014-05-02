using System.Collections.Generic;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>Interface for authentication provider.</summary>
    public interface IAuthProvider
    {
        /// <summary>Gets or sets the authentication realm.</summary>
        ///
        /// <value>The authentication realm.</value>
        string AuthRealm { get; set; }

        /// <summary>Gets or sets the provider.</summary>
        ///
        /// <value>The provider.</value>
        string Provider { get; set; }

        /// <summary>Gets or sets URL of the callback.</summary>
        ///
        /// <value>The callback URL.</value>
        string CallbackUrl { get; set; }

        /// <summary>
        /// Remove the Users Session
        /// </summary>
        /// <param name="service"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        object Logout(IServiceBase service, Auth request);

        /// <summary>
        /// The entry point for all AuthProvider providers. Runs inside the AuthService so exceptions are treated normally.
        /// Overridable so you can provide your own Auth implementation.
        /// </summary>
        object Authenticate(IServiceBase authService, IAuthSession session, Auth request);

        /// <summary>
        /// Determine if the current session is already authenticated with this AuthProvider
        /// </summary>
        bool IsAuthorized(IAuthSession session, IOAuthTokens tokens, Auth request = null);
    }

    /// <summary>Interface for i/o authentication provider.</summary>
    public interface IOAuthProvider : IAuthProvider
    {
        /// <summary>Gets or sets the authentication HTTP gateway.</summary>
        ///
        /// <value>The authentication HTTP gateway.</value>
        IAuthHttpGateway AuthHttpGateway { get; set; }

        /// <summary>Gets or sets the consumer key.</summary>
        ///
        /// <value>The consumer key.</value>
        string ConsumerKey { get; set; }

        /// <summary>Gets or sets the consumer secret.</summary>
        ///
        /// <value>The consumer secret.</value>
        string ConsumerSecret { get; set; }

        /// <summary>Gets or sets URL of the request token.</summary>
        ///
        /// <value>The request token URL.</value>
        string RequestTokenUrl { get; set; }

        /// <summary>Gets or sets URL of the authorize.</summary>
        ///
        /// <value>The authorize URL.</value>
        string AuthorizeUrl { get; set; }

        /// <summary>Gets or sets URL of the access token.</summary>
        ///
        /// <value>The access token URL.</value>
        string AccessTokenUrl { get; set; }
    }
}