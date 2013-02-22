namespace Ach.Fulfillment.Web.Common.Data
{
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Ach.Fulfillment.Business.Exceptions;

    internal static class OperationErrorExtension
    {
        public static void Fill(this OperationError operationError, BusinessValidationException ex)
        {
            Contract.Assert(operationError != null);
            var r = from e in ex.Errors
                    select
                        new OperationErrorEntry
                            {
                                ErrorCode = e.ErrorCode,
                                FieldName = e.PropertyName,
                                Message = e.ErrorMessage
                            };
            operationError.Errors = r.ToArray();
            operationError.Message = operationError.Errors.Select(e => e.Message).FirstOrDefault() ?? ex.Message;
            operationError.ErrorCode = ex.GetType().Name;
        }
    }
}