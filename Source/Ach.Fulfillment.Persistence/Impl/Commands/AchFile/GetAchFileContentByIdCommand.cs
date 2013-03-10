namespace Ach.Fulfillment.Persistence.Impl.Commands.AchFile
{
    using System.Data.SqlTypes;
    using System.IO;

    using Ach.Fulfillment.Data.Specifications.AchFiles;

    internal class GetAchFileContentByIdCommand : RelationalScalarQueryCommand<AchFileContentById, Stream>
    {
        public const string ContentPathSql = @"select [SystemFile].PathName() from [ach].[AchFileContent] where [AchFileId] = :id";

        public const string TransactionConextSql = "SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()";

        public override Stream ExecuteScalar(AchFileContentById queryData)
        {
            var query = this.Session.CreateSQLQuery(ContentPathSql);
            query.SetInt64("id", queryData.AchFileId);
            var path = query.UniqueResult<string>();

            Stream result = null;
            if (path != null)
            {
                query = this.Session.CreateSQLQuery(TransactionConextSql);
                var transactionContext = query.UniqueResult<byte[]>();

                result = new SqlFileStream(path, transactionContext, FileAccess.Read);
            }

            return result;
        }
    }
}