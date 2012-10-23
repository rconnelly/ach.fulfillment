namespace Ach.Fulfillment.Nacha.Exception
{
    using System;

    public class FieldNullException : ArgumentNullException
    {
        public FieldNullException()
        {
        }

        public FieldNullException(string fieldName)
            : base(fieldName)
        {
        }

        public FieldNullException(string fieldName, string message) : base(fieldName, message)
        {
        }

        public FieldNullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
