using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NServiceKit.Common;
using NServiceKit.Text;
using NServiceKit.WebHost.Endpoints;
using NServiceKit.WebHost.Endpoints.Extensions;

namespace NServiceKit.ServiceHost
{
    /// <summary>A service metadata.</summary>
    public class ServiceMetadata
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ServiceMetadata class.</summary>
        public ServiceMetadata()
        {
            this.RequestTypes = new HashSet<Type>();
            this.ServiceTypes = new HashSet<Type>();
            this.ResponseTypes = new HashSet<Type>();
            this.OperationsMap = new Dictionary<Type, Operation>();
            this.OperationsResponseMap = new Dictionary<Type, Operation>();
            this.OperationNamesMap = new Dictionary<string, Operation>();
            this.Routes = new ServiceRoutes();
        }

        /// <summary>Gets the operations map.</summary>
        ///
        /// <value>The operations map.</value>
        public Dictionary<Type, Operation> OperationsMap { get; protected set; }

        /// <summary>Gets the operations response map.</summary>
        ///
        /// <value>The operations response map.</value>
        public Dictionary<Type, Operation> OperationsResponseMap { get; protected set; }

        /// <summary>Gets the operation names map.</summary>
        ///
        /// <value>The operation names map.</value>
        public Dictionary<string, Operation> OperationNamesMap { get; protected set; }

        /// <summary>Gets a list of types of the requests.</summary>
        ///
        /// <value>A list of types of the requests.</value>
        public HashSet<Type> RequestTypes { get; protected set; }

        /// <summary>Gets a list of types of the services.</summary>
        ///
        /// <value>A list of types of the services.</value>
        public HashSet<Type> ServiceTypes { get; protected set; }

        /// <summary>Gets a list of types of the responses.</summary>
        ///
        /// <value>A list of types of the responses.</value>
        public HashSet<Type> ResponseTypes { get; protected set; }

        /// <summary>Gets or sets the routes.</summary>
        ///
        /// <value>The routes.</value>
        public ServiceRoutes Routes { get; set; }

        /// <summary>Gets the operations.</summary>
        ///
        /// <value>The operations.</value>
        public IEnumerable<Operation> Operations
        {
            get { return OperationsMap.Values; }
        }

        /// <summary>Adds serviceType.</summary>
        ///
        /// <param name="serviceType"> Type of the service.</param>
        /// <param name="requestType"> Type of the request.</param>
        /// <param name="responseType">Type of the response.</param>
        public void Add(Type serviceType, Type requestType, Type responseType)
        {
            this.ServiceTypes.Add(serviceType);
            this.RequestTypes.Add(requestType);

            var restrictTo = requestType.GetCustomAttributes(true)
                    .OfType<RestrictAttribute>().FirstOrDefault()
                ?? serviceType.GetCustomAttributes(true)
                    .OfType<RestrictAttribute>().FirstOrDefault();

            var operation = new Operation {
                ServiceType = serviceType,
                RequestType = requestType,
                ResponseType = responseType,
                RestrictTo = restrictTo,
                Actions = GetImplementedActions(serviceType, requestType),
                Routes = new List<RestPath>(),
            };

            this.OperationsMap[requestType] = operation;
            this.OperationNamesMap[requestType.Name.ToLower()] = operation;

            if (responseType != null)
            {
                this.ResponseTypes.Add(responseType);
                this.OperationsResponseMap[responseType] = operation;
            }
        }

        /// <summary>After initialise.</summary>
        public void AfterInit()
        {
            foreach (var restPath in Routes.RestPaths)
            {
                Operation operation;
                if (!OperationsMap.TryGetValue(restPath.RequestType, out operation))
                    continue;

                operation.Routes.Add(restPath);
            }
        }

        /// <summary>Gets operation dtos.</summary>
        ///
        /// <returns>The operation dtos.</returns>
        public List<OperationDto> GetOperationDtos()
        {
            return OperationsMap.Values
                .SafeConvertAll(x => x.ToOperationDto())
                .OrderBy(x => x.Name)
                .ToList();
        }

