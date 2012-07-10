namespace Ach.Fulfillment.Business.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    using Ach.Fulfillment.Common.Exceptions;

    [Serializable]
    public class BusinessException : RootException
    {
        #region Constructors

        public BusinessException()
        {
        }

        public BusinessException(string message)
            : base(message)
        {
        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected BusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}