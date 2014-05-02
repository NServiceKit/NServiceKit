// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace NServiceKit.Html.AntiXsrf
{
    // Abstracts out the serialization process for an anti-forgery token
    internal interface IAntiForgeryTokenSerializer
    {
        /// <summary>true this object to the given stream.</summary>
        ///
        /// <param name="serializedToken">The serialized token.</param>
        ///
        /// <returns>An AntiForgeryToken.</returns>
        AntiForgeryToken Deserialize(string serializedToken);

        /// <summary>true this object to the given stream.</summary>
        ///
        /// <param name="token">The token.</param>
        ///
        /// <returns>A string.</returns>
        string Serialize(AntiForgeryToken token);
    }
}