        /// <summary>Gets an operation.</summary>
        ///
        /// <param name="operationType">Type of the operation.</param>
        ///
        /// <returns>The operation.</returns>
        public Operation GetOperation(Type operationType)
        {
            Operation op;
            OperationsMap.TryGetValue(operationType, out op);
            return op;
        }

        /// <summary>Gets implemented actions.</summary>
        ///
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="requestType">Type of the request.</param>
        ///
        /// <returns>The implemented actions.</returns>
        public List<string> GetImplementedActions(Type serviceType, Type requestType)
        {
            if (typeof(IService).IsAssignableFrom(serviceType))
            {
                return serviceType.GetActions()
                    .Where(x => x.GetParameters()[0].ParameterType == requestType)
                    .Select(x => x.Name.ToUpper())
                    .ToList();
            }

            var oldApiActions = serviceType
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(x => ToNewApiAction(x.Name))
                .Where(x => x != null)
                .ToList();
            return oldApiActions;
        }

        /// <summary>Converts an oldApiAction to a new API action.</summary>
        ///
        /// <param name="oldApiAction">The old API action.</param>
        ///
        /// <returns>oldApiAction as a string.</returns>
        public static string ToNewApiAction(string oldApiAction)
        {
            switch (oldApiAction)
            {
                case "Get":
                case "OnGet":
                    return "GET";
                case "Put":
                case "OnPut":
                    return "PUT";
                case "Post":
                case "OnPost":
                    return "POST";
                case "Delete":
                case "OnDelete":
                    return "DELETE";
                case "Patch":
                case "OnPatch":
                    return "PATCH";
                case "Execute":
                case "Run":
                    return "ANY";
            }
            return null;
        }

        /// <summary>Gets operation type.</summary>
        ///
        /// <param name="operationTypeName">Name of the operation type.</param>
        ///
        /// <returns>The operation type.</returns>
        public Type GetOperationType(string operationTypeName)
        {
            Operation operation;
            OperationNamesMap.TryGetValue(operationTypeName.ToLower(), out operation);
            return operation != null ? operation.RequestType : null;
        }

        /// <summary>Gets service type by request.</summary>
        ///
        /// <param name="requestType">Type of the request.</param>
        ///
        /// <returns>The service type by request.</returns>
        public Type GetServiceTypeByRequest(Type requestType)
        {
            Operation operation;
            OperationsMap.TryGetValue(requestType, out operation);
            return operation != null ? operation.ServiceType : null;
        }

        /// <summary>Gets service type by response.</summary>
        ///
        /// <param name="responseType">Type of the response.</param>
        ///
        /// <returns>The service type by response.</returns>
        public Type GetServiceTypeByResponse(Type responseType)
        {
            Operation operation;
            OperationsResponseMap.TryGetValue(responseType, out operation);
            return operation != null ? operation.ServiceType : null;
        }

        /// <summary>Gets response type by request.</summary>
        ///
        /// <param name="requestType">Type of the request.</param>
        ///
        /// <returns>The response type by request.</returns>
        public Type GetResponseTypeByRequest(Type requestType)
        {
            Operation operation;
            OperationsMap.TryGetValue(requestType, out operation);
            return operation != null ? operation.ResponseType : null;
        }

        /// <summary>Gets all types.</summary>
        ///
        /// <returns>all types.</returns>
        public List<Type> GetAllTypes()
        {
            var allTypes = new List<Type>(RequestTypes);
            foreach (var responseType in ResponseTypes)
            {
                allTypes.AddIfNotExists(responseType);
            }
            return allTypes;
        }

        /// <summary>Gets all operation names.</summary>
        ///
        /// <returns>all operation names.</returns>
        public List<string> GetAllOperationNames()
        {
            return Operations.Select(x => x.RequestType.Name).OrderBy(operation => operation).ToList();
        }

        /// <summary>Gets operation names for metadata.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        ///
        /// <returns>The operation names for metadata.</returns>
        public List<string> GetOperationNamesForMetadata(IHttpRequest httpReq)
        {
            return GetAllOperationNames();
        }

