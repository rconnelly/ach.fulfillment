namespace Ach.Fulfillment.Web
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Runtime.Caching;
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
    using Ach.Fulfillment.Web.Areas.Main.Controllers;
    using Ach.Fulfillment.Web.Common;
    using Ach.Fulfillment.Web.Common.Controllers;
    using Ach.Fulfillment.Web.Common.Data;
    using Ach.Fulfillment.Web.Configuration;

    using Mvc.JQuery.Datatables;

    using global::Common.Logging;

    using Microsoft.Practices.ServiceLocation;

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

            DependencyResolver.SetResolver(ServiceLocator.Current);
            ControllerBuilder.Current.SetControllerFactory(typeof(CustomControllerFactory));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //ModelBinders.Binders.Add(typeof(Mvc.JQuery.Datatables.DataTablesParam), new Mvc.JQuery.Datatables.DataTablesModelBinder());
            ModelBinders.Binders.Add(typeof(Mvc.JQuery.Datatables.DataTablesParam), new DataTablesModelBinder());
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
                    var cache = ServiceLocator.Current.GetInstance<ObjectCache>();
                    var session = cache.Get(login) as PrincipalSession;

                    if (session == null)
                    {
                        var manager = ServiceLocator.Current.GetInstance<IUserManager>();
                        var user = manager.FindByLogin(login);

                        if (user != null && user.UserPasswordCredential != null)
                        {
                            // TODO: should not user be placed to cache with sliding expiration or absolute expiration with timeout correlated with forms auth timeout
                            session = user.Convert();
                        }
                    }

                    if (session != null)
                    {
                        principal = session.Convert();
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

                    RouteHelper.InitErrorRoute(status, routeData);

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

    public class DataTablesModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var obj = new DataTablesParam();
            var request = controllerContext.HttpContext.Request.Params;

            obj.iDisplayStart = Convert.ToInt32(request["iDisplayStart"]);
            obj.iDisplayLength = Convert.ToInt32(request["iDisplayLength"]);
            obj.iColumns = Convert.ToInt32(request["iColumns"]);
            obj.sSearch = request["sSearch"];
            obj.bEscapeRegex = Convert.ToBoolean(request["bEscapeRegex"]);
            obj.iSortingCols = Convert.ToInt32(request["iSortingCols"]);
            obj.sEcho = int.Parse(request["sEcho"]);

            for (int i = 0; i < obj.iColumns; i++)
            {
                obj.bSortable.Add(Convert.ToBoolean(request["bSortable_" + i]));
                obj.bSearchable.Add(Convert.ToBoolean(request["bSearchable_" + i]));
                obj.sSearchColumns.Add(request["sSearch_" + i]);
                obj.bEscapeRegexColumns.Add(Convert.ToBoolean(request["bEscapeRegex_" + i]));
                obj.iSortCol.Add(Convert.ToInt32(request["iSortCol_" + i]));
                obj.sSortDir.Add(request["sSortDir_" + i]);
            }
            return obj;
        }
    }
}