// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwaggerResourcesService.cs" company="Vistaprint">
//   Copyright (c) Vistaprint.  All rights reserved.
// </copyright>
// <summary>
//   The resources.
// </summary>
// <remarks>
//  This file was forked from the ServiceStack.Swagger plugin. The code is in a somewhat messy state, but it works relatively well.
// </remarks>
// --------------------------------------------------------------------------------------------------------------------

using NServiceKit.Common;

namespace NServiceKit.Api.Swagger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using NServiceKit.Common.Extensions;
    using NServiceKit.ServiceHost;
    using NServiceKit.ServiceInterface;
    using NServiceKit.WebHost.Endpoints;

    using NServiceKit.Api.Swagger.Models;

    /// <summary>
    /// The swagger resources service.
    /// </summary>
    [DefaultRequest(typeof(ResourcesRequest))]
    public class SwaggerResourcesService : Service
    {
        private readonly Regex resourcePathCleanerRegex = new Regex(@"/[^\/\{]*", RegexOptions.Compiled);

        /// <summary>
        /// The path to the swagger resources.
        /// </summary>
        public static string ResourcePath { get; set; }

        internal static Regex ResourceFilterRegex { get; set; }

        internal static Regex ResourcePathFilterRegex { get; set; }

        internal static string ApiVersion { get; set; }

        /// <summary>
        /// Return a swagger resources listing for this service.
        /// </summary>
        /// <param name="request">
        /// The request to list swagger resources.
        /// </param>
        /// <returns>
        /// A list of valid swagger endpoints on this service.
        /// </returns>
        public object Get(ResourcesRequest request)
        {
            var basePath = EndpointHost.Config.WebHostUrl;
            if (basePath == null)
            {
                basePath = EndpointHost.Config.UseHttpsLinks
                               ? this.Request.GetParentPathUrl().ToHttps()
                               : this.Request.GetParentPathUrl();
            }

            var result = new ResourcesResponse
            {
                SwaggerVersion = SwaggerFeature.SwaggerVersion,
                ApiVersion = ApiVersion,
                BasePath = basePath,
                Apis = new List<RestService>()
            };
            var operations = EndpointHost.Metadata;
            var allTypes = operations.GetAllTypes();
            var allOperationNames = operations.GetAllOperationNames();
            foreach (var operationName in allOperationNames)
            {
                if (ResourceFilterRegex != null && !ResourceFilterRegex.IsMatch(operationName))
                {
                    continue;
                }

                var name = operationName;
                var operationType = allTypes.FirstOrDefault(x => x.Name == name);
                if (operationType == null)
                {
                    continue;
                }

                if (operationType == typeof(ResourcesRequest)
                    || operationType == typeof(ResourceRequest))
                {
                    continue;
                }

                if (!operations.IsVisible(this.Request, Format.Json, operationName))
                {
                    continue;
                }

                this.CreateRestPaths(result.Apis, operationType, operationName);
            }

            result.Apis = result.Apis.OrderBy(a => a.Path).ToList();
            return result;
        }

        private void CreateRestPaths(List<RestService> apis, Type operationType, string operationName)
        {
            var map = EndpointHost.ServiceManager.ServiceController.RestPathMap;
            var paths = new List<string>();
            foreach (var key in map.Keys)
            {
                paths.AddRange(
                    map[key].Where(x => x.RequestType == operationType)
                        .Select(t => this.resourcePathCleanerRegex.Match(t.Path).Value));
            }

            if (paths.Count == 0)
            {
                return;
            }

            var basePaths =
                paths.Select(t => string.IsNullOrEmpty(t) ? null : t.Split('/'))
                    .Where(t => t != null && t.Length > 1)
                    .Select(t => t[1]);

            foreach (var bp in basePaths)
            {
                if (string.IsNullOrEmpty(bp))
                {
                    return;
                }

                if (ResourcePathFilterRegex != null && !ResourcePathFilterRegex.IsMatch(bp))
                {
                    continue;
                }

                if (apis.All(a => a.Path != string.Concat(ResourcePath, "/" + bp)))
                {
                    apis.Add(
                        new RestService
                        {
                            Path = string.Concat(ResourcePath, "/" + bp),
                            Description = operationType.GetDescription()
                        });
                }
            }
        }
    }
}