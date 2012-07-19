namespace Ach.Fulfillment.Web.Common.Filters
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    using Ach.Fulfillment.Common.Security;
    using Ach.Fulfillment.Common.Utils;
    using Ach.Fulfillment.Data;

    using global::Common.Logging;

    using Microsoft.Practices.ServiceLocation;

    // TODO: why not authorization attribute
    public class PrincipalRightPermissionAttribute : ActionFilterAttribute
    {
        #region Constants and Fields

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors and Destructors

        // TODO: we do not need add. add it is just several attributes
        public PrincipalRightPermissionAttribute(params AccessRight[] allowRightOr)
        {
            this.AllowRightsOr = allowRightOr;
        }

        #endregion

        #region Public Properties

        public AccessRight[] AllowRightsAnd { get; set; }

        public AccessRight[] AllowRightsOr { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // in general should not access this code since we control access by web.config
                Log.Fatal("Unauthenticated access to the restricted controller action has been occured.");

                // use the current url for the redirect
                var link = filterContext.HttpContext.Request.Url != null
                               ? filterContext.HttpContext.Request.Url.AbsolutePath
                               : string.Empty;
                
                // send them off to the login page
                var loginUrl = "{0}?ReturnUrl={1}".Fmt(FormsAuthentication.LoginUrl, link);
                filterContext.HttpContext.Response.Redirect(loginUrl, true);
            }
            else
            {
                var principal = ServiceLocator.Current.GetInstance<IApplicationPrincipal>();

                var allowAccess = false;

                // check OR rights
                if (this.AllowRightsOr != null) 
                {
                    foreach (var systemRight in this.AllowRightsOr)
                    {
                        var right = systemRight;
                        allowAccess = principal.HasPermission(right.ToString());
                        if (allowAccess)
                        {
                            break;
                        }
                    }
                }

                // check AND rights
                else if (this.AllowRightsAnd != null) 
                {
                    foreach (var systemRight in this.AllowRightsAnd)
                    {
                        var right = systemRight;
                        allowAccess = principal.HasPermission(right.ToString());
                        if (!allowAccess)
                        {
                            break;
                        }
                    }
                }

                if (!allowAccess)
                {
                    // don't want to show user that page exists - thus throw 404 instead of access deny
                    throw new HttpException(404, "Requested page was not found", new UnauthorizedAccessException("You are not authorized to view this page"));
                }
            }
        }

        #endregion
    }
}