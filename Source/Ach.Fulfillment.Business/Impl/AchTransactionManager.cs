namespace Ach.Fulfillment.Business.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;
    using Ach.Fulfillment.Nacha.Enumeration;
    using Ach.Fulfillment.Nacha.Message;
    using Ach.Fulfillment.Nacha.Record;

    using Microsoft.Practices.Unity;

    internal class AchTransactionManager : ManagerBase<AchTransactionEntity>, IAchTransactionManager
    {
        [Dependency]
        public IFileManager FileManager { get; set; }

        #region Public Methods and Operators

        public override AchTransactionEntity Create(AchTransactionEntity transaction)
        {
            Contract.Assert(transaction != null);

            this.DemandValid<AchTransactionValidator>(transaction);

            return base.Create(transaction);
        }

        public void Generate(string achfilesStore)
        {
            Contract.Assert(!string.IsNullOrEmpty(achfilesStore));

            var achTransactionEntities = Repository.FindAll(new AchTransactionInQueue()).ToList();

            // TODO add locking
            var transactionGroups = achTransactionEntities.GroupBy(tt => tt.Partner);
            var partnerTransactions = transactionGroups as List<IGrouping<PartnerEntity, AchTransactionEntity>> ?? transactionGroups.ToList();

            if (partnerTransactions.Any())
            {
                foreach (var transactions in partnerTransactions)
                {
                    var trns = transactions.ToList();
                    var partner = transactions.Key;
                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var fileEntity = this.FileManager.CreateFileForPartnerTransactions(partner, trns, newFileName);

                    var achFile = this.GenerateAchFileForPartner(partner, trns, fileEntity.Id);

                    // Todo add try/catch block 
                    if (!string.IsNullOrEmpty(achFile))
                    {
                        var newPath = System.IO.Path.Combine(achfilesStore, newFileName + ".ach");

                        using (var file = new System.IO.StreamWriter(newPath))
                        {
                            file.Write(achFile);
                            file.Flush();
                            file.Close();
                        }
                    }

                    this.ChangeAchTransactionStatus(trns, AchTransactionStatus.Batched);
                }
            }
        }
        
        public void ChangeAchTransactionStatus(List<AchTransactionEntity> transactions, AchTransactionStatus status)
        {
            Contract.Assert(transactions != null);

            using (var tx = new Transaction())
            {
                foreach (var achTransactionEntity in transactions)
                {
                    achTransactionEntity.TransactionStatus = status;
                    this.Update(achTransactionEntity);
                }

                tx.Complete();
            }
        }

        public void SendAchNotification(List<AchTransactionEntity> transactions)
        {
            Contract.Assert(transactions != null);
        }

        #endregion

        #region Private Methods

        private string GenerateAchFileForPartner(PartnerEntity partner, IEnumerable<AchTransactionEntity> transactions, long fileEntityId)
        {
            var transactionGroups = transactions.GroupBy(tt => tt.EntryDescription); // ToDo change to settelment date 
            var groupedTransactions = transactionGroups as List<IGrouping<string, AchTransactionEntity>> ?? transactionGroups.ToList();
            if (groupedTransactions.Any())
            {
                var achfile = new File
                    {
                        Header = this.CreateFileControlRecord(partner, "A", fileEntityId), // TODO put real fileIdModifier
                        Batches = new List<GeneralBatch>()
                    };
                var batchNumber = 0;
                foreach (var transaction in groupedTransactions)
                {
                    var batch = this.CreateBatch(partner, transaction.Key, transaction.ToList(), batchNumber);
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
                        BatchCount = batchNumber - 1,
                        BlockCount = batchNumber + 1,
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

        private IEnumerable<GeneralBatch> CreateBatch(PartnerEntity partner, string description, IEnumerable<AchTransactionEntity> transactions, int batchNumber)
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
                                                             StandardEntryClassCode = (StandardEntryClassCode)Enum.Parse(typeof(StandardEntryClassCode), trn.Key)
                                                         },
                                            Entries = new List<EntryDetailGeneralRecord>()
                                        };

                        foreach (var entry in achTransactionEntities.Select(this.CreateEntryDetailRecord))
                        {
                            batch.Entries.Add(entry);
                        }

                        var entryHash =
                            batch.Entries.Select(e => Convert.ToInt32(e.RDFIRoutingTransitNumber)).Sum().ToString(
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
                                                OriginatingDFIIdentification = batch.Header.OriginatingDfiIdentification,
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
                    RDFIRoutingTransitNumber = transaction.TransitRoutingNumber.Substring(0, 8),
                    CheckDigit = transaction.TransitRoutingNumber.Substring(8, 1),
                    IndividualOrCompanyName = transaction.ReceiverName,
                    RDFIAccountNumber = transaction.DfiAccountId,
                    TransactionCode = (TransactionCode)Enum.Parse(typeof(TransactionCode), transaction.TransactionCode)
            };
        }

        #endregion
    }
}