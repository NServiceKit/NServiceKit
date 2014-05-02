using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>The gets only.</summary>
    public class GetsOnly { }
    /// <summary>The posts only.</summary>
    public class PostsOnly { }
    /// <summary>The puts only.</summary>
    public class PutsOnly { }
    /// <summary>The deletes only.</summary>
    public class DeletesOnly { }
    /// <summary>any request.</summary>
    public class AnyRequest { }
    /// <summary>A response.</summary>
    public class Response { }

    /// <summary>A visible localhost.</summary>
    [Restrict(VisibleLocalhostOnly = true)]
    public class VisibleLocalhost { }
    /// <summary>A visible internal.</summary>
    [Restrict(VisibleInternalOnly = true)]
    public class VisibleInternal { }

    /// <summary>A localhost only.</summary>
    [Restrict(LocalhostOnly = true)]
    public class LocalhostOnly { }
    /// <summary>An internal only.</summary>
    [Restrict(InternalOnly = true)]
    public class InternalOnly { }

    /// <summary>An XML only.</summary>
    [Restrict(EndpointAttributes.Xml)]
    public class XmlOnly { }
    /// <summary>A JSON only.</summary>
    [Restrict(EndpointAttributes.Json)]
    public class JsonOnly { }
    /// <summary>A jsv only.</summary>
    [Restrict(EndpointAttributes.Jsv)]
    public class JsvOnly { }
    /// <summary>A CSV only.</summary>
    [Restrict(EndpointAttributes.Csv)]
    public class CsvOnly { }
    /// <summary>A prototype buffer only.</summary>
    [Restrict(EndpointAttributes.ProtoBuf)]
    public class ProtoBufOnly { }
    /// <summary>A SOAP 11 only.</summary>
    [Restrict(EndpointAttributes.Soap11)]
    public class Soap11Only { }
    /// <summary>A SOAP 12 only.</summary>
    [Restrict(EndpointAttributes.Soap12)]
    public class Soap12Only { }
    /// <summary>An other format only.</summary>
    [Restrict(EndpointAttributes.FormatOther)]
    public class OtherFormatOnly { }

    /// <summary>A JSON internal XML external.</summary>
    [Restrict(
        EndpointAttributes.InternalNetworkAccess | EndpointAttributes.Json,
        EndpointAttributes.External | EndpointAttributes.Xml)]
    public class JsonInternalXmlExternal { }

    /// <summary>A ssl only.</summary>
    [Restrict(EndpointAttributes.Secure)]
    public class SslOnly { }

    /// <summary>A ssl external and insecure internal.</summary>
    [Restrict(EndpointAttributes.Secure   | EndpointAttributes.External,
              EndpointAttributes.InSecure | EndpointAttributes.InternalNetworkAccess)]
    public class SslExternalAndInsecureInternal { }


    /// <summary>Encapsulates the result of the returns http.</summary>
    public class ReturnsHttpResult
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int Id { get; set; }
    }

    /// <summary>Encapsulates the result of the returns HTTP result with marker.</summary>
    public class ReturnsHttpResultWithMarkerResult{}
    /// <summary>The returns HTTP result with marker.</summary>
    public class ReturnsHttpResultWithMarker : IReturn<ReturnsHttpResultWithMarkerResult>
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int Id { get; set; }
    }
    /// <summary>The returns HTTP result with response.</summary>
    public class ReturnsHttpResultWithResponseResponse { }
    /// <summary>The returns HTTP result with response.</summary>
    public class ReturnsHttpResultWithResponse 
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int Id { get; set; }
    }

    /// <summary>An endpoint access service.</summary>
    public class EndpointAccessService : ServiceInterface.Service
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A Response.</returns>
        public Response Get(GetsOnly request)
        {
            return new Response();
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A Response.</returns>
        public Response Post(PostsOnly request)
        {
            return new Response();
        }

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A Response.</returns>
        public Response Put(PutsOnly request)
        {
            return new Response();
        }

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A Response.</returns>
        public Response Delete(DeletesOnly request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(AnyRequest request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(VisibleLocalhost request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(VisibleInternal request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(LocalhostOnly request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(InternalOnly request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(XmlOnly request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(JsonOnly request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(JsvOnly request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(CsvOnly request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(ProtoBufOnly request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(Soap11Only request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(Soap12Only request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(OtherFormatOnly request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public Response Any(JsonInternalXmlExternal request)
        {
            return new Response();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public HttpResult Any(ReturnsHttpResult request)
        {
            return new HttpResult();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public HttpResult Any(ReturnsHttpResultWithMarker request)
        {
            return new HttpResult();
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A HttpResult.</returns>
        public HttpResult Any(ReturnsHttpResultWithResponse request)
        {
            return new HttpResult();
        }
    }
}