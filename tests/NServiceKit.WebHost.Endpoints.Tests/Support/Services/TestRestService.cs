using System;
using System.ComponentModel.DataAnnotations;
using NServiceKit.Common;
using NServiceKit.DataAnnotations;
using NServiceKit.ServiceInterface;
using NServiceKit.Text;
using NServiceKit.ServiceHost;

namespace NServiceKit.WebHost.Endpoints.Tests.Support.Services
{
    /// <summary>A test rest service.</summary>
    /// <typeparam name="TRequest">Type of the request.</typeparam>
	public class TestRestService<TRequest> : IService
		where TRequest : class
	{
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Get(TRequest request)
		{
			return Run(request, ApplyTo.Get);
		}

        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Put(TRequest request)
		{
			return Run(request, ApplyTo.Put);
		}

        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Post(TRequest request)
		{
			return Run(request, ApplyTo.Post);
		}

        /// <summary>Patches the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Patch(TRequest request)
		{
			return Run(request, ApplyTo.Patch);
		}

        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Delete(TRequest request)
		{
			return Run(request, ApplyTo.Delete);
		}

        /// <summary>Heads the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Head(TRequest request)
		{
			return Run(request, ApplyTo.Head);
		}

        /// <summary>Options the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
		public object Options(TRequest request)
		{
			return Run(request, ApplyTo.Options);
		}

        /// <summary>Runs.</summary>
        ///
        /// <param name="request">The request.</param>
        /// <param name="method"> The method.</param>
        ///
        /// <returns>An object.</returns>
		protected virtual object Run(TRequest request, ApplyTo method)
		{
			return request.AsTypeString();
		}
	}

    /// <summary>An object extensions.</summary>
	public static class ObjectExtensions
	{
        /// <summary>An object extension method that converts a request to a type string.</summary>
        ///
        /// <param name="request">The request to act on.</param>
        ///
        /// <returns>A string.</returns>
		public static string AsTypeString(this object request)
		{
            return request.GetType().ToTypeString() + "\n" + request.Dump();
        }
	}
}
