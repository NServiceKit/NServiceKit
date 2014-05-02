/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 *
 * This software is subject to the Microsoft Public License (Ms-PL). 
 * A copy of the license can be found in the license.htm file included 
 * in this distribution.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;

namespace NServiceKit.Html
{
	internal delegate bool TryGetValueDelegate(object dictionary, string key, out object value);

	internal static class TypeHelpers
	{
		private static readonly Dictionary<Type, TryGetValueDelegate> _tryGetValueDelegateCache = new Dictionary<Type, TryGetValueDelegate>();
		private static readonly ReaderWriterLockSlim _tryGetValueDelegateCacheLock = new ReaderWriterLockSlim();

		private static readonly MethodInfo _strongTryGetValueImplInfo = typeof(TypeHelpers).GetMethod("StrongTryGetValueImpl", BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>The milliseconds cor library assembly.</summary>
		public static readonly Assembly MsCorLibAssembly = typeof(string).Assembly;
		//public static readonly Assembly MvcAssembly = typeof(Controller).Assembly;
        /// <summary>The system web assembly.</summary>
		public static readonly Assembly SystemWebAssembly = typeof(HttpContext).Assembly;

		// method is used primarily for lighting up new .NET Framework features even if MVC targets the previous version
		// thisParameter is the 'this' parameter if target method is instance method, should be null for static method
		public static TDelegate CreateDelegate<TDelegate>(Assembly assembly, string typeName, string methodName, object thisParameter) where TDelegate : class
		{
			// ensure target type exists
			Type targetType = assembly.GetType(typeName, false /* throwOnError */);
			if (targetType == null)
			{
				return null;
			}

			return CreateDelegate<TDelegate>(targetType, methodName, thisParameter);
		}

        /// <summary>Creates a delegate.</summary>
        ///
        /// <typeparam name="TDelegate">Type of the delegate.</typeparam>
        /// <param name="targetType">   Type of the target.</param>
        /// <param name="methodName">   Name of the method.</param>
        /// <param name="thisParameter">this parameter.</param>
        ///
        /// <returns>The new delegate.</returns>
		public static TDelegate CreateDelegate<TDelegate>(Type targetType, string methodName, object thisParameter) where TDelegate : class
		{
			// ensure target method exists
			ParameterInfo[] delegateParameters = typeof(TDelegate).GetMethod("Invoke").GetParameters();
			Type[] argumentTypes = Array.ConvertAll(delegateParameters, pInfo => pInfo.ParameterType);
			MethodInfo targetMethod = targetType.GetMethod(methodName, argumentTypes);
			if (targetMethod == null)
			{
				return null;
			}

			TDelegate d = Delegate.CreateDelegate(typeof(TDelegate), thisParameter, targetMethod, false /* throwOnBindFailure */) as TDelegate;
			return d;
		}

        /// <summary>Creates try get value delegate.</summary>
        ///
        /// <param name="targetType">Type of the target.</param>
        ///
        /// <returns>The new try get value delegate.</returns>
		public static TryGetValueDelegate CreateTryGetValueDelegate(Type targetType)
		{
			TryGetValueDelegate result;

			_tryGetValueDelegateCacheLock.EnterReadLock();
			try
			{
				if (_tryGetValueDelegateCache.TryGetValue(targetType, out result))
				{
					return result;
				}
			}
			finally
			{
				_tryGetValueDelegateCacheLock.ExitReadLock();
			}

			Type dictionaryType = ExtractGenericInterface(targetType, typeof(IDictionary<,>));

			// just wrap a call to the underlying IDictionary<TKey, TValue>.TryGetValue() where string can be cast to TKey
			if (dictionaryType != null)
			{
				Type[] typeArguments = dictionaryType.GetGenericArguments();
				Type keyType = typeArguments[0];
				Type returnType = typeArguments[1];

				if (keyType.IsAssignableFrom(typeof(string)))
				{
					MethodInfo strongImplInfo = _strongTryGetValueImplInfo.MakeGenericMethod(keyType, returnType);
					result = (TryGetValueDelegate)Delegate.CreateDelegate(typeof(TryGetValueDelegate), strongImplInfo);
				}
			}

			// wrap a call to the underlying IDictionary.Item()
			if (result == null && typeof(IDictionary).IsAssignableFrom(targetType))
			{
				result = TryGetValueFromNonGenericDictionary;
			}

			_tryGetValueDelegateCacheLock.EnterWriteLock();
			try
			{
				_tryGetValueDelegateCache[targetType] = result;
			}
			finally
			{
				_tryGetValueDelegateCacheLock.ExitWriteLock();
			}

			return result;
		}

        /// <summary>Extracts the generic interface.</summary>
        ///
        /// <param name="queryType">    Type of the query.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        ///
        /// <returns>The extracted generic interface.</returns>
		public static Type ExtractGenericInterface(Type queryType, Type interfaceType)
		{
			Func<Type, bool> matchesInterface = t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType;
			return (matchesInterface(queryType)) ? queryType : queryType.GetInterfaces().FirstOrDefault(matchesInterface);
		}

        /// <summary>Gets default value.</summary>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>The default value.</returns>
		public static object GetDefaultValue(Type type)
		{
			return (TypeAllowsNullValue(type)) ? null : Activator.CreateInstance(type);
		}

        /// <summary>Query if 'value' is compatible object.</summary>
        ///
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="value">The value.</param>
        ///
        /// <returns>true if compatible object, false if not.</returns>
		public static bool IsCompatibleObject<T>(object value)
		{
			return (value is T || (value == null && TypeAllowsNullValue(typeof(T))));
		}

        /// <summary>Query if 'type' is nullable value type.</summary>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>true if nullable value type, false if not.</returns>
		public static bool IsNullableValueType(Type type)
		{
			return Nullable.GetUnderlyingType(type) != null;
		}

		private static bool StrongTryGetValueImpl<TKey, TValue>(object dictionary, string key, out object value)
		{
			IDictionary<TKey, TValue> strongDict = (IDictionary<TKey, TValue>)dictionary;

			TValue strongValue;
			bool retVal = strongDict.TryGetValue((TKey)(object)key, out strongValue);
			value = strongValue;
			return retVal;
		}

		private static bool TryGetValueFromNonGenericDictionary(object dictionary, string key, out object value)
		{
			IDictionary weakDict = (IDictionary)dictionary;

			bool containsKey = weakDict.Contains(key);
			value = (containsKey) ? weakDict[key] : null;
			return containsKey;
		}

        /// <summary>Type allows null value.</summary>
        ///
        /// <param name="type">The type.</param>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
		public static bool TypeAllowsNullValue(Type type)
		{
			return (!type.IsValueType || IsNullableValueType(type));
		}
	}
}
