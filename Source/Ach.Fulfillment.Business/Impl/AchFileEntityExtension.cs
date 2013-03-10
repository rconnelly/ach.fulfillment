namespace Ach.Fulfillment.Business.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Nacha.Enumeration;
    using Ach.Fulfillment.Nacha.Message;
    using Ach.Fulfillment.Nacha.Record;

    using File = Ach.Fulfillment.Nacha.Message.File;

    public static class AchFileEntityExtension
    {
        public static void ToStream(this AchFileEntity fileEntity, Stream stream)
        {
            Contract.Assert(fileEntity != null);
            Contract.Assert(stream != null);
            var content = fileEntity.ToNachaContent();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
        }

        public static string ToNachaContent(this AchFileEntity fileEntity)
        {
            Contract.Assert(fileEntity != null);
            var file = fileEntity.ToNacha();
            Contract.Assert(file != null);
            var achFileString = file.Serialize();
            return achFileString;
        }

        public static File ToNacha(this AchFileEntity fileEntity)
        {
            Contract.Assert(fileEntity != null);

            // todo (ng): needs to be changed to settelment date??
            var transactionGroups = fileEntity.Transactions.GroupBy(tt => tt.EntryDescription);
            var groupedTransactions = transactionGroups as List<IGrouping<string, AchTransactionEntity>>
                                      ?? transactionGroups.ToList();

            Contract.Assert(groupedTransactions.Any());
            var batchNumber = 0;

            var achfile = new File
                              {
                                  Header = CreateFileControlRecord(fileEntity),
                                  Batches = new List<GeneralBatch>()
                              };

            foreach (var transaction in groupedTransactions)
            {
                var batch = CreateBatchRecord(
                    fileEntity.Partner, transaction.Key, transaction.ToList(), batchNumber);
                achfile.Batches.AddRange(batch);
                batchNumber++;
            }

            var entryCount = achfile.Batches.Select(b => b.Control.EntryAndAddendaCount).Sum();
            var entryHash =
                achfile.Batches.Select(o => Convert.ToInt32(o.Control.EntryHash))
                       .Sum()
                       .ToString(CultureInfo.InvariantCulture);
            if (entryHash.Length > 10)
            {
                entryHash = entryHash.Substring(entryHash.Length - 10, 10);
            }

            achfile.Control = new FileControlRecord
                                  {
                                      BatchCount = batchNumber,
                                      BlockCount = batchNumber + 2,
                                      EntryAndAddendaCount = entryCount,
                                      EntryHash = entryHash, // todo (ng): calculate right EntryHash
                                      TotalCreditAmount =
                                          achfile.Batches.Select(b => b.Control.TotalCreditAmount).Sum(),
                                      TotalDebitAmount =
                                          achfile.Batches.Select(b => b.Control.TotalDebitAmount).Sum()
                                  };

            return achfile;
        }

        private static FileHeaderRecord CreateFileControlRecord(AchFileEntity fileEntity)
        {
            Contract.Assert(fileEntity != null);
            Contract.Assert(fileEntity.Partner != null);
            Contract.Assert(fileEntity.Partner.Details != null);

            var partner = fileEntity.Partner;
            var details = partner.Details;

            return new FileHeaderRecord
                       {
                           Destination = partner.Details.Destination,
                           FileCreationDateTime = DateTime.UtcNow,
                           FileIdModifier = fileEntity.FileIdModifier,
                           ImmediateDestination = details.ImmediateDestination,
                           ImmediateOrigin = details.CompanyIdentification,
                           OriginOrCompanyName = details.OriginOrCompanyName,
                           ReferenceCode = fileEntity.Id.ToString(CultureInfo.InvariantCulture)
                       };
        }

        private static IEnumerable<GeneralBatch> CreateBatchRecord(PartnerEntity partner, string description, IEnumerable<AchTransactionEntity> transactions, int batchNumber)
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
                                            Header =
                                                new BatchHeaderGeneralRecord
                                                    {
                                                        BatchNumber = batchNumber,
                                                        CompanyEntryDescription = description,
                                                        CompanyIdentification = partner.Details.CompanyIdentification,
                                                        CompanyName = partner.Details.CompanyName,
                                                        EffectiveEntryDate = DateTime.UtcNow,
                                                        OriginatingDfiIdentification = partner.Details.DfiIdentification,
                                                        ServiceClassCode = ServiceClassCode.CreditAndDebit,
                                                        StandardEntryClassCode = (StandardEntryClassCode)Enum.Parse(typeof(StandardEntryClassCode), trn.Key.ToUpper())
                                                    },
                                            Entries = new List<EntryDetailGeneralRecord>()
                                        };

                        foreach (var entry in achTransactionEntities.Select(CreateEntryDetailRecord))
                        {
                            batch.Entries.Add(entry);
                        }

                        var entryHash =
                            batch.Entries.Select(e => Convert.ToInt32(e.RdfiRoutingTransitNumber))
                                 .Sum()
                                 .ToString(CultureInfo.InvariantCulture);

                        if (entryHash.Length > 10)
                        {
                            entryHash = entryHash.Substring(entryHash.Length - 10, 10);
                        }

                        batch.Control = new BatchControlRecord
                                            {
                                                BatchNumber = batchNumber,
                                                CompanyIdentification =
                                                    batch.Header.CompanyIdentification,
                                                EntryAndAddendaCount = batch.Entries.Count,
                                                EntryHash = entryHash,
                                                OriginatingDfiIdentification =
                                                    batch.Header.OriginatingDfiIdentification,
                                                ServiceClassCode = batch.Header.ServiceClassCode,
                                                TotalCreditAmount =
                                                    batch.Entries.Select(e => e.Amount).Sum(),
                                                TotalDebitAmount =
                                                    batch.Entries.Select(e => e.Amount).Sum(),
                                            };
                        batches.Add(batch);
                    }
                }
            }

            return batches;
        }

        private static EntryDetailGeneralRecord CreateEntryDetailRecord(AchTransactionEntity transaction)
        {
            return new EntryDetailGeneralRecord
                       {
                           AddendaRecordIndicator =
                               transaction.PaymentRelatedInfo != null ? "1" : "0",
                           Amount = transaction.Amount,
                           RdfiRoutingTransitNumber =
                               transaction.TransitRoutingNumber.Substring(0, 8),
                           CheckDigit = transaction.TransitRoutingNumber.Substring(8, 1),
                           IndividualOrCompanyName = transaction.ReceiverName,
                           RdfiAccountNumber = transaction.DfiAccountId,
                           TransactionCode =
                               (TransactionCode)
                               Enum.Parse(typeof(TransactionCode), transaction.TransactionCode)
                       };
        }
    }
}