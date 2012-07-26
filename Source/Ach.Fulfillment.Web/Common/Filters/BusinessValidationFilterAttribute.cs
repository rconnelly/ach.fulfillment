namespace Ach.Fulfillment.Web.Common.Filters
{
    using System.Linq;
    using System.Web.Mvc;

    using Ach.Fulfillment.Business.Exceptions;
    using Ach.Fulfillment.Common.Exceptions;
    using Ach.Fulfillment.Web.Configuration;

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
            var transformedException = filterContext.Exception.TransformException(WebContainerExtension.ValidationPolicy);
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

        // TODO (AS) temporary make it public
        public static void FillModelState(ModelStateDictionary modelStateDictionary, BusinessValidationException exception)
        {
            var pairs = (from info in exception.Errors
                         let property = !string.IsNullOrEmpty(info.PropertyName) ? info.PropertyName : info.ErrorCode
                         let index = property.LastIndexOf('.')
                         let key = index > 0 && index < property.Length ? property.Substring(index + 1) : property
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