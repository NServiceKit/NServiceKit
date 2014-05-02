using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
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

    /// <summary>Encapsulates the result of the returns http.</summary>
    public class ReturnsHttpResult { }

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
        public HttpResult Any(ReturnsHttpResult request)
        {
            return new HttpResult();
        }
    }


}