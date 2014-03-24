using NServiceKit.CacheAccess;
using NServiceKit.Configuration;

namespace NServiceKit.LogicFacade
{
	public interface IApplicationContext
	{
		IFactoryProvider Factory { get; }

		T Get<T>() where T : class;

		ICacheClient Cache { get; }

		IResourceManager Resources { get; }
	}
}