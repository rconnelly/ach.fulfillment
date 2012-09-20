using System;
using Ach.Fulfillment.Nacha.Attribute;
using Ach.Fulfillment.Nacha.Enumeration;
using Ach.Fulfillment.Nacha.Serialization;

namespace Ach.Fulfillment.Nacha.Record
{
	[Serializable]
	public class FileControlRecord : SerializableBase
	{
		/// <summary>
		/// Code identifying the File Control Record is "9"
		/// </summary>
		[Field(Position = 0, DataType = DataType.Alphanumeric, Length = 1, IsRequired = true, MaskOnAudit = false)]
		internal string RecordType = "9";

		/// <summary>
		/// Total number of Company/Batch Header Records (Record Type "5") in the file.
		/// </summary>
		[Field(Position = 1, DataType = DataType.Numeric, Length = 6, IsRequired = true, MaskOnAudit = false)]
		public long BatchCount;

		/// <summary>
		/// Total number of physical blocks (records) in the file, including the File Header and File Control Records.
		/// </summary>
		[Field(Position = 2, DataType = DataType.Numeric, Length = 6, IsRequired = true, MaskOnAudit = false)]
		public long BlockCount;

		/// <summary>
		/// Total number of Entry Detail and Addenda Records (Record Types "6" and "7") in the file.
		/// </summary>
		[Field(Position = 3, DataType = DataType.Numeric, Length = 8, IsRequired = true, MaskOnAudit = false)]
		public long EntryAndAddendaCount;

		/// <summary>
		/// Total of eight-character Transit Routing/ABA numbers in the file (field 3 of the Entry Detail Record). 
		/// Do not include the Transit Routing Check Digit. Enter the ten low-order (right most) digits of this number.
		/// </summary>
		/// <example>
		/// For example, if the sum were 112233445566, you would enter 2233445566.
		/// </example>
		[Field(Position = 4, DataType = DataType.Numeric, Length = 10, IsRequired = true, MaskOnAudit = false)]
		public string EntryHash;

		/// <summary>
		/// Dollar total of debit entries in the file.
		/// </summary>
		[Field(Position = 5, DataType = DataType.Numeric, Length = 12, IsRequired = true, MaskOnAudit = false, Scale = 100)]
		public decimal TotalDebitAmount;

		/// <summary>
		/// Dollar total of credit entries in the file.
		/// </summary>
		[Field(Position = 6, DataType = DataType.Numeric, Length = 12, IsRequired = true, MaskOnAudit = false, Scale = 100)]
		public decimal TotalCreditAmount;

		/// <summary>
		/// Leave blank
		/// </summary>
		[Field(Position = 7, DataType = DataType.Alphanumeric, Length = 39, IsRequired = false, MaskOnAudit = false)]
		public string Reserved_01;
	}
}
