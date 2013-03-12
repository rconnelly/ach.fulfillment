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

        #endregion

        #region Methods

        private AchFileEntity Create(PartnerEntity partner, IList<AchTransactionEntity> transactionEntities = null)
        {
            Contract.Assert(partner != null);

            // todo: possible concurrency issue. why we need Name at all
            // todo: generate filename before upload
            var newFileName = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var fileEntity = new AchFileEntity
            {
                Name = newFileName,
                FileStatus = AchFileStatus.Created,
                Partner = partner,
                Transactions = transactionEntities ?? new List<AchTransactionEntity>(),
                FileIdModifier = this.GetNextIdModifier(partner)
            };

            return this.Create(fileEntity);
        }

        private string GetNextIdModifier(PartnerEntity partner)
        {
            var fileIdModifier =
                this.Repository.Query(new AchFileForPartner(partner))
                    .Where(m => m.Created.Date == DateTime.Today.Date)
                    .Max(m => m.FileIdModifier);

            var result = 'A';

            if (!string.IsNullOrEmpty(fileIdModifier) && !string.Equals(fileIdModifier, "Z", StringComparison.InvariantCultureIgnoreCase))
            {
                var prev = fileIdModifier.First();
                result = (char)(prev + 1);
            }

            return result.ToString(CultureInfo.InvariantCulture);
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