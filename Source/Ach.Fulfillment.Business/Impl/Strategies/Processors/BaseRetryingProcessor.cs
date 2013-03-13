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

    internal abstract class BaseRetryingProcessor<TActionData, TReference>
        where TReference : class, IRetryReferenceEntity
        where TActionData : IQueueQueryData<TReference>, new()
    {
        #region Static Fields

        protected readonly ILog Logger = LogManager.GetCurrentClassLogger();

        private const int DefaultMaxOperationCount = 100;

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
            this.MaxOperationCount = DefaultMaxOperationCount;
        }

        #endregion

        #region Properties

        public int MaxOperationCount { get; set; }

        protected IQueue Queue { get; set; }

        protected IRepository Repository { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void Execute()
        {
            TReference reference;
            var operationCount = 0;
            do
            {
                using (var transaction = new Transaction())
                {
                    reference = this.Queue.Dequeue(new TActionData());
                    if (reference != null)
                    {
                        this.Process(reference);
                    }

                    // we should flush to not waste memory
                    transaction.Complete(true);
                }

                operationCount++;
            }
            while (this.ShouldRepeat(reference, operationCount));
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
                requiresCompletion = !this.ShouldBeRescheduled(reference);
                var message = requiresCompletion
                                  ? "Unable to complete operation. Reschedule declined."
                                  : "Unable to complete operation. Operation will be rescheduled.";
                this.Logger.WarnFormat(CultureInfo.InvariantCulture, message, ex);
            }

            if (requiresCompletion)
            {
                this.Complete(reference);
            }
            else
            {
                this.Reschedule(reference);
            }
        }

        protected virtual bool ShouldBeRescheduled(TReference reference)
        {
            // always reschedule
            return true;
        }

        protected abstract void ProcessCore(TReference reference);

        protected void Reschedule(TReference reference)
        {
            var actionData = new RescheduleConversation { Handle = reference.Handle, Timeout = this.timeout };
            this.Repository.Execute(actionData);
        }

        protected void Complete(TReference reference)
        {
            var actionData = new EndConversation { Handle = reference.Handle };
            this.Repository.Execute(actionData);
        }

        protected virtual bool ShouldRepeat(TReference reference, int operationCount)
        {
            var result = reference != null && operationCount < this.MaxOperationCount;
            return result;
        }

        #endregion
    }
}