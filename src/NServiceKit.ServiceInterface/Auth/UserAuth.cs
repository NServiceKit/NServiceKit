using System;
using System.Collections.Generic;
using NServiceKit.Common;
using NServiceKit.DataAnnotations;
using NServiceKit.Text;

namespace NServiceKit.ServiceInterface.Auth
{
    /// <summary>A user authentication.</summary>
    public class UserAuth
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.UserAuth class.</summary>
        public UserAuth()
        {
            this.Roles = new List<string>();
            this.Permissions = new List<string>();
        }

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        [AutoIncrement]
        public virtual int Id { get; set; }

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        public virtual string UserName { get; set; }

        /// <summary>Gets or sets the email.</summary>
        ///
        /// <value>The email.</value>
        public virtual string Email { get; set; }

        /// <summary>Gets or sets the primary email.</summary>
        ///
        /// <value>The primary email.</value>
        public virtual string PrimaryEmail { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        public virtual string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        public virtual string LastName { get; set; }

        /// <summary>Gets or sets the name of the display.</summary>
        ///
        /// <value>The name of the display.</value>
        public virtual string DisplayName { get; set; }

        /// <summary>Gets or sets the birth date.</summary>
        ///
        /// <value>The birth date.</value>
        public virtual DateTime? BirthDate { get; set; }

        /// <summary>Gets or sets the birth date raw.</summary>
        ///
        /// <value>The birth date raw.</value>
        public virtual string BirthDateRaw { get; set; }

        /// <summary>Gets or sets the country.</summary>
        ///
        /// <value>The country.</value>
        public virtual string Country { get; set; }

        /// <summary>Gets or sets the culture.</summary>
        ///
        /// <value>The culture.</value>
        public virtual string Culture { get; set; }

        /// <summary>Gets or sets the name of the full.</summary>
        ///
        /// <value>The name of the full.</value>
        public virtual string FullName { get; set; }

        /// <summary>Gets or sets the gender.</summary>
        ///
        /// <value>The gender.</value>
        public virtual string Gender { get; set; }

        /// <summary>Gets or sets the language.</summary>
        ///
        /// <value>The language.</value>
        public virtual string Language { get; set; }

        /// <summary>Gets or sets the mail address.</summary>
        ///
        /// <value>The mail address.</value>
        public virtual string MailAddress { get; set; }

        /// <summary>Gets or sets the nickname.</summary>
        ///
        /// <value>The nickname.</value>
        public virtual string Nickname { get; set; }

        /// <summary>Gets or sets the postal code.</summary>
        ///
        /// <value>The postal code.</value>
        public virtual string PostalCode { get; set; }

        /// <summary>Gets or sets the time zone.</summary>
        ///
        /// <value>The time zone.</value>
        public virtual string TimeZone { get; set; }

        /// <summary>Gets or sets the salt.</summary>
        ///
        /// <value>The salt.</value>
        public virtual string Salt { get; set; }

        /// <summary>Gets or sets the password hash.</summary>
        ///
        /// <value>The password hash.</value>
        public virtual string PasswordHash { get; set; }

        /// <summary>Gets or sets the digest ha 1 hash.</summary>
        ///
        /// <value>The digest ha 1 hash.</value>
        public virtual string DigestHa1Hash { get; set; }

        /// <summary>Gets or sets the roles.</summary>
        ///
        /// <value>The roles.</value>
        public virtual List<string> Roles { get; set; }

        /// <summary>Gets or sets the permissions.</summary>
        ///
        /// <value>The permissions.</value>
        public virtual List<string> Permissions { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        ///
        /// <value>The created date.</value>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the modified date.</summary>
        ///
        /// <value>The modified date.</value>
        public virtual DateTime ModifiedDate { get; set; }

        //Custom Reference Data
        public virtual int? RefId { get; set; }

        /// <summary>Gets or sets the reference identifier string.</summary>
        ///
        /// <value>The reference identifier string.</value>
        public virtual string RefIdStr { get; set; }

        /// <summary>Gets or sets the meta.</summary>
        ///
        /// <value>The meta.</value>
        public virtual Dictionary<string, string> Meta { get; set; }

        /// <summary>Gets the get.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
        public virtual T Get<T>()
        {
            string str = null;
            if (Meta != null) Meta.TryGetValue(typeof(T).Name, out str);
            return str == null ? default(T) : TypeSerializer.DeserializeFromString<T>(str);
        }

        /// <summary>Sets the given value.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value.</param>
        public virtual void Set<T>(T value)
        {
            if (Meta == null) Meta = new Dictionary<string, string>();
            Meta[typeof(T).Name] = TypeSerializer.SerializeToString(value);
        }

        /// <summary>Populate missing.</summary>
        ///
        /// <param name="authProvider">The authentication provider.</param>
        public virtual void PopulateMissing(UserOAuthProvider authProvider)
        {
            //Don't explicitly override after if values exist
            if (!authProvider.DisplayName.IsNullOrEmpty() && this.DisplayName.IsNullOrEmpty())
                this.DisplayName = authProvider.DisplayName;
            if (!authProvider.Email.IsNullOrEmpty() && this.PrimaryEmail.IsNullOrEmpty())
                this.PrimaryEmail = authProvider.Email;

            if (!authProvider.FirstName.IsNullOrEmpty())
                this.FirstName = authProvider.FirstName;
            if (!authProvider.LastName.IsNullOrEmpty())
                this.LastName = authProvider.LastName;
            if (!authProvider.FullName.IsNullOrEmpty())
                this.FullName = authProvider.FullName;
            if (authProvider.BirthDate != null)
                this.BirthDate = authProvider.BirthDate;
            if (!authProvider.BirthDateRaw.IsNullOrEmpty())
                this.BirthDateRaw = authProvider.BirthDateRaw;
            if (!authProvider.Country.IsNullOrEmpty())
                this.Country = authProvider.Country;
            if (!authProvider.Culture.IsNullOrEmpty())
                this.Culture = authProvider.Culture;
            if (!authProvider.Gender.IsNullOrEmpty())
                this.Gender = authProvider.Gender;
            if (!authProvider.MailAddress.IsNullOrEmpty())
                this.MailAddress = authProvider.MailAddress;
            if (!authProvider.Nickname.IsNullOrEmpty())
                this.Nickname = authProvider.Nickname;
            if (!authProvider.PostalCode.IsNullOrEmpty())
                this.PostalCode = authProvider.PostalCode;
            if (!authProvider.TimeZone.IsNullOrEmpty())
                this.TimeZone = authProvider.TimeZone;
        }
    }

    /// <summary>A user o authentication provider.</summary>
    public class UserOAuthProvider : IOAuthTokens
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Auth.UserOAuthProvider class.</summary>
        public UserOAuthProvider()
        {
            this.Items = new Dictionary<string, string>();
        }

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        [AutoIncrement]
        public virtual int Id { get; set; }

        /// <summary>Gets or sets the identifier of the user authentication.</summary>
        ///
        /// <value>The identifier of the user authentication.</value>
        public virtual int UserAuthId { get; set; }

        /// <summary>Gets or sets the provider.</summary>
        ///
        /// <value>The provider.</value>
        public virtual string Provider { get; set; }

        /// <summary>Gets or sets the identifier of the user.</summary>
        ///
        /// <value>The identifier of the user.</value>
        public virtual string UserId { get; set; }

        /// <summary>Gets or sets the name of the user.</summary>
        ///
        /// <value>The name of the user.</value>
        public virtual string UserName { get; set; }

        /// <summary>Gets or sets the name of the full.</summary>
        ///
        /// <value>The name of the full.</value>
        public virtual string FullName { get; set; }

        /// <summary>Gets or sets the name of the display.</summary>
        ///
        /// <value>The name of the display.</value>
        public virtual string DisplayName { get; set; }

        /// <summary>Gets or sets the person's first name.</summary>
        ///
        /// <value>The name of the first.</value>
        public virtual string FirstName { get; set; }

        /// <summary>Gets or sets the person's last name.</summary>
        ///
        /// <value>The name of the last.</value>
        public virtual string LastName { get; set; }

        /// <summary>Gets or sets the email.</summary>
        ///
        /// <value>The email.</value>
        public virtual string Email { get; set; }

        /// <summary>Gets or sets the birth date.</summary>
        ///
        /// <value>The birth date.</value>
        public virtual DateTime? BirthDate { get; set; }

        /// <summary>Gets or sets the birth date raw.</summary>
        ///
        /// <value>The birth date raw.</value>
        public virtual string BirthDateRaw { get; set; }

        /// <summary>Gets or sets the country.</summary>
        ///
        /// <value>The country.</value>
        public virtual string Country { get; set; }

        /// <summary>Gets or sets the culture.</summary>
        ///
        /// <value>The culture.</value>
        public virtual string Culture { get; set; }

        /// <summary>Gets or sets the gender.</summary>
        ///
        /// <value>The gender.</value>
        public virtual string Gender { get; set; }

        /// <summary>Gets or sets the language.</summary>
        ///
        /// <value>The language.</value>
        public virtual string Language { get; set; }

        /// <summary>Gets or sets the mail address.</summary>
        ///
        /// <value>The mail address.</value>
        public virtual string MailAddress { get; set; }

        /// <summary>Gets or sets the nickname.</summary>
        ///
        /// <value>The nickname.</value>
        public virtual string Nickname { get; set; }

        /// <summary>Gets or sets the postal code.</summary>
        ///
        /// <value>The postal code.</value>
        public virtual string PostalCode { get; set; }

        /// <summary>Gets or sets the time zone.</summary>
        ///
        /// <value>The time zone.</value>
        public virtual string TimeZone { get; set; }

        /// <summary>Gets or sets the refresh token.</summary>
        ///
        /// <value>The refresh token.</value>
        public virtual string RefreshToken { get; set; }

        /// <summary>Gets or sets the Date/Time of the refresh token expiry.</summary>
        ///
        /// <value>The refresh token expiry.</value>
        public virtual DateTime? RefreshTokenExpiry { get; set; }

        /// <summary>Gets or sets the request token.</summary>
        ///
        /// <value>The request token.</value>
        public virtual string RequestToken { get; set; }

        /// <summary>Gets or sets the request token secret.</summary>
        ///
        /// <value>The request token secret.</value>
        public virtual string RequestTokenSecret { get; set; }

        /// <summary>Gets or sets the items.</summary>
        ///
        /// <value>The items.</value>
        public virtual Dictionary<string, string> Items { get; set; }

        /// <summary>Gets or sets the access token.</summary>
        ///
        /// <value>The access token.</value>
        public virtual string AccessToken { get; set; }

        /// <summary>Gets or sets the access token secret.</summary>
        ///
        /// <value>The access token secret.</value>
        public virtual string AccessTokenSecret { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        ///
        /// <value>The created date.</value>
        public virtual DateTime CreatedDate { get; set; }

        /// <summary>Gets or sets the modified date.</summary>
        ///
        /// <value>The modified date.</value>
        public virtual DateTime ModifiedDate { get; set; }

        //Custom Reference Data
        public virtual int? RefId { get; set; }

        /// <summary>Gets or sets the reference identifier string.</summary>
        ///
        /// <value>The reference identifier string.</value>
        public virtual string RefIdStr { get; set; }

        /// <summary>Gets or sets the meta.</summary>
        ///
        /// <value>The meta.</value>
        public virtual Dictionary<string, string> Meta { get; set; }

        /// <summary>Gets the get.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
        public virtual T Get<T>()
        {
            string str = null;
            if (Meta != null) Meta.TryGetValue(typeof(T).Name, out str);
            return str == null ? default(T) : TypeSerializer.DeserializeFromString<T>(str);
        }

        /// <summary>Sets the given value.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value.</param>
        public virtual void Set<T>(T value)
        {
            if (Meta == null) Meta = new Dictionary<string, string>();
            Meta[typeof(T).Name] = TypeSerializer.SerializeToString(value);
        }

        /// <summary>Populate missing.</summary>
        ///
        /// <param name="withTokens">The with tokens.</param>
        public virtual void PopulateMissing(IOAuthTokens withTokens)
        {
            if (!withTokens.UserId.IsNullOrEmpty())
                this.UserId = withTokens.UserId;
            if (!withTokens.UserName.IsNullOrEmpty())
                this.UserName = withTokens.UserName;
            if (!withTokens.RefreshToken.IsNullOrEmpty())
                this.RefreshToken = withTokens.RefreshToken;
            if (withTokens.RefreshTokenExpiry.HasValue)
                this.RefreshTokenExpiry = withTokens.RefreshTokenExpiry;
            if (!withTokens.RequestToken.IsNullOrEmpty())
                this.RequestToken = withTokens.RequestToken;
            if (!withTokens.RequestTokenSecret.IsNullOrEmpty())
                this.RequestTokenSecret = withTokens.RequestTokenSecret;
            if (!withTokens.AccessToken.IsNullOrEmpty())
                this.AccessToken = withTokens.AccessToken;
            if (!withTokens.AccessTokenSecret.IsNullOrEmpty())
                this.AccessTokenSecret = withTokens.AccessTokenSecret;
            if (!withTokens.DisplayName.IsNullOrEmpty())
                this.DisplayName = withTokens.DisplayName;
            if (!withTokens.FirstName.IsNullOrEmpty())
                this.FirstName = withTokens.FirstName;
            if (!withTokens.LastName.IsNullOrEmpty())
                this.LastName = withTokens.LastName;
            if (!withTokens.Email.IsNullOrEmpty())
                this.Email = withTokens.Email;
            if (!withTokens.FullName.IsNullOrEmpty())
                this.FullName = withTokens.FullName;
            if (withTokens.BirthDate != null)
                this.BirthDate = withTokens.BirthDate;
            if (!withTokens.BirthDateRaw.IsNullOrEmpty())
                this.BirthDateRaw = withTokens.BirthDateRaw;
            if (!withTokens.Country.IsNullOrEmpty())
                this.Country = withTokens.Country;
            if (!withTokens.Culture.IsNullOrEmpty())
                this.Culture = withTokens.Culture;
            if (!withTokens.Gender.IsNullOrEmpty())
                this.Gender = withTokens.Gender;
            if (!withTokens.MailAddress.IsNullOrEmpty())
                this.MailAddress = withTokens.MailAddress;
            if (!withTokens.Nickname.IsNullOrEmpty())
                this.Nickname = withTokens.Nickname;
            if (!withTokens.PostalCode.IsNullOrEmpty())
                this.PostalCode = withTokens.PostalCode;
            if (!withTokens.TimeZone.IsNullOrEmpty())
                this.TimeZone = withTokens.TimeZone;
        }
    }

}
