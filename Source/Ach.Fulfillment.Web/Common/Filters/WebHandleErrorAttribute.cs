namespace Ach.Fulfillment.Web.Common.Filters
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Web;
    using System.Web.Mvc;

    using Ach.Fulfillment.Web.Configuration;

    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    internal class WebHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Contract.Assert(filterContext != null);
            if (filterContext.IsChildAction)
            {
                return;
            }

            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            var exception = filterContext.Exception;
            if (new HttpException(null, exception).GetHttpCode() != 500)
            {
                return;
            }

            if (!this.ExceptionType.IsInstanceOfType(exception))
            {
                return;
            }

            filterContext.Exception = TransformException(exception);

            base.OnException(filterContext);
        }

        private static Exception TransformException(Exception ex)
        {
            var exceptionToProcess = ex;
            if (ex != null)
            {
                Exception exceptionToThrow;
                var rethrow = ExceptionPolicy.HandleException(ex, WebContainerExtension.DefaultPolicy, out exceptionToThrow);
                if (rethrow && exceptionToThrow != null)
                {
                    exceptionToProcess = exceptionToThrow;
                }
            }

            return exceptionToProcess;
        }
    }
}