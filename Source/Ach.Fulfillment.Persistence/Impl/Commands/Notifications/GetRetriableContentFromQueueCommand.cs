namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using System;
    using System.Globalization;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;

    using NHibernate.Transform;

    using ServiceStack.Text;

    internal abstract class GetRetriableContentFromQueueCommand<TActionData, TData> : RelationalScalarQueryCommand<TActionData, TData>
        where TActionData : IQueryData<TData>
        where TData : class, IRetryReferenceEntity
    {
        private const string Sql = @"
declare @conversation_handle uniqueidentifier;
declare @message_type_name nvarchar(max);
declare @message_body varbinary(max);
declare @retry_count int;
	
receive top(1) 
	@conversation_handle = conversation_handle,
	@message_type_name = message_type_name,
	@message_body = message_body,
	@retry_count = 1
from [ach].[{0}];
	
if(@message_type_name = 'http://schemas.microsoft.com/SQL/ServiceBroker/DialogTimer')
begin
	select top(1)
		@message_body = message_body
	from [ach].[{0}]
	where 
		conversation_handle = @conversation_handle 
		and message_type_name = 'DEFAULT';

	select 
		@retry_count = count(*) 
	from [ach].[{0}]
	where 
		conversation_handle = @conversation_handle;
end
	
select 
	convert(nvarchar(max), @message_body) as Content, 
	@conversation_handle as Handle, 
	@retry_count as RetryCount 
where @conversation_handle is not null;";

        #region Fields

        private readonly string defaultQueueName;

        #endregion

        protected GetRetriableContentFromQueueCommand(string defaultQueueName)
        {
            this.defaultQueueName = defaultQueueName;
        }

        public override TData ExecuteScalar(TActionData queryData)
        {
            var queryString = string.Format(CultureInfo.InvariantCulture, Sql, this.defaultQueueName);
            var query = this.Session.CreateSQLQuery(queryString);
            query.SetResultTransformer(Transformers.AliasToBean<Record>());
            var record = query.UniqueResult<Record>();
            var result = default(TData);
            if (record != null)
            {
                result = JsonSerializer.DeserializeFromString<TData>(record.Content);
                result.RetryCount = record.RetryCount;
                result.Handle = record.Handle;
            }

            return result;
        }

        // ReSharper disable ClassNeverInstantiated.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        private class Record
        {
            public string Content { get; set; }

            public int RetryCount { get; set; }

            public Guid Handle { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore ClassNeverInstantiated.Local
    }
}