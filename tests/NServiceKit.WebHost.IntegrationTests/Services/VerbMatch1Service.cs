using System;
using System.Runtime.Serialization;
using NServiceKit.Common.Web;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.IntegrationTests.Services
{
    /// <summary>A verb match 1.</summary>
	[DataContract]
	[Route("/VerbMatch", "GET,DELETE")]
	[Route("/VerbMatch/{Name}", "GET,DELETE")]
	public class VerbMatch1
	{
        /// <summary>Gets or sets the name.</summary>
        ///
        /// <value>The name.</value>
		[DataMember]
		public string Name { get; set; }
	}

    /// <summary>A verb match 1 response.</summary>
	[DataContract]
	public class VerbMatch1Response
	{
        /// <summary>Gets or sets the result.</summary>
        ///
        /// <value>The result.</value>
		[DataMember]
		public string Result { get; set; }
	}

    /// <summary>A verb match 1 service.</summary>
	public class VerbMatch1Service : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Any(VerbMatch1 request)
		{
			throw new NotImplementedException();
		}

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(VerbMatch1 request)
		{
			return new VerbMatch1Response { Result = HttpMethods.Get };
		}

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(VerbMatch1 request)
		{
			return new VerbMatch1Response { Result = HttpMethods.Post };
		}

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Put(VerbMatch1 request)
		{
			return new VerbMatch1Response { Result = HttpMethods.Put };
		}

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Delete(VerbMatch1 request)
		{
			return new VerbMatch1Response { Result = HttpMethods.Delete };
		}

        /// <summary>Patches the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Patch(VerbMatch1 request)
		{
			return new VerbMatch1Response { Result = HttpMethods.Patch };
		}
	}

}