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

namespace Ach.Fulfillment.Business.Impl
{
    internal class AchTransactionManager : ManagerBase<AchTransactionEntity>, IAchTransactionManager
    {
        [Dependency]
        public FileManager FileManager { get; set; }

        #region Public Methods and Operators

        public override AchTransactionEntity Create(AchTransactionEntity transaction)
        {
            Contract.Assert(transaction != null);

            return base.Create(transaction);
        }

        public void Generate()
        {
            var achTransactionEntities = Repository.FindAll(new AchTransactionInQueue()).ToList();
           
            var transactionGroups = achTransactionEntities.GroupBy(tt => tt.Partner);
            var transactions = transactionGroups as List<IGrouping<PartnerEntity, AchTransactionEntity>> ?? transactionGroups.ToList();
            
            if(transactions.Any())
            {
                foreach (var transaction in transactions)
                {
                    var trns = transaction.ToList();
                    var achFile = GenerateAchFileForPartner(transaction.Key, trns);
                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    
                    CreateFileForPartnerTransactions(transaction.Key, trns);

                    if (achFile != null & achFile.Length > 0)
                    {
                        var achfilesStore = @"D:\"; //TODO put here real path
                        var newPath = System.IO.Path.Combine(achfilesStore, newFileName + ".txt");

                        var file = new System.IO.StreamWriter(newPath);
                        file.Write(achFile);
                        file.Flush();
                        file.Close();
                    }

                    RemoveTransactionFromQueue(trns);
                }
            }
        }
        
        public void RemoveTransactionFromQueue(List<AchTransactionEntity> transactions)
        {
            Contract.Assert(transactions != null);

            using (var tx = new Transaction())
            {
                foreach (var achTransactionEntity in transactions)
                {
                    achTransactionEntity.TransactionStatus = TransactionStatus.Batched;
                    base.Update(achTransactionEntity);
                }
                tx.Complete();
            }
        }

        public void CreateFileForPartnerTransactions(PartnerEntity partner, List<AchTransactionEntity> transactionEntities)
        {
            var fileEntity = new FileEntity
            {
                FileStatus = "Created",
                Partner = partner,
                Transactions = transactionEntities,
                FileIdModifier = "" //TODO calculate modifier
            };
            FileManager.Create(fileEntity);
        }

        #endregion

        private string GenerateAchFileForPartner(PartnerEntity partner, List<AchTransactionEntity> transactions)
        {
            var transactionGroups = transactions.GroupBy(tt => tt.EntryDescription);
            var groupedTransactions = transactionGroups as List<IGrouping<string, AchTransactionEntity>> ?? transactionGroups.ToList();
            if (groupedTransactions.Any())
            {
                var achfile = new File
                {
                    Header = CreateFileControlRecord(partner, ""),//TODO put real fileIdModifier
                    Batches = new List<GeneralBatch>()
                };
                var batchNumber = 0;
                foreach (var transaction in groupedTransactions)
                {
                    var batch = CreateBatch(partner, transaction.Key, transaction.ToList(), batchNumber);
                    achfile.Batches.AddRange(batch);
                    batchNumber++;
                }

                var entryCount = achfile.Batches.Select(b => b.Control.EntryAndAddendaCount).Sum();
                var entryHash =
                    achfile.Batches.Select(o => Convert.ToInt32(o.Control.EntryHash)).Sum().ToString();
                if (entryHash.Length > 10)
                    entryHash = entryHash.Substring(entryHash.Length - 10, 10);
                achfile.Control = new FileControlRecord
                {
                    BatchCount = batchNumber - 1,
                    BlockCount = batchNumber + 1,
                    EntryAndAddendaCount = entryCount,
                    EntryHash = entryHash, //TODO calculate right EntryHash
                    TotalCreditAmount =
                        achfile.Batches.Select(b => b.Control.TotalCreditAmount).Sum(),
                    TotalDebitAmount =
                        achfile.Batches.Select(b => b.Control.TotalDebitAmount).Sum()
                };
                var result = achfile.Serialize();
                return result;
            }
            return null;
        }

        private FileHeaderRecord CreateFileControlRecord(PartnerEntity partner, string fileIdModifier)
        {
            return new FileHeaderRecord
                       {
                           Destination = partner.Details.Destination,
                           FileCreationDateTime = DateTime.Now,
                           FileIdModifier = fileIdModifier,
                           ImmediateDestination = partner.Details.ImmediateDestination,
                           ImmediateOrigin = partner.Details.CompanyIdentification,
                           OriginOrCompanyName = partner.Details.OriginOrCompanyName
                       };
        }

        private List<GeneralBatch> CreateBatch(PartnerEntity partner, string description, IEnumerable<AchTransactionEntity> transactions, int batchNumber)
        {
            var batches = new List<GeneralBatch>();
            var trnsGroupedByClassCode = transactions.GroupBy(t => t.EntryClassCode).ToList();

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
                                                         OriginatingDFIIdentification = partner.Details.DFIIdentification,
                                                         ServiceClassCode = ServiceClassCode.CreditAndDebit,
                                                         StandardEntryClassCode = (StandardEntryClassCode)Enum.Parse(typeof(StandardEntryClassCode), trn.Key)
                                                     },
                                        Entries = new List<EntryDetailGeneralRecord>()
                                    };

                    foreach (var entry in transactions.Select(CreateEntryDetailRecord))
                    {
                        batch.Entries.Add(entry);
                    }
                    var entryHash =
                        batch.Entries.Select(e => Convert.ToInt32(e.RDFIRoutingTransitNumber)).Sum().ToString(
                            CultureInfo.InvariantCulture);
                    if (entryHash.Length > 10)
                        entryHash = entryHash.Substring(entryHash.Length - 10, 10);

                    batch.Control = new BatchControlRecord
                                        {
                                            BatchNumber = batchNumber,
                                            CompanyIdentification = batch.Header.CompanyIdentification,
                                            EntryAndAddendaCount = batch.Entries.Count,
                                            EntryHash = entryHash,
                                            OriginatingDFIIdentification = batch.Header.OriginatingDFIIdentification,
                                            ServiceClassCode = batch.Header.ServiceClassCode,
                                            TotalCreditAmount = batch.Entries.Select(e => e.Amount).Sum(),
                                            TotalDebitAmount = batch.Entries.Select(e => e.Amount).Sum(),
                                        };
                    batches.Add(batch);
                }
            }
            return batches;
        }

        private EntryDetailGeneralRecord CreateEntryDetailRecord(AchTransactionEntity transaction)
        {
            return new EntryDetailGeneralRecord
            {
                AddendaRecordIndicator = transaction.PaymentRelatedInfo!= null?"1":"0",
                Amount = transaction.Amount,
                RDFIRoutingTransitNumber = transaction.TransitRoutingNumber.Substring(0, 8),
                CheckDigit = transaction.TransitRoutingNumber.Substring(8, 1),
                IndividualOrCompanyName = transaction.ReceiverName,
                RDFIAccountNumber = transaction.DFIAccountId,
                TransactionCode = (TransactionCode)Enum.Parse(typeof(TransactionCode),transaction.TransactionCode)
            };
        }

     }
}
