#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
using System.ServiceModel;
using System.ServiceModel.Description;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>
    /// Generic Proxy for service calls.
    /// </summary>
    /// <typeparam name="T">The service Contract</typeparam>
    public class GenericProxy<T> : ClientBase<T> where T : class
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.GenericProxy&lt;T&gt; class.</summary>
        public GenericProxy()
            : base()
        {
            Initialize();
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.GenericProxy&lt;T&gt; class.</summary>
        ///
        /// <param name="endpoint">The endpoint.</param>
        public GenericProxy(string endpoint)
            : base(endpoint)
        {
            Initialize();
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.GenericProxy&lt;T&gt; class.</summary>
        ///
        /// <param name="endpoint">The endpoint.</param>
        public GenericProxy(ServiceEndpoint endpoint)
            : base(endpoint.Binding, endpoint.Address)
        {
            Initialize();
        }

        /// <summary>Initializes this object.</summary>
        public void Initialize()
        {
            //this.Endpoint.Behaviors.Add(new ServiceEndpointBehaviour());
        }

        /// <summary>
        /// Returns the transparent proxy for the service call
        /// </summary>
        public T Proxy
        {
            get
            {
                return base.Channel;
            }
        }
    }
}
#endif