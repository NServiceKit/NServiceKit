using System;
using System.Reflection;

namespace NServiceKit.LogicFacade
{
	/// <summary>
	/// The same functionality is on IServiceResolver
	/// </summary>
	[Obsolete]
	public interface IServiceModelFinder
	{
        /// <summary>Searches for the first type by operation.</summary>
        ///
        /// <param name="operationName">Name of the operation.</param>
        /// <param name="version">      The version.</param>
        ///
        /// <returns>The found type by operation.</returns>
		Type FindTypeByOperation(string operationName, int? version);
	}
}