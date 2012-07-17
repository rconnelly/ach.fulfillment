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

        #region Properties

        public IDisposable Transaction
        {
            get
            {
                var transaction = this.sessionProducer().Transaction;
                return transaction != null && transaction.IsActive ? transaction : null;
            }
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
            this.sessionProducer().Flush();
            transaction.Commit();
        }

        public void RollbackTransaction(IDisposable transactionToken)
        {
            Contract.Assert(transactionToken != null);
            var transaction = transactionToken as ITransaction;
            Contract.Assert(transaction != null);
            this.sessionProducer().Flush();
            transaction.Rollback();
        }

        #endregion
    }
}