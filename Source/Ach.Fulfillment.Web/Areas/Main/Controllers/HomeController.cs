namespace Ach.Fulfillment.Web.Areas.Main.Controllers
{
    using System;
    using System.Web.Mvc;

    using Ach.Fulfillment.Web.Areas.Api.Models;
    using Ach.Fulfillment.Web.Areas.Main.Models;
    using Ach.Fulfillment.Web.Common.Controllers;
    using Ach.Fulfillment.Web.Common.Filters;

    public class HomeController : Controller<HomeManager>
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;

            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        [WebValidationFilter]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (this.ModelState.IsValid)
            {
                if (this.Manager.Login(model))
                {
                    if (this.Url.IsLocalUrl(returnUrl))
                    {
                        return this.Redirect(returnUrl);
                    }

                    return this.RedirectToAction("Index", "Home");
                }

                this.ModelState.AddModelError(string.Empty, "The user name or password provided is incorrect.");
            }

            return this.View(model);
        }

        public ActionResult LogOff()
        {
            this.Manager.Logout();

            return this.RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            return this.View();
        }

        [AllowAnonymous]
        public ActionResult Transaction(bool test = false)
        {
            AchTransactionModel model = null;
            if (test)
            {
                model = new AchTransactionModel
                            {
                                ReceiverName = "John Doe",
                                TransitRoutingNumber = "123456789",
                                DfiAccountId = "12345678901234567",
                                Amount = (decimal)95.15,
                                ServiceClassCode = 200,
                                TransactionCode = 22,
                                EntryClassCode = "PPD",
                                EntryDate = DateTime.UtcNow.Date,
                                EntryDescription = "PAYROLL",
                                IndividualIdNumber = "abc456789012345",
                                CallbackUrl = "http://ya.ru"
                            };
            }

            return this.View(model);
        }
    }
}
