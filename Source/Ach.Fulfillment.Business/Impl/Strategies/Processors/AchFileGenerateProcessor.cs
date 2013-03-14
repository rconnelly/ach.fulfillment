namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    using Ach.Fulfillment.Business.Impl.Configuration;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchFiles;
    using Ach.Fulfillment.Data.Specifications.AchTransactions;
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    internal class AchFileGenerateProcessor : BaseAchFileRetryingProcessor<ReadyToGenerateAchFileReference>
    {
        #region Fields

        private readonly IAchFileManager manager;

        #endregion

        #region Constructors and Destructors

        public AchFileGenerateProcessor(IQueue queue, IRepository repository, IAchFileManager manager)
            : base(queue, repository, MetadataInfo.RepeatIntervalForNachaFileGeneration)
        {
            Contract.Assert(manager != null);
            this.manager = manager;
        }

        #endregion

        #region Methods

        protected override bool ProcessCore(RetryReferenceEntity reference, AchFileEntity achFile)
        {
            Contract.Assert(achFile != null);

            var actionData = new CreateAchFileContent
                                 {
                                     AchFileId = achFile.Id,
                                     WriteTo = stream => this.Serialize(achFile, stream)
                                 };
            this.Repository.Execute(actionData);
            this.manager.UpdateStatus(achFile, AchFileStatus.Generated);

            return true;
        }

        private void Serialize(AchFileEntity achFile, Stream stream)
        {
            Contract.Assert(achFile != null);
            Contract.Assert(stream != null);

            var transactions = this.Repository
                .FindAll(new UntrackingAchTransactionByAchFileId { AchFileId = achFile.Id });

            string content = null;
            try
            {
                content = achFile.ToNachaContent(transactions);
            }
            catch (Exception ex)
            {
                var rethrow = ExceptionPolicy.HandleException(ex, BusinessContainerExtension.OperationNachaGenerationPolicy);
                if (rethrow)
                {
                    throw;    
                }
            }

            if (!string.IsNullOrEmpty(content))
            {
                var writer = new StreamWriter(stream);
                writer.Write(content);
                writer.Flush();
            }
        }

        #endregion
    }
}