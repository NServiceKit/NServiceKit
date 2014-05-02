using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NServiceKit.Common.Reflection;
using NServiceKit.Common.Tests.Models;

namespace NServiceKit.Common.Tests.Perf
{
    /// <summary>A property accessor performance.</summary>
	[Ignore("Benchmark for comparing property access")]
	[TestFixture]
	public class PropertyAccessorPerf
		: PerfTestBase
	{
        /// <summary>Initializes a new instance of the NServiceKit.Common.Tests.Perf.PropertyAccessorPerf class.</summary>
		public PropertyAccessorPerf()
		{
			this.MultipleIterations = new List<int> { 1000000 };
		}

        /// <summary>A test acessor.</summary>
        /// <typeparam name="TEntity">Type of the entity.</typeparam>
		public static class TestAcessor<TEntity>
		{
            /// <summary>Typed get property function.</summary>
            ///
            /// <typeparam name="TId">Type of the identifier.</typeparam>
            /// <param name="pi">The pi.</param>
            ///
            /// <returns>A Func&lt;TEntity,TId&gt;</returns>
			public static Func<TEntity, TId> TypedGetPropertyFn<TId>(PropertyInfo pi)
			{
				var mi = pi.GetGetMethod();
				return (Func<TEntity, TId>)Delegate.CreateDelegate(typeof(Func<TEntity, TId>), mi);
			}

			/// <summary>
			/// Required to cast the return ValueType to an object for caching
			/// </summary>
			public static Func<TEntity, object> ValueUnTypedGetPropertyFn<TId>(PropertyInfo pi)
			{
				var typedPropertyFn = TypedGetPropertyFn<TId>(pi);
				return x => typedPropertyFn(x);
			}

            /// <summary>Value un typed get property type function reflection.</summary>
            ///
            /// <param name="pi">The pi.</param>
            ///
            /// <returns>A Func&lt;TEntity,object&gt;</returns>
			public static Func<TEntity, object> ValueUnTypedGetPropertyTypeFn_Reflection(PropertyInfo pi)
			{
				var mi = typeof(StaticAccessors<TEntity>).GetMethod("TypedGetPropertyFn");
				var genericMi = mi.MakeGenericMethod(pi.PropertyType);
				var typedGetPropertyFn = (Delegate)genericMi.Invoke(null, new[] { pi });
				return x => typedGetPropertyFn.Method.Invoke(x, new object[] { });
			}

            /// <summary>Value un typed get property type function expression.</summary>
            ///
            /// <param name="pi">The pi.</param>
            ///
            /// <returns>A Func&lt;TEntity,object&gt;</returns>
			public static Func<TEntity, object> ValueUnTypedGetPropertyTypeFn_Expr(PropertyInfo pi)
			{
				var mi = typeof(StaticAccessors<TEntity>).GetMethod("TypedGetPropertyFn");
				var genericMi = mi.MakeGenericMethod(pi.PropertyType);
				var typedGetPropertyFn = (Delegate)genericMi.Invoke(null, new[] { pi });

				var typedMi = typedGetPropertyFn.Method;
				var obj = Expression.Parameter(typeof(object), "oFunc");
				var expr = Expression.Lambda<Func<TEntity, object>>(
						Expression.Convert(
							Expression.Call(
								Expression.Convert(obj, typedMi.DeclaringType),
								typedMi
							),
							typeof(object)
						),
						obj
					);
				return expr.Compile();
			}


			/// <summary>
			/// Func to set the Strongly-typed field
			/// </summary>
			public static Action<TEntity, TId> TypedSetPropertyFn<TId>(PropertyInfo pi)
			{
				var mi = pi.GetSetMethod();
				return (Action<TEntity, TId>)Delegate.CreateDelegate(typeof(Action<TEntity, TId>), mi);
			}

			/// <summary>
			/// Required to cast the ValueType to an object for caching
			/// </summary>
			public static Action<TEntity, object> ValueUnTypedSetPropertyFn<TId>(PropertyInfo pi)
			{
				var typedPropertyFn = TypedSetPropertyFn<TId>(pi);
				return (x, y) => typedPropertyFn(x, (TId)y);
			}

            /// <summary>Value un typed set property type function reflection.</summary>
            ///
            /// <param name="pi">The pi.</param>
            ///
            /// <returns>An Action&lt;TEntity,object&gt;</returns>
			public static Action<TEntity, object> ValueUnTypedSetPropertyTypeFn_Reflection(PropertyInfo pi)
			{
				var mi = typeof (StaticAccessors<TEntity>).GetMethod("TypedSetPropertyFn");
				var genericMi = mi.MakeGenericMethod(pi.PropertyType);
				var typedSetPropertyFn = (Delegate) genericMi.Invoke(null, new[] {pi});

				return (x, y) => typedSetPropertyFn.Method.Invoke(x, new[] { y });
			}

            /// <summary>Value un typed set property type function expression.</summary>
            ///
            /// <param name="pi">The pi.</param>
            ///
            /// <returns>An Action&lt;TEntity,object&gt;</returns>
			public static Action<TEntity, object> ValueUnTypedSetPropertyTypeFn_Expr(PropertyInfo pi)
			{
				var mi = typeof(StaticAccessors<TEntity>).GetMethod("TypedSetPropertyFn");
				var genericMi = mi.MakeGenericMethod(pi.PropertyType);
				var typedSetPropertyFn = (Delegate)genericMi.Invoke(null, new[] { pi });

				var typedMi = typedSetPropertyFn.Method;
				var paramFunc = Expression.Parameter(typeof(object), "oFunc");
				var paramValue = Expression.Parameter(typeof(object), "oValue");
				var expr = Expression.Lambda<Action<TEntity, object>>(
						Expression.Call(
							Expression.Convert(paramFunc, typedMi.DeclaringType),
							typedMi,
							Expression.Convert(paramValue, pi.PropertyType)
						),
						paramFunc,
						paramValue
					);
				return expr.Compile();
			}
		}

		private void CompareGet<T>(Func<T, object> reflection, Func<T, object> expr)
			where T : new()
		{
			var obj = new T();
			CompareMultipleRuns(
				"GET Reflection", () => reflection(obj),
				"GET Expression", () => expr(obj)
			);
		}

		private void CompareSet<T, TArg>(
			Action<T, object> reflection, Action<T, object> expr, TArg arg)
			where T : new()
		{
			var obj = new T();
			CompareMultipleRuns(
				"SET Reflection", () => reflection(obj, arg),
				"SET Expression", () => expr(obj, arg)
			);
		}

        /// <summary>Compare get int.</summary>
		[Test]
		public void Compare_get_int()
		{
			var fieldPi = typeof(ModelWithIdAndName).GetProperty("Id");
			CompareGet
				(
					TestAcessor<ModelWithIdAndName>.ValueUnTypedGetPropertyTypeFn_Reflection(fieldPi),
					TestAcessor<ModelWithIdAndName>.ValueUnTypedGetPropertyTypeFn_Expr(fieldPi)
				);
			CompareSet
				(
					TestAcessor<ModelWithIdAndName>.ValueUnTypedSetPropertyTypeFn_Reflection(fieldPi),
					TestAcessor<ModelWithIdAndName>.ValueUnTypedSetPropertyTypeFn_Expr(fieldPi),
					1
				);
		}

        /// <summary>Compare get string.</summary>
		[Test]
		public void Compare_get_string()
		{
			var fieldPi = typeof(ModelWithIdAndName).GetProperty("Name");
			CompareGet
				(
					TestAcessor<ModelWithIdAndName>.ValueUnTypedGetPropertyTypeFn_Reflection(fieldPi),
					TestAcessor<ModelWithIdAndName>.ValueUnTypedGetPropertyTypeFn_Expr(fieldPi)
				);
			
			CompareSet
				(
					TestAcessor<ModelWithIdAndName>.ValueUnTypedSetPropertyTypeFn_Reflection(fieldPi),
					TestAcessor<ModelWithIdAndName>.ValueUnTypedSetPropertyTypeFn_Expr(fieldPi),
					"A"
				);
		}
	}
}