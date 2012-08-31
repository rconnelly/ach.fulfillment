using System;
using System.Collections.Generic;
using Ach.Fulfillment.Nacha.Attribute;
using Ach.Fulfillment.Nacha.Record;
using Ach.Fulfillment.Nacha.Serialization;

namespace Ach.Fulfillment.Nacha.Message
{
	[Serializable]
	public class GeneralBatch : SerializableBase
	{
		[Record(Position = 0, IsRequired = true, RecordType="5", Postfix = "\n")]
		public BatchHeaderGeneralRecord Header;

		[Record(Position = 1, IsRequired = true, RecordType = "6")]
		public List<EntryDetailGeneralRecord> Entries;

		[Record(Position = 2, IsRequired = true, RecordType = "8", Postfix = "\n")]
		public BatchControlRecord Control;
	}
}
