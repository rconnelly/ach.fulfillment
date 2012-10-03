﻿using System;
using Ach.Fulfillment.Nacha.Attribute;
using Ach.Fulfillment.Nacha.Enumeration;
using Ach.Fulfillment.Nacha.Serialization;

namespace Ach.Fulfillment.Nacha.Record
{
	[Serializable]
	public class EntryDetailGeneralRecord : SerializableBase
	{
		/// <summary>
		/// Code identifying the Entry Detail Record is "6"
		/// </summary>
		[Field(Position = 0, DataType = DataType.Alphanumeric, Length = 1, IsRequired = true, MaskOnAudit = false)]
		internal string RecordType = "6";

		/// <summary>
		/// Two-digit code that identifies checking and savings	accountcredits/debitsorprenotes.
		/// </summary>
		/// <value>Valid codes are:
		/// <list type="bullet">
		///	<item><description>22 = Automated deposit (checking credit)</description></item>
		///	<item><description>23 = Prenote of checking credit</description></item>
		///	<item><description>24 = Zero-dollar checking credit with remittance data (CCD & CTX entries only)</description></item>
		///	<item><description>27 = Automated payment (checking debit)</description></item>
		///	<item><description>28 = Prenote of checking debit</description></item>
		///	<item><description>29 = Zero-dollar checking debit with remittance data (CCD & CTX entries only)</description></item>
		///	<item><description>32 = Automated deposit (savings credit)</description></item>
		///	<item><description>33 = Prenote of savings credit</description></item>
		///	<item><description>34 = Zero-dollar savings credit with remittance data (CCD & CTX entries only)</description></item>
		///	<item><description>37 = Automated payment (savings debit)</description></item>
		///	<item><description>38 = Prenote of savings debit</description></item>
		///	<item><description>39 = Zero-dollar savings debit with remittance data (CCD & CTX entries only)</description></item>
		///	</list>
		///	</value>
		[Field(Position = 1, DataType = DataType.Numeric, Length = 2, IsRequired = true, MaskOnAudit = false)]
		public TransactionCode TransactionCode;

		/// <summary>
		/// Transit Routing number of the Receiver's financial institution.
		/// </summary>
		[Field(Position = 2, DataType = DataType.Numeric, Length = 8, IsRequired = true, MaskOnAudit = false)]
		public string RDFIRoutingTransitNumber;

		/// <summary>
		/// The ninth character in the Routing Transit number. Used to check for transpositions.
		/// </summary>
		[Field(Position = 3, DataType = DataType.Numeric, Length = 1, IsRequired = true, MaskOnAudit = false)]
		public string CheckDigit;

		/// <summary>
		/// Receiver's account number at the RDFI, a value generally found on the MICR line of a check. Enter the MICR Dash Cue Symbol as a hyphen ("-"). 
		/// Account numbers vary in format. If the account number has less than 17 characters, left-justify, blank-fill. Ignore any blank spaces within the account number.
		/// </summary>
		[Field(Position = 4, DataType = DataType.Alphanumeric, Length = 17, IsRequired = true, MaskOnAudit = false)]
		public string RDFIAccountNumber;

		/// <summary>
		/// Entry amount in dollars with two decimal places. Right-justified, left zero-filled, without a decimal point. Enter 10 zeros for non-dollar and prenote entries.
		/// </summary>
		[Field(Position = 5, DataType = DataType.Date, Length = 10, IsRequired = true, MaskOnAudit = false, Scale = 100)]
		public decimal Amount;

		/// <summary>
		/// This field contains the accounting number by which the Originator is known to the Receiver for descriptive purposes.
		///	NACHA Rules recommend but do not require the RDFI to print the contents of this field on the receiver's statement.
		/// </summary>
		[Field(Position = 6, DataType = DataType.Alphanumeric, Length = 15, IsRequired = false, MaskOnAudit = false)]
		public string IndividualIdentificationNumber;

		/// <summary>
		/// Name of Receiver.
		/// </summary>
		[Field(Position = 6, DataType = DataType.Alphanumeric, Length = 22, IsRequired = true, MaskOnAudit = false)]
		public string IndividualOrCompanyName;

		/// <summary>
		/// Use this field only if requested and the Bank has implemented draft production for your company and only if the Transaction Codes "27" or "37" 
		/// are present in Field 2 of this record. Bank of America Draft Indicator
		/// </summary>
		/// <value>Valid codes are:
		/// <list type="bullet">
		///	<item><description>"bb" = Electronic only</description></item>
		///	<item><description>"1*" = Preauthorized check only</description></item>
		///	<item><description>"  " For all other entries</description></item>
		///	</list>
		///	</value>
		///	<remarks>
		/// If you plan to use this feature, please contact Customer Service. 
		/// </remarks>
		[Field(Position = 7, DataType = DataType.Alphanumeric, Length = 2, IsRequired = false, MaskOnAudit = false)]
		public string DiscretionaryDataField;

		/// <summary>
		/// Indicates if addenda data is present for record
		/// </summary>
		/// <value>Valid codes are:
		/// <list type="bullet">
		///	<item><description>"0" = No addenda supplied.</description></item>
		///	<item><description>"1" = One addenda included.</description></item>
		///	</list>
		///	</value>
		[Field(Position = 8, DataType = DataType.Numeric, Length = 1, IsRequired = true, MaskOnAudit = false)]
		public string AddendaRecordIndicator;		
	}
}