using System;
using Ach.Fulfillment.Nacha.Attribute;
using Ach.Fulfillment.Nacha.Enumeration;
using Ach.Fulfillment.Nacha.Serialization;

namespace Ach.Fulfillment.Nacha.Record
{
	[Serializable]
	public class BatchHeaderGeneralRecord : SerializableBase
	{
		/// <summary>
		/// Code identifying the Company/Batch Header Record is "5"
		/// </summary>
		[Field(Position = 0, DataType = DataType.Alphanumeric, Length = 1, IsRequired = true, MaskOnAudit = false)]
		internal string RecordType = "5";

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
		[Field(Position = 1, DataType = DataType.Numeric, Length = 3, IsRequired = true, MaskOnAudit = false)]
		public ServiceClassCode ServiceClassCode;

		/// <summary>
		/// Your company name. NACHA rules require the RDFI to print this value on the receiver's statement so you will want to make this value as clear as possible.
		/// </summary>
		[Field(Position = 2, DataType = DataType.Alphanumeric, Length = 16, IsRequired = true, MaskOnAudit = false)]
		public string CompanyName;

		/// <summary>
		/// For your company's internal use. If you	include a value in this field, and your settlement occurs at the batch level, 
		/// or your returns settle at the item level, we will report the value in the settlement entries we create.
		/// </summary>
		[Field(Position = 3, DataType = DataType.Alphanumeric, Length = 20, IsRequired = false, MaskOnAudit = false)]
		public string CompanyDiscretionaryData;

		/// <summary>
		/// Your 10-digit company number assigned by Bank of America.
		/// </summary>
		[Field(Position = 4, DataType = DataType.Numeric, Length = 10, IsRequired = true, MaskOnAudit = false)]
		public string CompanyIdentification;

		/// <summary>
		/// A mnemonic, designated by NACHA, which permits entries to be distinguished. Identifies the specific computer record format used to carry 
		/// payment and payment-related information.
		/// </summary>
		[Field(Position = 5, DataType = DataType.Alphanumeric, Length = 3, IsRequired = true, MaskOnAudit = false)]
		public StandardEntryClassCode StandardEntryClassCode;

		/// <summary>
		/// You establish the value of this field to provide a description to be displayed to the Receiver. 
		/// Description should describe the purpose of the entries, such as "PAYROLL" or "ECHECKPAY" for consumer entries or "TRADE PAY" for corporate receivers. 
		/// NACHA Rules require that RDFIs print this value on the Receiver's account statement.
		/// </summary>
		[Field(Position = 6, DataType = DataType.Alphanumeric, Length = 10, IsRequired = true, MaskOnAudit = false)]
		public string CompanyEntryDescription;

		/// <summary>
		/// Description you choose to identify the date. NACHA recommends, but does not require, that RDFIs print this value on the receiver‟s statement.
		/// </summary>
		[Field(Position = 7, DataType = DataType.Alphanumeric, Length = 6, IsRequired = false, MaskOnAudit = false)]
		public string CompanyDescriptiveDate;

		/// <summary>
		/// Date you desire funds to post to receiver's	account.
		/// </summary>
		[Field(Position = 8, DataType = DataType.Date, Length = 6, IsRequired = true, MaskOnAudit = false, FormattingString = "yyMMdd")]
		public DateTime EffectiveEntryDate;

		/// <summary>
		/// Description you choose to identify the date. NACHA recommends, but does not require, that RDFIs print this value on the receiver‟s statement.
		/// </summary>
		[Field(Position = 9, DataType = DataType.Alphanumeric, Length = 3, IsRequired = false, MaskOnAudit = false)]
		internal string SettlementDate;
		
		/// <summary>
		/// Identifies the originator as a non Federal Government.
		/// </summary>
		[Field(Position = 10, DataType = DataType.Alphanumeric, Length = 1, IsRequired = true, MaskOnAudit = false)]
		internal string OriginatorStatusCode = "1";

		/// <summary>
		/// We will assign number based on where you will deliver your files
		/// </summary>
		/// <value>
		/// Possible values include ("b" indicates a blank space):
		/// <list type="bullet">
		/// <item><description>11100002 = Dallas</description></item>
		/// <item><description>05100001 = Richmond</description></item>
		/// <item><description>12110825 = San Francisco</description></item>
		/// <item><description>01190025 = Northeast</description></item>
		/// </list>
		/// </value>
		[Field(Position = 11, DataType = DataType.Numeric, Length = 8, IsRequired = true, MaskOnAudit = false)]
		public string OriginatingDFIIdentification;

		/// <summary>
		/// Assign batch numbers in ascending order	within each file.
		/// </summary>
		[Field(Position = 12, DataType = DataType.Numeric, Length = 7, IsRequired = true, MaskOnAudit = false)]
		public int BatchNumber;
	}
}
