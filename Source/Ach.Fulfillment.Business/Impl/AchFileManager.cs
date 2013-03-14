namespace Ach.Fulfillment.Business.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    using Ach.Fulfillment.Business.Impl.Strategies;
    using Ach.Fulfillment.Business.Impl.Strategies.Processors;
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchFiles;
    using Ach.Fulfillment.Data.Specifications.AchTransactions;
    using Ach.Fulfillment.Data.Specifications.Partners;
    using Ach.Fulfillment.Persistence;

    using global::Common.Logging;

    using Microsoft.Practices.Unity;

    internal class AchFileManager : ManagerBase<AchFileEntity>, IAchFileManager
    {
        #region Fields

        private static readonly ILog Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Properties

        [Dependency]
        public IPartnerManager PartnerManager { get; set; }

        [Dependency]
        public IApplicationEventRaiseManager ApplicationEventRaiseManager { get; set; }

        [Dependency]
        public Func<string, IRemoteAccessManager> RemoteAccessManagerBuilder { get; set; }

        [Dependency]
        public IQueue Queue { get; set; }

        #endregion

        #region Public Methods and Operators

        public void UpdateStatus(AchFileEntity file, AchFileStatus status)
        {
            Contract.Assert(file != null);

            using (var tr = new Transaction())
            {
                file.FileStatus = status;
                this.Update(file, true);

                var childStatus = status.ToAchTransactionStatus();
                if (childStatus.HasValue)
                {
                    this.UpdateChildrenStatuses(file, childStatus.Value);
                }

                this.ApplicationEventRaiseManager.RaiseAchFileStatusChangedNotification(file);

                tr.Complete();
            }
        }

        public void ProcessReadyToBeGroupedAchTransactions()
        {
            Logger.Info("Started ach transaction grouping process.");

            var partners = this.Repository.FindAll(new PartnerAll());
            foreach (var partner in partners)
            {
                var count = this.Repository.Count(new UngroupedAchTransactionByPartnerId { PartnerId = partner.Id });
                if (count > 0)
                {
                    using (var tr = new Transaction())
                    {
                        var achFile = this.Create(partner);

                        var actionData = new GroupAchTransactionToAchFile { AchFileId = achFile.Id };
                        this.Repository.Execute(actionData);

                        if (actionData.AffectedAchTransactionsCount > 0)
                        {
                            this.UpdateStatus(achFile, AchFileStatus.Created);
                            tr.Complete();

                            Logger.InfoFormat(CultureInfo.InvariantCulture, "Created '{0}'", achFile);
                        }
                    }
                }
            }

            Logger.Info("Finished ach transaction grouping process.");
        }

        public void ProcessReadyToBeGeneratedAchFile()
        {
            var processor = new AchFileGenerateProcessor(this.Queue, this.Repository, this);
            processor.Execute();
        }

        public void ProcessReadyToBeUploadedAchFile()
        {
            var processor = new AchFileUploadProcessor(this.Queue, this.Repository, this, this.RemoteAccessManagerBuilder);
            processor.Execute();
        }

        public void ProcessReadyToBeAcceptedAchFile()
        {
            var processor = new AchFileStatusCheckProcessor(this.Queue, this.Repository, this, this.RemoteAccessManagerBuilder);
            processor.Execute();
        }

        public void Cleanup()
        {
            this.Repository.Execute(new DeleteCompletedAchFiles());
        }

        #endregion

        #region Methods

        private AchFileEntity Create(PartnerEntity partner, IList<AchTransactionEntity> transactionEntities = null)
        {
            Contract.Assert(partner != null);

            var name = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var fileIdModifier = this.GetNextIdModifier(partner);
            var fileEntity = new AchFileEntity
            {
                Name = name,
                FileIdModifier = fileIdModifier,
                Partner = partner,
                Transactions = transactionEntities ?? new List<AchTransactionEntity>(),
                FileStatus = AchFileStatus.Created
            };

            return this.Create(fileEntity);
        }

        private string GetNextIdModifier(PartnerEntity partner)
        {
            var previousIdModifier = 
                this.Repository
                    .Query(new AchFileForPartner(partner))
                    .Where(m => m.Created.Date == DateTime.UtcNow.Date)
                    .Max(m => m.FileIdModifier);

            var previous = (previousIdModifier ?? "Z").FirstOrDefault();
            var current = 'A';
            if (previous >= 'A' && previous < 'Z')
            {
                current = (char)(previous + 1);
            }

            var nextIdModifier = current.ToString(CultureInfo.InvariantCulture);

            return nextIdModifier;
        }

        private void UpdateChildrenStatuses(AchFileEntity achFile, AchTransactionStatus status)
        {
            Contract.Assert(achFile != null);
            this.Repository.Execute(
                new UpdateAchTransactionStatusByAchFileId
                {
                    AchFileId = achFile.Id,
                    Status = status
                });
        }

        #endregion
    }
}