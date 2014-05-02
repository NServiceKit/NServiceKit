using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceKit.ServiceClient.Web;

namespace NServiceKit.Common.Tests.ServiceClient.Web
{
    /// <summary>A service client base tester.</summary>
    public class ServiceClientBaseTester : ServiceClientBase
    {
        /// <summary>Gets the type of the content.</summary>
        ///
        /// <value>The type of the content.</value>
        public override string ContentType { get { return String.Format("application/{0}", Format); } }

        /// <summary>Serialize to stream.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="request">       The request.</param>
        /// <param name="stream">        The stream.</param>
        public override void SerializeToStream(ServiceHost.IRequestContext requestContext, object request, System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>Deserialize from stream.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="stream">The stream.</param>
        ///
        /// <returns>A T.</returns>
        public override T DeserializeFromStream<T>(System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets the stream deserializer.</summary>
        ///
        /// <value>The stream deserializer.</value>
        public override ServiceHost.StreamDeserializerDelegate StreamDeserializer
        {
            get
            {
                return null;
            }
        }

        /// <summary>Gets the format to use.</summary>
        ///
        /// <value>The format.</value>
        public override string Format
        {
            get { return "TestFormat"; }
        }
    }
}