        /// <summary>Gets operation names for metadata.</summary>
        ///
        /// <param name="httpReq">The HTTP request.</param>
        /// <param name="format"> Describes the format to use.</param>
        ///
        /// <returns>The operation names for metadata.</returns>
        public List<string> GetOperationNamesForMetadata(IHttpRequest httpReq, Format format)
        {
            return GetAllOperationNames();
        }

        /// <summary>Query if 'httpReq' is visible.</summary>
        ///
        /// <param name="httpReq">  The HTTP request.</param>
        /// <param name="operation">The operation.</param>
        ///
        /// <returns>true if visible, false if not.</returns>
        public bool IsVisible(IHttpRequest httpReq, Operation operation)
        {
            if (EndpointHost.Config != null && !EndpointHost.Config.EnableAccessRestrictions)
                return true;

            if (operation.RestrictTo == null) return true;

            //Less fine-grained on /metadata pages. Only check Network and Format
            var reqAttrs = httpReq.GetAttributes();
            var showToNetwork = CanShowToNetwork(operation, reqAttrs);
            return showToNetwork;
        }

        /// <summary>Query if 'httpReq' is visible.</summary>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="format">       Describes the format to use.</param>
        /// <param name="operationName">Name of the operation.</param>
        ///
        /// <returns>true if visible, false if not.</returns>
        public bool IsVisible(IHttpRequest httpReq, Format format, string operationName)
        {
            if (EndpointHost.Config != null && !EndpointHost.Config.EnableAccessRestrictions)
                return true;

            Operation operation;
            OperationNamesMap.TryGetValue(operationName.ToLowerInvariant(), out operation);
            if (operation == null) return false;

            var canCall = HasImplementation(operation, format);
            if (!canCall) return false;

            var isVisible = IsVisible(httpReq, operation);
            if (!isVisible) return false;

            if (operation.RestrictTo == null) return true;
            var allowsFormat = operation.RestrictTo.CanShowTo((EndpointAttributes)(long)format);
            return allowsFormat;
        }

        /// <summary>Determine if we can access.</summary>
        ///
        /// <param name="httpReq">      The HTTP request.</param>
        /// <param name="format">       Describes the format to use.</param>
        /// <param name="operationName">Name of the operation.</param>
        ///
        /// <returns>true if we can access, false if not.</returns>
        public bool CanAccess(IHttpRequest httpReq, Format format, string operationName)
        {
            var reqAttrs = httpReq.GetAttributes();
            return CanAccess(reqAttrs, format, operationName);
        }

        /// <summary>Determine if we can access.</summary>
        ///
        /// <param name="reqAttrs">     The request attributes.</param>
        /// <param name="format">       Describes the format to use.</param>
        /// <param name="operationName">Name of the operation.</param>
        ///
        /// <returns>true if we can access, false if not.</returns>
        public bool CanAccess(EndpointAttributes reqAttrs, Format format, string operationName)
        {
            if (EndpointHost.Config != null && !EndpointHost.Config.EnableAccessRestrictions)
                return true;

            Operation operation;
            OperationNamesMap.TryGetValue(operationName.ToLower(), out operation);
            if (operation == null) return false;

            var canCall = HasImplementation(operation, format);
            if (!canCall) return false;

            if (operation.RestrictTo == null) return true;

            var allow = operation.RestrictTo.HasAccessTo(reqAttrs);
            if (!allow) return false;

            var allowsFormat = operation.RestrictTo.HasAccessTo((EndpointAttributes)(long)format);
            return allowsFormat;
        }

        /// <summary>Determine if we can access.</summary>
        ///
        /// <param name="format">       Describes the format to use.</param>
        /// <param name="operationName">Name of the operation.</param>
        ///
        /// <returns>true if we can access, false if not.</returns>
        public bool CanAccess(Format format, string operationName)
        {
            if (EndpointHost.Config != null && !EndpointHost.Config.EnableAccessRestrictions)
                return true;

            Operation operation;
            OperationNamesMap.TryGetValue(operationName.ToLower(), out operation);
            if (operation == null) return false;

            var canCall = HasImplementation(operation, format);
            if (!canCall) return false;

            if (operation.RestrictTo == null) return true;

            var allowsFormat = operation.RestrictTo.HasAccessTo((EndpointAttributes)(long)format);
            return allowsFormat;
        }

