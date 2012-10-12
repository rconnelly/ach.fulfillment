namespace Ach.Fulfillment.Business
{
    using System.Collections.Generic;

    using Ach.Fulfillment.Data;

    public interface IAchTransactionManager : IManager<AchTransactionEntity>
    {
        #region Public Methods and Operators

        string Generate();

        void RemoveTransactionFromQueue(List<AchTransactionEntity> transactions);

        #endregion
    }
}