using System;
using System.Collections.Generic;

namespace NServiceKit.DesignPatterns.Model
{
    /// <summary>Interface for has named list.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public interface IHasNamedList<T> : IHasNamed<IList<T>>
	{
	}
}