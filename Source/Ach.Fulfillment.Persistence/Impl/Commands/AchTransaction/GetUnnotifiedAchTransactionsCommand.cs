namespace Ach.Fulfillment.Persistence.Impl.Commands.AchTransaction
{
    using System.Collections.Generic;
    using System.Linq;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchTransactions;

    using Microsoft.Practices.Unity;

    using NHibernate;
    using NHibernate.Linq;

    internal class GetUnnotifiedAchTransactionsCommand :
        QueryCommandBase<UnnotifiedAchTransactions, AchTransactionEntity>
    {
        #region Public Properties

        [Dependency]
        public IStatelessSession Session { get; set; }

        #endregion

        #region Public Methods and Operators

        public override IEnumerable<AchTransactionEntity> Execute(UnnotifiedAchTransactions queryData)
        {
            // todo: remove comment
            /*var result = from f in this.Session.Query<AchFileEntity>()
                         where f.Id == queryData.AchFileId
                         from t in f.Transactions
                         where t.NotifiedStatus != t.Status
                         select t;*/
            var result = from t in this.Session.Query<AchTransactionEntity>()
                         where 
                            t.AchFile.Id == queryData.AchFileId
                            && t.NotifiedStatus != t.Status
                         select t;
            return result;
        }

        #endregion
    }
}