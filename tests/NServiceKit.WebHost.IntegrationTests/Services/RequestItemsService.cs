using System;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface.ServiceModel;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A request items.</summary>
    [Route("/req-items")]
    public class RequestItems
    {
    }

    /// <summary>A request items response.</summary>
    public class RequestItemsResponse : IHasResponseStatus
    {
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
        public string Result { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>A request items service.</summary>
    public class RequestItemsService : ServiceInterface.Service
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(RequestItems request)
        {
            if (!Request.Items.ContainsKey("_DataSetAtPreRequestFilters"))
                throw new InvalidOperationException("DataSetAtPreRequestFilters missing.");

            if (!Request.Items.ContainsKey("_DataSetAtRequestFilters"))
                throw new InvalidOperationException("DataSetAtRequestFilters data missing.");

            return new RequestItemsResponse { Result = "MissionSuccess" };
        }
    }
}