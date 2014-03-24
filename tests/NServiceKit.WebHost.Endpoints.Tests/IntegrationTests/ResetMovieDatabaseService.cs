using System;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.IntegrationTests
{
	public class ResetMovieDatabaseService
		: IService<ResetMovieDatabase>
	{
		public object Execute(ResetMovieDatabase request)
		{
			return new ResetMovieDatabaseResponse();
		}
	}
}