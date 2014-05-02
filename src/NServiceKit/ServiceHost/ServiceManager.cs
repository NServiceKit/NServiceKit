using System;
using System.Collections.Generic;
using System.Reflection;
using NServiceKit.Logging;
using Funq;
using NServiceKit.Text;

namespace NServiceKit.ServiceHost
{
    /// <summary>Manager for services.</summary>
	public class ServiceManager
		: IDisposable
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(ServiceManager));

        /// <summary>Gets the container.</summary>
        ///
        /// <value>The container.</value>
		public Container Container { get; private set; }

        /// <summary>Gets the service controller.</summary>
        ///
        /// <value>The service controller.</value>
		public ServiceController ServiceController { get; private set; }

        /// <summary>Gets the metadata.</summary>
        ///
        /// <value>The metadata.</value>
        public ServiceMetadata Metadata { get; internal set; }

        //public ServiceOperations ServiceOperations { get; set; }
        //public ServiceOperations AllServiceOperations { get; set; }

        /// <summary>Inject alternative container and strategy for resolving Service Types.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="assembliesWithServices">A variable-length parameters list containing assemblies with services.</param>
		public ServiceManager(params Assembly[] assembliesWithServices)
		{
			if (assembliesWithServices == null || assembliesWithServices.Length == 0)
				throw new ArgumentException(
					"No Assemblies provided in your AppHost's base constructor.\n"
					+ "To register your services, please provide the assemblies where your web services are defined.");

			this.Container = new Container { DefaultOwner = Owner.External };
            this.Metadata = new ServiceMetadata();
            this.ServiceController = new ServiceController(() => GetAssemblyTypes(assembliesWithServices), this.Metadata);
		}

        /// <summary>Inject alternative container and strategy for resolving Service Types.</summary>
        ///
        /// <param name="container">             The container.</param>
        /// <param name="assembliesWithServices">A variable-length parameters list containing assemblies with services.</param>
        public ServiceManager(Container container, params Assembly[] assembliesWithServices)
            : this(assembliesWithServices)
        {
            this.Container = container ?? new Container();
        }

        /// <summary>
        /// Inject alternative container and strategy for resolving Service Types
        /// </summary>
        public ServiceManager(Container container, ServiceController serviceController)
        {
            if (serviceController == null)
                throw new ArgumentNullException("serviceController");

            this.Container = container ?? new Container();
            this.Metadata = serviceController.Metadata; //always share the same metadata
            this.ServiceController = serviceController;
        }

		private List<Type> GetAssemblyTypes(Assembly[] assembliesWithServices)
		{
			var results = new List<Type>();
			string assemblyName = null;
			string typeName = null;

			try
			{
				foreach (var assembly in assembliesWithServices)
				{
					assemblyName = assembly.FullName;
					foreach (var type in assembly.GetTypes())
					{
						typeName = type.Name;
						results.Add(type);
					}
				}
				return results;
			}
			catch (Exception ex)
			{
				var msg = string.Format("Failed loading types, last assembly '{0}', type: '{1}'", assemblyName, typeName);
				Log.Error(msg, ex);
				throw new Exception(msg, ex);
			}
		}

		private ContainerResolveCache typeFactory;

        /// <summary>Initialises this object.</summary>
        ///
        /// <returns>A ServiceManager.</returns>
		public ServiceManager Init()
		{
			typeFactory = new ContainerResolveCache(this.Container);

			this.ServiceController.Register(typeFactory);

			this.Container.RegisterAutoWiredTypes(this.Metadata.ServiceTypes);

		    return this;
		}


        /// <summary>.</summary>
        [Obsolete("Use the New API (NServiceKit.ServiceInterface.Service) for future services. See: https://github.com/NServiceKit/NServiceKit/wiki/New-Api")]
		public void RegisterService<T>()
		{
		    if (!typeof (T).IsGenericType || typeof (T).GetGenericTypeDefinition() != typeof (IService<>))
		    {
		        throw new ArgumentException("Type {0} is not a Web Service that inherits IService<>".Fmt(typeof (T).FullName));
		    }

		    this.ServiceController.RegisterGService(typeFactory, typeof(T));
			this.Container.RegisterAutoWired<T>();
		}

        /// <summary>Registers the service described by serviceType.</summary>
        ///
        /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or illegal values.</exception>
        ///
        /// <param name="serviceType">Type of the service.</param>
        ///
        /// <returns>A Type.</returns>
		public Type RegisterService(Type serviceType)
		{

#pragma warning disable 618
            var genericServiceType = serviceType.GetTypeWithGenericTypeDefinitionOf(typeof(IService<>));

            try
			{
                if (genericServiceType != null)
                {
                    this.ServiceController.RegisterGService(typeFactory, serviceType);
                    this.Container.RegisterAutoWiredType(serviceType);
                    return genericServiceType;
                }
#pragma warning restore 618

                var isNService = typeof(IService).IsAssignableFrom(serviceType);
                if (isNService)
                {
                    this.ServiceController.RegisterNService(typeFactory, serviceType);
                    this.Container.RegisterAutoWiredType(serviceType);
                    return null;
                }

                throw new ArgumentException("Type {0} is not a Web Service that inherits IService<> or IService".Fmt(serviceType.FullName));
            }
			catch (Exception ex)
			{
				Log.Error(ex);
			    return genericServiceType;
			}
		}

        /// <summary>Executes the given dto.</summary>
        ///
        /// <param name="dto">The dto.</param>
        ///
        /// <returns>An object.</returns>
		public object Execute(object dto)
		{
			return this.ServiceController.Execute(dto, null);
		}

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose()
		{
			if (this.Container != null)
			{
				this.Container.Dispose();
			}
		}

        /// <summary>After initialise.</summary>
		public void AfterInit()
		{
			this.ServiceController.AfterInit();
		}
	}

}
