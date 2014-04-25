using System;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.IntegrationTests
{
	public class ResetMovieDatabaseService : ServiceInterface.Service
	{
        public ResetMovieDatabaseResponse Any(ResetMovieDatabase request)
		{
			return new ResetMovieDatabaseResponse();
		}
	}
}