using System.Runtime.Serialization;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.Common.ServiceClient.Web
{
    /// <summary>Copy from NServiceKit.ServiceInterface.Auth to avoid deps.</summary>
    [DataContract]
    public class Auth : IReturn<AuthResponse>
    {
        /// <summary>Gets or sets the provider.</summary>
        ///
        /// <value>The provider.</value>
        [DataMember(Order=1)] public string provider { get; set; }

        /// <summary>Gets or sets the state.</summary>
        ///
        /// <value>The state.</value>
        [DataMember(Order=2)] public string State { get; set; }

        /// <summary>Gets or sets the oauth token.</summary>
        ///
        /// <value>The oauth token.</value>
        [DataMember(Order=3)] public string oauth_token { get; set; }

        /// <summary>Gets or sets the oauth verifier.</summary>
        ///
        /// <value>The oauth verifier.</value>
        [DataMember(Order=4)] public string oauth_verifier { get; set; }

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        [DataMember(Order=5)] public string UserName { get; set; }

        /// <summary>Gets or sets the password.</summary>
        ///
        /// <value>The password.</value>
        [DataMember(Order=6)] public string Password { get; set; }

        /// <summary>Gets or sets the remember me.</summary>
        ///
        /// <value>The remember me.</value>
        [DataMember(Order=7)] public bool? RememberMe { get; set; }

        /// <summary>Gets or sets the continue.</summary>
        ///
        /// <value>The continue.</value>
        [DataMember(Order=8)] public string Continue { get; set; }

        /// <summary>Gets or sets the nonce.</summary>
        ///
        /// <value>The nonce.</value>
        [DataMember(Order=9)] public string nonce { get; set; }

        /// <summary>Gets or sets URI of the document.</summary>
        ///
        /// <value>The URI.</value>
        [DataMember(Order=10)] public string uri { get; set; }

        /// <summary>Gets or sets the response.</summary>
        ///
        /// <value>The response.</value>
        [DataMember(Order=11)] public string response { get; set; }

        /// <summary>Gets or sets the qop.</summary>
        ///
        /// <value>The qop.</value>
        [DataMember(Order=12)] public string qop { get; set; }

        /// <summary>Gets or sets the non-client.</summary>
        ///
        /// <value>The non-client.</value>
        [DataMember(Order=13)] public string nc { get; set; }

        /// <summary>Gets or sets the cnonce.</summary>
        ///
        /// <value>The cnonce.</value>
        [DataMember(Order=14)] public string cnonce { get; set; }
    }

    /// <summary>An authentication response.</summary>
    [DataContract]
    public class AuthResponse
    {
        /// <summary>Initializes a new instance of the NServiceKit.Common.ServiceClient.Web.AuthResponse class.</summary>
        public AuthResponse()
        {
            this.ResponseStatus = new ResponseStatus();
        }

        /// <summary>Gets or sets the identifier of the session.</summary>
        ///
        /// <value>The identifier of the session.</value>
        [DataMember(Order=1)] public string SessionId { get; set; }

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        [DataMember(Order=2)] public string UserName { get; set; }

        /// <summary>Gets or sets URL of the referrer.</summary>
        ///
        /// <value>The referrer URL.</value>
        [DataMember(Order=3)] public string ReferrerUrl { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        [DataMember(Order=4)] public ResponseStatus ResponseStatus { get; set; }
    }


    /// <summary>A registration.</summary>
    [DataContract]
    public class Registration : IReturn<RegistrationResponse>
    {
        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        [DataMember(Order = 1)] public string UserName { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        [DataMember(Order = 2)] public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        [DataMember(Order = 3)] public string LastName { get; set; }

        /// <summary>Gets or sets the name of the display.</summary>
        ///
        /// <value>The name of the display.</value>
        [DataMember(Order = 4)] public string DisplayName { get; set; }

        /// <summary>Gets or sets the email.</summary>
        ///
        /// <value>The email.</value>
        [DataMember(Order = 5)] public string Email { get; set; }

        /// <summary>Gets or sets the password.</summary>
        ///
        /// <value>The password.</value>
        [DataMember(Order = 6)] public string Password { get; set; }

        /// <summary>Gets or sets the automatic login.</summary>
        ///
        /// <value>The automatic login.</value>
        [DataMember(Order = 7)] public bool? AutoLogin { get; set; }

        /// <summary>Gets or sets the continue.</summary>
        ///
        /// <value>The continue.</value>
        [DataMember(Order = 8)] public string Continue { get; set; }
    }

    /// <summary>A registration response.</summary>
    [DataContract]
    public class RegistrationResponse
    {
        /// <summary>Initializes a new instance of the NServiceKit.Common.ServiceClient.Web.RegistrationResponse class.</summary>
        public RegistrationResponse()
        {
            this.ResponseStatus = new ResponseStatus();
        }

        /// <summary>Gets or sets the identifier of the user.</summary>
        ///
        /// <value>The identifier of the user.</value>
        [DataMember(Order = 1)] public string UserId { get; set; }

        /// <summary>Gets or sets the identifier of the session.</summary>
        ///
        /// <value>The identifier of the session.</value>
        [DataMember(Order = 2)] public string SessionId { get; set; }

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        [DataMember(Order = 3)] public string UserName { get; set; }

        /// <summary>Gets or sets URL of the referrer.</summary>
        ///
        /// <value>The referrer URL.</value>
        [DataMember(Order = 4)] public string ReferrerUrl { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        [DataMember(Order = 5)] public ResponseStatus ResponseStatus { get; set; }
    }
}