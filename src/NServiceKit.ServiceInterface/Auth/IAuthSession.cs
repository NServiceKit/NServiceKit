using System;
using System.Collections.Generic;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>Interface for authentication session.</summary>
    public interface IAuthSession
    {
        /// <summary>Gets or sets URL of the referrer.</summary>
        ///
        /// <value>The referrer URL.</value>
        string ReferrerUrl { get; set; }

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        string Id { get; set; }

        /// <summary>Gets or sets the identifier of the user authentication.</summary>
        ///
        /// <value>The identifier of the user authentication.</value>
        string UserAuthId { get; set; }

        /// <summary>Gets or sets the name of the user authentication.</summary>
        ///
        /// <value>The name of the user authentication.</value>
        string UserAuthName { get; set; }

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        string UserName { get; set; }

        /// <summary>Gets or sets the name of the display.</summary>
        ///
        /// <value>The name of the display.</value>
        string DisplayName { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        string LastName { get; set; }

        /// <summary>Gets or sets the email.</summary>
        ///
        /// <value>The email.</value>
        string Email { get; set; }

        /// <summary>Gets or sets the provider o authentication access.</summary>
        ///
        /// <value>The provider o authentication access.</value>
        List<IOAuthTokens> ProviderOAuthAccess { get; set; }

        /// <summary>Gets or sets the Date/Time of the created at.</summary>
        ///
        /// <value>The created at.</value>
        DateTime CreatedAt { get; set; }

        /// <summary>Gets or sets the Date/Time of the last modified.</summary>
        ///
        /// <value>The last modified.</value>
        DateTime LastModified { get; set; }

        /// <summary>Gets or sets the roles.</summary>
        ///
        /// <value>The roles.</value>
        List<string> Roles { get; set; }

        /// <summary>Gets or sets the permissions.</summary>
        ///
        /// <value>The permissions.</value>
        List<string> Permissions { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is authenticated.</summary>
        ///
        /// <value>true if this object is authenticated, false if not.</value>
        bool IsAuthenticated { get; set; }
        //Used for digest authentication replay protection
        string Sequence { get; set; }

        /// <summary>Query if 'role' has role.</summary>
        ///
        /// <param name="role">The role.</param>
        ///
        /// <returns>true if role, false if not.</returns>
        bool HasRole(string role);

        /// <summary>Query if 'permission' has permission.</summary>
        ///
        /// <param name="permission">The permission.</param>
        ///
        /// <returns>true if permission, false if not.</returns>
        bool HasPermission(string permission);

        /// <summary>Query if 'provider' is authorized.</summary>
        ///
        /// <param name="provider">The provider.</param>
        ///
        /// <returns>true if authorized, false if not.</returns>
        bool IsAuthorized(string provider);

        /// <summary>Executes the registered action.</summary>
        ///
        /// <param name="registrationService">The registration service.</param>
        void OnRegistered(IServiceBase registrationService);

        /// <summary>Executes the authenticated action.</summary>
        ///
        /// <param name="authService">The authentication service.</param>
        /// <param name="session">    The session.</param>
        /// <param name="tokens">     The tokens.</param>
        /// <param name="authInfo">   Information describing the authentication.</param>
        void OnAuthenticated(IServiceBase authService, IAuthSession session, IOAuthTokens tokens, Dictionary<string, string> authInfo);

        /// <summary>Executes the logout action.</summary>
        ///
        /// <param name="authService">The authentication service.</param>
        void OnLogout(IServiceBase authService);

        /// <summary>Executes the created action.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        void OnCreated(IHttpRequest httpReq);
    }
}