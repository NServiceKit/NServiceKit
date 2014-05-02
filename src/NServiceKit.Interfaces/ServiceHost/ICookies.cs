using System;
using System.Net;

namespace NServiceKit.ServiceHost
{
    /// <summary>Interface for cookies.</summary>
    public interface ICookies
    {
        /// <summary>Deletes the cookie described by cookieName.</summary>
        ///
        /// <param name="cookieName">Name of the cookie.</param>
        void DeleteCookie(string cookieName);

        /// <summary>Adds a cookie.</summary>
        ///
        /// <param name="cookie">The cookie.</param>
        void AddCookie(Cookie cookie);

        /// <summary>Adds a permanent cookie.</summary>
        ///
        /// <param name="cookieName"> Name of the cookie.</param>
        /// <param name="cookieValue">The cookie value.</param>
        /// <param name="secureOnly"> The secure only.</param>
        void AddPermanentCookie(string cookieName, string cookieValue, bool? secureOnly = null);

        /// <summary>Adds a session cookie.</summary>
        ///
        /// <param name="cookieName"> Name of the cookie.</param>
        /// <param name="cookieValue">The cookie value.</param>
        /// <param name="secureOnly"> The secure only.</param>
        void AddSessionCookie(string cookieName, string cookieValue, bool? secureOnly = null);
    }
}