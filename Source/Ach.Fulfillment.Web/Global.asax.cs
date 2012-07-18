namespace Ach.Fulfillment.Web
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Security;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Business.Security;
    using Ach.Fulfillment.Common;
    using Ach.Fulfillment.Initialization.Configuration;
    using Ach.Fulfillment.Web.Common;

    using Microsoft.Practices.ServiceLocation;

    using global::Common.Logging;

    public class MvcApplication : HttpApplication
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private UnitOfWork unitOfWork;

        protected void Application_Start()
        {
            Log.Debug("Starting application");

            AreaRegistration.RegisterAllAreas();

            // config
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);

            // shell
            Shell.Start<InitializationContainerExtension>();
        }

        protected void Application_End()
        {
            Log.Debug("Ending application");

            Shell.Shutdown();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (this.unitOfWork == null)
            {
                this.unitOfWork = new UnitOfWork();
            }
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (!PrincipalRequired())
            {
                return;
            }

            this.InitializePrincipal();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (this.unitOfWork != null)
            {
                this.unitOfWork.Dispose();
                this.unitOfWork = null;
            }
        }

        protected void Application_Error()
        {
            var error = Server.GetLastError();
            try
            {
                var message = string.Format(CultureInfo.InvariantCulture, "Server error has been occured while processing page: {0} ", HttpContext.Current != null ? HttpContext.Current.Request.Url.ToString() : "unknown");
                Log.Error(message, error);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            // TODO consider handling http errors - code below doesn't work on post
            //this.HandleNotFound(error);
        }

        /* private void HandleNotFound(Exception exception)
        {

            var httpException = exception as HttpException;

            if (httpException != null)
            {
                var status = httpException.GetHttpCode();

                if (status == 404 || status == 403)
                {
                    var routeData = new RouteData();
                    routeData.Values.Add("controller", "Common");
                    routeData.Values.Add("area", "Common");
                    routeData.Values.Add("action", "Error" + status);

                    // Pass exception details to the target error View.
                    routeData.Values.Add("error", exception);

                    // Clear the error on server.
                    Server.ClearError();

                    // Avoid IIS7 getting in the middle
                    Response.StatusCode = status;
                    Response.TrySkipIisCustomErrors = true;

                    // Call target Controller and pass the routeData.
                    IController errorController = new CommonController();
                    errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                }
            }
        }*/

        private void InitializePrincipal ()
        {
            var cookieName = FormsAuthentication.FormsCookieName;
            var authCookie = HttpContext.Current.Request.Cookies[cookieName];

            if (authCookie == null)
            {
                return;
            }

            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            var login = authTicket.Name;

            var user = CacheHelper.GetOrAdd(
                login,
                () =>
                    {
                        var manager = ServiceLocator.Current.GetInstance<IUserManager>();
                        return manager.FindByLogin(login);
                    });

            Contract.Assert(user != null);
            Contract.Assert(user.UserPasswordCredential != null);

            var identity = new Business.Security.ApplicationIdentity(user.UserPasswordCredential.Login, user.Name, user.Email);
            var role = new PrincipalRole(user.Role.Name, user.Role.Permissions.Select(p => p.Name).ToArray());
            var principal = new ApplicationPrincipal(identity, role);

            this.Context.User = principal;
        }

        private static bool PrincipalRequired()
        {
            var regEx = new Regex(Properties.Settings.Default.SkipPrincipalPattern);
            var path = HttpContext.Current.Request.Url.AbsolutePath;

            return !regEx.IsMatch(path);
        }
    }
}