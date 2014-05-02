using System;
using NServiceKit.Messaging;

namespace NServiceKit.ServiceHost
{
    /// <summary>
    /// Marker interfaces
    /// </summary>
    public interface IService { }

    /// <summary>Interface for return.</summary>
    public interface IReturn { }

    /// <summary>Interface for return.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IReturn<T> : IReturn { }
    /// <summary>Interface for return void.</summary>
    public interface IReturnVoid : IReturn { }

    /* Supported signatures: */
    //Not used or needed, here in-case someone wants to know what the correct signatures should be

    /// <summary>
    /// Interface for any.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAny<T>
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        object Any(T request);
    }

    /// <summary>Interface for get.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IGet<T>
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        ///
        /// <returns>An object.</returns>
        object Get(T request);
    }

    /// <summary>Interface for post.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IPost<T>
    {
        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        object Post(T request);
    }

    /// <summary>Interface for put.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IPut<T>
    {
        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request to put.</param>
        ///
        /// <returns>An object.</returns>
        object Put(T request);
    }

    /// <summary>Interface for delete.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IDelete<T>
    {
        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        ///
        /// <returns>An object.</returns>
        object Delete(T request);
    }

    /// <summary>Interface for patch.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IPatch<T>
    {
        /// <summary>Patches the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        object Patch(T request);
    }

    /// <summary>Interface for options.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IOptions<T>
    {
        /// <summary>Options the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        ///
        /// <returns>An object.</returns>
        object Options(T request);
    }

    /// <summary>Interface for any void.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IAnyVoid<T>
    {
        /// <summary>Anies the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        void Any(T request);
    }

    /// <summary>Interface for get void.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IGetVoid<T>
    {
        /// <summary>Gets the given request.</summary>
        ///
        /// <param name="request">The request to get.</param>
        void Get(T request);
    }

    /// <summary>Interface for post void.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IPostVoid<T>
    {
        /// <summary>Post this message.</summary>
        ///
        /// <param name="request">The request.</param>
        void Post(T request);
    }

    /// <summary>Interface for put void.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IPutVoid<T>
    {
        /// <summary>Puts the given request.</summary>
        ///
        /// <param name="request">The request to put.</param>
        void Put(T request);
    }

    /// <summary>Interface for delete void.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IDeleteVoid<T>
    {
        /// <summary>Deletes the given request.</summary>
        ///
        /// <param name="request">The request to delete.</param>
        void Delete(T request);
    }

    /// <summary>Interface for patch void.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IPatchVoid<T>
    {
        /// <summary>Patches the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        void Patch(T request);
    }

    /// <summary>Interface for options void.</summary>
    ///
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public interface IOptionsVoid<T>
    {
        /// <summary>Options the given request.</summary>
        ///
        /// <param name="request">The request.</param>
        void Options(T request);
    }
}