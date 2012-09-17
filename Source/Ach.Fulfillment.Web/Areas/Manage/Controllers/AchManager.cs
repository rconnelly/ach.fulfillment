using Ach.Fulfillment.Business.Impl;
using Ach.Fulfillment.Data;
using Ach.Fulfillment.Web.Areas.Manage.Models;
using Microsoft.Practices.Unity;
using Ach.Fulfillment.Business;

namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    public class AchManager 
    {
        [Dependency]
        public IAchTransactionManager Manager { get; set; }

        public long CreateAchTransaction(AchTransactionModel model)
        {
            var transaction = new AchTransactionEntity
                                  {
                                      MerchantName = model.Name,
                                      MerchantId = model.Id,
                                      MerchantDescription = model.Description,
                                      AccountId = model.AccountId,
                                      Amount = model.Amount,
                                      RoutingNumber = model.RoutingNumber,
                                      CallbackUrl = model.CallbackUrl,
                                      User = new UserEntity(){Id = 1},
                                      IsQueued = true
                                  };

            this.Manager.Create(transaction);

            return transaction.Id;
        }
    }
}