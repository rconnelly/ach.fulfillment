namespace Ach.Fulfillment.Business.Impl.Strategies
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

    internal static class AchFileEntityExtension
    {
        public static void ToStream(this INachaFileData data, IEnumerable<AchTransactionEntity> transactions, Stream stream)
        {
            Contract.Assert(data != null);
            Contract.Assert(stream != null);

            var content = data.ToNachaContent(transactions);
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
        }

        // todo: it wastes too many resources - transform to the same nacha hierarchy + serialize hierarchy to string
        public static string ToNachaContent(this INachaFileData data, IEnumerable<AchTransactionEntity> transactions)
        {
            Contract.Assert(data != null);
            Contract.Assert(transactions != null);
            var file = data.ToNacha(transactions);
            var achFileString = file.Serialize();
            return achFileString;
        }

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

            // todo (ng): needs to be changed to settelment date??
            var batches = transactions
                .GroupBy(tt => tt.EntryDescription)
                .SelectMany((g, i) => CreateBatchRecords(i, data, g.Key, g))
                .ToList();

            return batches;
        }

        private static IEnumerable<GeneralBatch> CreateBatchRecords(int batchNumber, INachaFileData data, string description, IEnumerable<AchTransactionEntity> transactions)
        {
            Contract.Assert(data != null);
            Contract.Assert(transactions != null);

            // ReSharper disable ImplicitlyCapturedClosure
            var enumerable =
                from t in transactions
                group t by t.EntryClassCode into g
                let header = CreateBatchHeader(batchNumber, data, description, g.Key)
                let entries = CreateBatchEntries(g)
                let control = CreateBatchControlRecord(batchNumber, header, entries)
                select new GeneralBatch
                            {
                                Header = header, 
                                Entries = entries, 
                                Control = control
                            };
            // ReSharper restore ImplicitlyCapturedClosure
            return enumerable;
        }

        #region BatchHeaderGeneralRecord

        private static BatchHeaderGeneralRecord CreateBatchHeader(int batchNumber, INachaFileData data, string description, string entryClassCode)
        {
            Contract.Assert(data != null);
            Contract.Assert(entryClassCode != null);

            // todo: can fail because entryClassCode it is any 3 chars
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
            Contract.Assert(transaction.TransactionCode != null);
            Contract.Assert(transaction.TransitRoutingNumber != null);
            
            // todo: how it works if TransactionCode = "22"
            var transactionCode = (TransactionCode)Enum.Parse(typeof(TransactionCode), transaction.TransactionCode);
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
            Contract.Assert(entries != null);
            var entryHash = entries
                .Select(e => Convert.ToInt32(e.RdfiRoutingTransitNumber))
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

            // todo (ng): calculate right EntryHash
            var entryHash = batches
                .Select(o => Convert.ToInt32(o.Control.EntryHash))
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
    }
}