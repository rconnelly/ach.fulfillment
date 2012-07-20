namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    using System.Web.Mvc;

    using Ach.Fulfillment.Web.Areas.Manage.Models;
    using Ach.Fulfillment.Web.Common.Controllers;
    using Ach.Fulfillment.Web.Common.Filters;

    public class UserController : Controller<UserManager>
    {
        public ActionResult Index()
        {
            return this.View();
        }

        public ActionResult Create()
        {
            var model = this.Manager.GetCreateModel();

            return this.View("UserCreate", model);
        }

        [BusinessValidationFilter("UserCreate")]
        [HttpPost]
        public ActionResult Create(UserModel model)
        {
            if (this.ModelState.IsValid)
            {
                this.Manager.CreateUser(model);

                return this.RedirectToAction("Index", "Home");
            }

            return this.View("UserCreate", model);
        }

        /*
         public ActionResult ChangePassword()
                {
                    return this.View();
                }

                [HttpPost]
                public ActionResult ChangePassword(ChangePasswordModel model)
                {
                    if (this.ModelState.IsValid)
                    {

                        // ChangePassword will throw an exception rather
                        // than return false in certain failure scenarios.
                        bool changePasswordSucceeded;
                        try
                        {
                            MembershipUser currentUser = Membership.GetUser(this.User.Identity.Name, userIsOnline: true);
                            changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                        }
                        catch (Exception)
                        {
                            changePasswordSucceeded = false;
                        }

                        if (changePasswordSucceeded)
                        {
                            return this.RedirectToAction("ChangePasswordSuccess");
                        }
                        else
                        {
                            this.ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                        }
                    }

                    // If we got this far, something failed, redisplay form
                    return this.View(model);
                }

                public ActionResult ChangePasswordSuccess()
                {
                    return this.View();
                }
         */
    }
}