using System;
using System.Net;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for API response description.</summary>
    public interface IApiResponseDescription
    {
        /// <summary>
        /// The status code of a response
        /// </summary>
        int StatusCode { get; }

        /// <summary>
        /// The description of a response status code
        /// </summary>
        string Description { get; }
    }

    /// <summary>Attribute for API response.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ApiResponseAttribute : Attribute, IApiResponseDescription
    {
        /// <summary>The status code of a response.</summary>
        ///
        /// <value>The status code.</value>
        public int StatusCode { get; set; }

        /// <summary>The description of a response status code.</summary>
        ///
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ApiResponseAttribute class.</summary>
        ///
        /// <param name="statusCode"> The status code.</param>
        /// <param name="description">The description.</param>
        public ApiResponseAttribute(HttpStatusCode statusCode, string description)
        {
            StatusCode = (int)statusCode;
            Description = description;
        }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ApiResponseAttribute class.</summary>
        ///
        /// <param name="statusCode"> The status code.</param>
        /// <param name="description">The description.</param>
        public ApiResponseAttribute(int statusCode, string description)
        {
            StatusCode = statusCode;
            Description = description;
        }
    }
}