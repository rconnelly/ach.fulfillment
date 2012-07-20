namespace Ach.Fulfillment.Api.Common
{
    using System;

    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    using ServiceStack.ServiceInterface;

    internal abstract class AppRestServiceBase<T> : RestServiceBase<T>
    {
        #region Methods

        protected override object HandleException(T request, Exception ex)
        {
            ex = HandleException(ex);
            return base.HandleException(request, ex);
        }

        private static Exception HandleException(Exception ex)
        {
            var exceptionToProcess = ex;
            Exception exceptionToThrow;
            var rethrow = ExceptionPolicy.HandleException(ex, "Communication.Api", out exceptionToThrow);
            if (rethrow && exceptionToThrow != null)
            {
                exceptionToProcess = exceptionToThrow;
            }

            return exceptionToProcess;
        }

        #endregion
    }
}