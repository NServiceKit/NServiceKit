using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>An authentication user session.</summary>
    [DataContract]
    public class AuthUserSession : IAuthSession
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.AuthUserSession class.</summary>
        public AuthUserSession()
        {
            this.ProviderOAuthAccess = new List<IOAuthTokens>();
        }

        /// <summary>Gets or sets URL of the referrer.</summary>
        ///
        /// <value>The referrer URL.</value>
        [DataMember(Order = 01)] public string ReferrerUrl { get; set; }

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        [DataMember(Order = 02)] public string Id { get; set; }

        /// <summary>Gets or sets the identifier of the user authentication.</summary>
        ///
        /// <value>The identifier of the user authentication.</value>
        [DataMember(Order = 03)] public string UserAuthId { get; set; }

        /// <summary>Gets or sets the name of the user authentication.</summary>
        ///
        /// <value>The name of the user authentication.</value>
        [DataMember(Order = 04)] public string UserAuthName { get; set; }

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        [DataMember(Order = 05)] public string UserName { get; set; }

        /// <summary>Gets or sets the identifier of the twitter user.</summary>
        ///
        /// <value>The identifier of the twitter user.</value>
        [DataMember(Order = 06)] public string TwitterUserId { get; set; }

        /// <summary>Gets or sets the name of the twitter screen.</summary>
        ///
        /// <value>The name of the twitter screen.</value>
        [DataMember(Order = 07)] public string TwitterScreenName { get; set; }

        /// <summary>Gets or sets the identifier of the facebook user.</summary>
        ///
        /// <value>The identifier of the facebook user.</value>
        [DataMember(Order = 08)] public string FacebookUserId { get; set; }

        /// <summary>Gets or sets the name of the facebook user.</summary>
        ///
        /// <value>The name of the facebook user.</value>
        [DataMember(Order = 09)] public string FacebookUserName { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        [DataMember(Order = 10)] public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        [DataMember(Order = 11)] public string LastName { get; set; }

        /// <summary>Gets or sets the name of the display.</summary>
        ///
        /// <value>The name of the display.</value>
        [DataMember(Order = 12)] public string DisplayName { get; set; }

        /// <summary>Gets or sets the email.</summary>
        ///
        /// <value>The email.</value>
        [DataMember(Order = 13)] public string Email { get; set; }

        /// <summary>Gets or sets the primary email.</summary>
        ///
        /// <value>The primary email.</value>
        [DataMember(Order = 14)] public string PrimaryEmail { get; set; }

        /// <summary>Gets or sets the birth date.</summary>
        ///
        /// <value>The birth date.</value>
        [DataMember(Order = 15)] public DateTime? BirthDate { get; set; }

        /// <summary>Gets or sets the birth date raw.</summary>
        ///
        /// <value>The birth date raw.</value>
        [DataMember(Order = 16)] public string BirthDateRaw { get; set; }

        /// <summary>Gets or sets the country.</summary>
        ///
        /// <value>The country.</value>
        [DataMember(Order = 17)] public string Country { get; set; }

        /// <summary>Gets or sets the culture.</summary>
        ///
        /// <value>The culture.</value>
        [DataMember(Order = 18)] public string Culture { get; set; }

        /// <summary>Gets or sets the name of the full.</summary>
        ///
        /// <value>The name of the full.</value>
        [DataMember(Order = 19)] public string FullName { get; set; }

        /// <summary>Gets or sets the gender.</summary>
        ///
        /// <value>The gender.</value>
        [DataMember(Order = 20)] public string Gender { get; set; }

        /// <summary>Gets or sets the language.</summary>
        ///
        /// <value>The language.</value>
        [DataMember(Order = 21)] public string Language { get; set; }

        /// <summary>Gets or sets the mail address.</summary>
        ///
        /// <value>The mail address.</value>
        [DataMember(Order = 22)] public string MailAddress { get; set; }

        /// <summary>Gets or sets the nickname.</summary>
        ///
        /// <value>The nickname.</value>
        [DataMember(Order = 23)] public string Nickname { get; set; }

        /// <summary>Gets or sets the postal code.</summary>
        ///
        /// <value>The postal code.</value>
        [DataMember(Order = 24)] public string PostalCode { get; set; }

        /// <summary>Gets or sets the time zone.</summary>
        ///
        /// <value>The time zone.</value>
        [DataMember(Order = 25)] public string TimeZone { get; set; }

        /// <summary>Gets or sets the request token secret.</summary>
        ///
        /// <value>The request token secret.</value>
        [DataMember(Order = 26)] public string RequestTokenSecret { get; set; }

        /// <summary>Gets or sets the Date/Time of the created at.</summary>
        ///
        /// <value>The created at.</value>
        [DataMember(Order = 27)] public DateTime CreatedAt { get; set; }

        /// <summary>Gets or sets the Date/Time of the last modified.</summary>
        ///
        /// <value>The last modified.</value>
        [DataMember(Order = 28)] public DateTime LastModified { get; set; }

        /// <summary>Gets or sets the provider o authentication access.</summary>
        ///
        /// <value>The provider o authentication access.</value>
        [DataMember(Order = 29)] public List<IOAuthTokens> ProviderOAuthAccess { get; set; }

        /// <summary>Gets or sets the roles.</summary>
        ///
        /// <value>The roles.</value>
        [DataMember(Order = 30)] public List<string> Roles { get; set; }

        /// <summary>Gets or sets the permissions.</summary>
        ///
        /// <value>The permissions.</value>
        [DataMember(Order = 31)] public List<string> Permissions { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is authenticated.</summary>
        ///
        /// <value>true if this object is authenticated, false if not.</value>
        [DataMember(Order = 32)] public virtual bool IsAuthenticated { get; set; }

        /// <summary>Gets or sets the sequence.</summary>
        ///
        /// <value>The sequence.</value>
        [DataMember(Order = 33)] public virtual string Sequence { get; set; }

        /// <summary>Gets or sets the tag.</summary>
        ///
        /// <value>The tag.</value>
        [DataMember(Order = 34)] public virtual long Tag { get; set; }

        /// <summary>Query if 'provider' is authorized.</summary>
        ///
        /// <param name="provider">The provider.</param>
        ///
        /// <returns>true if authorized, false if not.</returns>
        public virtual bool IsAuthorized(string provider)
        {
            var tokens = ProviderOAuthAccess.FirstOrDefault(x => x.Provider == provider);
            return AuthService.GetAuthProvider(provider).IsAuthorizedSafe(this, tokens);
        }

        /// <summary>Query if 'permission' has permission.</summary>
        ///
        /// <param name="permission">The permission.</param>
        ///
        /// <returns>true if permission, false if not.</returns>
        public virtual bool HasPermission(string permission)
        {
            return this.Permissions != null && this.Permissions.Contains(permission);
        }

        /// <summary>Query if 'role' has role.</summary>
        ///
        /// <param name="role">The role.</param>
        ///
        /// <returns>true if role, false if not.</returns>
        public virtual bool HasRole(string role)
        {
            return this.Roles != null && this.Roles.Contains(role);
        }

        /// <summary>Executes the registered action.</summary>
        ///
        /// <param name="registrationService">The registration service.</param>
        public virtual void OnRegistered(IServiceBase registrationService) {}

        /// <summary>Executes the authenticated action.</summary>
        ///
        /// <param name="authService">The authentication service.</param>
        /// <param name="session">    The session.</param>
        /// <param name="tokens">     The tokens.</param>
        /// <param name="authInfo">   Information describing the authentication.</param>
        public virtual void OnAuthenticated(IServiceBase authService, IAuthSession session, IOAuthTokens tokens, Dictionary<string, string> authInfo) {}

        /// <summary>Executes the logout action.</summary>
        ///
        /// <param name="authService">The authentication service.</param>
        public virtual void OnLogout(IServiceBase authService) {}

        /// <summary>Executes the created action.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        public virtual void OnCreated(IHttpRequest httpReq) {}
    }

    /// <summary>An authentication session extensions.</summary>
    public static class AuthSessionExtensions
    {
        /// <summary>An IAuthSession extension method that gets o authentication tokens.</summary>
        ///
        /// <param name="session"> The session to act on.</param>
        /// <param name="provider">The provider.</param>
        ///
        /// <returns>The o authentication tokens.</returns>
        public static IOAuthTokens GetOAuthTokens(this IAuthSession session, string provider)
        {
            foreach (var tokens in session.ProviderOAuthAccess)
            {
                if (string.Compare(tokens.Provider, provider, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return tokens;
            }
            return null;
        }
    }
}