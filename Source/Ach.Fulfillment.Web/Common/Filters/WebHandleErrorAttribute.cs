namespace Ach.Fulfillment.Web.Common.Filters
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Web;
    using System.Web.Mvc;

    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Web.Configuration;

    using global::Common.Logging;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    internal class WebHandleErrorAttribute : HandleErrorAttribute
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

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
            if (!this.ExceptionType.IsInstanceOfType(exception))
            {
                return;
            }

            // avoid showing death screen even for not 500 error in production
            if (new HttpException(null, exception).GetHttpCode() != 500)
            {
                Log.Error("Server error has been occured while processing page", exception);

                var controllerName = (string) filterContext.RouteData.Values["controller"];
                var actionName = (string)filterContext.RouteData.Values["action"];
                var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
                filterContext.Result = new ViewResult
                    {
                        // if view is not defined - use action name by default
                        ViewName = "Error",
                        ViewData = new ViewDataDictionary(model)
                    };
                filterContext.ExceptionHandled = true;

                return;
            }

            filterContext.Exception = exception.TransformException(WebContainerExtension.DefaultPolicy);

            base.OnException(filterContext);
        }
    }
}