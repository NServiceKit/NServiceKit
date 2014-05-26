using NServiceKit.Common;

namespace NServiceKit.Api.Swagger
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;

    using NServiceKit.Common.Extensions;
    using NServiceKit.Common.Web;
    using NServiceKit.ServiceHost;
    using NServiceKit.ServiceInterface;
    using NServiceKit.Text;
    using NServiceKit.WebHost.Endpoints;

    using NServiceKit.Api.Swagger.Models;

    /// <summary>
    /// The swagger 12 api service.
    /// </summary>
    [DefaultRequest(typeof(ResourceRequest))]
    public class SwaggerApiService : Service
    {
        private static readonly Dictionary<Type, string> ClrTypesToSwaggerScalarTypes =
            new Dictionary<Type, string>
                {
                    { typeof(byte), SwaggerType.Byte }, 
                    { typeof(sbyte), SwaggerType.Byte }, 
                    { typeof(bool), SwaggerType.Boolean }, 
                    { typeof(short), SwaggerType.Int }, 
                    { typeof(ushort), SwaggerType.Int }, 
                    { typeof(int), SwaggerType.Int }, 
                    { typeof(uint), SwaggerType.Int }, 
                    { typeof(long), SwaggerType.Long }, 
                    { typeof(ulong), SwaggerType.Long }, 
                    { typeof(float), SwaggerType.Float }, 
                    { typeof(double), SwaggerType.Double }, 
                    { typeof(decimal), SwaggerType.Double }, 
                    { typeof(string), SwaggerType.String }, 
                    { typeof(DateTime), SwaggerType.Date }
                };

        private readonly Regex nicknameCleanerRegex = new Regex(@"[\{\}\*\-_/]*", RegexOptions.Compiled);

        /// <summary>
        /// The api version for the current service.
        /// </summary>
        public static string ApiVersion { get; set; }

        internal static bool DisableAutoDtoInBodyParam { get; set; }

        internal static bool UseCamelCaseModelPropertyNames { get; set; }

        internal static bool UseLowercaseUnderscoreModelPropertyNames { get; set; }

        /// <summary>
        /// Retrieve the swagger resource data for this service endpoint.
        /// </summary>
        /// <param name="request">
        /// The swagger resource request.
        /// </param>
        /// <returns>
        /// An http result containing the json swagger model output for the requested service endpoint.
        /// </returns>
        public HttpResult Get(ResourceRequest request)
        {
            var httpReq = this.RequestContext.Get<IHttpRequest>();
            var path = "/" + request.Name;
            var map = EndpointHost.ServiceManager.ServiceController.RestPathMap;
            var paths = new List<RestPath>();

            var basePath = this.GetAppHost().Config.WebHostUrl;
            if (basePath == null)
            {
                basePath = EndpointHost.Config.UseHttpsLinks
                               ? httpReq.GetParentPathUrl().ToHttps()
                               : httpReq.GetParentPathUrl();
            }

            if (basePath.EndsWith(SwaggerResourcesService.ResourcePath, StringComparison.OrdinalIgnoreCase))
            {
                basePath = basePath.Substring(0, basePath.LastIndexOf(SwaggerResourcesService.ResourcePath, StringComparison.OrdinalIgnoreCase));
            }

            var meta = EndpointHost.Metadata;
            foreach (var key in map.Keys)
            {
                paths.AddRange(
                    map[key].Where(
                        x =>
                        ((x.Path == path
                         || x.Path.StartsWith(path + "/"))
                         && meta.IsVisible(this.Request, Format.Json, x.RequestType.Name))));
            }

            var models = new Dictionary<string, SwaggerModel>();
            foreach (var restPath in paths)
            {
                ParseModel(models, restPath.RequestType);
            }

            var response = new ResourceResponse
            {
                SwaggerVersion = SwaggerFeature.SwaggerVersion,
                ApiVersion = ApiVersion,
                ResourcePath = path,
                BasePath = basePath,
                Apis =
                    new List<MethodDescription>(
                    paths.Select(p => this.FormateMethodDescription(p, models))
                    .ToArray()
                    .OrderBy(md => md.Path)),
                Models = models
            };
            return new HttpResult(response, "text/json");
        }

        private static ParameterAllowableValues GetAllowableValue(ApiAllowableValuesAttribute attr)
        {
            if (attr != null)
            {
                return new ParameterAllowableValues
                {
                    ValueType = attr.Type,
                    Values = attr.Values,
                    Max = attr.Max,
                    Min = attr.Min
                };
            }

            return null;
        }

        private static IEnumerable<T> GetAttributesOfType<T>(PropertyInfo propertyInfo)
        {
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(T), true);

            return customAttributes.Cast<T>();
        }

        private static Type GetListElementType(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (!type.IsGenericType)
            {
                return null;
            }

            var genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(List<>) || genericType == typeof(IList<>)
                || genericType == typeof(IEnumerable<>))
            {
                return type.GetGenericArguments()[0];
            }

            return null;
        }

        private static List<ErrorResponseStatus> GetMethodResponseCodes(Type requestType)
        {
            var attributes = TypeDescriptor.GetAttributes(requestType).OfType<IApiResponseDescription>();
            return
                attributes.Select(
                    x => new ErrorResponseStatus { StatusCode = x.StatusCode, Reason = x.Description })
                    .ToList();
        }

        private static string GetModelPropertyName(PropertyInfo prop)
        {
            return UseCamelCaseModelPropertyNames
                       ? (UseLowercaseUnderscoreModelPropertyNames
                              ? prop.Name.ToLowercaseUnderscore()
                              : prop.Name.ToCamelCase())
                       : prop.Name;
        }

        private static string GetResponseClass(IRestPath restPath, IDictionary<string, SwaggerModel> models)
        {
            // Given: class MyDto : IReturn<X>. Determine the type X.
            foreach (var i in restPath.RequestType.GetInterfaces())
            {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReturn<>))
                {
                    var returnType = i.GetGenericArguments()[0];

                    // Handle IReturn<List<SomeClass>> or IReturn<SomeClass[]>
                    if (IsListType(returnType))
                    {
                        var listItemType = GetListElementType(returnType);
                        ParseModel(models, listItemType);
                        return string.Format("List[{0}]", GetSwaggerTypeName(listItemType));
                    }

                    ParseModel(models, returnType);
                    return GetSwaggerTypeName(i.GetGenericArguments()[0]);
                }
            }

            return null;
        }

        private static string GetSwaggerTypeName(Type type)
        {
            var lookupType = Nullable.GetUnderlyingType(type) ?? type;

            return ClrTypesToSwaggerScalarTypes.ContainsKey(lookupType)
                       ? ClrTypesToSwaggerScalarTypes[lookupType]
                       : lookupType.Name;
        }

        private static bool IsListType(Type type)
        {
            return GetListElementType(type) != null;
        }

        private static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static bool IsSwaggerScalarType(Type type)
        {
            return ClrTypesToSwaggerScalarTypes.ContainsKey(type) || type.IsEnum;
        }

        private static void ParseModel(IDictionary<string, SwaggerModel> models, Type modelType)
        {
            if (IsSwaggerScalarType(modelType))
            {
                return;
            }

            var modelId = modelType.Name;
            if (models.ContainsKey(modelId))
            {
                return;
            }

            var model = new SwaggerModel
            {
                Id = modelId,
                Properties = new Dictionary<string, ModelProperty>()
            };
            models[model.Id] = model;

            foreach (var prop in modelType.GetProperties())
            {
                var apiMemberAttributes = GetAttributesOfType<ApiMemberAttribute>(prop);
                var allApiDocAttributes =
                    apiMemberAttributes.Where(
                        attr => prop.Name.Equals(attr.Name, StringComparison.InvariantCultureIgnoreCase))
                        .ToList();

                var apiDoc = allApiDocAttributes.FirstOrDefault(attr => attr.ParameterType == "body");

                if (allApiDocAttributes.Any() && apiDoc == null)
                {
                    continue;
                }

                var propertyType = prop.PropertyType;
                var modelProp = new ModelProperty
                {
                    Type = GetSwaggerTypeName(propertyType),
                    Required = !IsNullable(propertyType)
                };

                if (IsListType(propertyType))
                {
                    modelProp.Type = SwaggerType.Array;
                    var listItemType = GetListElementType(propertyType);
                    modelProp.Items = new Dictionary<string, string>
                                          {
                                              {
                                                  IsSwaggerScalarType(listItemType)
                                                      ? "type"
                                                      : "$ref", 
                                                  GetSwaggerTypeName(listItemType)
                                              }
                                          };
                    ParseModel(models, listItemType);
                }
                else if (propertyType.IsEnum)
                {
                    modelProp.Type = SwaggerType.String;
                    modelProp.AllowableValues = new ParameterAllowableValues
                    {
                        Values = Enum.GetNames(propertyType),
                        ValueType = "LIST"
                    };
                }
                else
                {
                    ParseModel(models, propertyType);
                }

                var descriptionAttr = prop.FirstAttribute<DescriptionAttribute>();
                if (descriptionAttr != null)
                {
                    modelProp.Description = descriptionAttr.Description;
                }

                if (apiDoc != null)
                {
                    modelProp.Description = apiDoc.Description;
                }

                var allowableValues = prop.FirstAttribute<ApiAllowableValuesAttribute>();
                if (allowableValues != null)
                {
                    modelProp.AllowableValues = GetAllowableValue(allowableValues);
                }

                model.Properties[GetModelPropertyName(prop)] = modelProp;
            }
        }

        private static List<MethodOperationParameter> ParseParameters(
            string verb,
            Type operationType,
            IDictionary<string, SwaggerModel> models)
        {
            var hasDataContract = operationType.HasAttribute<DataContractAttribute>();

            var properties = operationType.GetProperties();
            var paramAttrs = new Dictionary<string, ApiMemberAttribute[]>();
            var allowableParams = new List<ApiAllowableValuesAttribute>();

            foreach (var property in properties)
            {
                var propertyName = property.Name;
                if (hasDataContract)
                {
                    var dataMemberAttr = property.FirstAttribute<DataMemberAttribute>();
                    if (dataMemberAttr != null && dataMemberAttr.Name != null)
                    {
                        propertyName = dataMemberAttr.Name;
                    }
                }

                paramAttrs[propertyName] = GetAttributesOfType<ApiMemberAttribute>(property).ToArray();
                allowableParams.AddRange(GetAttributesOfType<ApiAllowableValuesAttribute>(property));
            }

            var methodOperationParameters = new List<MethodOperationParameter>();
            foreach (var key in paramAttrs.Keys)
            {
                var value = paramAttrs[key];
                methodOperationParameters.AddRange(
                    from ApiMemberAttribute member in value
                    where
                        member.Verb == null
                        || string.Compare(member.Verb, verb, StringComparison.InvariantCultureIgnoreCase) == 0
                    select
                        new MethodOperationParameter
                        {
                            DataType = member.DataType,
                            AllowMultiple = member.AllowMultiple,
                            Description = member.Description,
                            Name = member.Name ?? key,
                            ParamType = member.ParameterType.ToLowerInvariant(),
                            Required = member.IsRequired,
                            AllowableValues =
                                GetAllowableValue(
                                    allowableParams.FirstOrDefault(
                                        attr => attr.Name == member.Name))
                        });
            }

            if (!DisableAutoDtoInBodyParam)
            {
                if (!HttpMethods.Get.Equals(verb, StringComparison.OrdinalIgnoreCase)
                    && !methodOperationParameters.Any(
                        p => p.ParamType.Equals("body", StringComparison.OrdinalIgnoreCase)))
                {
                    ParseModel(models, operationType);
                    methodOperationParameters.Add(
                        new MethodOperationParameter
                        {
                            DataType = GetSwaggerTypeName(operationType),
                            ParamType = "body"
                        });
                }
            }

            return methodOperationParameters;
        }

        private MethodDescription FormateMethodDescription(
            RestPath restPath,
            Dictionary<string, SwaggerModel> models)
        {
            var verbs = new List<string>();
            var summary = restPath.Summary;
            var notes = restPath.Notes;

            if (restPath.AllowsAllVerbs)
            {
                verbs.AddRange(new[] { "GET", "POST", "PUT", "DELETE" });
            }
            else
            {
                verbs.AddRange(
                    restPath.AllowedVerbs.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }

            var md = new MethodDescription
            {
                Path = restPath.Path,
                Description = summary,
                Operations = verbs.Select(
                    verb =>
                        new MethodOperation
                        {
                            HttpMethod = verb,
                            Nickname = this.GenerateNickname(verb, restPath),
                            Summary = summary,
                            Notes = notes,
                            Parameters = ParseParameters(verb, restPath.RequestType, models),
                            ResponseClass = GetResponseClass(restPath, models),
                            ErrorResponses = GetMethodResponseCodes(restPath.RequestType)
                        }).ToList()
            };
            return md;
        }

        private string GenerateNickname(string verb, RestPath restPath)
        {
            string nickName = verb.ToLowerInvariant() + restPath.Path;

            var routeAttributes = restPath.RequestType.GetCustomAttributes(typeof(NamedRouteAttribute), true);

            var attr = routeAttributes.FirstOrDefault(
                    attrib => ((NamedRouteAttribute)attrib).Verbs == restPath.AllowedVerbs);

            if (attr != null)
            {
                var routeName = ((NamedRouteAttribute)attr).Name;

                if (!string.IsNullOrEmpty(routeName))
                {
                    nickName = routeName;
                }
            }

            return this.nicknameCleanerRegex.Replace(nickName, string.Empty);
        }
    }
}