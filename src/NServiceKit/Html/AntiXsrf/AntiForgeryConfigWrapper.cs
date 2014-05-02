// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace NServiceKit.Html.AntiXsrf
{
    internal sealed class AntiForgeryConfigWrapper : IAntiForgeryConfig
    {
        /// <summary>Gets the additional data provider.</summary>
        ///
        /// <value>The additional data provider.</value>
        public IAntiForgeryAdditionalDataProvider AdditionalDataProvider
        {
            get
            {
                return AntiForgeryConfig.AdditionalDataProvider;
            }
        }

        /// <summary>Gets the name of the cookie.</summary>
        ///
        /// <value>The name of the cookie.</value>
        public string CookieName
        {
            get { return AntiForgeryConfig.CookieName; }
        }

        /// <summary>Gets the name of the form field.</summary>
        ///
        /// <value>The name of the form field.</value>
        public string FormFieldName
        {
            get { return AntiForgeryConfig.AntiForgeryTokenFieldName; }
        }

        /// <summary>Gets a value indicating whether the require ssl.</summary>
        ///
        /// <value>true if require ssl, false if not.</value>
        public bool RequireSSL
        {
            get { return AntiForgeryConfig.RequireSsl; }
        }

        /// <summary>Gets a value indicating whether the suppress identity heuristic checks.</summary>
        ///
        /// <value>true if suppress identity heuristic checks, false if not.</value>
        public bool SuppressIdentityHeuristicChecks
        {
            get { return AntiForgeryConfig.SuppressIdentityHeuristicChecks; }
        }

        /// <summary>Gets the identifier of the unique claim type.</summary>
        ///
        /// <value>The identifier of the unique claim type.</value>
        public string UniqueClaimTypeIdentifier
        {
            get { return AntiForgeryConfig.UniqueClaimTypeIdentifier; }
        }
    }
}
