// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace NServiceKit.Html.AntiXsrf
{
    // Provides an abstraction around the cryptographic subsystem for the anti-XSRF helpers.
    internal interface ICryptoSystem
    {
        /// <summary>Protects the given data.</summary>
        ///
        /// <param name="data">The data.</param>
        ///
        /// <returns>A string.</returns>
        string Protect(byte[] data);

        /// <summary>Unprotects.</summary>
        ///
        /// <param name="protectedData">Information describing the protected.</param>
        ///
        /// <returns>A byte[].</returns>
        byte[] Unprotect(string protectedData);
    }
}
