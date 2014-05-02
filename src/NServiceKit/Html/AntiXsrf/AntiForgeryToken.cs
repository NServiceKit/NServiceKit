// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;

namespace NServiceKit.Html.AntiXsrf
{
    // Represents the security token for the Anti-XSRF system.
    // The token is a random 128-bit value that correlates the session with the request body.
    internal sealed class AntiForgeryToken
    {
        internal const int SecurityTokenBitLength = 128;
        internal const int ClaimUidBitLength = 256;

        private string _additionalData;
        private BinaryBlob _securityToken;
        private string _username;

        /// <summary>Gets or sets information describing the additional.</summary>
        ///
        /// <value>Information describing the additional.</value>
        public string AdditionalData
        {
            get
            {
                return _additionalData ?? String.Empty;
            }
            set
            {
                _additionalData = value;
            }
        }

        /// <summary>Gets or sets the claim UID.</summary>
        ///
        /// <value>The claim UID.</value>
        public BinaryBlob ClaimUid { get; set; }

        /// <summary>Gets or sets a value indicating whether this object is session token.</summary>
        ///
        /// <value>true if this object is session token, false if not.</value>
        public bool IsSessionToken { get; set; }

        /// <summary>Gets or sets the security token.</summary>
        ///
        /// <value>The security token.</value>
        public BinaryBlob SecurityToken
        {
            get
            {
                if (_securityToken == null) {
                    _securityToken = new BinaryBlob(SecurityTokenBitLength);
                }
                return _securityToken;
            }
            set
            {
                _securityToken = value;
            }
        }

        /// <summary>Gets or sets the username.</summary>
        ///
        /// <value>The username.</value>
        public string Username
        {
            get
            {
                return _username ?? String.Empty;
            }
            set
            {
                _username = value;
            }
        }
    }
}
