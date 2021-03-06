namespace Ach.Fulfillment.Persistence.Impl.Commands
{
    using System;
    using System.Collections.Generic;

    using Ach.Fulfillment.Data.Common;

    internal abstract class RelationalScalarQueryCommand<TQueryData, TResult> : RelationalQueryCommand<TQueryData, TResult>
        where TQueryData : IQueryData<TResult>
        where TResult : class
    {
        public override TResult ExecuteScalar(TQueryData queryData)
        {
            throw new InvalidOperationException("Should be overloaded in chile class");
        }
        
        public override IEnumerable<TResult> Execute(TQueryData queryData)
        {
            var instance = this.ExecuteScalar(queryData);
            var result = instance != null ? new[] { instance } : new TResult[0];
            return result;
        }
    }
}