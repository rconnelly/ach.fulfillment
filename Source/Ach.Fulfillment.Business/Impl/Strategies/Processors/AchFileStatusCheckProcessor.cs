namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    public class AchFileStatusCheckProcessor :
        BaseAchFileRetryingProcessor<ReadyToBeAcceptedAchFileReference>
    {
        #region Fields

        private const int DefaultResponseCheckRepeatDelay = 60;

        private readonly IAchFileManager manager;

        #endregion

        #region Constructors and Destructors

        public AchFileStatusCheckProcessor(
            IQueue queue, IRepository repository, IAchFileManager manager)
            : base(queue, repository, TimeSpan.FromSeconds(DefaultResponseCheckRepeatDelay))
        {
            Contract.Assert(manager != null);
            this.manager = manager;
        }

        #endregion

        #region Methods

        protected override void ProcessCore(RetryReferenceEntity reference, AchFileEntity achFile)
        {
            Contract.Assert(achFile != null);

            // todo: implement it
            this.Logger.Warn("------------------------- CheckUploadedAchFileStatus is mock");

            this.manager.UpdateStatus(achFile, AchFileStatus.Accepted);
        }

        #endregion
    }
}