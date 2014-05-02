// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Security.Principal;

namespace NServiceKit.Html.AntiXsrf
{
    // Can extract unique identifers for a claims-based identity
    internal interface IClaimUidExtractor
    {
        /// <summary>Extracts the claim UID described by identity.</summary>
        ///
        /// <param name="identity">The identity.</param>
        ///
        /// <returns>The extracted claim UID.</returns>
        BinaryBlob ExtractClaimUid(IIdentity identity);
    }
}
