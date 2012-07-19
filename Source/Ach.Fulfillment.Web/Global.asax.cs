namespace Ach.Fulfillment.Web
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Security.Principal;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Security;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common;
    using Ach.Fulfillment.Common.Security;
    using Ach.Fulfillment.Web.App_Start;
    using Ach.Fulfillment.Web.Areas.Common.Controllers;
    using Ach.Fulfillment.Web.Common.Cache;
    using Ach.Fulfillment.Web.Common.Controllers;
    using Ach.Fulfillment.Web.Configuration;

    using global::Common.Logging;

    using Microsoft.Practices.ServiceLocation;

    using ApplicationIdentity = Ach.Fulfillment.Common.Security.ApplicationIdentity;

    public class MvcApplication : HttpApplication
    {
        #region Fields

        private const string UnitOfWorkKey = "uow";

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        protected void Application_Start()
        {
            Shell.Start<WebContainerExtension>();

            //DependencyResolver.SetResolver(ServiceLocator.Current);
            ControllerBuilder.Current.SetControllerFactory(typeof(UnityControllerFactory));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            this.Context.Items.Add(UnitOfWorkKey, new UnitOfWork());
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            this.Context.User = this.GetPrincipal();
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            var uow = (UnitOfWork)this.Context.Items[UnitOfWorkKey];
            Contract.Assert(uow != null);
            uow.Dispose();
            this.Context.Items.Remove(UnitOfWorkKey);
        }

        protected void Application_Error()
        {
            var error = Server.GetLastError();
            try
            {
                Log.ErrorFormat(
                    CultureInfo.InvariantCulture, 
                    "Server error has been occured while processing page: {0} ", 
                    error,
                    HttpContext.Current != null ? HttpContext.Current.Request.Url.ToString() : "unknown");
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            this.HandleCustomErrors(error);
        }

        protected void Application_End()
        {
            Shell.Shutdown();
        }

        private IPrincipal GetPrincipal()
        {
            IPrincipal principal = ApplicationPrincipal.Anonymous;
            if (this.IsPrincipalRequired())
            {
                var cookieName = FormsAuthentication.FormsCookieName;
                var authCookie = HttpContext.Current.Request.Cookies[cookieName];
                if (authCookie != null)
                {
                    var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    var login = authTicket.Name;

                    var cache = ServiceLocator.Current.GetInstance<ICacheClient>();
                    var user = cache.GetOrAdd(
                        login,
                        () =>
                            {
                                var manager = ServiceLocator.Current.GetInstance<IUserManager>();
                                return manager.FindByLogin(login);
                            });

                    if (user != null)
                    {
                        var name = user.UserPasswordCredential != null ? user.UserPasswordCredential.Login : login;

                        var identity = new ApplicationIdentity(
                            user.Id, 
                            name, 
                            user.Name, 
                            user.Email);
                        var roles = user.Role != null ? new[] { user.Role.Name } : new string[0];
                        var permissions = user.Role != null && user.Role.Permissions != null ? user.Role.Permissions.Select(p => p.Name.ToString("G")).ToArray() : new string[0];
                        principal = new ApplicationPrincipal(
                            identity,
                            roles,
                            permissions);
                    }
                }
            }

            return principal;
        }

        private bool IsPrincipalRequired()
        {
            var regEx = new Regex(Properties.Settings.Default.SkipPrincipalPattern);
            var path = this.Context.Request.Url.AbsolutePath;
            return !regEx.IsMatch(path);
        }

        private void HandleCustomErrors(Exception exception)
        {
            var httpException = exception as HttpException;

            if (httpException != null)
            {
                var status = httpException.GetHttpCode();

                if (status == 404 || status == 403)
                {
                    var routeData = new RouteData();
                    routeData.Values.Add("area", "Common");
                    routeData.Values.Add("controller", "Error");
                    routeData.Values.Add("action", "Error" + status);

                    // Clear the error on server.
                    Server.ClearError();

                    // Avoid IIS7 getting in the middle
                    Response.StatusCode = status;
                    Response.TrySkipIisCustomErrors = true;

                    // idially we should get controller throught servicelocator
                    IController errorController = new ErrorController();
                    errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                }
            }
        }

        #endregion
    }
}