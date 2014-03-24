namespace NServiceKit.CacheAccess
{
	public interface IHasCacheClient
	{
		ICacheClient CacheClient { get; }
	}
}