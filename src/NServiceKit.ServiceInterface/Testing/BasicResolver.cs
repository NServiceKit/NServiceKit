using Funq;
using NServiceKit.ServiceHost;

namespace NServiceKit.ServiceInterface.Testing
{
    /// <summary>A basic resolver.</summary>
    public class BasicResolver : IResolver
    {
        private readonly Container container;

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Testing.BasicResolver class.</summary>
        public BasicResolver() : this(new Container()) {}

        /// <summary>Initializes a new instance of the NServiceKit.ServiceInterface.Testing.BasicResolver class.</summary>
        ///
        /// <param name="container">The container.</param>
        public BasicResolver(Container container)
        {
            this.container = container;
        }

        /// <summary>Try resolve.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
        public T TryResolve<T>()
        {
            return this.container.TryResolve<T>();
        }
    }
}