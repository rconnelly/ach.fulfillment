namespace Ach.Fulfillment.Business.Impl
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Ach.Fulfillment.Business.Impl.Validation;
    using Ach.Fulfillment.Common.Transactions;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications;

    internal class AchTransactionManager : ManagerBase<AchTransactionEntity>, IAchTransactionManager
    {
        #region Public Methods and Operators

        public override AchTransactionEntity Create(AchTransactionEntity transaction)
        {
            Contract.Assert(transaction != null);

            this.DemandValid<AchTransactionValidator>(transaction); 

            transaction.Status = AchTransactionStatus.Created;
            var entity = base.Create(transaction);

            return entity;
        }
        
        public void ChangeAchTransactionStatus(List<AchTransactionEntity> transactions, AchTransactionStatus status)
        {
            Contract.Assert(transactions != null);

            using (var tx = new Transaction())
            {
                foreach (var achTransactionEntity in transactions)
                {
                    achTransactionEntity.Status = status;
                    this.Update(achTransactionEntity);
                }

                tx.Complete();
            }

        }

        public List<AchTransactionEntity> GetAllInQueue(bool toLock = true)
        {
            List<AchTransactionEntity> transactions;

            using (var tx = new Transaction())
            {
                transactions = Repository.FindAll(new AchTransactionInQueue()).ToList();

                foreach (var achTransactionEntity in transactions)
                {
                    achTransactionEntity.Locked = toLock;
                    this.Update(achTransactionEntity);
                }

                tx.Complete();
            }

            return transactions;
        }

        public List<AchTransactionEntity> GetAllInQueueForPartner(PartnerEntity partner, bool toLock = true)
        {
            List<AchTransactionEntity> transactions;
            
            using (var tx = new Transaction())
            {
                transactions = Repository.FindAll(new AchTransactionInQueueForPartner(partner)).ToList();

                foreach (var achTransactionEntity in transactions)
                {
                    achTransactionEntity.Locked = toLock;
                    this.Update(achTransactionEntity);
                }

                tx.Complete();
            }

            return transactions;
        }

        public void UnLock(List<AchTransactionEntity> transactions)
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

        #endregion
    }
}