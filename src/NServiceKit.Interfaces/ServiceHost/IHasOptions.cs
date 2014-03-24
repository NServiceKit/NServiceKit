using System.Collections.Generic;

namespace NServiceKit.ServiceHost
{
	public interface IHasOptions
	{
		IDictionary<string, string> Options { get; }
	}
}