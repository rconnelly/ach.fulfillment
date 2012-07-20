namespace Ach.Fulfillment.Web.Common.Filters
{
    using System.Web.Mvc;

    using Ach.Fulfillment.Business.Exceptions;

    // we could use Controller.OnException but I havn't found the way how to know what view is being rendered if it's not the same as action
    public class BusinessValidationFilterAttribute : ActionFilterAttribute
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
            var exception = filterContext.Exception as BusinessValidationException;
            if (exception != null)
            {
                // put error into ViewData for custom handling - it's not used right now
                filterContext.Controller.ViewData.Add("Error", exception.Errors);

                foreach (var info in exception.Errors)
                {
                    // try to match business property name and model property name
                    filterContext.Controller.ViewData.ModelState.AddModelError(info.PropertyName, info.ErrorMessage);
                }

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
    }
}