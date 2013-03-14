namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Persistence;

    internal abstract class BaseAchFileRetryingProcessor<TActionData> :
        BaseRetryingProcessor<TActionData, RetryReferenceEntity>
        where TActionData : IQueueQueryData<RetryReferenceEntity>, new()
    {
        #region Constructors and Destructors

        protected BaseAchFileRetryingProcessor(IQueue queue, IRepository repository, TimeSpan timeout)
            : base(queue, repository, timeout)
        {
        }

        #endregion

        #region Methods

        protected override bool ProcessCore(RetryReferenceEntity reference)
        {
            Contract.Assert(reference != null);

            var result = true;
            var achFile = this.Repository.Get<AchFileEntity>(reference.Id);
            if (achFile != null)
            {
                this.Logger.DebugFormat(
                    CultureInfo.InvariantCulture, 
                    "Retrieved '{0}' to be processed by '{1}'", 
                    achFile, 
                    this.GetType().Name);

                result = this.ProcessCore(reference, achFile);

                this.Logger.InfoFormat(
                    CultureInfo.InvariantCulture, 
                    "'{0}' successfully processed by '{1}'", 
                    achFile, 
                    this.GetType().Name);
            }
            else
            {
                this.Logger.WarnFormat(
                    CultureInfo.InvariantCulture, 
                    "AchFile '{0}' record cannot be found. Operation '{1}' skipped.", 
                    reference.Id, 
                    this.GetType().Name);
            }

            return result;
        }

        protected abstract bool ProcessCore(RetryReferenceEntity reference, AchFileEntity achFile);

        #endregion
    }
}