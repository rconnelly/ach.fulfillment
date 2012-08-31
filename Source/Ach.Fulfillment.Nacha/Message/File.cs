using System;
using System.Collections.Generic;
using Ach.Fulfillment.Nacha.Attribute;
using Ach.Fulfillment.Nacha.Record;
using Ach.Fulfillment.Nacha.Serialization;

namespace Ach.Fulfillment.Nacha.Message
{
	[Serializable]
	public class File : SerializableBase
	{
		[Record(Position = 0, IsRequired = true, RecordType = "1", Postfix = "\n")]
		public FileHeaderRecord Header;

		[Record(Position = 1, IsRequired = true, RecordType = "5")]
		public List<GeneralBatch> Batches;

		[Record(Position = 2, IsRequired = true, RecordType = "9", Postfix = "\n")]
		public FileControlRecord Control;
	}
}
