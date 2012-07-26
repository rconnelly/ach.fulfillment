namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;
    using Ach.Fulfillment.Web.Areas.Manage.Models;

    using Lib.Web.Mvc.JQuery.JqGrid;

    using Microsoft.Practices.Unity;

    using Mvc.JQuery.Datatables;

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

        public JqGridJsonResult GetUsersGridModel(JqGridRequest request)
        {
            var enumerable = this.Manager.FindAll(new UserAll(true));

            var users = new List<UserEntity>(enumerable);

            var totalRecordsCount = users.Count();

            var list = (from u in users
                        select
                            new JqGridRecord<UserGridModel>(
                                u.Id.ToString(CultureInfo.InvariantCulture), 
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

            return new JqGridJsonResult { Data = response };
        }

        public DataTablesResult GetUsersGridModel(DataTablesParam dataTableParam)
        {
            Contract.Assert(dataTableParam != null);
            Contract.Assert(dataTableParam.iDisplayLength != 0);

            var pageIndex = dataTableParam.iDisplayStart / dataTableParam.iDisplayLength;
            var queryData = new UserAll
                {
                    PageIndex = pageIndex, 
                    PageSize = dataTableParam.iDisplayLength
                };

            var query = this.Manager.FindAll(queryData);

            var list = (from u in query
                        select new[] 
                                { 
                                    u.Id.ToString(CultureInfo.InvariantCulture),
                                    u.Name,
                                    u.Email,
                                    u.UserPasswordCredential != null ? u.UserPasswordCredential.Login : string.Empty
                                }).OfType<object>().ToArray();

            var count = this.Manager.Count(queryData);

            // TODO (AS) replace 3rd party wrapper with own one if we use database side paging 
            var result = new DataTablesResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                    Data = new DataTablesData
                        {
                            iTotalRecords = list.Count(),
                            iTotalDisplayRecords = count, 
                            sEcho = dataTableParam.sEcho, 
                            aaData = list
                        }
                };

            return result;
        }

        public DataTablesResult<UserGridModel> GetUsersGridModel2(DataTablesParam dataTableParam)
        {
            Contract.Assert(dataTableParam != null);
            Contract.Assert(dataTableParam.iDisplayLength != 0);

            var list = this.Manager.FindAll(new UserAll());

            var query = (from u in list
                         select new UserGridModel
                                 {
                                     Id = u.Id,
                                     Name = u.Name,
                                     Email = u.Email,
                                     Login = u.UserPasswordCredential != null ? u.UserPasswordCredential.Login : string.Empty,
                                 }).AsQueryable();

            return DataTablesResult.Create(query, dataTableParam);
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