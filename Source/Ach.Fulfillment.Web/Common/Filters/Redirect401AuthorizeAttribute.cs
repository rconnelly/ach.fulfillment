namespace Ach.Fulfillment.Web.Common.Filters
{
    using System;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class Redirect401AuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            throw new HttpException((int)HttpStatusCode.Unauthorized, "You are not authorized to see this page");
        }
    }
}