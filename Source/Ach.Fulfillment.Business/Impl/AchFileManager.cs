namespace Ach.Fulfillment.Business.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchFiles;
    using Ach.Fulfillment.Data.Specifications.AchTransactions;
    using Ach.Fulfillment.Data.Specifications.Notifications;

    using Microsoft.Practices.Unity;

    using Renci.SshNet;

    public class AchFileManager : ManagerBase<AchFileEntity>, IAchFileManager
    {
        #region Fields

        private const int BulkCreationLimit = 100 * 1000;

        private const int DefaultResponseCheckRepeatDelay = 5;

        #endregion

        #region Public Properties

        [Dependency]
        public IPartnerManager PartnerManager { get; set; }

        [Dependency]
        public INotificationManager NotificationManager { get; set; }

        #endregion

        #region Public Methods and Operators

        public void ProcessReadyToBeGroupedAchTransactions()
        {
            using (var tr = new Transaction())
            {
                // get all available transactionReferences
                var transactionReferences = this.NotificationManager
                    .GetNextReadyToBeGroupedAchTransactionReferences(BulkCreationLimit);

                // create ach file for each partner and group transactions
                var grouped = from r in transactionReferences
                              group r by r.PartnerId into grps
                              let partner = this.Repository.Get<PartnerEntity>(grps.Key)
                              where partner != null
                              let file = this.Create(partner)
                              select new
                              {
                                  File = file,
                                  References = grps
                              };

                foreach (var g in grouped)
                {
                    var achFile = g.File;

                    // insert ach transactions into corresponding ach files
                    achFile.Transactions = new List<AchTransactionEntity>(
                        from reference in g.References
                        let id = reference.Id
                        let achTransaction = this.Repository.LazyLoad<AchTransactionEntity>(id)
                        select achTransaction);

                    this.UpdateStatus(achFile, AchFileStatus.Created);
                }

                tr.Complete();
            }
        }

        public bool ProcessReadyToBeGeneratedAchFile()
        {
            bool fetched;
            using (var transaction = new Transaction())
            {
                AchFileEntity achFile;
                fetched = this.NotificationManager.TryGetNextReadyToGenerateAchFile(out achFile);
                if (achFile != null)
                {
                    var actionData = new CreateAchFileContent
                                         {
                                             AchFileId = achFile.Id,
                                             WriteTo = achFile.ToStream
                                         };
                    this.Repository.Execute(actionData);

                    this.UpdateStatus(achFile, AchFileStatus.Generated);
                }

                transaction.Complete();
            }

            return fetched;
        }

        public bool ProcessReadyToBeUploadedAchFile(PasswordConnectionInfo connectionInfo)
        {
            bool fetched;
            using (var transaction = new Transaction())
            {
                AchFileEntity achFile;
                fetched = this.NotificationManager.TryGetNextReadyToUploadAchFile(out achFile);
                if (achFile != null)
                {
                    using (var stream = this.Repository.Load(new AchFileContentById { AchFileId = achFile.Id }))
                    {
                        this.Upload(connectionInfo, achFile, stream);
                    }

                    this.UpdateStatus(achFile, AchFileStatus.Uploaded);
                }

                transaction.Complete();
            }

            return fetched;
        }

        public bool ProcessReadyToBeAcceptedAchFile()
        {
            var processed = this.Repository
                .ExecuteWithRetry<ReadyToBeAcceptedAchFileReference>(
                this.CheckUploadedAchFileStatus, TimeSpan.FromSeconds(DefaultResponseCheckRepeatDelay));
            return processed;
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

        private void UpdateStatus(AchFileEntity file, AchFileStatus status)
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

                this.NotificationManager.RaiseAchFileStatusChangedNotification(file);

                tr.Complete();
            }
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

        private void Upload(ConnectionInfo connectionInfo, AchFileEntity achFile, Stream stream)
        {
            Contract.Assert(connectionInfo != null);
            Contract.Assert(achFile != null);
            Contract.Assert(stream != null);

            // todo: refactor SftpClient dependency
            return;
            
            using (var sftp = new SftpClient(connectionInfo))
            {
                try
                {
                    sftp.Connect();

                    var fileName = achFile.Name + ".ach";
                    sftp.UploadFile(stream, fileName);
                }
                finally
                {
                    sftp.Disconnect();
                }
            }
        }

        private void CheckUploadedAchFileStatus(AchFileEntity achFile)
        {
            Contract.Assert(achFile != null);

            // todo: implement it
            return;
            
            throw new NotImplementedException();
        }

        #endregion
    }
}