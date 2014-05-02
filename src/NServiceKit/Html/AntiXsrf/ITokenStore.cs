// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Web;

namespace NServiceKit.Html.AntiXsrf
{
    // Provides an abstraction around how tokens are persisted and retrieved for a request
    internal interface ITokenStore
    {
        /// <summary>Gets cookie token.</summary>
        ///
        /// <param name="httpContext">Context for the HTTP.</param>
        ///
        /// <returns>The cookie token.</returns>
        AntiForgeryToken GetCookieToken(HttpContextBase httpContext);

        /// <summary>Gets form token.</summary>
        ///
        /// <param name="httpContext">Context for the HTTP.</param>
        ///
        /// <returns>The form token.</returns>
        AntiForgeryToken GetFormToken(HttpContextBase httpContext);

        /// <summary>Saves a cookie token.</summary>
        ///
        /// <param name="httpContext">Context for the HTTP.</param>
        /// <param name="token">      The token.</param>
        void SaveCookieToken(HttpContextBase httpContext, AntiForgeryToken token);
    }
}
