using System;
using System.Collections.Generic;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>Interface for i/o authentication tokens.</summary>
    public interface IOAuthTokens
    {
        /// <summary>Gets or sets the provider.</summary>
        ///
        /// <value>The provider.</value>
        string Provider { get; set; }

        /// <summary>Gets or sets the identifier of the user.</summary>
        ///
        /// <value>The identifier of the user.</value>
        string UserId { get; set; }

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

        /// <summary>Gets or sets the birth date.</summary>
        ///
        /// <value>The birth date.</value>
        DateTime? BirthDate { get; set; }

        /// <summary>Gets or sets the birth date raw.</summary>
        ///
        /// <value>The birth date raw.</value>
        string BirthDateRaw { get; set; }

        /// <summary>Gets or sets the country.</summary>
        ///
        /// <value>The country.</value>
        string Country { get; set; }

        /// <summary>Gets or sets the culture.</summary>
        ///
        /// <value>The culture.</value>
        string Culture { get; set; }

        /// <summary>Gets or sets the name of the full.</summary>
        ///
        /// <value>The name of the full.</value>
        string FullName { get; set; }

        /// <summary>Gets or sets the gender.</summary>
        ///
        /// <value>The gender.</value>
        string Gender { get; set; }

        /// <summary>Gets or sets the language.</summary>
        ///
        /// <value>The language.</value>
        string Language { get; set; }

        /// <summary>Gets or sets the mail address.</summary>
        ///
        /// <value>The mail address.</value>
        string MailAddress { get; set; }

        /// <summary>Gets or sets the nickname.</summary>
        ///
        /// <value>The nickname.</value>
        string Nickname { get; set; }

        /// <summary>Gets or sets the postal code.</summary>
        ///
        /// <value>The postal code.</value>
        string PostalCode { get; set; }

        /// <summary>Gets or sets the time zone.</summary>
        ///
        /// <value>The time zone.</value>
        string TimeZone { get; set; }

        /// <summary>Gets or sets the access token.</summary>
        ///
        /// <value>The access token.</value>
        string AccessToken { get; set; }

        /// <summary>Gets or sets the access token secret.</summary>
        ///
        /// <value>The access token secret.</value>
        string AccessTokenSecret { get; set; }

        /// <summary>Gets or sets the refresh token.</summary>
        ///
        /// <value>The refresh token.</value>
        string RefreshToken { get; set; }

        /// <summary>Gets or sets the Date/Time of the refresh token expiry.</summary>
        ///
        /// <value>The refresh token expiry.</value>
        DateTime? RefreshTokenExpiry { get; set; }

        /// <summary>Gets or sets the request token.</summary>
        ///
        /// <value>The request token.</value>
        string RequestToken { get; set; }

        /// <summary>Gets or sets the request token secret.</summary>
        ///
        /// <value>The request token secret.</value>
        string RequestTokenSecret { get; set; }

        /// <summary>Gets or sets the items.</summary>
        ///
        /// <value>The items.</value>
        Dictionary<string, string> Items { get; set; }
    }

    /// <summary>An authentication tokens.</summary>
    public class OAuthTokens : IOAuthTokens
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.OAuthTokens class.</summary>
        public OAuthTokens()
        {
            this.Items = new Dictionary<string, string>();
        }

        /// <summary>Gets or sets the provider.</summary>
        ///
        /// <value>The provider.</value>
        public string Provider { get; set; }

        /// <summary>Gets or sets the identifier of the user.</summary>
        ///
        /// <value>The identifier of the user.</value>
        public string UserId { get; set; }

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        public string UserName { get; set; }

        /// <summary>Gets or sets the name of the display.</summary>
        ///
        /// <value>The name of the display.</value>
        public string DisplayName { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        public string LastName { get; set; }

        /// <summary>Gets or sets the email.</summary>
        ///
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>Gets or sets the birth date.</summary>
        ///
        /// <value>The birth date.</value>
        public DateTime? BirthDate { get; set; }

        /// <summary>Gets or sets the birth date raw.</summary>
        ///
        /// <value>The birth date raw.</value>
        public string BirthDateRaw { get; set; }

        /// <summary>Gets or sets the country.</summary>
        ///
        /// <value>The country.</value>
        public string Country { get; set; }

        /// <summary>Gets or sets the culture.</summary>
        ///
        /// <value>The culture.</value>
        public string Culture { get; set; }

        /// <summary>Gets or sets the name of the full.</summary>
        ///
        /// <value>The name of the full.</value>
        public string FullName { get; set; }

        /// <summary>Gets or sets the gender.</summary>
        ///
        /// <value>The gender.</value>
        public string Gender { get; set; }

        /// <summary>Gets or sets the language.</summary>
        ///
        /// <value>The language.</value>
        public string Language { get; set; }

        /// <summary>Gets or sets the mail address.</summary>
        ///
        /// <value>The mail address.</value>
        public string MailAddress { get; set; }

        /// <summary>Gets or sets the nickname.</summary>
        ///
        /// <value>The nickname.</value>
        public string Nickname { get; set; }

        /// <summary>Gets or sets the postal code.</summary>
        ///
        /// <value>The postal code.</value>
        public string PostalCode { get; set; }

        /// <summary>Gets or sets the time zone.</summary>
        ///
        /// <value>The time zone.</value>
        public string TimeZone { get; set; }

        /// <summary>Gets or sets the access token.</summary>
        ///
        /// <value>The access token.</value>
        public string AccessToken { get; set; }

        /// <summary>Gets or sets the access token secret.</summary>
        ///
        /// <value>The access token secret.</value>
        public string AccessTokenSecret { get; set; }

        /// <summary>Gets or sets the refresh token.</summary>
        ///
        /// <value>The refresh token.</value>
        public string RefreshToken { get; set; }

        /// <summary>Gets or sets the Date/Time of the refresh token expiry.</summary>
        ///
        /// <value>The refresh token expiry.</value>
        public DateTime? RefreshTokenExpiry { get; set; }

        /// <summary>Gets or sets the request token.</summary>
        ///
        /// <value>The request token.</value>
        public string RequestToken { get; set; }

        /// <summary>Gets or sets the request token secret.</summary>
        ///
        /// <value>The request token secret.</value>
        public string RequestTokenSecret { get; set; }

        /// <summary>Gets or sets the items.</summary>
        ///
        /// <value>The items.</value>
        public Dictionary<string, string> Items { get; set; }
    }
}