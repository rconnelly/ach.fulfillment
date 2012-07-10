namespace Ach.Fulfillment.Business.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using FluentValidation.Results;

    [Serializable]
    public class BusinessValidationException : BusinessException
    {
        #region Constructors and Destructors

        public BusinessValidationException(IList<ValidationFailure> errors)
            : base(BuildErrorMesage(errors))
        {
            this.Errors = errors;
        }

        public BusinessValidationException()
        {
        }

        public BusinessValidationException(string message)
            : base(message)
        {
        }

        public BusinessValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected BusinessValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Public Properties

        public IEnumerable<ValidationFailure> Errors { get; private set; }

        #endregion

        #region Methods

        private static string BuildErrorMesage(IEnumerable<ValidationFailure> errors)
        {
            var arr = (from x in errors select "\r\n -- " + x.ErrorMessage).ToArray<string>();
            return "Validation failed: " + string.Join(string.Empty, arr);
        }

        #endregion
    }
}