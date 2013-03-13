namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchFiles;
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    internal class AchFileGenerateProcessor : BaseAchFileRetryingProcessor<ReadyToGenerateAchFileReference>
    {
        #region Fields

        private const int DefaultFileGenerateRepeatDelay = 60;

        private readonly IAchFileManager manager;

        #endregion

        #region Constructors and Destructors

        public AchFileGenerateProcessor(IQueue queue, IRepository repository, IAchFileManager manager)
            : base(queue, repository, TimeSpan.FromSeconds(DefaultFileGenerateRepeatDelay))
        {
            Contract.Assert(manager != null);
            this.manager = manager;
        }

        #endregion

        #region Methods

        protected override void ProcessCore(RetryReferenceEntity reference, AchFileEntity achFile)
        {
            Contract.Assert(achFile != null);

            var actionData = new CreateAchFileContent
                                 {
                                     AchFileId = achFile.Id,
                                     WriteTo = stream => Serialize(achFile, stream)
                                 };
            this.Repository.Execute(actionData);
            this.manager.UpdateStatus(achFile, AchFileStatus.Generated);
        }

        private static void Serialize(AchFileEntity achFile, Stream stream)
        {
            // todo: use ehab here to wrap necessary exceptions into BusinessException
            // todo: do not use achFile.Transactions because of performance
            var achTransactionEntities = achFile.Transactions;
            achFile.ToStream(achTransactionEntities, stream);
        }

        #endregion
    }
}