namespace Ach.Fulfillment.Business.Impl
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    using global::Common.Logging;

    public static class RetryingProcessingStrategyExtension
    {
        #region Static Fields

        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods and Operators

        public static bool ExecuteWithRetry<TActionData>(
            this IRepository repository, Action<AchFileEntity> action, TimeSpan timeout)
            where TActionData : IQueryData<RetryReferenceEntity>, new()
        {
            var processed = repository.ExecuteWithRetry<TActionData, RetryReferenceEntity>(action, timeout);
            return processed;
        }

        public static bool ExecuteWithRetry<TActionData, TReference>(
            this IRepository repository, Action<AchFileEntity> action, TimeSpan timeout)
            where TReference : class, IRetryReferenceEntity where TActionData : IQueryData<TReference>, new()
        {
            Action<long> internalAction = id => ProcessAchFile(repository, action, id);
            var processed = repository.ExecuteWithRetry<TActionData, TReference>(internalAction, timeout);
            return processed;
        }

        public static bool ExecuteWithRetry<TActionData, TReference>(this IRepository repository, Action<long> action, TimeSpan timeout)
            where TReference : class, IRetryReferenceEntity
            where TActionData : IQueryData<TReference>, new()
        {
            Contract.Assert(repository != null);
            Contract.Assert(action != null);
            var processed = false;
            using (var transaction = new Transaction())
            {
                var reference = repository.FindOne(new TActionData());
                if (reference != null)
                {
                    Process(repository, reference, action, timeout);
                    processed = true;
                }

                transaction.Complete();
            }

            return processed;
        }

        #endregion

        #region Methods

        private static void ProcessAchFile(IRepository repository, Action<AchFileEntity> action, long achFileId)
        {
            Contract.Assert(repository != null);
            Contract.Assert(action != null);

            var achFile = repository.Get<AchFileEntity>(achFileId);

            if (achFile != null)
            {
                action(achFile);
            }
            else
            {
                Logger.WarnFormat(CultureInfo.InvariantCulture, "AchFile '{0}' record cannot be found. Skipping operation.", achFileId);
            }
        }

        private static void Process(IRepository repository, IRetryReferenceEntity reference, Action<long> action, TimeSpan timeout)
        {
            Contract.Assert(repository != null);
            Contract.Assert(reference != null);
            Contract.Assert(action != null);

            var requiresCompletion = true;
            try
            {
                action(reference.Id);
            }
            catch (RootException ex)
            {
                var rescheduled = RescheduleNotification(repository, reference, timeout);
                var message = rescheduled
                                  ? "Unable to complete operation. Reschedule declined."
                                  : "Unable to complete operation. Rescheduled.";
                Logger.WarnFormat(CultureInfo.InvariantCulture, message, ex);
                requiresCompletion = !rescheduled;
            }

            if (requiresCompletion)
            {
                CompleteNotification(repository, reference);
            }
        }

        private static bool RescheduleNotification(IRepository repository, IRetryReferenceEntity reference, TimeSpan timeout)
        {
            repository.Execute(new RescheduleConversation { Handle = reference.Handle, Timeout = timeout });
            
            // always reschedule
            return true;
        }

        private static void CompleteNotification(IRepository repository, IRetryReferenceEntity reference)
        {
            repository.Execute(new EndConversation { Handle = reference.Handle });
        }

        #endregion
    }
}