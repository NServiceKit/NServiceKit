using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Funq;
using NServiceKit.Configuration;

namespace NServiceKit.ServiceHost.Tests.TypeFactory
{
	/// <summary>
	/// Reflection example provided for performance comparisons
	/// </summary>
	public class ReflectionTypeFunqContainer
		: ITypeFactory
	{
        /// <summary>The container.</summary>
		protected Container container;

        /// <summary>Gets or sets the scope.</summary>
        ///
        /// <value>The scope.</value>
		public ReuseScope Scope { get; set; }

        /// <summary>Initializes a new instance of the NServiceKit.ServiceHost.Tests.TypeFactory.ReflectionTypeFunqContainer class.</summary>
        ///
        /// <param name="container">The container.</param>
		public ReflectionTypeFunqContainer(Container container)
		{
			this.container = container;
			this.Scope = ReuseScope.None;
		}

        /// <summary>Gets resolve method.</summary>
        ///
        /// <param name="typeWithResolveMethod">The type with resolve method.</param>
        /// <param name="serviceType">          Type of the service.</param>
        ///
        /// <returns>The resolve method.</returns>
		protected static MethodInfo GetResolveMethod(Type typeWithResolveMethod, Type serviceType)
		{
			var methodInfo = typeWithResolveMethod.GetMethod("Resolve", new Type[0]);
			return methodInfo.MakeGenericMethod(new[] { serviceType });
		}

        /// <summary>Gets constructor with most parameters.</summary>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>The constructor with most parameters.</returns>
		public static ConstructorInfo GetConstructorWithMostParams(Type type)
		{
			return type.GetConstructors()
				.OrderByDescending(x => x.GetParameters().Length)
				.First(ctor => !ctor.IsStatic);
		}

        /// <summary>Automatic wire.</summary>
        ///
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="resolveFn">The resolve function.</param>
        ///
        /// <returns>A Func&lt;TService&gt;</returns>
		public Func<TService> AutoWire<TService>(Func<Type, object> resolveFn)
		{
			var serviceType = typeof(TService);
			var ci = GetConstructorWithMostParams(serviceType);

			var paramValues = new List<object>();
			var ciParams = ci.GetParameters();
			foreach (var parameterInfo in ciParams)
			{
				var paramValue = resolveFn(parameterInfo.ParameterType);
				paramValues.Add(paramValue);
			}

			var service = ci.Invoke(paramValues.ToArray());

			foreach (var propertyInfo in serviceType.GetProperties())
			{
				if (propertyInfo.PropertyType.IsValueType) continue;

				var propertyValue = resolveFn(propertyInfo.PropertyType);
				var propertySetter = propertyInfo.GetSetMethod();
				if (propertySetter != null)
				{
					propertySetter.Invoke(service, new[] { propertyValue });
				}
			}

			return () => (TService)service;
		}

		private static Func<Type, object> Resolve(Container container)
		{
			return delegate(Type serviceType) {
				var resolveMethodInfo = GetResolveMethod(container.GetType(), serviceType);
				return resolveMethodInfo.Invoke(container, new object[0]);
			};
		}

        /// <summary>Registers this object.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
		public void Register<T>()
		{
			//Everything from here needs to be optimized
			Func<Container, T> registerFn = delegate(Container c) {
				Func<T> serviceFactoryFn = AutoWire<T>(Resolve(c));
				return serviceFactoryFn();
			};

			this.container.Register(registerFn).ReusedWithin(this.Scope);
		}

        /// <summary>Registers this object.</summary>
        ///
        /// <param name="serviceTypes">List of types of the services.</param>
		public void Register(params Type[] serviceTypes)
		{
			RegisterTypes(serviceTypes);
		}

        /// <summary>Registers the types described by serviceTypes.</summary>
        ///
        /// <param name="serviceTypes">List of types of the services.</param>
		public void RegisterTypes(IEnumerable<Type> serviceTypes)
		{
			foreach (var serviceType in serviceTypes)
			{
				var methodInfo = GetType().GetMethod("Register", new Type[0]);
				var registerMethodInfo = methodInfo.MakeGenericMethod(new[] { serviceType });
				registerMethodInfo.Invoke(this, new object[0]);
			}
		}

        /// <summary>Creates an instance.</summary>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>The new instance.</returns>
		public object CreateInstance(Type type)
		{
			var factoryFn = Resolve(this.container);
			return factoryFn(type);
		}
	}
}