        /// <summary>Query if 'operation' has implementation.</summary>
        ///
        /// <param name="operation">The operation.</param>
        /// <param name="format">   Describes the format to use.</param>
        ///
        /// <returns>true if implementation, false if not.</returns>
        public bool HasImplementation(Operation operation, Format format)
        {
            if (format == Format.Soap11 || format == Format.Soap12)
            {
                if (operation.Actions == null) return false;

                return operation.Actions.Contains("POST")
                    || operation.Actions.Contains(ActionContext.AnyAction);
            }
            return true;
        }

        private static bool CanShowToNetwork(Operation operation, EndpointAttributes reqAttrs)
        {
            if (reqAttrs.IsLocalhost())
                return operation.RestrictTo.CanShowTo(EndpointAttributes.Localhost)
                       || operation.RestrictTo.CanShowTo(EndpointAttributes.LocalSubnet);

            return operation.RestrictTo.CanShowTo(
                reqAttrs.IsLocalSubnet()
                    ? EndpointAttributes.LocalSubnet
                    : EndpointAttributes.External);
        }

    }

    /// <summary>An operation.</summary>
    public class Operation
    {
        /// <summary>Gets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get { return RequestType.Name; } }

        /// <summary>Gets or sets the type of the request.</summary>
        ///
        /// <value>The type of the request.</value>
        public Type RequestType { get; set; }

        /// <summary>Gets or sets the type of the service.</summary>
        ///
        /// <value>The type of the service.</value>
        public Type ServiceType { get; set; }

        /// <summary>Gets or sets the type of the response.</summary>
        ///
        /// <value>The type of the response.</value>
        public Type ResponseType { get; set; }

        /// <summary>Gets or sets the restrict to.</summary>
        ///
        /// <value>The restrict to.</value>
        public RestrictAttribute RestrictTo { get; set; }

        /// <summary>Gets or sets the actions.</summary>
        ///
        /// <value>The actions.</value>
        public List<string> Actions { get; set; }

        /// <summary>Gets or sets the routes.</summary>
        ///
        /// <value>The routes.</value>
        public List<RestPath> Routes { get; set; }

        /// <summary>Gets a value indicating whether this object is one way.</summary>
        ///
        /// <value>true if this object is one way, false if not.</value>
        public bool IsOneWay { get { return ResponseType == null; } }
    }

    /// <summary>An operation dto.</summary>
    public class OperationDto
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the name of the response.</summary>
        ///
        /// <value>The name of the response.</value>
        public string ResponseName { get; set; }

        /// <summary>Gets or sets the name of the service.</summary>
        ///
        /// <value>The name of the service.</value>
        public string ServiceName { get; set; }

        /// <summary>Gets or sets the restrict to.</summary>
        ///
        /// <value>The restrict to.</value>
        public List<string> RestrictTo { get; set; }

        /// <summary>Gets or sets the visible to.</summary>
        ///
        /// <value>The visible to.</value>
        public List<string> VisibleTo { get; set; }

        /// <summary>Gets or sets the actions.</summary>
        ///
        /// <value>The actions.</value>
        public List<string> Actions { get; set; }

        /// <summary>Gets or sets the routes.</summary>
        ///
        /// <value>The routes.</value>
        public Dictionary<string, string> Routes { get; set; }
    }

    /// <summary>An XSD metadata.</summary>
    public class XsdMetadata
    {
        /// <summary>Gets or sets the metadata.</summary>
        ///
        /// <value>The metadata.</value>
        public ServiceMetadata Metadata { get; set; }

        /// <summary>Gets or sets a value indicating whether the flash.</summary>
        ///
        /// <value>true if flash, false if not.</value>
        public bool Flash { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.XsdMetadata class.</summary>
        ///
        /// <param name="metadata">The metadata.</param>
        /// <param name="flash">   true to flash.</param>
        public XsdMetadata(ServiceMetadata metadata, bool flash = false)
        {
            Metadata = metadata;
            Flash = flash;
        }

        /// <summary>Gets all types.</summary>
        ///
        /// <returns>all types.</returns>
        public List<Type> GetAllTypes()
        {
            var allTypes = new List<Type>(Metadata.RequestTypes);
            allTypes.AddRange(Metadata.ResponseTypes);
            return allTypes;
        }

        /// <summary>Gets reply operation names.</summary>
        ///
        /// <param name="format">Describes the format to use.</param>
        ///
        /// <returns>The reply operation names.</returns>
        public List<string> GetReplyOperationNames(Format format)
        {
            return Metadata.OperationsMap.Values
                .Where(x => EndpointHost.Config != null
                    && EndpointHost.Config.MetadataPagesConfig.CanAccess(format, x.Name))
                .Where(x => !x.IsOneWay)
                .Select(x => x.RequestType.Name)
                .ToList();
        }

        /// <summary>Gets one way operation names.</summary>
        ///
        /// <param name="format">Describes the format to use.</param>
        ///
        /// <returns>The one way operation names.</returns>
        public List<string> GetOneWayOperationNames(Format format)
        {
            return Metadata.OperationsMap.Values
                .Where(x => EndpointHost.Config != null
                    && EndpointHost.Config.MetadataPagesConfig.CanAccess(format, x.Name))
                .Where(x => x.IsOneWay)
                .Select(x => x.RequestType.Name)
                .ToList();
        }

        /// <summary>
        /// Gets the name of the base most type in the heirachy tree with the same.
        /// 
        /// We get an exception when trying to create a schema with multiple types of the same name
        /// like when inheriting from a DataContract with the same name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Type GetBaseTypeWithTheSameName(Type type)
        {
            var typesWithSameName = new Stack<Type>();
            var baseType = type;
            do
            {
                if (baseType.Name == type.Name)
                    typesWithSameName.Push(baseType);
            }
            while ((baseType = baseType.BaseType) != null);

            return typesWithSameName.Pop();
        }
    }

    /// <summary>A service metadata extensions.</summary>
    public static class ServiceMetadataExtensions
    {
        /// <summary>An Operation extension method that converts an operation to an operation dto.</summary>
        ///
        /// <param name="operation">The operation to act on.</param>
        ///
        /// <returns>operation as an OperationDto.</returns>
        public static OperationDto ToOperationDto(this Operation operation)
        {
            var to = new OperationDto {
                Name = operation.Name,
                ResponseName = operation.IsOneWay ? null : operation.ResponseType.Name,
                ServiceName = operation.ServiceType.Name,
                Actions = operation.Actions,
                Routes = operation.Routes.ToDictionary(x => x.Path.PairWith(x.AllowedVerbs)),
            };
            
            if (operation.RestrictTo != null)
            {
                to.RestrictTo = operation.RestrictTo.AccessibleToAny.ToList().ConvertAll(x => x.ToString());
                to.VisibleTo = operation.RestrictTo.VisibleToAny.ToList().ConvertAll(x => x.ToString());
            }

            return to;
        }

        /// <summary>A Type extension method that gets a description.</summary>
        ///
        /// <param name="operationType">The operationType to act on.</param>
        ///
        /// <returns>The description.</returns>
        public static string GetDescription(this Type operationType)
        {
            var apiAttr = operationType.GetCustomAttributes(typeof(ApiAttribute), true).OfType<ApiAttribute>().FirstOrDefault();
            return apiAttr != null ? apiAttr.Description : "";
        }

        /// <summary>A Type extension method that gets API members.</summary>
        ///
        /// <param name="operationType">The operationType to act on.</param>
        ///
        /// <returns>The API members.</returns>
        public static List<ApiMemberAttribute> GetApiMembers(this Type operationType)
        {
            var members = operationType.GetMembers(BindingFlags.Instance | BindingFlags.Public);
            List<ApiMemberAttribute> attrs = new List<ApiMemberAttribute>();
            foreach (var member in members)
            {
                var memattr = member.GetCustomAttributes(typeof(ApiMemberAttribute), true)
                    .OfType<ApiMemberAttribute>()
                    .Select(x => { x.Name = x.Name ?? member.Name; return x; });

                attrs.AddRange(memattr);
            }

            return attrs;
        }
    }


}
