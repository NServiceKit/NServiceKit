using System;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.IntegrationTests
{
    /// <summary>A reset movie database service.</summary>
	public class ResetMovieDatabaseService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A ResetMovieDatabaseResponse.</returns>
        public ResetMovieDatabaseResponse Any(ResetMovieDatabase request)
		{
			return new ResetMovieDatabaseResponse();
		}
	}
}