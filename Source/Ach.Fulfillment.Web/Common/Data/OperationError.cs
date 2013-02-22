namespace Ach.Fulfillment.Web.Common.Data
{
    using System.Diagnostics.Contracts;

    internal class OperationError
    {
        public OperationError()
        {
            this.Errors = new OperationErrorEntry[0];
        }

        public OperationError(string code, string message) : this()
        {
            Contract.Assert(code != null);
            Contract.Assert(message != null);
            this.Message = message;
            this.ErrorCode = code;
        }

        public string ErrorCode { get; set; }

        public string Message { get; set; }

        public OperationErrorEntry[] Errors { get; set; }
    }
}