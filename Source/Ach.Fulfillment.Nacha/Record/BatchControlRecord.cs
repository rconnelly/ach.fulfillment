using System;
using Ach.Fulfillment.Nacha.Attribute;
using Ach.Fulfillment.Nacha.Enumeration;
using Ach.Fulfillment.Nacha.Serialization;

namespace Ach.Fulfillment.Nacha.Record
{
	[Serializable]
	public class BatchControlRecord : SerializableBase
	{
		/// <summary>
		/// Code identifying the Company/Batch Control Record is "8"
		/// </summary>
		[Field(Position = 0, DataType = DataType.Alphanumeric, Length = 1, IsRequired = true, MaskOnAudit = false)]
		internal string RecordType = "8";

		/// <summary>
		/// Identifies the type of entries in the batch.
		/// </summary>
		/// <value>
		/// Valid values:
		/// <list type="bullet">
		/// <item><description>Code "200" indicates a mixed batch (one containing debit and/or credit entries)</description></item>
		/// <item><description>Code "220"  is for credits only</description></item>
		/// <item><description>Code "225" is for debits only.</description></item>
		/// </list>
		/// </value>
		/// <remarks>Must match the value you used in the Batch Header Record</remarks>
		[Field(Position = 1, DataType = DataType.Numeric, Length = 3, IsRequired = true, MaskOnAudit = false)]
		public ServiceClassCode ServiceClassCode;

		/// <summary>
		/// Total number of Entry Detail Records plus addenda records (Record Types "6" and "7") in the batch
		/// </summary>
		[Field(Position = 2, DataType = DataType.Numeric, Length = 6, IsRequired = true, MaskOnAudit = false)]
		public long EntryAndAddendaCount;

		/// <summary>
		/// Total of eight-character Transit Routing/ABA numbers in the batch (field 3 of the Entry Detail Record). 
		/// Do not include the Transit Routing Check Digit. Enter the ten low-order (right most) digits of this number.
		/// </summary>
		/// <example>
		/// For example, if the sum were 112233445566, you would enter 2233445566.
		/// </example>
		[Field(Position = 3, DataType = DataType.Numeric, Length = 10, IsRequired = true, MaskOnAudit = false)]
		public string EntryHash;

		/// <summary>
		/// Dollar total of debit entries in the batch.
		/// </summary>
		[Field(Position = 4, DataType = DataType.Numeric, Length = 12, IsRequired = true, MaskOnAudit = false, Scale = 100)]
		public decimal TotalDebitAmount;

		/// <summary>
		/// Dollar total of credit entries in the batch.
		/// </summary>
		[Field(Position = 5, DataType = DataType.Numeric, Length = 12, IsRequired = true, MaskOnAudit = false, Scale = 100)]
		public decimal TotalCreditAmount;

		/// <summary>
		/// Your 10-digit company number assigned by Bank of America
		/// </summary>
		/// <remarks>Must match the value you used in the Batch Header Record</remarks>
		[Field(Position = 6, DataType = DataType.Numeric, Length = 10, IsRequired = true, MaskOnAudit = false)]
		public string CompanyIdentification;

		/// <summary>
		/// Leave blank
		/// </summary>
		[Field(Position = 7, DataType = DataType.Alphanumeric, Length = 19, IsRequired = false, MaskOnAudit = false)]
		public string MessageAuthenticationCode;

		/// <summary>
		/// Leave blank
		/// </summary>
		[Field(Position = 8, DataType = DataType.Alphanumeric, Length = 6, IsRequired = false, MaskOnAudit = false)]
		public string Reserved_01;

		/// <summary>
		/// We will assign number based on where you will deliver your files
		/// </summary>
		/// <value>
		/// Valid values:
		/// <list type="bullet">
		/// <item><description>11100002 = Dallas</description></item>
		/// <item><description>05100001 = Richmond</description></item>
		/// <item><description>12110825 = San Francisco</description></item>
		/// <item><description>01190025 = Northeast</description></item>
		/// </list>
		/// </value>
		[Field(Position = 9, DataType = DataType.Numeric, Length = 8, IsRequired = true, MaskOnAudit = false)]
		public string OriginatingDFIIdentification;

		/// <summary>
		/// Number of the batch for which this is a control record. Number the batches sequentially within each file.
		/// </summary>
		/// <remarks>Must match the value you used in the Batch Header Record</remarks>
		[Field(Position = 10, DataType = DataType.Numeric, Length = 7, IsRequired = true, MaskOnAudit = false)]
		public long BatchNumber;
	}
}
