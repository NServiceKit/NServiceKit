using System.Collections.Generic;

namespace NServiceKit.DesignPatterns.Model
{
    /// <summary>Interface for has named collection.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public interface IHasNamedCollection<T> : IHasNamed<ICollection<T>>
	{
	}
}