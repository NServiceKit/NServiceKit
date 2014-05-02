// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Web;

namespace NServiceKit.Html.AntiXsrf
{
    /// <summary>Exception for signalling HTTP anti forgery errors.</summary>
    [Serializable]
    public sealed class HttpAntiForgeryException : HttpException
    {
        /// <summary>Initializes a new instance of the NServiceKit.Html.AntiXsrf.HttpAntiForgeryException class.</summary>
        public HttpAntiForgeryException()
        {
        }

        private HttpAntiForgeryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Html.AntiXsrf.HttpAntiForgeryException class.</summary>
        ///
        /// <param name="message">The message.</param>
        public HttpAntiForgeryException(string message)
            : base(message)
        {
        }

        private HttpAntiForgeryException(string message, params object[] args)
            : this(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

        /// <summary>Initializes a new instance of the NServiceKit.Html.AntiXsrf.HttpAntiForgeryException class.</summary>
        ///
        /// <param name="message">       The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public HttpAntiForgeryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal static HttpAntiForgeryException CreateAdditionalDataCheckFailedException()
        {
            return new HttpAntiForgeryException(MvcResources.AntiForgeryToken_AdditionalDataCheckFailed);
        }

        internal static HttpAntiForgeryException CreateClaimUidMismatchException()
        {
            return new HttpAntiForgeryException(MvcResources.AntiForgeryToken_ClaimUidMismatch);
        }

        internal static HttpAntiForgeryException CreateCookieMissingException(string cookieName)
        {
            return new HttpAntiForgeryException(MvcResources.AntiForgeryToken_CookieMissing, cookieName);
        }

        internal static HttpAntiForgeryException CreateDeserializationFailedException()
        {
            return new HttpAntiForgeryException(MvcResources.AntiForgeryToken_DeserializationFailed);
        }

        internal static HttpAntiForgeryException CreateFormFieldMissingException(string formFieldName)
        {
            return new HttpAntiForgeryException(MvcResources.AntiForgeryToken_FormFieldMissing, formFieldName);
        }

        internal static HttpAntiForgeryException CreateSecurityTokenMismatchException()
        {
            return new HttpAntiForgeryException(MvcResources.AntiForgeryToken_SecurityTokenMismatch);
        }

        internal static HttpAntiForgeryException CreateTokensSwappedException(string cookieName, string formFieldName)
        {
            return new HttpAntiForgeryException(MvcResources.AntiForgeryToken_TokensSwapped, cookieName, formFieldName);
        }

        internal static HttpAntiForgeryException CreateUsernameMismatchException(string usernameInToken, string currentUsername)
        {
            return new HttpAntiForgeryException(MvcResources.AntiForgeryToken_UsernameMismatch, usernameInToken, currentUsername);
        }
    }
}
