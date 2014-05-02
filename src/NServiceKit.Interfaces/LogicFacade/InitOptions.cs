using System;

namespace NServiceKit.LogicFacade
{
    /// <summary>Bitfield of flags for specifying InitOptions.</summary>
	[Flags]
	public enum InitOptions
	{
        /// <summary>A binary constant representing the none flag.</summary>
		None = 0,

        /// <summary>A binary constant representing the initialise only flag.</summary>
		InitialiseOnly = 1
	}
}