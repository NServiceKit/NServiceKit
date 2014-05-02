using System;
using System.Collections.Generic;
using NServiceKit.OrmLite;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.ServiceInterface.ServiceModel;
using NServiceKit.WebHost.Endpoints.Tests.Support.Host;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A funq request scope.</summary>
    public class FunqRequestScope
    {
        /// <summary>Number of.</summary>
        public static int Count = 0;
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.FunqRequestScope class.</summary>
        public FunqRequestScope() { Count++; }
    }

    /// <summary>A funq singleton scope.</summary>
    public class FunqSingletonScope
    {
        /// <summary>Number of.</summary>
        public static int Count = 0;
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.FunqSingletonScope class.</summary>
        public FunqSingletonScope() { Count++; }
    }

    /// <summary>A funq none scope.</summary>
    public class FunqNoneScope
    {
        /// <summary>Number of.</summary>
        public static int Count = 0;
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.FunqNoneScope class.</summary>
        public FunqNoneScope() { Count++; }
    }

    /// <summary>A funq request scope disposable.</summary>
    public class FunqRequestScopeDisposable : IDisposable
    {
        /// <summary>Number of.</summary>
        public static int Count = 0;
        /// <summary>Number of disposes.</summary>
        public static int DisposeCount = 0;
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.FunqRequestScopeDisposable class.</summary>
        public FunqRequestScopeDisposable() { Count++; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            DisposeCount++;
        }
    }

    /// <summary>A funq singleton scope disposable.</summary>
    public class FunqSingletonScopeDisposable : IDisposable
    {
        /// <summary>Number of.</summary>
        public static int Count = 0;
        /// <summary>Number of disposes.</summary>
        public static int DisposeCount = 0;
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.FunqSingletonScopeDisposable class.</summary>
        public FunqSingletonScopeDisposable() { Count++; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            DisposeCount++;
        }
    }

    /// <summary>A funq none scope disposable.</summary>
    public class FunqNoneScopeDisposable : IDisposable
    {
        /// <summary>Number of.</summary>
        public static int Count = 0;
        /// <summary>Number of disposes.</summary>
        public static int DisposeCount = 0;
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.FunqNoneScopeDisposable class.</summary>
        public FunqNoneScopeDisposable() { Count++; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            DisposeCount++;
        }
    }

    /// <summary>A funq request scope dep disposable property.</summary>
    public class FunqRequestScopeDepDisposableProperty : IDisposable
    {
        /// <summary>Number of.</summary>
        public static int Count = 0;
        /// <summary>Number of disposes.</summary>
        public static int DisposeCount = 0;
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.FunqRequestScopeDepDisposableProperty class.</summary>
        public FunqRequestScopeDepDisposableProperty() { Count++; }
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() { DisposeCount++; }
    }

    /// <summary>An alternate request scope dep disposable property.</summary>
    public class AltRequestScopeDepDisposableProperty : IDisposable
    {
        /// <summary>Number of.</summary>
        public static int Count = 0;
        /// <summary>Number of disposes.</summary>
        public static int DisposeCount = 0;
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.AltRequestScopeDepDisposableProperty class.</summary>
        public AltRequestScopeDepDisposableProperty() { Count++; }
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() { DisposeCount++; }
    }

    /// <summary>A funq dep constructor.</summary>
    public class FunqDepCtor { }
    /// <summary>An alternate dep constructor.</summary>
    public class AltDepCtor { }

    /// <summary>A funq dep property.</summary>
    public class FunqDepProperty { }
    /// <summary>An alternate dep property.</summary>
    public class AltDepProperty { }

    /// <summary>A funq dep disposable property.</summary>
    public class FunqDepDisposableProperty : IDisposable
    {
        /// <summary>Number of disposes.</summary>
        public static int DisposeCount = 0;
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() { DisposeCount++; }
    }
    /// <summary>An alternate dep disposable property.</summary>
    public class AltDepDisposableProperty : IDisposable
    {
        /// <summary>Number of disposes.</summary>
        public static int DisposeCount = 0;
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() { DisposeCount++; }
    }

    /// <summary>An ioc.</summary>
    public class Ioc { }

    /// <summary>An IOC response.</summary>
    public class IocResponse : IHasResponseStatus
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.IocResponse class.</summary>
        public IocResponse()
        {
            this.ResponseStatus = new ResponseStatus();
            this.Results = new List<string>();
        }

        /// <summary>Gets or sets the results.</summary>
        ///
        /// <value>The results.</value>
        public List<string> Results { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }


    /// <summary>Attribute for action.</summary>
    [Route("/action-attr")]
    public class ActionAttr : IReturn<IocResponse> {}

    /// <summary>Attribute for action level.</summary>
    public class ActionLevelAttribute : RequestFilterAttribute
    {
        /// <summary>Gets or sets a context for the request.</summary>
        ///
        /// <value>The request context.</value>
        public IRequestContext RequestContext { get; set; }

        /// <summary>Gets or sets the funq dep property.</summary>
        ///
        /// <value>The funq dep property.</value>
        public FunqDepProperty FunqDepProperty { get; set; }

        /// <summary>Gets or sets the funq dep disposable property.</summary>
        ///
        /// <value>The funq dep disposable property.</value>
        public FunqDepDisposableProperty FunqDepDisposableProperty { get; set; }

        /// <summary>Gets or sets the alternate dep property.</summary>
        ///
        /// <value>The alternate dep property.</value>
        public AltDepProperty AltDepProperty { get; set; }

        /// <summary>Gets or sets the alternate dep disposable property.</summary>
        ///
        /// <value>The alternate dep disposable property.</value>
        public AltDepDisposableProperty AltDepDisposableProperty { get; set; }

        /// <summary>This method is only executed if the HTTP method matches the <see cref="ApplyTo"/> property.</summary>
        ///
        /// <param name="req">       The http request wrapper.</param>
        /// <param name="res">       The http response wrapper.</param>
        /// <param name="requestDto">The request DTO.</param>
        public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            var response = new IocResponse();

            var deps = new object[] {
				FunqDepProperty, FunqDepDisposableProperty, 
				AltDepProperty, AltDepDisposableProperty
			};

            foreach (var dep in deps)
            {
                if (dep != null)
                    response.Results.Add(dep.GetType().Name);
            }

            req.Items["action-attr"] = response;
        }
    }


    /// <summary>An IOC service.</summary>
    public class IocService : IService, IDisposable, IRequiresRequestContext
    {
        private readonly FunqDepCtor funqDepCtor;
        private readonly AltDepCtor altDepCtor;

        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.IocService class.</summary>
        ///
        /// <param name="funqDepCtor">The funq dep constructor.</param>
        /// <param name="altDepCtor"> The alternate dep constructor.</param>
        public IocService(FunqDepCtor funqDepCtor, AltDepCtor altDepCtor)
        {
            this.funqDepCtor = funqDepCtor;
            this.altDepCtor = altDepCtor;
        }

        /// <summary>Gets or sets a context for the request.</summary>
        ///
        /// <value>The request context.</value>
        public IRequestContext RequestContext { get; set; }

        /// <summary>Gets or sets the funq dep property.</summary>
        ///
        /// <value>The funq dep property.</value>
        public FunqDepProperty FunqDepProperty { get; set; }

        /// <summary>Gets or sets the funq dep disposable property.</summary>
        ///
        /// <value>The funq dep disposable property.</value>
        public FunqDepDisposableProperty FunqDepDisposableProperty { get; set; }

        /// <summary>Gets or sets the alternate dep property.</summary>
        ///
        /// <value>The alternate dep property.</value>
        public AltDepProperty AltDepProperty { get; set; }

        /// <summary>Gets or sets the alternate dep disposable property.</summary>
        ///
        /// <value>The alternate dep disposable property.</value>
        public AltDepDisposableProperty AltDepDisposableProperty { get; set; }

        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An IocResponse.</returns>
        public object Any(Ioc request)
        {
            var response = new IocResponse();

            var deps = new object[] {
				funqDepCtor, altDepCtor, 
				FunqDepProperty, FunqDepDisposableProperty, 
				AltDepProperty, AltDepDisposableProperty
			};

            foreach (var dep in deps)
            {
                if (dep != null)
                    response.Results.Add(dep.GetType().Name);
            }

            if (ThrowErrors) throw new ArgumentException("This service has intentionally failed");

            return response;
        }

        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An IocResponse.</returns>
        [ActionLevel]
        public IocResponse Any(ActionAttr request)
        {
            return RequestContext.Get<IHttpRequest>().Items["action-attr"] as IocResponse;
        }
        
        /// <summary>Number of disposed.</summary>
        public static int DisposedCount = 0;
        /// <summary>The throw errors.</summary>
        public static bool ThrowErrors = false;

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            DisposedCount++;
        }
    }


    /// <summary>An IOC scope.</summary>
    public class IocScope
    {
        /// <summary>Gets or sets a value indicating whether the throw.</summary>
        ///
        /// <value>true if throw, false if not.</value>
        public bool Throw { get; set; }
    }

    /// <summary>An IOC scope response.</summary>
    public class IocScopeResponse : IHasResponseStatus
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.IocScopeResponse class.</summary>
        public IocScopeResponse()
        {
            this.ResponseStatus = new ResponseStatus();
            this.Results = new Dictionary<string, int>();
        }

        /// <summary>Gets or sets the results.</summary>
        ///
        /// <value>The results.</value>
        public Dictionary<string, int> Results { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>An IOC scope service.</summary>
    [IocRequestFilter]
    public class IocScopeService : IService, IDisposable
    {
        /// <summary>Gets or sets the funq request scope.</summary>
        ///
        /// <value>The funq request scope.</value>
        public FunqRequestScope FunqRequestScope { get; set; }

        /// <summary>Gets or sets the funq singleton scope.</summary>
        ///
        /// <value>The funq singleton scope.</value>
        public FunqSingletonScope FunqSingletonScope { get; set; }

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

        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(IocScope request)
        {
            if (request.Throw)
                throw new Exception("Exception requested by user");

            var response = new IocScopeResponse {
                Results = {
                    { typeof(FunqSingletonScope).Name, FunqSingletonScope.Count },
                    { typeof(FunqRequestScope).Name, FunqRequestScope.Count },
                    { typeof(FunqNoneScope).Name, FunqNoneScope.Count },
                },                
            };

            return response;
        }

        /// <summary>Number of disposed.</summary>
        public static int DisposedCount = 0;
        /// <summary>The throw errors.</summary>
        public static bool ThrowErrors = false;

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            DisposedCount++;
        }    
    }

    /// <summary>An IOC dispose.</summary>
    public class IocDispose : IReturn<IocDisposeResponse>
    {
        /// <summary>Gets or sets a value indicating whether the throw.</summary>
        ///
        /// <value>true if throw, false if not.</value>
        public bool Throw { get; set; }
    }

    /// <summary>An IOC dispose response.</summary>
    public class IocDisposeResponse : IHasResponseStatus
    {
        /// <summary>Initializes a new instance of the NServiceKit.WebHost.Endpoints.Tests.Support.Services.IocDisposeResponse class.</summary>
        public IocDisposeResponse()
        {
            this.ResponseStatus = new ResponseStatus();
            this.Results = new Dictionary<string, int>();
        }

        /// <summary>Gets or sets the results.</summary>
        ///
        /// <value>The results.</value>
        public Dictionary<string, int> Results { get; set; }

        /// <summary>Gets or sets the response status.</summary>
        ///
        /// <value>The response status.</value>
        public ResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>An IOC disposable service.</summary>
    public class IocDisposableService : IService, IDisposable
    {
        /// <summary>Gets or sets the funq request scope disposable.</summary>
        ///
        /// <value>The funq request scope disposable.</value>
        public FunqRequestScopeDisposable FunqRequestScopeDisposable { get; set; }

        /// <summary>Gets or sets the funq singleton scope disposable.</summary>
        ///
        /// <value>The funq singleton scope disposable.</value>
        public FunqSingletonScopeDisposable FunqSingletonScopeDisposable { get; set; }

        /// <summary>Gets or sets the funq none scope disposable.</summary>
        ///
        /// <value>The funq none scope disposable.</value>
        public FunqNoneScopeDisposable FunqNoneScopeDisposable { get; set; }

        /// <summary>Gets or sets the funq request scope dep disposable property.</summary>
        ///
        /// <value>The funq request scope dep disposable property.</value>
        public FunqRequestScopeDepDisposableProperty FunqRequestScopeDepDisposableProperty { get; set; }

        /// <summary>Gets or sets the alternate request scope dep disposable property.</summary>
        ///
        /// <value>The alternate request scope dep disposable property.</value>
        public AltRequestScopeDepDisposableProperty AltRequestScopeDepDisposableProperty { get; set; }

        /// <summary>Anies the given request.</summary>
        ///
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        public object Any(IocDispose request)
        {
            if (request.Throw)
                throw new Exception("Exception requested by user");

            var response = new IocDisposeResponse
            {
                Results = {
                    { typeof(FunqSingletonScopeDisposable).Name, FunqSingletonScopeDisposable.DisposeCount },
                    { typeof(FunqRequestScopeDisposable).Name, FunqRequestScopeDisposable.DisposeCount },
                    { typeof(FunqNoneScopeDisposable).Name, FunqNoneScopeDisposable.DisposeCount },
                    { typeof(FunqRequestScopeDepDisposableProperty).Name, FunqRequestScopeDepDisposableProperty.DisposeCount },
                    { typeof(AltRequestScopeDepDisposableProperty).Name, AltRequestScopeDepDisposableProperty.DisposeCount },
                },
            };

            return response;
        }

        /// <summary>Number of disposes.</summary>
        public static int DisposeCount = 0;

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            DisposeCount++;
        }
    }

}