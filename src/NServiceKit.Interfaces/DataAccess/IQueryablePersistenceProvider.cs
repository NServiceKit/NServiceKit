using System.Collections.Generic;
using NServiceKit.DataAccess.Criteria;

namespace NServiceKit.DataAccess
{
	public interface IQueryablePersistenceProvider : IPersistenceProvider, IQueryable
	{
		IList<T> GetAll<T>(ICriteria criteria)
			where T : class, new();
	}
}