namespace Ach.Fulfillment.Web.Areas.Manage.Controllers
{
    using System.Configuration;

    using Ach.Fulfillment.Business;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Web.Areas.Manage.Models;

    using Microsoft.Practices.Unity;

    public class AchManager 
    {
        [Dependency]
        public IAchTransactionManager Manager { get; set; }

        [Dependency]
        public IUserManager UserManager { get; set; }

        public long CreateAchTransaction(AchTransactionModel model)
        {
            var login = ConfigurationManager.AppSettings["DefaultUser"];
            var user = this.UserManager.FindByLogin(login);
            var partner = user.Partner;
           
            var transaction = new AchTransactionEntity
                                  {
                                      ReceiverName = model.ReceiverName,
                                      IndividualIdNumber = model.IndividualIdNumber,
                                      EntryDescription = model.EntryDescription,
                                      DfiAccountId = model.DfiAccountId,
                                      Amount = model.Amount,
                                      TransitRoutingNumber = model.TransitRoutingNumber,
                                      CallbackUrl = model.CallbackUrl,
                                      TransactionCode = model.TransactionCode,
                                      EntryClassCode = model.EntryClassCode,
                                      EntryDate = model.EntryDate,
                                      ServiceClassCode = model.ServiceClassCode,
                                      PaymentRelatedInfo = model.PaymentRelatedInfo,
                                      Partner = partner,
                                      TransactionStatus = AchTransactionStatus.Received
                                  };

            this.Manager.Create(transaction);

            return transaction.Id;
        }
    }
}