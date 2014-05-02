using System;
using NServiceKit.Messaging;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for service runner.</summary>
    public interface IServiceRunner
    {
        /// <summary>Process this object.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="instance">      The instance.</param>
        /// <param name="request">       The request.</param>
        ///
        /// <returns>An object.</returns>
        object Process(IRequestContext requestContext, object instance, object request);
    }

    /// <summary>Interface for service runner.</summary>
    ///
    /// <typeparam name="TRequest">Type of the request.</typeparam>
    public interface IServiceRunner<TRequest> : IServiceRunner
    {
        /// <summary>Executes the before execute action.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="request">       The request.</param>
        void OnBeforeExecute(IRequestContext requestContext, TRequest request);

        /// <summary>Executes the after execute action.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="response">      The response.</param>
        ///
        /// <returns>An object.</returns>
        object OnAfterExecute(IRequestContext requestContext, object response);

        /// <summary>Handles the exception.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="request">       The request.</param>
        /// <param name="ex">            The ex.</param>
        ///
        /// <returns>An object.</returns>
        object HandleException(IRequestContext requestContext, TRequest request, Exception ex);

        /// <summary>Executes.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="instance">      The instance.</param>
        /// <param name="request">       The request.</param>
        ///
        /// <returns>An object.</returns>
        object Execute(IRequestContext requestContext, object instance, TRequest request);

        /// <summary>Executes.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="instance">      The instance.</param>
        /// <param name="request">       The request.</param>
        ///
        /// <returns>An object.</returns>
        object Execute(IRequestContext requestContext, object instance, IMessage<TRequest> request);

        /// <summary>Executes the one way operation.</summary>
        ///
        /// <param name="requestContext">Context for the request.</param>
        /// <param name="instance">      The instance.</param>
        /// <param name="request">       The request.</param>
        ///
        /// <returns>An object.</returns>
        object ExecuteOneWay(IRequestContext requestContext, object instance, TRequest request);
    }
}