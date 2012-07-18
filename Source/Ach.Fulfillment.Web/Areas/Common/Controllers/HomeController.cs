namespace Ach.Fulfillment.Web.Areas.Common.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Data.Specifications;
    using Ach.Fulfillment.Web.Areas.Common.Managers;
    using Ach.Fulfillment.Web.Common.Controllers;

    using Microsoft.Practices.ServiceLocation;

    public class HomeController : Controller<HomeManager>
    {
        public ActionResult Index()
        {
            this.ViewBag.Message = "Modify this template to kick-start your ASP.NET MVC application.";

            var m = ServiceLocator.Current.GetInstance<IPartnerManager>().FindAll(new PartnerAll());

            this.ViewBag.Message = "TODO: " + m.Count();

            return this.View();
        }

        public ActionResult About()
        {
            this.ViewBag.Message = "Your app description page.";

            return this.View();
        }

        public ActionResult Contact()
        {
            this.ViewBag.Message = "Your contact page.";

            return this.View();
        }
    }
}
