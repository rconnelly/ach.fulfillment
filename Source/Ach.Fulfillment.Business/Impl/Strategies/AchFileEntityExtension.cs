namespace Ach.Fulfillment.Business.Impl.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Nacha.Enumeration;
    using Ach.Fulfillment.Nacha.Message;
    using Ach.Fulfillment.Nacha.Record;

    using File = Ach.Fulfillment.Nacha.Message.File;

    internal static class AchFileEntityExtension
    {
        // warn: it wastes too many resources because of transformation to the same nacha hierarchy + serialization hierarchy to string
        public static string ToNachaContent(this INachaFileData data, IEnumerable<AchTransactionEntity> transactions)
        {
            Contract.Assert(data != null);
            Contract.Assert(transactions != null);
            var file = data.ToNacha(transactions);
            var achFileString = file.Serialize();
            return achFileString;
        }

        #region ToNacha

        public static File ToNacha(this INachaFileData data, IEnumerable<AchTransactionEntity> transactions)
        {
            Contract.Assert(data != null);
            Contract.Assert(transactions != null);
            var header = CreateFileHeaderRecord(data);
            var batches = CreateBatches(data, transactions);
            var control = CreateFileControlRecord(batches);

            var achfile = new File
                              {
                                  Header = header,
                                  Batches = batches,
                                  Control = control
                              };
            return achfile;
        }

        #region FileHeaderRecord

        private static FileHeaderRecord CreateFileHeaderRecord(INachaFileData details)
        {
            Contract.Assert(details != null);
            var result = new FileHeaderRecord
                       {
                           Destination = details.Destination,
                           FileCreationDateTime = DateTime.UtcNow,
                           FileIdModifier = details.FileIdModifier,
                           ImmediateDestination = details.ImmediateDestination,
                           ImmediateOrigin = details.CompanyIdentification,
                           OriginOrCompanyName = details.OriginOrCompanyName,
                           ReferenceCode = details.ReferenceCode
                       };

            return result;
        }

        #endregion

        #region Batches

        private static List<GeneralBatch> CreateBatches(INachaFileData data, IEnumerable<AchTransactionEntity> transactions)
        {
            Contract.Assert(data != null);
            Contract.Assert(transactions != null);

            var batches = transactions
                .GroupBy(tt => new { tt.EntryDescription, tt.EntryClassCode })
                .Select((g, i) => CreateBatch(i, data, g.Key.EntryDescription, g.Key.EntryClassCode, g))
                .ToList();

            return batches;
        }

        private static GeneralBatch CreateBatch(int batchNumber, INachaFileData data, string description, string entryClassCode, IEnumerable<AchTransactionEntity> transactions)
        {
            var header = CreateBatchHeader(batchNumber, data, description, entryClassCode);
            var entries = CreateBatchEntries(transactions);
            var control = CreateBatchControlRecord(batchNumber, header, entries);

            var result = new GeneralBatch
                {
                    Header = header,
                    Entries = entries,
                    Control = control
                };
            return result;
        }

        #region BatchHeaderGeneralRecord

        private static BatchHeaderGeneralRecord CreateBatchHeader(int batchNumber, INachaFileData data, string description, string entryClassCode)
        {
            Contract.Assert(data != null);
            Contract.Assert(entryClassCode != null);

            var standardEntryClassCode = (StandardEntryClassCode)Enum.Parse(typeof(StandardEntryClassCode), entryClassCode.ToUpper());
            var result = new BatchHeaderGeneralRecord
                       {
                           BatchNumber = batchNumber,
                           CompanyEntryDescription = description,
                           CompanyIdentification = data.CompanyIdentification,
                           CompanyName = data.CompanyName,
                           EffectiveEntryDate = DateTime.UtcNow,
                           OriginatingDfiIdentification = data.DfiIdentification,
                           ServiceClassCode = ServiceClassCode.CreditAndDebit,
                           StandardEntryClassCode = standardEntryClassCode
                       };

            return result;
        }

        #endregion

        #region Entries

        private static List<EntryDetailGeneralRecord> CreateBatchEntries(IEnumerable<AchTransactionEntity> transactions)
        {
            Contract.Assert(transactions != null);
            var result = transactions.Select(CreateEntryDetailRecord).ToList();
            return result;
        }

        private static EntryDetailGeneralRecord CreateEntryDetailRecord(AchTransactionEntity transaction)
        {
            Contract.Assert(transaction != null);
            Contract.Assert(transaction.TransitRoutingNumber != null);
            
            var transactionCode = (TransactionCode)transaction.TransactionCode;
            var record = new EntryDetailGeneralRecord
                             {
                                 AddendaRecordIndicator = transaction.PaymentRelatedInfo != null ? "1" : "0", 
                                 Amount = transaction.Amount, 
                                 RdfiRoutingTransitNumber = transaction.TransitRoutingNumber.Substring(0, 8), 
                                 CheckDigit = transaction.TransitRoutingNumber.Substring(8, 1), 
                                 IndividualOrCompanyName = transaction.ReceiverName, 
                                 RdfiAccountNumber = transaction.DfiAccountId,
                                 TransactionCode = transactionCode
                             };
            return record;
        }

        #endregion

        #region BatchControlRecord

        private static BatchControlRecord CreateBatchControlRecord(int batchNumber, BatchHeaderGeneralRecord header, ICollection<EntryDetailGeneralRecord> entries)
        {
            Contract.Assert(header != null);
            Contract.Assert(entries != null);
            
            var result = new BatchControlRecord
            {
                BatchNumber = batchNumber,
                CompanyIdentification = header.CompanyIdentification,
                EntryAndAddendaCount = entries.Count,
                EntryHash = GenerateEntryHash(entries),
                OriginatingDfiIdentification = header.OriginatingDfiIdentification,
                ServiceClassCode = header.ServiceClassCode,
                TotalCreditAmount = CalculateTotalCreditAmount(entries),
                TotalDebitAmount = CalculateTotalDebitAmount(entries)
            };

            return result;
        }

        private static string GenerateEntryHash(IEnumerable<EntryDetailGeneralRecord> entries)
        {
            // todo: reread documentation and check hashing algorithm
            Contract.Assert(entries != null);
            var entryHash = entries
                .Select(e => Convert.ToInt64(e.RdfiRoutingTransitNumber))
                .Sum()
                .ToString(CultureInfo.InvariantCulture);
            if (entryHash.Length > 10)
            {
                entryHash = entryHash.Substring(entryHash.Length - 10, 10);
            }

            return entryHash;
        }

        private static decimal CalculateTotalCreditAmount(IEnumerable<EntryDetailGeneralRecord> entries)
        {
            Contract.Assert(entries != null);
            var result = entries.Select(e => e.Amount).Sum();
            return result;
        }

        private static decimal CalculateTotalDebitAmount(IEnumerable<EntryDetailGeneralRecord> entries)
        {
            Contract.Assert(entries != null);
            var result = entries.Select(e => e.Amount).Sum();
            return result;
        }

        #endregion

        #endregion

        #region FileControlRecord

        private static FileControlRecord CreateFileControlRecord(ICollection<GeneralBatch> batches)
        {
            Contract.Assert(batches != null);
            var batchCount = batches.Count;
            var result = new FileControlRecord
            {
                BatchCount = batchCount,
                BlockCount = batchCount + 2,
                EntryAndAddendaCount = CalculateEntryCount(batches),
                EntryHash = GenerateEntryHash(batches),
                TotalCreditAmount = CalculateTotalCreditAmount(batches),
                TotalDebitAmount = CalculateTotalDebitAmount(batches)
            };

            return result;
        }

        private static long CalculateEntryCount(IEnumerable<GeneralBatch> batches)
        {
            Contract.Assert(batches != null);
            var entryCount = batches.Select(b => b.Control.EntryAndAddendaCount).Sum();
            return entryCount;
        }

        private static string GenerateEntryHash(IEnumerable<GeneralBatch> batches)
        {
            Contract.Assert(batches != null);

            var entryHash = batches
                .Select(o => Convert.ToInt64(o.Control.EntryHash))
                .Sum()
                .ToString(CultureInfo.InvariantCulture);
            if (entryHash.Length > 10)
            {
                entryHash = entryHash.Substring(entryHash.Length - 10, 10);
            }

            return entryHash;
        }

        private static decimal CalculateTotalCreditAmount(IEnumerable<GeneralBatch> batches)
        {
            Contract.Assert(batches != null);
            var amount = batches.Select(b => b.Control.TotalCreditAmount).Sum();
            return amount;
        }

        private static decimal CalculateTotalDebitAmount(IEnumerable<GeneralBatch> batches)
        {
            Contract.Assert(batches != null);
            var amount = batches.Select(b => b.Control.TotalDebitAmount).Sum();
            return amount;
        }

        #endregion

        #endregion
    }
}