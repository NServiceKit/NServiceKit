using System;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints
{
    /// <summary>
    /// Context to capture IService action
    /// </summary>
    public class ActionContext
    {
        /// <summary>any action.</summary>
        public const string AnyAction = "ANY";

        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>Gets or sets the type of the request.</summary>
        ///
        /// <value>The type of the request.</value>
        public Type RequestType { get; set; }

        /// <summary>Gets or sets the type of the service.</summary>
        ///
        /// <value>The type of the service.</value>
        public Type ServiceType { get; set; }

        /// <summary>Gets or sets the service action.</summary>
        ///
        /// <value>The service action.</value>
        public ActionInvokerFn ServiceAction { get; set; }

        /// <summary>Gets or sets the request filters.</summary>
        ///
        /// <value>The request filters.</value>
        public IHasRequestFilter[] RequestFilters { get; set; }

        /// <summary>Gets or sets the response filters.</summary>
        ///
        /// <value>The response filters.</value>
        public IHasResponseFilter[] ResponseFilters { get; set; }

        /// <summary>Keys.</summary>
        ///
        /// <param name="method">        The method.</param>
        /// <param name="requestDtoName">Name of the request dto.</param>
        ///
        /// <returns>A string.</returns>
        public static string Key(string method, string requestDtoName)
        {
            return method.ToUpper() + " " + requestDtoName;
        }

        /// <summary>Any key.</summary>
        ///
        /// <param name="requestDtoName">Name of the request dto.</param>
        ///
        /// <returns>A string.</returns>
        public static string AnyKey(string requestDtoName)
        {
            return AnyAction + " " + requestDtoName;
        }
    }
}