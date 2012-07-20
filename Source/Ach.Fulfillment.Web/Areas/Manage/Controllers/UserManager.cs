namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Data;
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

        public long CreateUser(UserModel model)
        {
            PartnerEntity partner = null;
            if(model.PartnerId.HasValue)
            {
                partner = this.PartnerManager.Load(model.PartnerId.Value);
            }

            var user = new UserEntity
                {
                    Name = model.Name,
                    Email = model.Email,
                    Partner = partner
                };

            this.Manager.Create(user, model.Login, model.Password);

            return user.Id;
        }
    }
}