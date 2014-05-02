using System;
using System.Data;
using NServiceKit.CacheAccess;
using NServiceKit.Messaging;
using NServiceKit.OrmLite;
using NServiceKit.Redis;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints;

namespace NServiceKit.ServiceInterface
{
    /// <summary>
    /// Generic + Useful IService base class
    /// </summary>
    public class Service : IService, IRequiresRequestContext, IServiceBase, IDisposable
    {
        /// <summary>Gets or sets a context for the request.</summary>
        ///
        /// <value>The request context.</value>
        public IRequestContext RequestContext { get; set; }

        private IResolver resolver;

        /// <summary>Gets the resolver.</summary>
        ///
        /// <returns>The resolver.</returns>
        public virtual IResolver GetResolver()
        {
            return resolver ?? EndpointHost.AppHost;
        }

        /// <summary>Gets application host.</summary>
        ///
        /// <returns>The application host.</returns>
        public virtual IAppHost GetAppHost()
        {
            return (resolver as IAppHost) ?? EndpointHost.AppHost;
        }

        /// <summary>Sets a resolver.</summary>
        ///
        /// <param name="resolver">The resolver.</param>
        ///
        /// <returns>A Service.</returns>
        public virtual Service SetResolver(IResolver resolver)
        {
            this.resolver = resolver;
            return this;
        }

        /// <summary>(This method is obsolete) sets application host.</summary>
        ///
        /// <param name="appHost">The application host.</param>
        ///
        /// <returns>A Service.</returns>
        [Obsolete("Use SetResolver")]
        public virtual Service SetAppHost(IAppHost appHost)
        {
            this.resolver = appHost;
            return this;
        }

        /// <summary>Try resolve.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        ///
        /// <returns>A T.</returns>
        public virtual T TryResolve<T>()
        {
            return this.GetResolver() == null
                ? default(T)
                : this.GetResolver().TryResolve<T>();
        }

        /// <summary>Resolve an alternate Web Service from NServiceKit's IOC container.</summary>
        ///
        /// <typeparam name="T">.</typeparam>
        ///
        /// <returns>A T.</returns>
        public virtual T ResolveService<T>()
        {
            var service = TryResolve<T>();
            var requiresContext = service as IRequiresRequestContext;
            if (requiresContext != null)
            {
                requiresContext.RequestContext = this.RequestContext;
            }
            return service;
        }

        private IHttpRequest request;

        /// <summary>Gets the request.</summary>
        ///
        /// <value>The request.</value>
        protected virtual IHttpRequest Request
        {
            get { return request ?? (request = RequestContext != null ? RequestContext.Get<IHttpRequest>() : TryResolve<IHttpRequest>()); }
        }

        private IHttpResponse response;

        /// <summary>Gets the response.</summary>
        ///
        /// <value>The response.</value>
        protected virtual IHttpResponse Response
        {
            get { return response ?? (response = RequestContext != null ? RequestContext.Get<IHttpResponse>() : TryResolve<IHttpResponse>()); }
        }

        private ICacheClient cache;

        /// <summary>Gets the cache.</summary>
        ///
        /// <value>The cache.</value>
        public virtual ICacheClient Cache
        {
            get
            {
                return cache ??
                    (cache = TryResolve<ICacheClient>()) ??
                    (cache = (TryResolve<IRedisClientsManager>() != null ? TryResolve<IRedisClientsManager>().GetCacheClient() : null));
            }
        }

        private IDbConnection db;

        /// <summary>Gets the database.</summary>
        ///
        /// <value>The database.</value>
        public virtual IDbConnection Db
        {
            get { return db ?? (db = TryResolve<IDbConnectionFactory>().Open()); }
        }

        private IRedisClient redis;

        /// <summary>Gets the redis.</summary>
        ///
        /// <value>The redis.</value>
        public virtual IRedisClient Redis
        {
            get { return redis ?? (redis = TryResolve<IRedisClientsManager>().GetClient()); }
        }

        private IMessageProducer messageProducer;

        /// <summary>Gets the message producer.</summary>
        ///
        /// <value>The message producer.</value>
        public virtual IMessageProducer MessageProducer
        {
            get { return messageProducer ?? (messageProducer = TryResolve<IMessageFactory>().CreateMessageProducer()); }
        }

        private ISessionFactory sessionFactory;

        /// <summary>Gets the session factory.</summary>
        ///
        /// <value>The session factory.</value>
        public virtual ISessionFactory SessionFactory
        {
            get { return sessionFactory ?? (sessionFactory = TryResolve<ISessionFactory>()) ?? new SessionFactory(Cache); }
        }

        /// <summary>
        /// Dynamic Session Bag
        /// </summary>
        private ISession session;

        /// <summary>Gets the session.</summary>
        ///
        /// <value>The session.</value>
        public virtual ISession Session
        {
            get
            {
                return session ?? (session = TryResolve<ISession>() //Easier to mock
                    ?? SessionFactory.GetOrCreateSession(Request, Response));
            }
        }

        /// <summary>
        /// Typed UserSession
        /// </summary>
        private object userSession;

        /// <summary>Session as.</summary>
        ///
        /// <typeparam name="TUserSession">Type of the user session.</typeparam>
        ///
        /// <returns>A TUserSession.</returns>
        protected virtual TUserSession SessionAs<TUserSession>()
        {
            if (userSession == null)
            {
                userSession = TryResolve<TUserSession>(); //Easier to mock
                if (userSession == null)
                    userSession = Cache.SessionAs<TUserSession>(Request, Response);
            }
            return (TUserSession)userSession;
        }

        /// <summary>Publish message.</summary>
        ///
        /// <exception cref="NullReferenceException">Thrown when a value was unexpectedly null.</exception>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="message">The message.</param>
        public virtual void PublishMessage<T>(T message)
        {
            //TODO: Register In-Memory IMessageFactory by default
            if (MessageProducer == null)
                throw new NullReferenceException("No IMessageFactory was registered, cannot PublishMessage");

            MessageProducer.Publish(message);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public virtual void Dispose()
        {
            if (db != null)
                db.Dispose();
            if (redis != null)
                redis.Dispose();
            if (messageProducer != null)
                messageProducer.Dispose();
        }
    }

}
