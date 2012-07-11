namespace Ach.Fulfillment.Persistence.Impl
{
    using System;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Common.Transactions;

    using NHibernate;

    internal class TransactionManager : ITransactionManager
    {
        #region Fields

        private readonly Func<ISession> sessionProducer;

        #endregion

        #region Constructors

        public TransactionManager(Func<ISession> sessionProducer)
        {
            Contract.Assert(sessionProducer != null);
            this.sessionProducer = sessionProducer;
        }

        #endregion

        #region Methods

        public IDisposable BeginTransaction()
        {
            return this.sessionProducer().BeginTransaction();
        }

        public void CommitTransaction(IDisposable transactionToken)
        {
            Contract.Assert(transactionToken != null);
            var transaction = transactionToken as ITransaction;
            Contract.Assert(transaction != null);
            transaction.Commit();
        }

        public void RollbackTransaction(IDisposable transactionToken)
        {
            Contract.Assert(transactionToken != null);
            var transaction = transactionToken as ITransaction;
            Contract.Assert(transaction != null);
            transaction.Rollback();
        }

        #endregion
    }
}