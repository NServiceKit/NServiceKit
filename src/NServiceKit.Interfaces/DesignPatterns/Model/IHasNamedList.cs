using System;
using System.Collections.Generic;

namespace NServiceKit.DesignPatterns.Model
{
	public interface IHasNamedList<T> : IHasNamed<IList<T>>
	{
	}
}