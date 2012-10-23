namespace Ach.Fulfillment.Nacha.Record
{
    using System;

    using Ach.Fulfillment.Nacha.Attribute;
    using Ach.Fulfillment.Nacha.Enumeration;
    using Ach.Fulfillment.Nacha.Serialization;

    [Serializable]
	public class FileHeaderRecord : SerializableBase
	{
		/// <summary>
		/// Code identifying the File Header Record is "1"
		/// </summary>
		[Field(Position = 0, DataType = DataType.Alphanumeric, Length = 1, IsRequired = true, MaskOnAudit = false)]
		internal string RecordType = "1";

		/// <summary>
		/// Currently, only "01" is used
		/// </summary>
		[Field(Position = 1, DataType = DataType.Alphanumeric, Length = 2, IsRequired = true, MaskOnAudit = false)]
		internal string PriorityCode = "01";

		/// <summary>
		/// Number that identifies the Bank of America site where we will process your files.
		/// </summary>
		/// <value>
		/// Possible values include ("b" indicates a blank space):
		/// <list type="bullet">
		/// <item><description>b111000025 = Dallas</description></item>
		/// <item><description>b051000017 = Richmond</description></item>
		/// <item><description>b121108250 = San Francisco</description></item>
		/// <item><description>b011900254 = Northeast</description></item>
		/// </list>
		/// </value>
		/// <remarks>
		/// Bank of America will assign this. You will be instructed with the correct Immediate Destination Field for your file. 
		/// </remarks>
		[Field(Position = 2, DataType = DataType.Alphanumeric, Length = 10, IsRequired = true, MaskOnAudit = false, PaddingType = PaddingType.SpacePadLeft)]
		public string ImmediateDestination;

		/// <summary>
		/// Your 10-digit company number assigned by Bank of America.
		/// </summary>
		[Field(Position = 3, DataType = DataType.Alphanumeric, Length = 10, IsRequired = true, MaskOnAudit = false)]
		public string ImmediateOrigin;

		/// <summary>
		/// The date and time you create or transmit the input file.
		/// </summary>
		[Field(Position = 4, DataType = DataType.Date, Length = 10, IsRequired = true, MaskOnAudit = false, FormattingString = "yyMMddHHmm")]
		public DateTime FileCreationDateTime;

		/// <summary>
		/// Code to distinguish among multiple input files sent per day. Label the first (or only) file "A" (or "0") and continue in sequence.
		/// </summary>
		/// <value>
		/// Value should be in one of the following formats:
		/// <list type="bullet">
		/// <item><description>A-Z (Upper Case)</description></item>
		/// <item><description>0-9</description></item>
		/// </list>
		/// </value>
		[Field(Position = 5, DataType = DataType.Alphanumeric, Length = 1, IsRequired = true, MaskOnAudit = false)]
		public string FileIdModifier;

		/// <summary>
		/// Number of bytes per record-always 94.
		/// </summary>
		[Field(Position = 6, DataType = DataType.Numeric, Length = 3, IsRequired = true, MaskOnAudit = false, PaddingType = PaddingType.Default)]
		internal long RecordSize = 94;

		/// <summary>
		/// Number of records per block
		/// </summary>
		[Field(Position = 7, DataType = DataType.Numeric, Length = 2, IsRequired = true, MaskOnAudit = false, PaddingType = PaddingType.Default)]
		internal long BlockingFactor = 10;

		/// <summary>
		/// Currently only "1" is used
		/// </summary>
		[Field(Position = 8, DataType = DataType.Alphanumeric, Length = 1, IsRequired = true, MaskOnAudit = false)]
		internal string FormatCode = "1";

		/// <summary>
		/// Identifies the Bank of America processing site as the destination.
		/// </summary>
		/// <value>
		/// Values are:
		/// <list type="bullet">
		/// <item><description>Bank of America DAL = Dallas</description></item>
		/// <item><description>Bank of America RIC = Richmond</description></item>
		/// <item><description>Bank of America SFO = San Francisco</description></item>
		/// <item><description>Bank of America NE = Northeast</description></item>
		/// </list>
		/// </value>
		[Field(Position = 9, DataType = DataType.Alphanumeric, Length = 23, IsRequired = true, MaskOnAudit = false, PaddingType = PaddingType.Default)]
		public string Destination;

		/// <summary>
		/// Your company‟s name, up to 23 characters including spaces.
		/// </summary>
		[Field(Position = 10, DataType = DataType.Alphanumeric, Length = 23, IsRequired = true, MaskOnAudit = false, PaddingType = PaddingType.Default)]
		public string OriginOrCompanyName;

		/// <summary>
		/// You may use this field to describe the input file for internal accounting purposes or fill with spaces. Blanks are not allowed.
		/// </summary>
		[Field(Position = 11, DataType = DataType.Alphanumeric, Length = 8, IsRequired = false, MaskOnAudit = false, PaddingType = PaddingType.Default)]
		public string ReferenceCode;
	}
}
