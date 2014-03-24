using System;

namespace NServiceKit.DataAccess
{
    public interface ITransactionContext : IDisposable
    {
        bool Commit();
        bool Rollback();
    }
}