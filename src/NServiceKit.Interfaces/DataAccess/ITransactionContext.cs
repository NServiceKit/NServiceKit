using System;

namespace NServiceKit.DataAccess
{
    /// <summary>Interface for transaction context.</summary>
    public interface ITransactionContext : IDisposable
    {
        /// <summary>Commits this object.</summary>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool Commit();

        /// <summary>Rollbacks this object.</summary>
        ///
        /// <returns>true if it succeeds, false if it fails.</returns>
        bool Rollback();
    }
}