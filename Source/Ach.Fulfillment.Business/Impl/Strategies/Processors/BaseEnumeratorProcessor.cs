namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Persistence;

    using global::Common.Logging;

    public abstract class BaseEnumeratorProcessor
    {
        #region Static Fields

        protected readonly ILog Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        protected BaseEnumeratorProcessor(IQueue queue, IRepository repository)
        {
            Contract.Assert(queue != null);
            Contract.Assert(repository != null);
            this.Repository = repository;
            this.Queue = queue;
        }

        #endregion

        #region Properties

        protected IQueue Queue { get; set; }

        protected IRepository Repository { get; private set; }

        #endregion

        #region Methods

        public void Execute()
        {
            using (var enumerator = this.CreateEnumerator())
            {
                this.Execute(enumerator);
            }
        }

        protected abstract IEnumerator<AchFileEntity> CreateEnumerator();

        protected void Execute(IEnumerator<AchFileEntity> enumerator)
        {
            Contract.Assert(enumerator != null);
            bool fetched;
            do
            {
                // enumerator.MoveNext() should be started in transaction
                using (var transaction = new Transaction())
                {
                    fetched = enumerator.MoveNext();
                    if (fetched)
                    {
                        var achFile = enumerator.Current;
                        if (achFile != null)
                        {
                            this.ExecuteCore(achFile);
                        }
                    }

                    // it is important to commit result of enumerator.MoveNext even when no processing occured
                    // brecause enumerator.MoveNext reads item from queue
                    // and we should commit otherwise item will be returned to queue by rollback
                    transaction.Complete(true);
                }
            }
            while (fetched);
        }

        protected abstract void ExecuteCore(AchFileEntity achFile);

        #endregion
    }
}