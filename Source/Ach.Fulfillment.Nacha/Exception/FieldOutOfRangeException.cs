namespace Ach.Fulfillment.Nacha.Exception
{
    public class FieldOutOfRangeException : System.ArgumentOutOfRangeException
    {
        public FieldOutOfRangeException()
        {
        }

        public FieldOutOfRangeException(string fieldName)
            : base(fieldName)
        {
        }

        public FieldOutOfRangeException(string fieldName, string message)
            : base(fieldName, message)
        {
        }

        public FieldOutOfRangeException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        public FieldOutOfRangeException(string fieldName, object actualValue, string message)
            : base(fieldName, actualValue, message)
        {
        }

        public FieldOutOfRangeException(
            System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
