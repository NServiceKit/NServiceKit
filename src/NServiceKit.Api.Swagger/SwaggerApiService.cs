using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using NServiceKit.Text;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.Api.Swagger
{
    /// <summary>A resource request.</summary>
    [DataContract]
    public class ResourceRequest
    {
        /// <summary>Gets or sets the API key.</summary>
        ///
        /// <value>The API key.</value>
        [DataMember(Name = "apiKey")]
        public string ApiKey { get; set; }

        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    /// <summary>A resource response.</summary>
    [DataContract]
    public class ResourceResponse
    {
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

        /// <summary>Gets or sets the full pathname of the resource file.</summary>
        ///
        /// <value>The full pathname of the resource file.</value>
        [DataMember(Name = "resourcePath")]
        public string ResourcePath { get; set; }

        /// <summary>Gets or sets the apis.</summary>
        ///
        /// <value>The apis.</value>
        [DataMember(Name = "apis")]
        public List<MethodDescription> Apis { get; set; }

        /// <summary>Gets or sets the models.</summary>
        ///
        /// <value>The models.</value>
        [DataMember(Name = "models")]
        public Dictionary<string, SwaggerModel> Models { get; set; }
    }

    /// <summary>A data Model for the swagger.</summary>
    [DataContract]
    public class SwaggerModel
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>Gets or sets the properties.</summary>
        ///
        /// <value>The properties.</value>
        [DataMember(Name = "properties")]
        public Dictionary<string, ModelProperty> Properties { get; set; }
    }

    /// <summary>Description of the method.</summary>
    [DataContract]
    public class MethodDescription
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

        /// <summary>Gets or sets the operations.</summary>
        ///
        /// <value>The operations.</value>
        [DataMember(Name = "operations")]
        public List<MethodOperation> Operations { get; set; }
    }

    /// <summary>A method operation.</summary>
    [DataContract]
    public class MethodOperation
    {
        /// <summary>Gets or sets the HTTP method.</summary>
        ///
        /// <value>The HTTP method.</value>
        [DataMember(Name = "httpMethod")]
        public string HttpMethod { get; set; }

        /// <summary>Gets or sets the nickname.</summary>
        ///
        /// <value>The nickname.</value>
        [DataMember(Name = "nickname")]
        public string Nickname { get; set; }

        /// <summary>Gets or sets the summary.</summary>
        ///
        /// <value>The summary.</value>
        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        /// <summary>Gets or sets the notes.</summary>
        ///
        /// <value>The notes.</value>
        [DataMember(Name = "notes")]
        public string Notes { get; set; }

        /// <summary>Gets or sets options for controlling the operation.</summary>
        ///
        /// <value>The parameters.</value>
        [DataMember(Name = "parameters")]
        public List<MethodOperationParameter> Parameters { get; set; }

        /// <summary>Gets or sets the response class.</summary>
        ///
        /// <value>The response class.</value>
        [DataMember(Name = "responseClass")]
        public string ResponseClass { get; set; }

        /// <summary>Gets or sets the error responses.</summary>
        ///
        /// <value>The error responses.</value>
        [DataMember(Name = "errorResponses")]
        public List<ErrorResponseStatus> ErrorResponses { get; set; }
    }

    /// <summary>An error response status.</summary>
    [DataContract]
    public class ErrorResponseStatus
    {
        /// <summary>Gets or sets the status code.</summary>
        ///
        /// <value>The status code.</value>
        [DataMember(Name = "code")]
        public int StatusCode { get; set; }

        /// <summary>Gets or sets the reason.</summary>
        ///
        /// <value>The reason.</value>
        [DataMember(Name = "reason")]
        public string Reason { get; set; }
    }

    /// <summary>A model property.</summary>
    [DataContract]
    public class ModelProperty
    {
        /// <summary>Gets or sets the description.</summary>
        ///
        /// <value>The description.</value>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>Gets or sets the type.</summary>
        ///
        /// <value>The type.</value>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>Gets or sets the items.</summary>
        ///
        /// <value>The items.</value>
        [DataMember(Name = "items")]
        public Dictionary<string, string> Items { get; set; }

        /// <summary>Gets or sets the allowable values.</summary>
        ///
        /// <value>The allowable values.</value>
        [DataMember(Name = "allowableValues")]
        public ParameterAllowableValues AllowableValues { get; set; }

        /// <summary>Gets or sets a value indicating whether the required.</summary>
        ///
        /// <value>true if required, false if not.</value>
        [DataMember(Name = "required")]
        public bool Required { get; set; }
    }

    /// <summary>A method operation parameter.</summary>
    [DataContract]
    public class MethodOperationParameter
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>Gets or sets the description.</summary>
        ///
        /// <value>The description.</value>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>Gets or sets the type of the parameter.</summary>
        ///
        /// <value>The type of the parameter.</value>
        [DataMember(Name = "paramType")]
        public string ParamType { get; set; }

        /// <summary>Gets or sets a value indicating whether we allow multiple.</summary>
        ///
        /// <value>true if allow multiple, false if not.</value>
        [DataMember(Name = "allowMultiple")]
        public bool AllowMultiple { get; set; }

        /// <summary>Gets or sets a value indicating whether the required.</summary>
        ///
        /// <value>true if required, false if not.</value>
        [DataMember(Name = "required")]
        public bool Required { get; set; }

        /// <summary>Gets or sets the type of the data.</summary>
        ///
        /// <value>The type of the data.</value>
        [DataMember(Name = "dataType")]
        public string DataType { get; set; }

        /// <summary>Gets or sets the allowable values.</summary>
        ///
        /// <value>The allowable values.</value>
        [DataMember(Name = "allowableValues")]
        public ParameterAllowableValues AllowableValues { get; set; }
    }

    /// <summary>A parameter allowable values.</summary>
    [DataContract]
    public class ParameterAllowableValues
    {
        /// <summary>Gets or sets the type of the value.</summary>
        ///
        /// <value>The type of the value.</value>
        [DataMember(Name = "valueType")]
        public string ValueType { get; set; }

        /// <summary>Gets or sets the values.</summary>
        ///
        /// <value>The values.</value>
        [DataMember(Name = "values")]
        public string[] Values { get; set; }

        /// <summary>Gets or sets the minimum.</summary>
        ///
        /// <value>The minimum value.</value>
        [DataMember(Name = "min")]
        public int? Min { get; set; }

        /// <summary>Gets or sets the maximum.</summary>
        ///
        /// <value>The maximum value.</value>
        [DataMember(Name = "max")]
        public int? Max { get; set; }
    }

    /// <summary>A swagger API service.</summary>
    [DefaultRequest(typeof(ResourceRequest))]
    public class SwaggerApiService : ServiceInterface.Service
    {
        internal static bool UseCamelCaseModelPropertyNames { get; set; }
        internal static bool UseLowercaseUnderscoreModelPropertyNames { get; set; }
        internal static bool DisableAutoDtoInBodyParam { get; set; }

        private readonly Regex nicknameCleanerRegex = new Regex(@"[\{\}\*\-_/]*", RegexOptions.Compiled);

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(ResourceRequest request)
        {
            var httpReq = RequestContext.Get<IHttpRequest>();
            var path = "/" + request.Name;
            var map = EndpointHost.ServiceManager.ServiceController.RestPathMap;
            var paths = new List<RestPath>();

            var basePath = EndpointHost.Config.WebHostUrl;
            if (basePath == null)
            {
                basePath = EndpointHost.Config.UseHttpsLinks
                    ? Common.StringExtensions.ToHttps(httpReq.GetParentPathUrl())
                    : httpReq.GetParentPathUrl();
            }

            if (basePath.EndsWith(SwaggerResourcesService.RESOURCE_PATH, StringComparison.OrdinalIgnoreCase))
            {
                basePath = basePath.Substring(0, basePath.LastIndexOf(SwaggerResourcesService.RESOURCE_PATH, StringComparison.OrdinalIgnoreCase));
            }
            var meta = EndpointHost.Metadata;
            foreach (var key in map.Keys)
            {
                paths.AddRange(map[key].Where(x => (x.Path == path || x.Path.StartsWith(path + "/") && meta.IsVisible(Request, Format.Json, x.RequestType.Name))));
            }

            var models = new Dictionary<string, SwaggerModel>();
            foreach (var restPath in paths)
            {
                ParseModel(models, restPath.RequestType);
            }

            return new ResourceResponse
            {
                ResourcePath = path,
                BasePath = basePath,
                Apis = new List<MethodDescription>(paths.Select(p => FormateMethodDescription(p, models)).ToArray().OrderBy(md => md.Path)),
                Models = models
            };
        }

        private static readonly Dictionary<Type, string> ClrTypesToSwaggerScalarTypes = new Dictionary<Type, string> {
            {typeof(byte), SwaggerType.Byte},
            {typeof(sbyte), SwaggerType.Byte},
            {typeof(bool), SwaggerType.Boolean},
            {typeof(short), SwaggerType.Int},
            {typeof(ushort), SwaggerType.Int},
            {typeof(int), SwaggerType.Int},
            {typeof(uint), SwaggerType.Int},
            {typeof(long), SwaggerType.Long},
            {typeof(ulong), SwaggerType.Long},
            {typeof(float), SwaggerType.Float},
            {typeof(double), SwaggerType.Double},
            {typeof(decimal), SwaggerType.Double},
            {typeof(string), SwaggerType.String},
            {typeof(DateTime), SwaggerType.Date}
        };

        private static bool IsSwaggerScalarType(Type type)
        {
            return ClrTypesToSwaggerScalarTypes.ContainsKey(type) || (Nullable.GetUnderlyingType(type) ?? type).IsEnum;
        }

        private static string GetSwaggerTypeName(Type type)
        {
            var lookupType = Nullable.GetUnderlyingType(type) ?? type;

            return ClrTypesToSwaggerScalarTypes.ContainsKey(lookupType)
                ? ClrTypesToSwaggerScalarTypes[lookupType]
                : lookupType.Name;
        }

        private static Type GetListElementType(Type type)
        {
            if (type.IsArray) return type.GetElementType();

            if (!type.IsGenericType) return null;
            var genericType = type.GetGenericTypeDefinition();
            if (genericType == typeof(List<>) || genericType == typeof(IList<>) || genericType == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];
            return null;
        }

        private static bool IsListType(Type type)
        {
            return GetListElementType(type) != null;
        }

        private static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static void ParseModel(IDictionary<string, SwaggerModel> models, Type modelType)
        {
            if (IsSwaggerScalarType(modelType)) return;

            var modelId = modelType.Name;
            if (models.ContainsKey(modelId)) return;

            var model = new SwaggerModel
            {
                Id = modelId,
                Properties = new Dictionary<string, ModelProperty>()
            };
            models[model.Id] = model;

            var hasDataContract = modelType.HasAttr<DataContractAttribute>();
            
            foreach (var prop in modelType.GetProperties())
            {
                DataMemberAttribute dataMemberAttribute = null;
                if (hasDataContract)
                {
                    dataMemberAttribute = prop.GetDataMember();
                    if (dataMemberAttribute == null)
                    {
                        continue;
                    }
                } 
                else if (prop.IsDefined(typeof(IgnoreDataMemberAttribute), true))
                {
                    continue;
                }

                var allApiDocAttributes = prop
                    .GetCustomAttributes(typeof(ApiMemberAttribute), true)
                    .OfType<ApiMemberAttribute>()
                    .Where(attr => prop.Name.Equals(attr.Name, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
                var apiDoc = allApiDocAttributes.FirstOrDefault(attr => attr.ParameterType == "body");

                if (allApiDocAttributes.Any() && apiDoc == null) continue;

                var propertyType = prop.PropertyType;
                
                var isRequired = dataMemberAttribute == null
                    ? !IsNullable(propertyType)
                    : dataMemberAttribute.IsRequired;

                var modelProp = new ModelProperty { Type = GetSwaggerTypeName(propertyType), Required = isRequired };

                if (IsListType(propertyType))
                {
                    modelProp.Type = SwaggerType.Array;
                    var listItemType = GetListElementType(propertyType);
                    modelProp.Items = new Dictionary<string, string> {
                        { IsSwaggerScalarType(listItemType) ? "type" : "$ref", GetSwaggerTypeName(listItemType) }
                    };
                    ParseModel(models, listItemType);
                }
                else if ((Nullable.GetUnderlyingType(propertyType) ?? propertyType).IsEnum)
                {
                    var enumType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                    if (enumType.IsNumericType())
                    {
                        var underlyingType = Enum.GetUnderlyingType(enumType);
                        modelProp.Type = GetSwaggerTypeName(underlyingType);
                        modelProp.AllowableValues = new ParameterAllowableValues
                        {
                            Values = GetNumericValues(enumType, underlyingType).ToArray(),
                            ValueType = "LIST"
                        };
                    }
                    else
                    {
                        modelProp.Type = SwaggerType.String;
                        modelProp.AllowableValues = new ParameterAllowableValues
                        {
                            Values = Enum.GetNames(enumType),
                            ValueType = "LIST"
                        };
                    }                 
                }
                else
                {
                    ParseModel(models, propertyType);
                }

                var descriptionAttr = prop.GetCustomAttributes(typeof(DescriptionAttribute), true).OfType<DescriptionAttribute>().FirstOrDefault();
                if (descriptionAttr != null)
                    modelProp.Description = descriptionAttr.Description;

                if (apiDoc != null)
                    modelProp.Description = apiDoc.Description;

                var allowableValues = prop.GetCustomAttributes(typeof(ApiAllowableValuesAttribute), true).OfType<ApiAllowableValuesAttribute>().FirstOrDefault();
                if (allowableValues != null)
                    modelProp.AllowableValues = GetAllowableValue(allowableValues);

                model.Properties[GetModelPropertyName(prop, dataMemberAttribute)] = modelProp;
            }
        }

        private static IEnumerable<string> GetNumericValues(Type propertyType, Type underlyingType)
        {
            var values = Enum.GetValues(propertyType);
            foreach (var value in values)
            {
                yield return string.Format("{0} ({1})", Convert.ChangeType(value, underlyingType), value);                
            }            
        }

        private static string GetModelPropertyName(PropertyInfo prop, DataMemberAttribute dataMemberAttribute)
        {
            var name = dataMemberAttribute == null ? prop.Name : (dataMemberAttribute.Name ?? prop.Name);

            return UseCamelCaseModelPropertyNames
                ? (UseLowercaseUnderscoreModelPropertyNames ? name.ToLowercaseUnderscore() : name.ToCamelCase())
                : name;
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

        private static List<ErrorResponseStatus> GetMethodResponseCodes(Type requestType)
        {
            return requestType
                .GetCustomAttributes(typeof(IApiResponseDescription), true)
                .OfType<IApiResponseDescription>()
                .Select(x => new ErrorResponseStatus
                {
                    StatusCode = (int)x.StatusCode,
                    Reason = x.Description
                }).ToList();
        }

        private MethodDescription FormateMethodDescription(RestPath restPath, Dictionary<string, SwaggerModel> models)
        {
            var verbs = new List<string>();
            var summary = restPath.Summary;
            var notes = restPath.Notes;

            if (restPath.AllowsAllVerbs)
            {
                verbs.AddRange(new[] { "GET", "POST", "PUT", "DELETE" });
            }
            else
                verbs.AddRange(restPath.AllowedVerbs.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));

            var nickName = nicknameCleanerRegex.Replace(restPath.Path, "");

            var md = new MethodDescription
            {
                Path = restPath.Path,
                Description = summary,
                Operations = verbs.Select(verb =>
                    new MethodOperation
                    {
                        HttpMethod = verb,
                        Nickname = verb.ToLowerInvariant() + nickName,
                        Summary = summary,
                        Notes = notes,
                        Parameters = ParseParameters(verb, restPath.RequestType, models),
                        ResponseClass = GetResponseClass(restPath, models),
                        ErrorResponses = GetMethodResponseCodes(restPath.RequestType)
                    }).ToList()
            };
            return md;
        }

        private static ParameterAllowableValues GetAllowableValue(ApiAllowableValuesAttribute attr)
        {
            if (attr != null)
            {
                return new ParameterAllowableValues()
                {
                    ValueType = attr.Type,
                    Values = attr.Values,
                    Max = attr.Max,
                    Min = attr.Min
                };
            }
            return null;
        }

        private static List<MethodOperationParameter> ParseParameters(string verb, Type operationType, IDictionary<string, SwaggerModel> models)
        {
            var hasDataContract = operationType.GetCustomAttributes(typeof(DataContractAttribute), inherit: true).Length > 0;

            var properties = operationType.GetProperties();
            var paramAttrs = new Dictionary<string, ApiMemberAttribute[]>();
            var allowableParams = new List<ApiAllowableValuesAttribute>();

            foreach (var property in properties)
            {
                var propertyName = property.Name;
                if (hasDataContract)
                {
                    var dataMemberAttr = property.GetCustomAttributes(typeof(DataMemberAttribute), inherit: true)
                        .FirstOrDefault() as DataMemberAttribute;
                    if (dataMemberAttr != null && dataMemberAttr.Name != null)
                    {
                        propertyName = dataMemberAttr.Name;
                    }
                }
                paramAttrs[propertyName] = (ApiMemberAttribute[])property.GetCustomAttributes(typeof(ApiMemberAttribute), true);
                allowableParams.AddRange(property.GetCustomAttributes(typeof(ApiAllowableValuesAttribute), true).Cast<ApiAllowableValuesAttribute>().ToArray());
            }

            var methodOperationParameters = new List<MethodOperationParameter>();
            foreach (var key in paramAttrs.Keys)
            {
                var value = paramAttrs[key];
                methodOperationParameters.AddRange(
                    from ApiMemberAttribute member in value
                    where member.Verb == null || string.Compare(member.Verb, verb, StringComparison.InvariantCultureIgnoreCase) == 0
                    select new MethodOperationParameter
                    {
                        DataType = member.DataType,
                        AllowMultiple = member.AllowMultiple,
                        Description = member.Description,
                        Name = member.Name ?? key,
                        ParamType = member.ParameterType,
                        Required = member.IsRequired,
                        AllowableValues = GetAllowableValue(allowableParams.FirstOrDefault(attr => attr.Name == member.Name))
                    });
            }

            if (!DisableAutoDtoInBodyParam)
            {
                if (!NServiceKit.Common.Web.HttpMethods.Get.Equals(verb, StringComparison.OrdinalIgnoreCase) && !methodOperationParameters.Any(p => p.ParamType.Equals("body", StringComparison.OrdinalIgnoreCase)))
                {
                    ParseModel(models, operationType);
                    methodOperationParameters.Add(new MethodOperationParameter()
                    {
                        DataType = GetSwaggerTypeName(operationType),
                        ParamType = "body"
                    });
                }
            }
            return methodOperationParameters;
        }

    }
}