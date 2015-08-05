using System;
using NServiceKit.WebHost.Endpoints;
using System.Linq;
using System.Collections.Generic;
///<summary> Checks HTTP Content Type</summary>
///Adds request filter, checks if request type is on list of accepted types
///Returns 406 error to http response if not
namespace NServiceKit
{
    public class ContentTypeCheckerFeature : IPlugin
    {
        List<string> AcceptedRequests = new List<string>{};

        public ContentTypeCheckerFeature(List<string> acceptedRequestTypes)
        {
            AcceptedRequests = acceptedRequestTypes;
        }
        /// <summary> Adds a request filter to apphost </summary>
        /// <param name="appHost"> the app host</param>
        public void Register(IAppHost appHost) {           
            appHost.RequestFilters.Add( (request, response, dto) => {
              if (!AcceptedRequests.Contains(request.ContentType, StringComparer.OrdinalIgnoreCase)) {
                  response.StatusCode = 406;
                  response.Close();
              }              
            });
        }
    }
}
