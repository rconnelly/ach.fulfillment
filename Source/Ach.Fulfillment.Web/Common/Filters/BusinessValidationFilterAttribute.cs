namespace Ach.Fulfillment.Web.Common.Filters
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Web.Configuration;

    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    // we could use Controller.OnException but I havn't found the way how to know what view is being rendered if it's not the same as action
    internal class BusinessValidationFilterAttribute : ActionFilterAttribute
    {
        private readonly string view;

        public BusinessValidationFilterAttribute()
            : this(null)
        {
        }

        public BusinessValidationFilterAttribute(string view)
        {
            this.view = view;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var transformedException = TransformException(filterContext.Exception);
            var exception = transformedException as BusinessValidationException;
            if (exception != null)
            {
                // put error into ViewData for custom handling - it's not used right now
                filterContext.Controller.ViewData.Add("Error", exception.Errors);

                FillModelState(filterContext.Controller.ViewData.ModelState, exception);

                filterContext.ExceptionHandled = true;
                filterContext.Result = new ViewResult
                    {
                        // if view is not defined - use action name by default
                        ViewName = this.view ?? filterContext.RouteData.Values["action"].ToString(),
                        TempData = filterContext.Controller.TempData,
                        ViewData = filterContext.Controller.ViewData
                    };
            }
        }

        private static Exception TransformException(Exception ex)
        {
            var exceptionToProcess = ex;
            if (ex != null)
            {
                Exception exceptionToThrow;
                var rethrow = ExceptionPolicy.HandleException(ex, WebContainerExtension.ValidationPolicy, out exceptionToThrow);
                if (rethrow && exceptionToThrow != null)
                {
                    exceptionToProcess = exceptionToThrow;
                }
            }

            return exceptionToProcess;
        }

        private static void FillModelState(ModelStateDictionary modelStateDictionary, BusinessValidationException exception)
        {
            var pairs = (from info in exception.Errors
                         let key = !string.IsNullOrEmpty(info.PropertyName) ? info.PropertyName : info.ErrorCode
                         select new { key, info.ErrorMessage }).ToList();

            foreach (var p in pairs.Where(i => !string.IsNullOrEmpty(i.key)))
            {
                // try to match business property name and model property name
                modelStateDictionary.AddModelError(p.key, p.ErrorMessage);
            }

            var messages = pairs.Where(i => string.IsNullOrEmpty(i.key)).Select(i => i.ErrorMessage).ToArray();
            if (messages.Length > 0)
            {
                modelStateDictionary.AddModelError(string.Empty, string.Join("\r\n", messages));
            }
        }
    }
}