namespace Ach.Fulfillment.Persistence.Impl.Commands.AchFile
{
    using System.Data.SqlTypes;
    using System.IO;

    using Ach.Fulfillment.Data.Specifications.AchFiles;

    internal class CreateAchFileContentCommand : RelationalActionCommand<CreateAchFileContent>
    {
        #region Constants

        public const string ContentCreationSql =
            @"insert into [ach].[AchFileContent]([AchFileId],[SystemFile]) values(:id, cast('' as varbinary(max)))";

        public const string ContentPathSql =
            @"select [SystemFile].PathName() from [ach].[AchFileContent] where [AchFileId] = :id";

        public const string TransactionConextSql = "SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()";

        #endregion

        #region Public Methods and Operators

        public override void Execute(CreateAchFileContent actionData)
        {
            var query = this.Session.CreateSQLQuery(ContentCreationSql);
            query.SetInt64("id", actionData.AchFileId);
            query.ExecuteUpdate();

            query = this.Session.CreateSQLQuery(ContentPathSql);
            query.SetInt64("id", actionData.AchFileId);
            var path = query.UniqueResult<string>();

            query = this.Session.CreateSQLQuery(TransactionConextSql);
            var transactionContext = query.UniqueResult<byte[]>();

            using (var fs = new SqlFileStream(path, transactionContext, FileAccess.Write))
            {
                actionData.WriteTo(fs);
            }
        }

        #endregion
    }
}