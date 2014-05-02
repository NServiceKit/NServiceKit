#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
using System;
using System.ServiceModel.Channels;
using NServiceKit.Service;
using System.Xml;

namespace NServiceKit.ServiceClient.Web
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWcfServiceClient : IServiceClient
    {
        /// <summary>Gets or sets URI of the document.</summary>
        ///
        /// <value>The URI.</value>
        string Uri { get; set; }

        /// <summary>Sets a proxy.</summary>
        ///
        /// <param name="proxyAddress">The proxy address.</param>
        void SetProxy(Uri proxyAddress);

        /// <summary>Send this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A Message.</returns>
        Message Send(object request);

        /// <summary>Send this message.</summary>
        ///
        /// <param name="request">The request.</param>
        /// <param name="action"> The action.</param>
        ///
        /// <returns>A Message.</returns>
        Message Send(object request, string action);

        /// <summary>Send this message.</summary>
        ///
        /// <param name="reader">The reader.</param>
        /// <param name="action">The action.</param>
        ///
        /// <returns>A Message.</returns>
        Message Send(XmlReader reader, string action);

        /// <summary>Send this message.</summary>
        ///
        /// <param name="message">The message.</param>
        ///
        /// <returns>A Message.</returns>
        Message Send(Message message);

        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="request">The request.</param>
        /// <param name="action"> The action.</param>
        void SendOneWay(object request, string action);

        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="reader">The reader.</param>
        /// <param name="action">The action.</param>
        void SendOneWay(XmlReader reader, string action);

        /// <summary>Sends an one way.</summary>
        ///
        /// <param name="message">The message.</param>
        void SendOneWay(Message message);
    }
}
#endif