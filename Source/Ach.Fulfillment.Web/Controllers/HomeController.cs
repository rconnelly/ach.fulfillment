namespace Ach.Fulfillment.Web.Controllers
{
    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Data.Specifications;

    using Microsoft.Practices.ServiceLocation;

    using System.Linq;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to kick-start your ASP.NET MVC application.";

            var m = ServiceLocator.Current.GetInstance<IPartnerManager>().FindAll(new PartnerAll());

            ViewBag.Message = "TODO: " + m.Count();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
