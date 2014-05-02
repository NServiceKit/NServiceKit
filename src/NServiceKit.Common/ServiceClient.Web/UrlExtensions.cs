using NServiceKit.Common.Web;
using NServiceKit.Net30.Collections.Concurrent;
using NServiceKit.ServiceHost;
using NServiceKit.Text;
using System;
#if NETFX_CORE
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>
    /// Donated by Ivan Korneliuk from his post:
    /// http://korneliuk.blogspot.com/2012/08/NServiceKit-reusing-dtos.html
    /// 
    /// Modified to only allow using routes matching the supplied HTTP Verb
    /// </summary>
    public static class UrlExtensions
    {
        private static readonly ConcurrentDictionary<Type, List<RestRoute>> routesCache =
            new ConcurrentDictionary<Type, List<RestRoute>>();

        /// <summary>An IReturn extension method that converts this object to an URL.</summary>
        ///
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        ///
        /// <param name="request">                        The request to act on.</param>
        /// <param name="httpMethod">                     The HTTP method.</param>
        /// <param name="formatFallbackToPredefinedRoute">The format fallback to predefined route.</param>
        ///
        /// <returns>The given data converted to a string.</returns>
        public static string ToUrl(this IReturn request, string httpMethod, string formatFallbackToPredefinedRoute = null)
        {
            httpMethod = httpMethod.ToUpper();

            var requestType = request.GetType();
            var requestRoutes = routesCache.GetOrAdd(requestType, GetRoutesForType);
            if (requestRoutes.Count == 0)
            {
                if (formatFallbackToPredefinedRoute == null)
                    throw new InvalidOperationException("There are no rest routes mapped for '{0}' type. ".Fmt(requestType)
                        + "(Note: The automatic route selection only works with [Route] attributes on the request DTO and"
                        + "not with routes registered in the IAppHost!)");

                var predefinedRoute = "/{0}/syncreply/{1}".Fmt(formatFallbackToPredefinedRoute, requestType.Name);
                if (httpMethod == "GET" || httpMethod == "DELETE" || httpMethod == "OPTIONS" || httpMethod == "HEAD")
                {
                    var queryProperties = RestRoute.GetQueryProperties(request.GetType());
                    predefinedRoute += "?" + RestRoute.GetQueryString(request, queryProperties);
                }

                return predefinedRoute;
            }

            var routesApplied = requestRoutes.Select(route => route.Apply(request, httpMethod)).ToList();
            var matchingRoutes = routesApplied.Where(x => x.Matches).ToList();
            if (matchingRoutes.Count == 0)
            {
                var errors = string.Join(String.Empty, routesApplied.Select(x => "\r\n\t{0}:\t{1}".Fmt(x.Route.Path, x.FailReason)).ToArray());
                var errMsg = "None of the given rest routes matches '{0}' request:{1}"
                    .Fmt(requestType.Name, errors);

                throw new InvalidOperationException(errMsg);
            }

            RouteResolutionResult matchingRoute;
            if (matchingRoutes.Count > 1)
            {
                matchingRoute = FindMostSpecificRoute(matchingRoutes);
                if (matchingRoute == null)
                {
                    var errors = String.Join(String.Empty, matchingRoutes.Select(x => "\r\n\t" + x.Route.Path).ToArray());
                    var errMsg = "Ambiguous matching routes found for '{0}' request:{1}".Fmt(requestType.Name, errors);
                    throw new InvalidOperationException(errMsg);
                }
            }
            else
            {
                matchingRoute = matchingRoutes[0];
            }

            var url = matchingRoute.Uri;
            if (httpMethod == HttpMethods.Get || httpMethod == HttpMethods.Delete || httpMethod == HttpMethods.Head)
            {
                var queryParams = matchingRoute.Route.FormatQueryParameters(request);
                if (!String.IsNullOrEmpty(queryParams))
                {
                    url += "?" + queryParams;
                }
            }

            return url;
        }

        private static List<RestRoute> GetRoutesForType(Type requestType)
        {

#if NETFX_CORE
            var restRoutes = requestType.AttributesOfType<RouteAttribute>()
                .Select(attr => new RestRoute(requestType, attr.Path, attr.Verbs))
                .ToList();
#elif WINDOWS_PHONE || SILVERLIGHT
            var restRoutes = requestType.AttributesOfType<RouteAttribute>()
                .Select(attr => new RestRoute(requestType, attr.Path, attr.Verbs))
                .ToList();
#else
            var restRoutes = TypeDescriptor.GetAttributes(requestType)
                .OfType<RouteAttribute>()
                .Select(attr => new RestRoute(requestType, attr.Path, attr.Verbs))
                .ToList();
#endif

            return restRoutes;
        }

        private static RouteResolutionResult FindMostSpecificRoute(IEnumerable<RouteResolutionResult> routes)
        {
            RouteResolutionResult bestMatch = default(RouteResolutionResult);
            var otherMatches = new List<RouteResolutionResult>();

            foreach (var route in routes)
            {
                if (bestMatch == null)
                {
                    bestMatch = route;
                }
                else if (route.VariableCount > bestMatch.VariableCount)
                {
                    otherMatches.Clear();
                    bestMatch = route;
                }
                else if (route.VariableCount == bestMatch.VariableCount)
                {
                    // Choose
                    //     /product-lines/{productId}/{lineNumber}
                    // over
                    //     /products/{productId}/product-lines/{lineNumber}
                    // (shortest one)
                    if (route.PathLength < bestMatch.PathLength)
                    {
                        otherMatches.Add(bestMatch);
                        bestMatch = route;
                    }
                    else
                    {
                        otherMatches.Add(route);
                    }
                }
            }

            // We may find several different routes {code}/{id} and {code}/{name} having the same number of variables. 
            // Such case will be handled by the next check.
            return bestMatch == null || otherMatches.All(r => r.HasSameVariables(bestMatch))
                ? bestMatch
                : null;
        }
    }

    /// <summary>A rest route.</summary>
    public class RestRoute
    {
        private static readonly char[] ArrayBrackets = new[] { '[', ']'};

        private static string FormatValue(object value)
        {
            if (value == null) return null;

            var jsv = value.ToJsv().Trim(ArrayBrackets);
            return jsv;
        }

        /// <summary>The value.</summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Using field is just easier.")]
        public static Func<object, string> FormatVariable = value =>
        {
            if (value == null) return null;

            var valueString = value as string;
            valueString = valueString ?? FormatValue(value);
            return Uri.EscapeDataString(valueString);
        };

        /// <summary>The value.</summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Using field is just easier.")]
        public static Func<object, string> FormatQueryParameterValue = value =>
        {
            if (value == null) return null;

            // Perhaps custom formatting needed for DateTimes, lists, etc.
            var valueString = value as string;
            valueString = valueString ?? FormatValue(value);
            return Uri.EscapeDataString(valueString);
        };

        private const char PathSeparatorChar = '/';
        private const string VariablePrefix = "{";
        private const char VariablePrefixChar = '{';
        private const string VariablePostfix = "}";
        private const char VariablePostfixChar = '}';

        private readonly IDictionary<string, RouteMember> queryProperties;
        private readonly IDictionary<string, RouteMember> variablesMap = new Dictionary<string, RouteMember>(StringExtensions.InvariantComparerIgnoreCase());

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.RestRoute class.</summary>
        ///
        /// <param name="type"> The type.</param>
        /// <param name="path"> The full pathname of the file.</param>
        /// <param name="verbs">The verbs.</param>
	    public RestRoute(Type type, string path, string verbs)
        {
            this.HttpMethods = (verbs ?? string.Empty).Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            this.Type = type;
            this.Path = path;

            this.queryProperties = GetQueryProperties(type);
            foreach (var variableName in GetUrlVariables(path))
            {
                var safeVarName = variableName.TrimEnd('*');

                RouteMember propertyInfo;
                if (!this.queryProperties.TryGetValue(safeVarName, out propertyInfo))
                {
	                this.AppendError("Variable '{0}' does not match any property.".Fmt(variableName));
	                continue;
                }

                this.queryProperties.Remove(safeVarName);
                this.variablesMap[variableName] = propertyInfo;
            }
        }

        /// <summary>Gets a message describing the error.</summary>
        ///
        /// <value>A message describing the error.</value>
        public string ErrorMsg { get; private set; }

        /// <summary>Gets the type.</summary>
        ///
        /// <value>The type.</value>
        public Type Type { get; private set; }

        /// <summary>Gets a value indicating whether this object is valid.</summary>
        ///
        /// <value>true if this object is valid, false if not.</value>
        public bool IsValid
        {
            get { return string.IsNullOrEmpty(this.ErrorMsg); }
        }

        /// <summary>Gets the full pathname of the file.</summary>
        ///
        /// <value>The full pathname of the file.</value>
        public string Path { get; private set; }

        /// <summary>Gets the HTTP methods.</summary>
        ///
        /// <value>The HTTP methods.</value>
        public string[] HttpMethods { get; private set; }

        /// <summary>Gets the variables.</summary>
        ///
        /// <value>The variables.</value>
        public ICollection<string> Variables
        {
            get { return this.variablesMap.Keys; }
        }

        /// <summary>Applies this object.</summary>
        ///
        /// <param name="request">   The request.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        ///
        /// <returns>A RouteResolutionResult.</returns>
        public RouteResolutionResult Apply(object request, string httpMethod)
        {
            if (!this.IsValid)
            {
                return RouteResolutionResult.Error(this, this.ErrorMsg);
            }

            if (HttpMethods != null && HttpMethods.Length != 0 && httpMethod != null && !HttpMethods.Contains(httpMethod) && !HttpMethods.Contains("ANY"))
            {
                return RouteResolutionResult.Error(this, "Allowed HTTP methods '{0}' does not support the specified '{1}' method."
                    .Fmt(HttpMethods.Join(", "), httpMethod));
            }

            var uri = this.Path;

            var unmatchedVariables = new List<string>();
            foreach (var variable in this.variablesMap)
            {
                var property = variable.Value;
                var value = property.GetValue(request);
                var isWildCard = variable.Key.EndsWith("*");
                if (value == null && !isWildCard)
                {
                    unmatchedVariables.Add(variable.Key);
                    continue;
                }

                var variableValue = FormatVariable(value);
                uri = uri.Replace(VariablePrefix + variable.Key + VariablePostfix, variableValue);
            }

            if (unmatchedVariables.Any())
            {
                var errMsg = "Could not match following variables: " + string.Join(",", unmatchedVariables.ToArray());
                return RouteResolutionResult.Error(this, errMsg);
            }

            return RouteResolutionResult.Success(this, uri);
        }

        /// <summary>Format query parameters.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>The formatted query parameters.</returns>
        public string FormatQueryParameters(object request)
        {
            return GetQueryString(request, this.queryProperties);
        }

        internal static string GetQueryString(object request, IDictionary<string, RouteMember> propertyMap)
        {
            var result = new StringBuilder();

            foreach (var queryProperty in propertyMap)
            {
                var value = queryProperty.Value.GetValue(request, true);
                if (value == null)
                {
                    continue;
                }

                result.Append(queryProperty.Key)
                    .Append('=')
                    .Append(RestRoute.FormatQueryParameterValue(value))
                    .Append('&');
            }

            if (result.Length > 0) result.Length -= 1;
            return result.ToString();
        }

        internal static IDictionary<string, RouteMember> GetQueryProperties(Type requestType)
        {
            var result = new Dictionary<string, RouteMember>(StringExtensions.InvariantComparerIgnoreCase()); 
            var hasDataContract = requestType.HasAttr<DataContractAttribute>();

            foreach (var propertyInfo in requestType.GetPublicProperties())
            {
                var propertyName = propertyInfo.Name;

                if (!propertyInfo.CanRead) continue;
                if (hasDataContract)
                {
                    if (!propertyInfo.IsDefined(typeof(DataMemberAttribute), true)) continue;

                    var dataMember = propertyInfo.FirstAttribute<DataMemberAttribute>();
                    if (!string.IsNullOrEmpty(dataMember.Name))
                    {
                        propertyName = dataMember.Name;
                    }
                }
                else
                {
                    if (propertyInfo.IsDefined(typeof(IgnoreDataMemberAttribute), true)) continue;
                }

                result[propertyName.ToCamelCase()] = new PropertyRouteMember(propertyInfo);
            }

			if (JsConfig.IncludePublicFields)
			{
                foreach (var fieldInfo in requestType.GetPublicFields())
                {
					var fieldName = fieldInfo.Name;

					if (fieldInfo.IsDefined(typeof(IgnoreDataMemberAttribute), true)) continue;

					result[fieldName.ToCamelCase()] = new FieldRouteMember(fieldInfo);
				}

			}

            return result;
        }

        private IEnumerable<string> GetUrlVariables(string path)
        {
            var components = path.Split(PathSeparatorChar);
            foreach (var component in components)
            {
                if (string.IsNullOrEmpty(component))
                {
                    continue;
                }

                if (component.Contains(VariablePrefix) || component.Contains(VariablePostfix))
                {
                    var variableName = component.Substring(1, component.Length - 2);

                    // Accept only variables matching this format: '/{property}/'
                    // Incorrect formats: '/{property/' or '/{property}-some-other-text/'
                    // I'm not sure that the second one will be parsed correctly at server side.
                    if (component[0] != VariablePrefixChar || component[component.Length - 1] != VariablePostfixChar || variableName.Contains(VariablePostfix))
                    {
                        this.AppendError("Component '{0}' can not be parsed".Fmt(component));
                        continue;
                    }

                    yield return variableName;
                }
            }
        }

        private void AppendError(string msg)
        {
            if (string.IsNullOrEmpty(this.ErrorMsg))
            {
                this.ErrorMsg = msg;
            }
            else
            {
                this.ErrorMsg += "\r\n" + msg;
            }
        }
    }

    /// <summary>Encapsulates the result of a route resolution.</summary>
    public class RouteResolutionResult
    {
        /// <summary>Gets the fail reason.</summary>
        ///
        /// <value>The fail reason.</value>
        public string FailReason { get; private set; }

        /// <summary>Gets URI of the document.</summary>
        ///
        /// <value>The URI.</value>
        public string Uri { get; private set; }

        /// <summary>Gets the route.</summary>
        ///
        /// <value>The route.</value>
        public RestRoute Route { get; private set; }

        /// <summary>Gets a value indicating whether the matches.</summary>
        ///
        /// <value>true if matches, false if not.</value>
        public bool Matches
        {
            get { return string.IsNullOrEmpty(this.FailReason); }
        }

        /// <summary>Errors.</summary>
        ///
        /// <param name="route">   The route.</param>
        /// <param name="errorMsg">Message describing the error.</param>
        ///
        /// <returns>A RouteResolutionResult.</returns>
        public static RouteResolutionResult Error(RestRoute route, string errorMsg)
        {
            return new RouteResolutionResult { Route = route, FailReason = errorMsg };
        }

        /// <summary>Success.</summary>
        ///
        /// <param name="route">The route.</param>
        /// <param name="uri">  URI of the document.</param>
        ///
        /// <returns>A RouteResolutionResult.</returns>
        public static RouteResolutionResult Success(RestRoute route, string uri)
        {
            return new RouteResolutionResult { Route = route, Uri = uri };
        }

        internal int VariableCount
        {
            get { return Route.Variables.Count; }
        }

        internal int PathLength
        {
            get { return Route.Path.Length; }
        }

        internal bool HasSameVariables(RouteResolutionResult other)
        {
            return Route.Variables.All(v => other.Route.Variables.Contains(v));
        }
    }


}