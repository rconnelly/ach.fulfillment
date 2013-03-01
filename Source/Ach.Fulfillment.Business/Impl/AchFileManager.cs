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
    using Ach.Fulfillment.Data.Specifications;

    using Microsoft.Practices.Unity;

    using Renci.SshNet;

    public class AchFileManager : ManagerBase<AchFileEntity>, IAchFileManager
    {
        #region Public Properties

        [Dependency]
        public IAchTransactionManager AchTransactionManager { get; set; }

        [Dependency]
        public IPartnerManager PartnerManager { get; set; }

        #endregion

        #region Public Methods and Operators

        #region Generate

        public void Generate()
        {
            var partners = this.PartnerManager.FindAll(new PartnerAll());

            // todo: probably we will need flag for partner in future which will say that we need to genarate achfiles for it
            foreach (var partner in partners)
            {
                this.Generate(partner);
            }
        }

        public void Generate(PartnerEntity partner)
        {
            var achTransactions = this.AchTransactionManager.GetEnqueued(partner).ToList();
            if (achTransactions.Any())
            {
                using (var tr = new Transaction())
                {
                    this.AchTransactionManager.Lock(achTransactions);

                    this.Create(partner, achTransactions);

                    this.AchTransactionManager.UpdateStatus(AchTransactionStatus.Batched, achTransactions);

                    this.AchTransactionManager.UnLock(achTransactions);

                    tr.Complete();
                }
            }
        }

        public AchFileEntity Create(PartnerEntity partner, params AchTransactionEntity[] transactionEntities)
        {
            return this.Create(partner, (IList<AchTransactionEntity>)transactionEntities);
        }

        public AchFileEntity Create(PartnerEntity partner, IList<AchTransactionEntity> transactionEntities)
        {
            Contract.Assert(partner != null);
            Contract.Assert(transactionEntities != null);

            var newFileName = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var fileEntity = new AchFileEntity
            {
                Name = newFileName,
                FileStatus = AchFileStatus.Created,
                Partner = partner,
                Transactions = transactionEntities,
                FileIdModifier = this.GetNextIdModifier(partner)
            };

            return this.Create(fileEntity);
        }

        public string GetNextIdModifier(PartnerEntity partner)
        {
            var fileIdModifier =
                this.Repository.Query<AchFileEntity>(new AchFileForPartner(partner))
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

        #endregion

        #region Upload

        public void Upload(PasswordConnectionInfo connectionInfo)
        {
            var files = this.GetAchFilesForUploading();

            var achFileEntities = files as IList<AchFileEntity> ?? files.ToList();
            var contents = 
                from file in achFileEntities
                select Tuple.Create<AchFileEntity, Func<Stream>>(file, file.ToNachaStream);

            try
            {
                this.Lock(achFileEntities);
                this.Upload(connectionInfo, contents);
            }
            finally
            {
                this.UnLock(achFileEntities);
            }

        }

        public IEnumerable<AchFileEntity> GetAchFilesForUploading()
        {
            var achFiles = this.Repository.FindAll(new AchFileCreated());

            return achFiles;
        }

        public void Upload(ConnectionInfo connectionInfo, IEnumerable<Tuple<AchFileEntity, Func<Stream>>> achFilesContents)
        {
            Contract.Assert(connectionInfo != null);
            Contract.Assert(achFilesContents != null);

            using (var sftp = new SftpClient(connectionInfo))
            {
                try
                {
                    sftp.Connect();

                    foreach (var tuple in achFilesContents)
                    {
                        Contract.Assert(tuple.Item1 != null);
                        Contract.Assert(tuple.Item2 != null);

                        var achFile = tuple.Item1;
                        var streamBuilder = tuple.Item2;
                        var fileName = achFile.Name + ".ach";
                        using (var stream = streamBuilder())
                        {
                            sftp.UploadFile(stream, fileName);
                            this.UpdateStatus(achFile, AchFileStatus.Uploaded);
                        }
                    }
                }
                finally
                {
                    sftp.Disconnect();
                }
            }
        }

        public void UpdateStatus(AchFileEntity file, AchFileStatus status)
        {
            Contract.Assert(file != null);

            using (var tr = new Transaction())
            {
                file.FileStatus = status;
                this.Update(file);

                switch (status)
                {
                    case AchFileStatus.Uploaded:
                        this.AchTransactionManager.UpdateStatus(
                            AchTransactionStatus.InProgress, file.Transactions.ToList());
                        break;
                    case AchFileStatus.Completed:
                        this.AchTransactionManager.UpdateStatus(
                            AchTransactionStatus.Completed, file.Transactions.ToList());
                        break;
                    case AchFileStatus.Rejected:
                        this.AchTransactionManager.UpdateStatus(AchTransactionStatus.Error, file.Transactions.ToList());
                        break;
                }

                tr.Complete();
            }
        }

        public void Lock(IEnumerable<AchFileEntity> files)
        {
            using (var tx = new Transaction())
            {
                foreach (var file in files)
                {
                    file.Locked = true;
                    this.Update(file);
                }

                tx.Complete();
            }
        }

        public void UnLock(IEnumerable<AchFileEntity> files)
        {
            using (var tx = new Transaction())
            {
                foreach (var file in files)
                {
                    file.Locked = false;
                    this.Update(file);
                }

                tx.Complete();
            }
        }

        #endregion

        #region Cleanup

        public void Cleanup()
        {
            Repository.Execute(new DeleteCompletedAchFiles());
        }

        #endregion

        #endregion
    }
}