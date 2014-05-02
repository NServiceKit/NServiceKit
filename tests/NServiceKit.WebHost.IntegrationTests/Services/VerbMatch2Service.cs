using System;
using System.Runtime.Serialization;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A verb match 2.</summary>
	[DataContract]
	[Route("/VerbMatch", "POST,PUT,PATCH")]
	[Route("/VerbMatch/{Name}", "POST,PUT,PATCH")]
	public class VerbMatch2
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		[DataMember]
		public string Name { get; set; }
	}

    /// <summary>A verb match 2 response.</summary>
	[DataContract]
	public class VerbMatch2Response
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>A verb match 2 service.</summary>
	public class VerbMatch2Service : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(VerbMatch2 request)
		{
			throw new NotImplementedException();
		}

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(VerbMatch2 request)
		{
			return new VerbMatch2Response { Result = HttpMethods.Get };
		}

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(VerbMatch2 request)
		{
			return new VerbMatch2Response { Result = HttpMethods.Post };
		}

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Put(VerbMatch2 request)
		{
			return new VerbMatch2Response { Result = HttpMethods.Put };
		}

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Delete(VerbMatch2 request)
		{
			return new VerbMatch2Response { Result = HttpMethods.Delete };
		}

        /// <summary>Patches the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Patch(VerbMatch2 request)
		{
			return new VerbMatch2Response { Result = HttpMethods.Patch };
		}
	}

}