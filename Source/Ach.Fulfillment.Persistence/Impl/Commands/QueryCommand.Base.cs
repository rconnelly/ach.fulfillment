namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    using Ach.Fulfillment.Data.Common;

    internal abstract class QueryCommandBase<TQueryData, TResult> : IQueryCommand<TQueryData, TResult>
        where TQueryData : IQueryData<TResult>
    {
        #region Public Methods and Operators

        public abstract IEnumerable<TResult> Execute(TQueryData queryData);

        public virtual TResult ExecuteScalar(TQueryData queryData)
        {
            return this.Execute(queryData).FirstOrDefault();
        }

        public virtual int RowCount(TQueryData queryData)
        {
            return this.Execute(queryData).Count();
        }

        #endregion

        #region Explicit Interface Methods

        IEnumerable<TResult> IQueryCommand<TResult>.Execute(IQueryData queryData)
        {
            return this.Execute((TQueryData)queryData);
        }

        TResult IQueryCommand<TResult>.ExecuteScalar(IQueryData queryData)
        {
            return this.ExecuteScalar((TQueryData)queryData);
        }

        int IQueryCommand<TResult>.RowCount(IQueryData queryData)
        {
            return this.RowCount((TQueryData)queryData);
        }

        #endregion
    }
}