using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using NServiceKit.Common;
using NServiceKit.Common.Extensions;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Api.Swagger
{
    /// <summary>A resources.</summary>
    [DataContract]
    public class Resources
    {
        /// <summary>Gets or sets the API key.</summary>
        ///
        /// <value>The API key.</value>
        [DataMember(Name = "apiKey")]
        public string ApiKey { get; set; }
    }

    /// <summary>The resources response.</summary>
    [DataContract]
    public class ResourcesResponse
    {
        /// <summary>Gets or sets the swagger version.</summary>
        ///
        /// <value>The swagger version.</value>
        [DataMember(Name = "swaggerVersion")]
        public string SwaggerVersion { get; set; }

        /// <summary>Gets or sets the API version.</summary>
        ///
        /// <value>The API version.</value>
        [DataMember(Name = "apiVersion")]
        public string ApiVersion { get; set; }

        /// <summary>Gets or sets the full pathname of the base file.</summary>
        ///
        /// <value>The full pathname of the base file.</value>
        [DataMember(Name = "basePath")]
        public string BasePath { get; set; }

        /// <summary>Gets or sets the apis.</summary>
        ///
        /// <value>The apis.</value>
        [DataMember(Name = "apis")]
        public List<RestService> Apis { get; set; }
    }

    /// <summary>A rest service.</summary>
    [DataContract]
    public class RestService
    {
        /// <summary>Gets or sets the full pathname of the file.</summary>
        ///
        /// <value>The full pathname of the file.</value>
        [DataMember(Name = "path")]
        public string Path { get; set; }

        /// <summary>Gets or sets the description.</summary>
        ///
        /// <value>The description.</value>
        [DataMember(Name = "description")]
        public string Description { get; set; }
    }

    /// <summary>A swagger resources service.</summary>
    [DefaultRequest(typeof(Resources))]
    public class SwaggerResourcesService : ServiceInterface.Service
    {
        private readonly Regex resourcePathCleanerRegex = new Regex(@"/[^\/\{]*", RegexOptions.Compiled);
        internal static Regex resourceFilterRegex;

        internal const string RESOURCE_PATH = "/resource";

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(Resources request)
        {
            var basePath = EndpointHost.Config.WebHostUrl;
            if (basePath == null)
            {
                basePath = EndpointHost.Config.UseHttpsLinks
                    ? Request.GetParentPathUrl().ToHttps()
                    : Request.GetParentPathUrl();
            }

            var result = new ResourcesResponse
            {
                SwaggerVersion = "1.1",
                BasePath = basePath,
                Apis = new List<RestService>()
            };
            var operations = EndpointHost.Metadata;
            var allTypes = operations.GetAllTypes();
            var allOperationNames = operations.GetAllOperationNames();
            foreach (var operationName in allOperationNames)
            {
                if (resourceFilterRegex != null && !resourceFilterRegex.IsMatch(operationName)) continue;
                var name = operationName;
                var operationType = allTypes.FirstOrDefault(x => x.Name == name);
                if (operationType == null) continue;
                if (operationType == typeof(Resources) || operationType == typeof(ResourceRequest))
                    continue;
                if (!operations.IsVisible(Request, Format.Json, operationName)) continue;

                CreateRestPaths(result.Apis, operationType, operationName);
            }

            result.Apis = result.Apis.OrderBy(a => a.Path).ToList();
            return result;
        }

        /// <summary>Creates rest paths.</summary>
        ///
        /// <param name="apis">         The apis.</param>
        /// <param name="operationType">Type of the operation.</param>
        /// <param name="operationName">Name of the operation.</param>
        protected void CreateRestPaths(List<RestService> apis, Type operationType, String operationName)
        {
            var map = EndpointHost.ServiceManager.ServiceController.RestPathMap;
            var paths = new List<string>();
            foreach (var key in map.Keys)
            {
                paths.AddRange(map[key].Where(x => x.RequestType == operationType).Select(t => resourcePathCleanerRegex.Match(t.Path).Value));
            }

            if (paths.Count == 0) return;

            var basePaths = paths.Select(t => string.IsNullOrEmpty(t) ? null : t.Split('/'))
                .Where(t => t != null && t.Length > 1)
                .Select(t => t[1]);

            foreach (var bp in basePaths)
            {
                if (string.IsNullOrEmpty(bp)) continue;
                if (apis.All(a => a.Path != string.Concat(RESOURCE_PATH, "/" + bp)))
                {
                    apis.Add(new RestService
                    {
                        Path = string.Concat(RESOURCE_PATH, "/" + bp),
                        Description = operationType.GetDescription()
                    });
                }
            }
        }
    }
}
