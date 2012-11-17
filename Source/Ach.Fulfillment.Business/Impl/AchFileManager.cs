namespace Ach.Fulfillment.Business.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;
    using Ach.Fulfillment.Nacha.Enumeration;
    using Ach.Fulfillment.Nacha.Message;
    using Ach.Fulfillment.Nacha.Record;

    using Microsoft.Practices.Unity;

    using File = Ach.Fulfillment.Nacha.Message.File;

    internal class AchFileManager : ManagerBase<AchFileEntity>, IAchFileManager
    {
        [Dependency]
        public IAchTransactionManager AchTransactionManager { get; set; }

        #region Public Methods

        public AchFileEntity Create(PartnerEntity partner, List<AchTransactionEntity> transactionEntities)
        {
            var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            var fileEntity = new AchFileEntity
                                 {
                                     Name = newFileName,
                                     FileStatus = AchFileStatus.Created,
                                     Partner = partner,
                                     Transactions = transactionEntities,
                                     FileIdModifier = "A" // TODO calculate modifier
                                 };
            return this.Create(fileEntity);
        }

        public void CleanUpCompletedFiles()
        {
            List<AchFileEntity> completedFiles;
            using (var tx = new Transaction())
            {
                completedFiles = Repository.FindAll(new AchFileCompleted()).ToList();

                foreach (var achFile in completedFiles)
                {
                    achFile.Locked = true;
                    this.Update(achFile);
                }

                tx.Complete();
            }

            // foreach (var completedFile in completedFiles)
            // {
                // completedFile.Name;
            // }
            using (var tx = new Transaction())
            {
                foreach (var completedFile in completedFiles)
                {
                    Repository.Delete(completedFile);
                }

                tx.Complete();
            }
        }

        public void ChangeAchFilesStatus(AchFileEntity file, AchFileStatus status)
        {
            file.FileStatus = status;
            this.Update(file);

            switch (status)
            {
                case AchFileStatus.Uploaded:
                    this.AchTransactionManager.ChangeAchTransactionStatus(
                        file.Transactions.ToList(), AchTransactionStatus.InProgress);
                    break;
                case AchFileStatus.Completed:
                    this.AchTransactionManager.ChangeAchTransactionStatus(
                        file.Transactions.ToList(), AchTransactionStatus.Completed);
                    break;
                case AchFileStatus.Rejected:
                    this.AchTransactionManager.ChangeAchTransactionStatus(
                        file.Transactions.ToList(), AchTransactionStatus.Error);
                    break;
            }
        }

        public void CreateFile(string achFile, string achfilesStore, string newFileName)
        {
            Contract.Assert(!string.IsNullOrEmpty(achFile));
            Contract.Assert(!string.IsNullOrEmpty(achfilesStore));
            Contract.Assert(!string.IsNullOrEmpty(newFileName));

            var newPath = System.IO.Path.Combine(achfilesStore, newFileName + ".ach");

            using (var file = new System.IO.StreamWriter(newPath))
            {
                    file.Write(achFile);
                    file.Flush();
                    file.Close();
            }
        }

        public List<AchFileEntity> AchFilesUpload()
        {
            var achFilesList = Repository.FindAll(new AchFileCreated()).ToList();

            // ToDo lock entities
            return achFilesList;
        }

        public void UploadCompleted(AchFileEntity achFile)
        {
            achFile.Locked = false;
            this.Update(achFile);
        }

        public string GetNextIdModifier(PartnerEntity partner)
        {
            var lastAchFile = Repository.Query<AchFileEntity>(new AchFileForPartner(partner)).Last(
                    m => m.Created.Date == DateTime.Today.Date);
             
             if (string.IsNullOrEmpty(lastAchFile.FileIdModifier))
             {
                 return "A";
             }

            return "B"; // ToDo calculate next Id
        }

        public void Generate(string achfilesStore)
        {
            Contract.Assert(!string.IsNullOrEmpty(achfilesStore));

            var achTransactionEntities = this.AchTransactionManager.GetTransactionsInQueue();

            var transactionGroups = achTransactionEntities.GroupBy(tt => tt.Partner);
            var partnerTransactions = transactionGroups as List<IGrouping<PartnerEntity, AchTransactionEntity>> ?? transactionGroups.ToList();

            if (partnerTransactions.Any())
            {
                foreach (var transactions in partnerTransactions)
                {
                    var trns = transactions.ToList();
                    var partner = transactions.Key;
                    var fileEntity = this.Create(partner, trns);
                    var achFile = this.GenerateAchFileForPartner(partner, trns, fileEntity.Id);
                    this.CreateFile(achFile, achfilesStore, fileEntity.Name);
                    this.AchTransactionManager.ChangeAchTransactionStatus(trns, AchTransactionStatus.Batched);
                    this.AchTransactionManager.UnLockTransactions(trns);
                }
            }
        }

        #endregion

        #region Private Methods

        private string GenerateAchFileForPartner(PartnerEntity partner, IEnumerable<AchTransactionEntity> transactions, long fileEntityId)
        {
            var transactionGroups = transactions.GroupBy(tt => tt.EntryDescription); // ToDo change to settelment date 
            var groupedTransactions = transactionGroups as List<IGrouping<string, AchTransactionEntity>> ?? transactionGroups.ToList();
            if (groupedTransactions.Any())
            {
                var fileIdModifier = this.GetNextIdModifier(partner);
                var batchNumber = 0;

                var achfile = new File
                {
                    Header = this.CreateFileControlRecord(partner, fileIdModifier, fileEntityId),
                    Batches = new List<GeneralBatch>()
                };

                foreach (var transaction in groupedTransactions)
                {
                    var batch = this.CreateBatchRecord(partner, transaction.Key, transaction.ToList(), batchNumber);
                    achfile.Batches.AddRange(batch);
                    batchNumber++;
                }

                var entryCount = achfile.Batches.Select(b => b.Control.EntryAndAddendaCount).Sum();
                var entryHash =
                    achfile.Batches.Select(o => Convert.ToInt32(o.Control.EntryHash)).Sum().ToString(
                        CultureInfo.InvariantCulture);
                if (entryHash.Length > 10)
                {
                    entryHash = entryHash.Substring(entryHash.Length - 10, 10);
                }

                achfile.Control = new FileControlRecord
                {
                    BatchCount = batchNumber,
                    BlockCount = batchNumber + 2,
                    EntryAndAddendaCount = entryCount,
                    EntryHash = entryHash, // TODO calculate right EntryHash
                    TotalCreditAmount = achfile.Batches.Select(b => b.Control.TotalCreditAmount).Sum(),
                    TotalDebitAmount = achfile.Batches.Select(b => b.Control.TotalDebitAmount).Sum()
                };
                var result = achfile.Serialize();
                return result;
            }

            return null;
        }

        private FileHeaderRecord CreateFileControlRecord(PartnerEntity partner, string fileIdModifier, long fileEntityId)
        {
            Contract.Assert(partner != null);
            Contract.Assert(partner.Details != null);

            return new FileHeaderRecord
            {
                Destination = partner.Details.Destination,
                FileCreationDateTime = DateTime.Now,
                FileIdModifier = fileIdModifier,
                ImmediateDestination = partner.Details.ImmediateDestination,
                ImmediateOrigin = partner.Details.CompanyIdentification,
                OriginOrCompanyName = partner.Details.OriginOrCompanyName,
                ReferenceCode = fileEntityId.ToString(CultureInfo.InvariantCulture)
            };
        }

        private IEnumerable<GeneralBatch> CreateBatchRecord(PartnerEntity partner, string description, IEnumerable<AchTransactionEntity> transactions, int batchNumber)
        {
            var batches = new List<GeneralBatch>();
            if (transactions != null)
            {
                var achTransactionEntities = transactions as List<AchTransactionEntity> ?? transactions.ToList();
                var trnsGroupedByClassCode = achTransactionEntities.GroupBy(t => t.EntryClassCode).ToList();

                if (trnsGroupedByClassCode.Any())
                {
                    foreach (var trn in trnsGroupedByClassCode)
                    {
                        var batch = new GeneralBatch
                        {
                            Header = new BatchHeaderGeneralRecord
                            {
                                BatchNumber = batchNumber,
                                CompanyEntryDescription = description,
                                CompanyIdentification = partner.Details.CompanyIdentification,
                                CompanyName = partner.Details.CompanyName,
                                EffectiveEntryDate = DateTime.Now,
                                OriginatingDfiIdentification = partner.Details.DfiIdentification,
                                ServiceClassCode = ServiceClassCode.CreditAndDebit,
                                StandardEntryClassCode = (StandardEntryClassCode)Enum.Parse(typeof(StandardEntryClassCode), trn.Key.ToUpper())
                            },
                            Entries = new List<EntryDetailGeneralRecord>()
                        };

                        foreach (var entry in achTransactionEntities.Select(this.CreateEntryDetailRecord))
                        {
                            batch.Entries.Add(entry);
                        }

                        var entryHash =
                            batch.Entries.Select(e => Convert.ToInt32(e.RdfiRoutingTransitNumber)).Sum().ToString(
                                CultureInfo.InvariantCulture);

                        if (entryHash.Length > 10)
                        {
                            entryHash = entryHash.Substring(entryHash.Length - 10, 10);
                        }

                        batch.Control = new BatchControlRecord
                        {
                            BatchNumber = batchNumber,
                            CompanyIdentification = batch.Header.CompanyIdentification,
                            EntryAndAddendaCount = batch.Entries.Count,
                            EntryHash = entryHash,
                            OriginatingDfiIdentification = batch.Header.OriginatingDfiIdentification,
                            ServiceClassCode = batch.Header.ServiceClassCode,
                            TotalCreditAmount = batch.Entries.Select(e => e.Amount).Sum(),
                            TotalDebitAmount = batch.Entries.Select(e => e.Amount).Sum(),
                        };
                        batches.Add(batch);
                    }
                }
            }

            return batches;
        }

        private EntryDetailGeneralRecord CreateEntryDetailRecord(AchTransactionEntity transaction)
        {
            return new EntryDetailGeneralRecord
            {
                AddendaRecordIndicator = transaction.PaymentRelatedInfo != null ? "1" : "0",
                Amount = transaction.Amount,
                RdfiRoutingTransitNumber = transaction.TransitRoutingNumber.Substring(0, 8),
                CheckDigit = transaction.TransitRoutingNumber.Substring(8, 1),
                IndividualOrCompanyName = transaction.ReceiverName,
                RdfiAccountNumber = transaction.DfiAccountId,
                TransactionCode = (TransactionCode)Enum.Parse(typeof(TransactionCode), transaction.TransactionCode)
            };
        }

        #endregion
    }
}
