using System.Runtime.Serialization;
using System.Reflection;
using System;

namespace NServiceKit.ServiceHost.Tests.Support
{
    /// <summary>A generic 1.</summary>
    public class Generic1 { }

    /// <summary>A generic 1 response.</summary>
    public class Generic1Response
    {
        /// <summary>Gets or sets the data.</summary>
        ///
        /// <value>The data.</value>
        public string Data { get; set; }
    }

    /// <summary>A generic 2.</summary>
    public class Generic2 { }

    /// <summary>A generic 3.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    [DataContract]
    public class Generic3<T>
    {
        /// <summary>Gets or sets the data.</summary>
        ///
        /// <value>The data.</value>
        public T Data { get; set; }
    }

    /// <summary>A generic service.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
	public class GenericService<T> : ServiceInterface.Service
	{
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>A Generic1Response.</returns>
        public Generic1Response Any(T request)
        {
            return new Generic1Response() { Data = request.GetType().FullName };
        }
    }
}
