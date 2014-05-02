using System;
using System.Runtime.Serialization;

namespace NServiceKit.ServiceHost.Tests.Support
{
    /// <summary>A rest test.</summary>
	[DataContract]
	public class RestTest { }

    /// <summary>A rest test response.</summary>
	[DataContract]
	public class RestTestResponse
	{
        /// <summary>Gets or sets the name of the method.</summary>
        ///
        /// <value>The name of the method.</value>
		[DataMember]
		public string MethodName { get; set; }
	}

    /// <summary>A rest test service.</summary>
	public class RestTestService : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>A RestTestResponse.</returns>
        public RestTestResponse Any(RestTest request)
		{
			return new RestTestResponse { MethodName = "Execute" };
		}

        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(RestTest request)
		{
			return new RestTestResponse { MethodName = "Get" };
		}

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Put(RestTest request)
		{
			return new RestTestResponse { MethodName = "Put" };
		}

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(RestTest request)
		{
			return new RestTestResponse { MethodName = "Post" };
		}

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
		public object Delete(RestTest request)
		{
			return new RestTestResponse { MethodName = "Delete" };
		}
	}
}