namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    using global::Common.Logging;

    public abstract class BaseRetryingProcessor<TActionData, TReference>
        where TReference : class, IRetryReferenceEntity
        where TActionData : IQueueQueryData<TReference>, new()
    {
        #region Static Fields

        protected readonly ILog Logger = LogManager.GetCurrentClassLogger();

        private readonly TimeSpan timeout;

        #endregion

        #region Constructors

        protected BaseRetryingProcessor(IQueue queue, IRepository repository, TimeSpan timeout)
        {
            Contract.Assert(queue != null);
            Contract.Assert(repository != null);
            this.Repository = repository;
            this.Queue = queue;
            this.timeout = timeout;
        }

        #endregion

        #region Properties

        protected IQueue Queue { get; set; }

        protected IRepository Repository { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void Execute()
        {
            var fetched = false;
            do
            {
                using (var transaction = new Transaction())
                {
                    var reference = this.Queue.Dequeue(new TActionData());
                    if (reference != null)
                    {
                        this.Process(reference);
                        fetched = true;
                    }

                    transaction.Complete(true);
                }
            }
            while (fetched);
        }

        #endregion

        #region Methods

        protected void Process(TReference reference)
        {
            Contract.Assert(reference != null);

            var requiresCompletion = true;
            try
            {
                this.ProcessCore(reference);
            }
            catch (BusinessException ex)
            {
                var rescheduled = this.Reschedule(reference);
                var message = rescheduled
                                  ? "Unable to complete operation. Reschedule declined."
                                  : "Unable to complete operation. Rescheduled.";
                this.Logger.WarnFormat(CultureInfo.InvariantCulture, message, ex);
                requiresCompletion = !rescheduled;
            }

            if (requiresCompletion)
            {
                this.Complete(reference);
            }
        }

        protected abstract void ProcessCore(TReference reference);

        protected virtual bool Reschedule(TReference reference)
        {
            var actionData = new RescheduleConversation { Handle = reference.Handle, Timeout = this.timeout };
            this.Repository.Execute(actionData);

            // always reschedule
            return true;
        }

        protected void Complete(TReference reference)
        {
            var actionData = new EndConversation { Handle = reference.Handle };
            this.Repository.Execute(actionData);
        }

        #endregion
    }
}