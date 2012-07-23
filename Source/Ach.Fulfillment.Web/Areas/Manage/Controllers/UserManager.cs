namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;
    using Ach.Fulfillment.Web.Areas.Manage.Models;

    using AutoMapper;

    using Lib.Web.Mvc.JQuery.JqGrid;

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

        public JqGridJsonResult GetUsersGridModel (JqGridRequest request)
        {
            // TODO change FindAll return type
            var enumerable = this.Manager.FindAll(new UserAll(true));

            var users = new List<UserEntity>(enumerable);

            var totalRecordsCount = users.Count();

            var list = (from u in users
                        select
                            new JqGridRecord<UserGridModel>(u.Id.ToString(CultureInfo.InvariantCulture), 
                                new UserGridModel
                                {
                                    Id = u.Id,
                                    Name = u.Name,
                                    Email = u.Email,
                                    Login = u.UserPasswordCredential != null ? u.UserPasswordCredential.Login : string.Empty,
                                })).ToList();

            var response = new JqGridResponse
                {
                    TotalPagesCount = (int)Math.Ceiling(totalRecordsCount / (float)request.RecordsCount),
                    PageIndex = request.PageIndex,
                    TotalRecordsCount = totalRecordsCount,
                };

            response.Records.AddRange(list);

            return new JqGridJsonResult() { Data = response };
        }

        public long CreateUser(UserModel model)
        {
            PartnerEntity partner = null;
            if (model.PartnerId.HasValue)
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

        public UserModel GetEditModel(long id)
        {
            var user = this.Manager.Load(id);

            var model = new UserModel
                {
                    Name = user.Name,
                    Email = user.Email,
                    Login = user.UserPasswordCredential != null ? user.UserPasswordCredential.Login : string.Empty,
                };

            return model;
        }
    }
}