namespace Ach.Fulfillment.Web.Areas.Api.Controllers
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Web.Areas.Api.Models;

    using AutoMapper;

    using Microsoft.Practices.Unity;

    public class AchManager
    {
        #region Public Properties

        [Dependency]
        public IAchTransactionManager Manager { get; set; }

        [Dependency]
        public IUserManager UserManager { get; set; }

        #endregion

         #region Public Methods and Operators

        public long CreateAchTransaction(AchTransactionModel model)
        {
            Contract.Assert(model != null);
            
            var user = this.UserManager.GetDefaultUser();
            Contract.Assert(user != null);
            var partner = user.Partner;

            var transaction = Mapper.Map<AchTransactionModel, AchTransactionEntity>(model);
            transaction.Partner = partner;

            this.Manager.Create(transaction);

            return transaction.Id;
        }

        public AchTransactionStatusModel GetAchTransactionById(long transactionId)
        {
            var achEntity = this.Manager.Load(transactionId);
            var achModel = new AchTransactionStatusModel
                               {
                                    AchTransactionId = achEntity.Id,
                                    Status = achEntity.Status.ToString()
                               };
            return achModel;
        }

        #endregion
    }
}