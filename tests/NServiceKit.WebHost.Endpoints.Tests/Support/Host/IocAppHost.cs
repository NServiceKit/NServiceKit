using System;
using System.Collections.Generic;
using Funq;
using NServiceKit.Common;
using NServiceKit.Configuration;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints.Tests.Support.Services;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Host
{
    /// <summary>An IOC application host.</summary>
    public class IocAppHost : AppHostHttpListenerBase
	{
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Host.IocAppHost class.</summary>
		public IocAppHost()
			: base("IocApp Service", typeof(IocService).Assembly)
		{
			Instance = null;
		}

        private IocAdapter iocAdapter;

        /// <summary>Configures the given container.</summary>
        ///
        /// <param name="container">The container.</param>
		public override void Configure(Container container)
		{
			container.Adapter = iocAdapter = new IocAdapter();
			container.Register(c => new FunqDepCtor());
			container.Register(c => new FunqDepProperty());
			container.Register(c => new FunqDepDisposableProperty());

            container.Register(c => new FunqSingletonScope()).ReusedWithin(ReuseScope.Default);
            container.Register(c => new FunqRequestScope()).ReusedWithin(ReuseScope.Request);
            container.Register(c => new FunqNoneScope()).ReusedWithin(ReuseScope.None);
            container.Register(c => new FunqRequestScopeDepDisposableProperty()).ReusedWithin(ReuseScope.Request);

            container.Register(c => new FunqSingletonScopeDisposable()).ReusedWithin(ReuseScope.Default);
            container.Register(c => new FunqRequestScopeDisposable()).ReusedWithin(ReuseScope.Request);
            container.Register(c => new FunqNoneScopeDisposable()).ReusedWithin(ReuseScope.None);

            Routes.Add<Ioc>("/ioc");
            Routes.Add<IocScope>("/iocscope");
		}

        /// <summary>Releases the given instance.</summary>
        ///
        /// <param name="instance">The instance.</param>
        public override void Release(object instance)
        {
            iocAdapter.Release(instance);
        }
	}

    /// <summary>An IOC adapter.</summary>
    public class IocAdapter : IContainerAdapter, IRelease
	{
        /// <summary>Try resolve.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
		public T TryResolve<T>()
		{
			if (typeof(T) == typeof(IRequestContext))
				throw new ArgumentException("should not ask for IRequestContext");

			if (typeof(T) == typeof(AltDepProperty))
				return (T)(object)new AltDepProperty();
            if (typeof(T) == typeof(AltDepDisposableProperty))
                return (T)(object)new AltDepDisposableProperty();
            if (typeof(T) == typeof(AltRequestScopeDepDisposableProperty))
                return (T)(object)HostContext.Instance.GetOrCreate(() => new AltRequestScopeDepDisposableProperty());
            
			return default(T);
		}

        /// <summary>Gets the resolve.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
		public T Resolve<T>()
		{
			if (typeof(T) == typeof(AltDepCtor))
				return (T)(object)new AltDepCtor();

			return default(T);
		}

        /// <summary>Releases the given instance.</summary>
        ///
        /// <param name="instance">The instance.</param>
        public void Release(object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }


    /// <summary>Attribute for IOC request filter.</summary>
    public class IocRequestFilterAttribute : Attribute, IHasRequestFilter
    {
        /// <summary>Gets or sets the funq singleton scope.</summary>
        ///
        /// <value>The funq singleton scope.</value>
        public FunqSingletonScope FunqSingletonScope { get; set; }

        /// <summary>Gets or sets the funq request scope.</summary>
        ///
        /// <value>The funq request scope.</value>
        public FunqRequestScope FunqRequestScope { get; set; }

        /// <summary>Gets or sets the funq none scope.</summary>
        ///
        /// <value>The funq none scope.</value>
        public FunqNoneScope FunqNoneScope { get; set; }

        /// <summary>Gets or sets the funq request scope dep disposable property.</summary>
        ///
        /// <value>The funq request scope dep disposable property.</value>
        public FunqRequestScopeDepDisposableProperty FunqRequestScopeDepDisposableProperty { get; set; }

        /// <summary>Gets or sets the alternate request scope dep disposable property.</summary>
        ///
        /// <value>The alternate request scope dep disposable property.</value>
        public AltRequestScopeDepDisposableProperty AltRequestScopeDepDisposableProperty { get; set; }

        /// <summary>Order in which Request Filters are executed. &lt;0 Executed before global request filters &gt;0 Executed after global request filters.</summary>
        ///
        /// <value>The priority.</value>
        public int Priority { get; set; }

        /// <summary>The request filter is executed before the service.</summary>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The request DTO.</param>
        public void RequestFilter(IHttpRequest req, IHttpResponse res, object requestDto)
        {
        }

        /// <summary>A new shallow copy of this filter is used on every request.</summary>
        ///
        /// <returns>An IHasRequestFilter.</returns>
        public IHasRequestFilter Copy()
        {
            return (IHasRequestFilter) this.MemberwiseClone();
        }
    }
}