using System;

namespace NServiceKit.ServiceHost
{
    /// <summary>Attribute for api.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ApiAttribute : Attribute
    {
        /// <summary>
        /// The overall description of an API. Used by Swagger.
        /// </summary>
        public string Description { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ApiAttribute class.</summary>
        public ApiAttribute() {}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.ApiAttribute class.</summary>
        ///
        /// <param name="description">The overall description of an API. Used by Swagger.</param>
        public ApiAttribute(string description)
        {
            Description = description;
        }
    }
}