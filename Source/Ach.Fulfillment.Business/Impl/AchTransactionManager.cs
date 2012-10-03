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

namespace Ach.Fulfillment.Business.Impl
{
    internal class AchTransactionManager : ManagerBase<AchTransactionEntity>, IAchTransactionManager
    {
        #region Constants and Fields

        private const string originOrCompanyName = "PriorityPaymentSystems";
        private const string companyName = "PPS";
        private const string immediateOrigin = "1234567890";
        private const string immediateDestination = "b111000025";
        private const string originatingDFIIdentification = "11100002";
        private const string destination = "Bank of America RIC";
        private const string companyIdentification = "1234567890";
        
        #endregion

        #region Public Methods and Operators

        public override AchTransactionEntity Create(AchTransactionEntity transaction)
        {
            Contract.Assert(transaction != null);

            return base.Create(transaction);
        }

        public string Generate()
        {

            var trs = Repository.FindAll(new AchTransactionInQueue());
            var achTransactionEntities = trs as List<AchTransactionEntity> ?? trs.ToList();

            var transactionGroups = achTransactionEntities.GroupBy(tt => tt.MerchantDescription);

            var transactionses = transactionGroups as List<IGrouping<string, AchTransactionEntity>> ?? transactionGroups.ToList();
            if (transactionses.Any())
            {
                    var achfile = new File
                                      {
                                          Header = CreateFileControlRecord(),
                                          Batches = new List<GeneralBatch>()
                                      };
                    var batchNumber = 0;
                    foreach (var transactions in transactionses)
                    {
                        var batch = CreateBatch(transactions.Key, transactions.ToList(), batchNumber);
                        achfile.Batches.Add(batch);
                        batchNumber++;
                    }

                    var entryCount = achfile.Batches.Select(b => b.Control.EntryAndAddendaCount).Sum();
                    var entryHash =
                        achfile.Batches.Select(o => Convert.ToInt32(o.Control.EntryHash)).Sum().ToString(
                            CultureInfo.InvariantCulture);
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

                    RemoveTransactionFromQueue(achTransactionEntities);
                    
                return result;
                
            }
           
            return null;
        }

        public void RemoveTransactionFromQueue(List<AchTransactionEntity> transactions)
        {
            Contract.Assert(transactions != null);

            using (var tx = new Transaction())
            {
                foreach (var achTransactionEntity in transactions)
                {
                    achTransactionEntity.IsQueued = false;
                    base.Update(achTransactionEntity);
                }
                tx.Complete();
            }
        }

        #endregion

        private FileHeaderRecord CreateFileControlRecord()
        {
            return new FileHeaderRecord
                       {
                           Destination = destination,
                           FileCreationDateTime = DateTime.Now,
                           FileIdModifier = "0",
                           ImmediateDestination = immediateDestination,
                           ImmediateOrigin = immediateOrigin,
                           OriginOrCompanyName = originOrCompanyName
                       };
        }

        private EntryDetailGeneralRecord CreateEntryDetailRecord(AchTransactionEntity transaction)
        {
            return new EntryDetailGeneralRecord
            {
                AddendaRecordIndicator = "0",
                Amount = transaction.Amount,
                RDFIRoutingTransitNumber = transaction.RoutingNumber.Substring(0, 8),
                CheckDigit = transaction.RoutingNumber.Substring(8, 1),
                IndividualOrCompanyName = transaction.MerchantName,
                RDFIAccountNumber = transaction.AccountId,
                TransactionCode = TransactionCode.CheckingPrenoteDebit
            };
        }

        private GeneralBatch CreateBatch(string description, IEnumerable<AchTransactionEntity> transactions, int batchNumber )
        {
            var batch = new GeneralBatch
            {
                Header = new BatchHeaderGeneralRecord
                {
                    BatchNumber = batchNumber,
                    CompanyEntryDescription = description,
                    CompanyIdentification = companyIdentification,
                    CompanyName = companyName,
                    EffectiveEntryDate = DateTime.Now,
                    OriginatingDFIIdentification = originatingDFIIdentification,
                    ServiceClassCode = ServiceClassCode.CreditOnly,
                    StandardEntryClassCode = StandardEntryClassCode.PPD
                },
                Entries = new List<EntryDetailGeneralRecord>()
            };

            foreach (var entry in transactions.Select(CreateEntryDetailRecord))
            {
                batch.Entries.Add(entry);
            }
            var entryHash = batch.Entries.Select(e => Convert.ToInt32(e.RDFIRoutingTransitNumber)).Sum().ToString(CultureInfo.InvariantCulture);
            if (entryHash.Length > 10)
                entryHash = entryHash.Substring(entryHash.Length - 10, 10);

            batch.Control = new BatchControlRecord
            {
                BatchNumber = batchNumber,
                CompanyIdentification = companyIdentification,
                EntryAndAddendaCount = batch.Entries.Count,
                EntryHash = entryHash,
                OriginatingDFIIdentification = originatingDFIIdentification,
                ServiceClassCode = ServiceClassCode.CreditOnly,
                TotalCreditAmount = batch.Entries.Select(e => e.Amount).Sum(),
                TotalDebitAmount = batch.Entries.Select(e => e.Amount).Sum(),
            };
            return batch;
        }
    }
}
