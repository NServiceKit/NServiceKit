using System;

namespace NServiceKit.ServiceHost
{

    /// <summary>
    /// Lets you Register new Services and the optional restPaths will be registered against 
    /// this default Request Type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DefaultRequestAttribute : Attribute
    {
        /// <summary>Gets or sets the type of the request.</summary>
        ///
        /// <value>The type of the request.</value>
        public Type RequestType { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.DefaultRequestAttribute class.</summary>
        ///
        /// <param name="requestType">Type of the request.</param>
        public DefaultRequestAttribute(Type requestType)
        {
            RequestType = requestType;
        }
    }
}