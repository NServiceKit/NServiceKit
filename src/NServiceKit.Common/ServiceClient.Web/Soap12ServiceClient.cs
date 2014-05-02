#if !(SILVERLIGHT || MONOTOUCH || XBOX || __ANDROID__)
using System;
using System.IO;
using NServiceKit.Service;
using System.Net;

namespace NServiceKit.ServiceClient.Web
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using NServiceKit.Text;
    using NServiceKit.Service;

    /// <summary>A SOAP 12 service client.</summary>
    public class Soap12ServiceClient : WcfServiceClient
    {
        /// <summary>Initializes a new instance of the NServiceKit.ServiceClient.Web.Soap12ServiceClient class.</summary>
        ///
        /// <param name="uri">URI of the document.</param>
        public Soap12ServiceClient(string uri)
        {
            this.Uri = uri.WithTrailingSlash() + "Soap12";
            this.StoreCookies = true;
        }

        private WSHttpBinding binding;

        private Binding WsHttpBinding
        {
            get
            {
                if (this.binding == null)
                {
                    this.binding = new WSHttpBinding {
                        MaxReceivedMessageSize = int.MaxValue,
                        HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,						
                        MaxBufferPoolSize = 524288,
                    };
                    this.binding.Security.Mode = SecurityMode.None;
                    // CCB Custom
                    // Yes, you need this to manage cookies yourself.  Seems counterintutive, but set to true,
                    // it only means that the framework will manage cookie propagation for the same call, which is
                    // not what we want.
                    if (StoreCookies)
                        this.binding.AllowCookies = false;
                }
                return this.binding;
            }
        }

        /// <summary>Gets the binding.</summary>
        ///
        /// <value>The binding.</value>
        protected override Binding Binding
        {
            get { return this.WsHttpBinding; }
        }

        /// <summary>Gets the message version.</summary>
        ///
        /// <value>The message version.</value>
        protected override MessageVersion MessageVersion
        {
            get { return MessageVersion.Default; }
        }

        /// <summary>Sets a proxy.</summary>
        ///
        /// <param name="proxyAddress">The proxy address.</param>
        public override void SetProxy(Uri proxyAddress)
        {
            var wsHttpBinding = (WSHttpBinding)Binding;

            wsHttpBinding.ProxyAddress = proxyAddress;
            wsHttpBinding.UseDefaultWebProxy = false;
            wsHttpBinding.BypassProxyOnLocal = false;
            return;
        }
    }
}
#endif