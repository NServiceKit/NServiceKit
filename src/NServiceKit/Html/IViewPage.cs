using System;

namespace NServiceKit
{
    /// <summary>Interface for view page.</summary>
	public interface IViewPage
	{
        /// <summary>Gets a value indicating whether this object is compiled.</summary>
        ///
        /// <value>true if this object is compiled, false if not.</value>
		bool IsCompiled { get; }

        /// <summary>Compiles the given force.</summary>
        ///
        /// <param name="force">true to force.</param>
		void Compile(bool force=false);
	}
}

