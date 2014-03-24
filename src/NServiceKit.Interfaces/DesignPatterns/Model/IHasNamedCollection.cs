using System.Collections.Generic;

namespace NServiceKit.DesignPatterns.Model
{
	public interface IHasNamedCollection<T> : IHasNamed<ICollection<T>>
	{
	}
}