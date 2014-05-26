namespace NServiceKit.Api.Swagger
{
    using NServiceKit.ServiceHost;

    /// <summary>
    /// The api method attribute.
    /// </summary>
    public class NamedRouteAttribute : RouteAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedRouteAttribute"/> class.
        /// </summary>
        /// <param name="path">
        /// The path to bind the route to.
        /// </param>
        /// <param name="verbs">
        /// The HTTP verbs that the route responds to.
        /// </param>
        /// <param name="name">
        /// The nickname of the route to show in documentation.
        /// </param>
        public NamedRouteAttribute(string path, string verbs, string name)
            : base(path, verbs)
        {
            this.Name = name;
        }

        /// <inheritdoc/>
        public NamedRouteAttribute(string path, string verbs)
            : base(path, verbs)
        {
        }

        /// <summary>
        /// The sugested method name.
        /// </summary>
        public string Name { get; set; }
    }
}
