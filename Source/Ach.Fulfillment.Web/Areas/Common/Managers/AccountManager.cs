﻿namespace Ach.Fulfillment.Web.Areas.Common.Managers
{
    using System.Web.Security;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common.Security;
    using Ach.Fulfillment.Web.Areas.Common.Models;
    using Ach.Fulfillment.Web.Common;

    using Microsoft.Practices.Unity;

    public class AccountManager
    {
        [Dependency]
        public IUserManager UserManager { get; set; }

        [Dependency]
        public IApplicationPrincipal Principal { get; set; }

        public bool Login(LoginModel model)
        {
            var user = this.UserManager.FindByPasswordCredential(model.UserName, model.Password);

            var succeed = user != null;

            if (succeed)
            {
                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
            }

            return succeed;
        }

        public void Logout()
        {
            CacheHelper.Remove(this.Principal.Identity.Name);

            FormsAuthentication.SignOut();
        }
    }
}