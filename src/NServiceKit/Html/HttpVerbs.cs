// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.
using System;

namespace NServiceKit.Html
{
    /// <summary>Bitfield of flags for specifying HttpVerbs.</summary>
	[Flags]
	public enum HttpVerbs
	{
        /// <summary>A binary constant representing the get flag.</summary>
        Get = 1 << 0,

        /// <summary>A binary constant representing the post flag.</summary>
        Post = 1 << 1,

        /// <summary>A binary constant representing the put flag.</summary>
        Put = 1 << 2,

        /// <summary>A binary constant representing the delete flag.</summary>
        Delete = 1 << 3,

        /// <summary>A binary constant representing the head flag.</summary>
        Head = 1 << 4,

        /// <summary>A binary constant representing the patch flag.</summary>
        Patch = 1 << 5,

        /// <summary>A binary constant representing the options flag.</summary>
        Options = 1 << 6,
	}
}
