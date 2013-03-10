namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    using Ach.Fulfillment.Data.Common;

    using NHibernate;
    using NHibernate.Transform;

    using ServiceStack.Text;

    internal abstract class GetSimpleContentFromQueueCommand<TActionData, TData> : RelationalQueryCommand<TActionData, TData>
        where TActionData : IQueryData<TData>
    {
        #region Constants

        private const string Sql =
            @"
RECEIVE {0} 
    conversation_handle as Id,
    CONVERT(NVARCHAR(MAX), message_body) AS Content 
FROM [ach].[{1}]";

        #endregion

        #region Fields

        private readonly string defaultQueueName;

        #endregion

        #region Constructors and Destructors

        protected GetSimpleContentFromQueueCommand()
        {
        }

        protected GetSimpleContentFromQueueCommand(string defaultQueueName)
        {
            this.defaultQueueName = defaultQueueName;
        }

        #endregion

        #region Public Methods and Operators

        public override IEnumerable<TData> Execute(TActionData queryData)
        {
            var result = this.Execute(false, this.defaultQueueName);
            return result;
        }

        public override TData ExecuteScalar(TActionData queryData)
        {
            var result = this.Execute(true, this.defaultQueueName).FirstOrDefault();
            return result;
        }

        #endregion

        #region Methods

        protected virtual IEnumerable<TData> Execute(bool topOne, string queueName)
        {
            var model = this.Receive(topOne, queueName).List<Record>();
            var record = model.FirstOrDefault();

            var serializer = new JsonSerializer<TData>();
            var result = from item in model
                         select serializer.DeserializeFromString(item.Content);

            this.EndConversation(record);

            return result;
        }

        private static string GetFormattedSql(bool topOne, string queueName)
        {
            Contract.Assert(!string.IsNullOrEmpty(queueName));
            return string.Format(CultureInfo.InvariantCulture, Sql, topOne ? " top(1) " : string.Empty, queueName);
        }

        private ISQLQuery Receive(bool topOne, string queueName)
        {
            var query = this.Session.CreateSQLQuery(GetFormattedSql(topOne, queueName));
            query.SetResultTransformer(Transformers.AliasToBean<Record>());
            return query;
        }

        private void EndConversation(Record record)
        {
            if (record != null)
            {
                this.Session.EndConversation(record.Id);
            }
        }

        #endregion

        // ReSharper disable ClassNeverInstantiated.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private class Record
        {
            #region Public Properties

            public string Content { get; set; }

            public Guid Id { get; set; }

            #endregion
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore ClassNeverInstantiated.Local
    }
}