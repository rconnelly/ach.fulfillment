namespace Ach.Fulfillment.Business.Impl
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;

    using Microsoft.Practices.Unity;

    internal class AchTransactionManager : ManagerBase<AchTransactionEntity>, IAchTransactionManager
    {
        [Dependency]
        public IWebhookManager WebhookManager { get; set; }

        #region Public Methods and Operators

        public override AchTransactionEntity Create(AchTransactionEntity transaction)
        {
            Contract.Assert(transaction != null);

            this.DemandValid<AchTransactionValidator>(transaction);
            transaction.Status = AchTransactionStatus.Created;

            using (var tx = new Transaction())
            {
                var instance = base.Create(transaction);
                this.OnTransactionCreated(instance);
                tx.Complete();
                return instance;
            }
        }

        public void UpdateStatus(AchTransactionStatus status, IList<AchTransactionEntity> transactions)
        {
            Contract.Assert(transactions != null);

            using (var tx = new Transaction())
            {
                foreach (var transactionEntity in transactions)
                {
                    transactionEntity.Status = status;
                    this.Update(transactionEntity);
                }

                this.CreateWebhook(transactions);

                tx.Complete();
            }
        }

        public IEnumerable<AchTransactionEntity> GetEnqueued(PartnerEntity partner)
        {
            Contract.Assert(partner != null);

            var transactions = Repository.FindAll(new AchTransactionInQueueForPartner(partner)).ToList();

            return transactions;
        }

        public void Lock(IList<AchTransactionEntity> transactions)
        {
            Contract.Assert(transactions != null);

            using (var tx = new Transaction())
            {
                foreach (var achTransactionEntity in transactions)
                {
                    achTransactionEntity.Locked = true;
                    this.Update(achTransactionEntity);
                }

                tx.Complete();
            }
        }

        public void UnLock(IList<AchTransactionEntity> transactions)
        {
            Contract.Assert(transactions != null);

            using (var tx = new Transaction())
            {
                foreach (var achTransactionEntity in transactions)
                {
                    achTransactionEntity.Locked = false;
                    this.Update(achTransactionEntity);
                }

                tx.Complete();
            }
        }

        public void CreateWebhook(IList<AchTransactionEntity> transactions)
        {
            Contract.Assert(transactions != null);

            foreach (var achTransactionEntity in transactions)
            {
                var webhook = new WebhookEntity
                                  {
                                      Url = achTransactionEntity.CallbackUrl,
                                      Body = "{\"Status\":"+ achTransactionEntity.Status + "}" //todo improve
                                  };
                WebhookManager.Create(webhook);
            }
        }

        private void OnTransactionCreated(AchTransactionEntity instance)
        {
            this.CreateWebhook(new List<AchTransactionEntity> { instance });
        }

        #endregion
    }
}