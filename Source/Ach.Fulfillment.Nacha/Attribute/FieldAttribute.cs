using System;
using Ach.Fulfillment.Nacha.Enumeration;
using Ach.Fulfillment.Shared.Reflection;

namespace Ach.Fulfillment.Nacha.Attribute
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class FieldAttribute : System.Attribute, IReflectAttribute
	{
	    public DataType DataType { get; set; }
		public PaddingType PaddingType { get; set; }
		public char PaddingCharacter { get; set; }
		public int Length { get; set; }
		public bool IsRequired { get; set; }
		public string Prefix { get; set; }
		public string Postfix { get; set; }
		public bool MaskOnAudit { get; set; }
		public string FormattingString { get; set; }
		public int Scale { get; set; }
		public int Precision { get; set; }

		#region IReflectAttribute Members

		public int Position { get; set; }

		#endregion
	}
}
