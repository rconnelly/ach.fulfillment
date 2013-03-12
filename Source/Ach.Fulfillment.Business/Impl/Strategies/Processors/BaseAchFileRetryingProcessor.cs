namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Persistence;

    public abstract class BaseAchFileRetryingProcessor<TActionData> : BaseRetryingProcessor<TActionData, RetryReferenceEntity>
        where TActionData : IQueueQueryData<RetryReferenceEntity>, new()
    {
        protected BaseAchFileRetryingProcessor(IQueue queue, IRepository repository, TimeSpan timeout)
            : base(queue, repository, timeout)
        {
        }

        protected override void ProcessCore(RetryReferenceEntity reference)
        {
            Contract.Assert(reference != null);
            var achFile = this.Repository.Get<AchFileEntity>(reference.Id);

            // todo: looks the same with AchEnumerators + ForEach
            if (achFile != null)
            {
                this.ProcessCore(reference, achFile);
            }
            else
            {
                this.Logger.WarnFormat(CultureInfo.InvariantCulture, "AchFile '{0}' record cannot be found. Skipping operation.", reference.Id);
            }
        }

        protected abstract void ProcessCore(RetryReferenceEntity reference, AchFileEntity achFile);
    }
}