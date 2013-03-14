namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System;
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Business.Impl.Configuration;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    internal class AchFileStatusCheckProcessor : BaseAchFileRetryingProcessor<ReadyToBeAcceptedAchFileReference>
    {
        #region Fields

        private readonly IAchFileManager achFileManager;

        private readonly Func<string, IRemoteAccessManager> remoteAccessManagerBuiler;

        #endregion

        #region Constructors and Destructors

        public AchFileStatusCheckProcessor(
            IQueue queue, IRepository repository, IAchFileManager achFileManager, Func<string, IRemoteAccessManager> remoteAccessManagerBuiler)
            : base(queue, repository, MetadataInfo.RepeatIntervalForResponseCheck)
        {
            Contract.Assert(achFileManager != null);
            Contract.Assert(remoteAccessManagerBuiler != null);
            this.achFileManager = achFileManager;
            this.remoteAccessManagerBuiler = remoteAccessManagerBuiler;
        }

        #endregion

        #region Methods

        protected override bool ProcessCore(RetryReferenceEntity reference, AchFileEntity achFile)
        {
            Contract.Assert(achFile != null);

            var status = AchFileStatus.None;
            try
            {
                status = this.remoteAccessManagerBuiler.GetStatus(achFile);
                Contract.Assert(status == AchFileStatus.None || status == AchFileStatus.Accepted || status == AchFileStatus.Rejected);
            }
            catch (Exception ex)
            {
                var rethrow = ExceptionPolicy.HandleException(ex, BusinessContainerExtension.OperationStatusCheckPolicy);
                if (rethrow)
                {
                    throw;
                }
            }

            var complete = status != AchFileStatus.None;
            if (complete)
            {
                Contract.Assert(status == AchFileStatus.Accepted || status == AchFileStatus.Rejected);
                this.achFileManager.UpdateStatus(achFile, status);
            }

            return complete;
        }

        #endregion
    }
}