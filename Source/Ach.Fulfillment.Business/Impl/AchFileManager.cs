namespace Ach.Fulfillment.Business.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    using Ach.Fulfillment.Business.Impl.Strategies.Enumerators;
    using Ach.Fulfillment.Business.Impl.Strategies.Processors;
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchFiles;
    using Ach.Fulfillment.Data.Specifications.AchTransactions;
    using Ach.Fulfillment.Persistence;

    using Microsoft.Practices.Unity;

    using Renci.SshNet;

    public class AchFileManager : ManagerBase<AchFileEntity>, IAchFileManager
    {
        #region Fields

        private const int BulkCreationLimit = 100 * 1000;

        #endregion

        #region Public Properties

        [Dependency]
        public IPartnerManager PartnerManager { get; set; }

        [Dependency]
        public IApplicationEventRaiseManager ApplicationEventRaiseManager { get; set; }

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
            using (var tr = new Transaction())
            {
                // todo: rewrite method to use transactionReferences one-by-one without loading all records

                // get all available transactionReferences
                using (var transactionReferences = new ReadyToBeGroupedAchTransactionReferenceEnumerator(this.Queue, BulkCreationLimit))
                {
                    // create ach file for each partner and group transactions
                    var grouped = from r in transactionReferences
                                  group r by r.PartnerId
                                  into grps 
                                  let partner = this.Repository.Get<PartnerEntity>(grps.Key)
                                  where partner != null 
                                  let file = this.Create(partner)
                                  select new { File = file, References = grps };

                    foreach (var g in grouped)
                    {
                        var achFile = g.File;

                        // insert ach transactions into corresponding ach files
                        // todo: is it good idea to fill all items into list?
                        achFile.Transactions = new List<AchTransactionEntity>(
                            from reference in g.References
                            let id = reference.Id
                            let achTransaction = this.Repository.LazyLoad<AchTransactionEntity>(id)
                            select achTransaction);

                        this.UpdateStatus(achFile, AchFileStatus.Created);
                    }
                }

                tr.Complete();
            }
        }

        public void ProcessReadyToBeGeneratedAchFile()
        {
            var processor = new AchFileGenerateProcessor(this.Queue, this.Repository, this);
            processor.Execute();
        }

        public void ProcessReadyToBeUploadedAchFile(PasswordConnectionInfo connectionInfo)
        {
            var processor = new AchFileUploadProcessor(this.Queue, this.Repository, this, connectionInfo);
            processor.Execute();
        }

        public void ProcessReadyToBeAcceptedAchFile()
        {
            var processor = new AchFileStatusCheckProcessor(this.Queue, this.Repository, this);
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