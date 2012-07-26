namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;
    using Ach.Fulfillment.Web.Areas.Manage.Models;

    using Microsoft.Practices.Unity;

    using Mvc.JQuery.Datatables;

    public class PartnerManager
    {
        [Dependency]
        public IPartnerManager Manager { get; set; }

        [Dependency]
        public IUserManager UserManager { get; set; }

        [Dependency]
        public IRoleManager RoleManager { get; set; }

        public PartnerModel GetCreateModel()
        {
            var model = new PartnerModel();

            this.FillPartnerModel(model);

            return model;
        }

        public void FillPartnerModel(PartnerModel model)
        {
        }

        public DataTablesResult<PartnerGridModel> GetPartnersGridModel(DataTablesParam dataTableParam)
        {
            Contract.Assert(dataTableParam != null);
            Contract.Assert(dataTableParam.iDisplayLength != 0);

            var list = this.Manager.FindAll(new PartnerAll());

            var query = (from u in list
                         select new PartnerGridModel
                         {
                             Id = u.Id,
                             Name = u.Name
                         }).AsQueryable();

            return DataTablesResult.Create(query, dataTableParam);
        }

        public long CreatePartner(PartnerModel model)
        {
            var partner = new PartnerEntity { Name = model.Name };

            this.Manager.Create(partner);

            return partner.Id;
        }

        public PartnerModel GetEditModel(long id)
        {
            var partner = this.Manager.Load(id);

            var model = new PartnerModel { PartnerId = partner.Id, Name = partner.Name };

            this.FillPartnerModel(model);

            return model;
        }

        public long UpdatePartner(PartnerModel model)
        {
            Contract.Assert(model.PartnerId.HasValue);

            var partner = this.Manager.Load(model.PartnerId.Value);

            partner.Name = model.Name;

            using (var tx = new Transaction())
            {
                this.Manager.Update(partner);

                tx.Complete();
            }

            return partner.Id;
        }
    }
}