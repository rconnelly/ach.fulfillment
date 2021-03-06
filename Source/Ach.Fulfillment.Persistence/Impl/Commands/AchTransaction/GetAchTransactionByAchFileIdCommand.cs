namespace Ach.Fulfillment.Persistence.Impl.Commands.AchTransaction
{
    using System.Collections.Generic;
    using System.Linq;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchTransactions;

    using Microsoft.Practices.Unity;

    using NHibernate;
    using NHibernate.Linq;

    internal class GetAchTransactionByAchFileIdCommand :
        QueryCommandBase<UntrackingAchTransactionByAchFileId, AchTransactionEntity>
    {
        #region Public Properties

        [Dependency]
        public IStatelessSession Session { get; set; }

        #endregion

        #region Public Methods and Operators

        public override IEnumerable<AchTransactionEntity> Execute(UntrackingAchTransactionByAchFileId queryData)
        {
            var result = from t in this.Session.Query<AchTransactionEntity>()
                         where
                             t.AchFile.Id == queryData.AchFileId
                         select t;
            return result;
        }

        #endregion
    }
}