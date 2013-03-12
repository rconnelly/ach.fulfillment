namespace Ach.Fulfillment.Web.Areas.Api.Controllers
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Business.Exceptions;
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
            if (user == null)
            {
                throw new BusinessException("Cannot find default user associated with partner");
            }
            
            var partner = user.Partner;
            var transaction = Mapper.Map<AchTransactionModel, AchTransactionEntity>(model);
            transaction.Partner = partner;
            transaction = this.Manager.Create(transaction);
            var result = transaction.Id;
            return result;
        }

        public AchTransactionStatusModel LoadAchTransactionStatus(long transactionId)
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