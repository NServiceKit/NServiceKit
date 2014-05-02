// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
#if NET_4_0
using System.Diagnostics.Contracts;
#endif
using System.Globalization;
using System.Security.Principal;
using System.Web;

namespace NServiceKit.Html.AntiXsrf
{
    internal sealed class TokenValidator : ITokenValidator
    {
        private readonly IClaimUidExtractor _claimUidExtractor;
        private readonly IAntiForgeryConfig _config;

        internal TokenValidator(IAntiForgeryConfig config, IClaimUidExtractor claimUidExtractor)
        {
            _config = config;
            _claimUidExtractor = claimUidExtractor;
        }

        /// <summary>Generates a cookie token.</summary>
        ///
        /// <returns>The cookie token.</returns>
        public AntiForgeryToken GenerateCookieToken()
        {
            return new AntiForgeryToken()
            {
                // SecurityToken will be populated automatically.
                IsSessionToken = true
            };
        }

        /// <summary>Generates a form token.</summary>
        ///
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        ///
        /// <param name="httpContext">Context for the HTTP.</param>
        /// <param name="identity">   The identity.</param>
        /// <param name="cookieToken">The cookie token.</param>
        ///
        /// <returns>The form token.</returns>
        public AntiForgeryToken GenerateFormToken(HttpContextBase httpContext, IIdentity identity, AntiForgeryToken cookieToken)
        {
#if NET_4_0
            Contract.Assert(IsCookieTokenValid(cookieToken));
#endif
            AntiForgeryToken formToken = new AntiForgeryToken()
            {
                SecurityToken = cookieToken.SecurityToken,
                IsSessionToken = false
            };

            bool requireAuthenticatedUserHeuristicChecks = false;
            // populate Username and ClaimUid
            if (identity != null && identity.IsAuthenticated) {
                if (!_config.SuppressIdentityHeuristicChecks) {
                    // If the user is authenticated and heuristic checks are not suppressed,
                    // then Username, ClaimUid, or AdditionalData must be set.
                    requireAuthenticatedUserHeuristicChecks = true;
                }

                formToken.ClaimUid = _claimUidExtractor.ExtractClaimUid(identity);
                if (formToken.ClaimUid == null) {
                    formToken.Username = identity.Name;
                }
            }

            // populate AdditionalData
            if (_config.AdditionalDataProvider != null) {
                formToken.AdditionalData = _config.AdditionalDataProvider.GetAdditionalData(httpContext);
            }

            if (requireAuthenticatedUserHeuristicChecks
                && String.IsNullOrEmpty(formToken.Username)
                && formToken.ClaimUid == null
                && String.IsNullOrEmpty(formToken.AdditionalData)) {
                // Application says user is authenticated, but we have no identifier for the user.
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    MvcResources.TokenValidator_AuthenticatedUserWithoutUsername, identity.GetType()));
            }

            return formToken;
        }

        /// <summary>Query if 'cookieToken' is cookie token valid.</summary>
        ///
        /// <param name="cookieToken">The cookie token.</param>
        ///
        /// <returns>true if cookie token valid, false if not.</returns>
        public bool IsCookieTokenValid(AntiForgeryToken cookieToken)
        {
            return (cookieToken != null && cookieToken.IsSessionToken);
        }

        /// <summary>Validates the tokens.</summary>
        ///
        /// <exception cref="HttpAntiForgeryException">            Thrown when a Create Cookie Missing error condition occurs.</exception>
        /// <exception cref="HttpAntiForgeryException">         Thrown when a Create Form Field Missing error condition occurs.</exception>
        /// <exception cref="HttpAntiForgeryException">            Thrown when a Create Tokens Swapped error condition occurs.</exception>
        /// <exception cref="HttpAntiForgeryException">    Thrown when a Create Security Token Mismatch error condition occurs.</exception>
        /// <exception cref="HttpAntiForgeryException">         Thrown when a Create Username Mismatch error condition occurs.</exception>
        /// <exception cref="HttpAntiForgeryException">         Thrown when a Create Claim UID Mismatch error condition occurs.</exception>
        /// <exception cref="HttpAntiForgeryException">Thrown when a Create Additional Data Check Failed error condition occurs.</exception>
        ///
        /// <param name="httpContext"> Context for the HTTP.</param>
        /// <param name="identity">    The identity.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="fieldToken">  The field token.</param>
        public void ValidateTokens(HttpContextBase httpContext, IIdentity identity, AntiForgeryToken sessionToken, AntiForgeryToken fieldToken)
        {
            // Were the tokens even present at all?
            if (sessionToken == null) {
                throw HttpAntiForgeryException.CreateCookieMissingException(_config.CookieName);
            }
            if (fieldToken == null) {
                throw HttpAntiForgeryException.CreateFormFieldMissingException(_config.FormFieldName);
            }

            // Do the tokens have the correct format?
            if (!sessionToken.IsSessionToken || fieldToken.IsSessionToken) {
                throw HttpAntiForgeryException.CreateTokensSwappedException(_config.CookieName, _config.FormFieldName);
            }

            // Are the security tokens embedded in each incoming token identical?
            if (!Equals(sessionToken.SecurityToken, fieldToken.SecurityToken)) {
                throw HttpAntiForgeryException.CreateSecurityTokenMismatchException();
            }

            // Is the incoming token meant for the current user?
            string currentUsername = String.Empty;
            BinaryBlob currentClaimUid = null;

            if (identity != null && identity.IsAuthenticated) {
                currentClaimUid = _claimUidExtractor.ExtractClaimUid(identity);
                if (currentClaimUid == null) {
                    currentUsername = identity.Name ?? String.Empty;
                }
            }

            // OpenID and other similar authentication schemes use URIs for the username.
            // These should be treated as case-sensitive.
            bool useCaseSensitiveUsernameComparison = currentUsername.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || currentUsername.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

            if (!String.Equals(fieldToken.Username, currentUsername, (useCaseSensitiveUsernameComparison) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)) {
                throw HttpAntiForgeryException.CreateUsernameMismatchException(fieldToken.Username, currentUsername);
            }
            if (!Equals(fieldToken.ClaimUid, currentClaimUid)) {
                throw HttpAntiForgeryException.CreateClaimUidMismatchException();
            }

            // Is the AdditionalData valid?
            if (_config.AdditionalDataProvider != null && !_config.AdditionalDataProvider.ValidateAdditionalData(httpContext, fieldToken.AdditionalData)) {
                throw HttpAntiForgeryException.CreateAdditionalDataCheckFailedException();
            }
        }
    }
}
