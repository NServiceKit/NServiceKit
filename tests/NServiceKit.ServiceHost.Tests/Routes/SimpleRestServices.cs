using System.Net;
using NServiceKit.Common.Web;
using NServiceKit.ServiceInterface;

namespace NServiceKit.ServiceHost.Tests.Routes
{
#pragma warning disable 618
    /// <summary>An old API request dto.</summary>
    public class OldApiRequestDto
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>An old API rest service with some verbs implemented.</summary>
    public class OldApiRestServiceWithSomeVerbsImplemented : RestServiceBase<OldApiRequestDto>
    {
        /// <summary>Executes the get action.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public override object OnGet(OldApiRequestDto request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }

        /// <summary>Executes the put action.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public override object OnPut(OldApiRequestDto request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }
    }

    /// <summary>An old API request dto 2.</summary>
	public class OldApiRequestDto2
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		public string Name { get; set; }
	}

    /// <summary>An old API rest service with all verbs implemented.</summary>
	public class OldApiRestServiceWithAllVerbsImplemented : RestServiceBase<OldApiRequestDto2>
    {
        /// <summary>Executes the get action.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public override object OnGet(OldApiRequestDto2 request)
        {
            return new HttpResult {StatusCode = HttpStatusCode.OK};
        }

        /// <summary>Executes the put action.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public override object OnPut(OldApiRequestDto2 request)
        {
            return new HttpResult {StatusCode = HttpStatusCode.OK};
        }

        /// <summary>Executes the post action.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public override object OnPost(OldApiRequestDto2 request)
        {
            return new HttpResult {StatusCode = HttpStatusCode.OK};
        }

        /// <summary>Executes the delete action.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public override object OnDelete(OldApiRequestDto2 request)
        {
            return new HttpResult {StatusCode = HttpStatusCode.OK};
        }

        /// <summary>Executes the patch action.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public override object OnPatch(OldApiRequestDto2 request)
        {
            return new HttpResult {StatusCode = HttpStatusCode.OK};
        }
    }
#pragma warning restore 618

    /// <summary>A new API request dto.</summary>
    public class NewApiRequestDto
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>A new API request dto 2.</summary>
    public class NewApiRequestDto2
    {
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>A new API rest service with all verbs implemented.</summary>
    public class NewApiRestServiceWithAllVerbsImplemented : IService
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(NewApiRequestDto request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Put(NewApiRequestDto request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Post(NewApiRequestDto request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Delete(NewApiRequestDto request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }

        /// <summary>Patches the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Patch(NewApiRequestDto request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(NewApiRequestDto2 request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Put(NewApiRequestDto2 request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Post(NewApiRequestDto2 request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Delete(NewApiRequestDto2 request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }

        /// <summary>Patches the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Patch(NewApiRequestDto2 request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }
    }

    /// <summary>A new API request dto with identifier.</summary>
    public class NewApiRequestDtoWithId
    {
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
        public int Id { get; set; }
    }

    /// <summary>A new API request dto with identifier service.</summary>
    public class NewApiRequestDtoWithIdService : IService
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Get(NewApiRequestDtoWithId request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(NewApiRequestDtoWithId request)
        {
            return new HttpResult { StatusCode = HttpStatusCode.OK };
        }
    }

    /// <summary>A new API request dto with field identifier.</summary>
	public class NewApiRequestDtoWithFieldId
	{
        /// <summary>Gets or sets the identifier.</summary>
        ///
        /// <value>The identifier.</value>
		public int Id { get; set; }
	}

    /// <summary>A new API request dto with field identifier service.</summary>
	public class NewApiRequestDtoWithFieldIdService : IService
	{
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(NewApiRequestDtoWithFieldId request)
		{
			return new HttpResult { StatusCode = HttpStatusCode.OK };
		}

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(NewApiRequestDtoWithFieldId request)
		{
			return new HttpResult { StatusCode = HttpStatusCode.OK };
		}
	}

}
