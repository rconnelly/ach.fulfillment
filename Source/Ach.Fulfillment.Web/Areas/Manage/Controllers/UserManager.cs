namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Web.Areas.Manage.Models;

    using Microsoft.Practices.Unity;

    public class UserManager
    {
        [Dependency]
        public IUserManager Manager { get; set; }

        [Dependency]
        public IPartnerManager PartnerManager { get; set; }

        public UserModel GetCreateModel()
        {
            var model = new UserModel();

            return model;
        }

        public void CreateUser(UserModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}