using System;

namespace Ach.Fulfillment.Nacha.Exception
{
	public class FieldNullException : ArgumentNullException
	{ 
		public FieldNullException() : base()
		{}

		public FieldNullException(string fieldName) : base(fieldName)
		{}

		public FieldNullException(string fieldName, string message) : base(fieldName, message)
		{}

		public FieldNullException(string message, System.Exception innerException) : base(message, innerException)
		{}
	}
}
