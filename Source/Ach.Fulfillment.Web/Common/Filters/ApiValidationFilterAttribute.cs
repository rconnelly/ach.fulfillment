namespace Ach.Fulfillment.Web.Common.Filters
{
    using System.Net;
    using System.Web.Mvc;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Web.Common.Data;
    using Ach.Fulfillment.Web.Configuration;

    internal class ApiValidationFilterAttribute : ActionFilterAttribute
    {
        public ApiValidationFilterAttribute()
        {
            this.Order = 10;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception == null)
            {
                return;
            }

            var transformedException = filterContext.Exception.TransformException(WebContainerExtension.ApiValidationPolicy);

            HttpStatusCode statusCode;
            OperationError operationError;

            var exception = transformedException as BusinessValidationException;
            if (exception != null)
            {
                operationError = new OperationError();
                operationError.Fill(exception);
                
                statusCode = HttpStatusCode.Conflict;
            }
            else
            {
                var message = (transformedException ?? filterContext.Exception).Message;
                operationError = new OperationError(HttpStatusCode.InternalServerError.ToString(), message);

                statusCode = HttpStatusCode.InternalServerError;
            }

            filterContext.Result = new JsonResult { Data = operationError };
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = (int)statusCode;
        }
    }
}