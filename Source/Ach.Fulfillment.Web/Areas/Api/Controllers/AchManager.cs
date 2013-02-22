namespace Ach.Fulfillment.Web.Areas.Api.Controllers
{
    using System.Collections.Generic;
    using System.Configuration;
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
            var login = ConfigurationManager.AppSettings["DefaultUser"];
            var user = this.UserManager.FindByLogin(login);
            var partner = user.Partner;

            var transaction = Mapper.Map<AchTransactionModel, AchTransactionEntity>(model);
            transaction.Partner = partner;

            // todo: why do you need to repeat prefix "Transaction"
            // todo: why we setting status here and not in business layer
            transaction.TransactionStatus = AchTransactionStatus.Received;

            this.Manager.Create(transaction);

            // todo: why do we send notification from presentation
            this.Manager.SendAchTransactionNotification(new List<AchTransactionEntity> { transaction });

            return transaction.Id;
        }

        public AchTransactionModel LoadAchTransaction(long transactionId)
        {
            var achEntity = this.Manager.Load(transactionId);
            var achModel = new AchTransactionModel
                               {
                                   AchTransactionId = achEntity.Id,
                                  /*Status = achEntity.TransactionStatus.ToString()*/
                               };

            return achModel;
        }

        public void SendCallBack(string url, string data)
        {
            this.Manager.SendCallBack(url, data);
        }

        #endregion
    }
}