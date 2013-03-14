namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    using Ach.Fulfillment.Business.Impl.Configuration;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchFiles;
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    internal class AchFileUploadProcessor : BaseAchFileRetryingProcessor<ReadyToUploadAchFileReference>
    {
        #region Fields

        private readonly IAchFileManager achFileManager;

        private readonly Func<string, IRemoteAccessManager> remoteAccessManagerBuilder;

        #endregion

        #region Constructors and Destructors

        public AchFileUploadProcessor(IQueue queue, IRepository repository, IAchFileManager achFileManager, Func<string, IRemoteAccessManager> remoteAccessManagerBuilder)
            : base(queue, repository, MetadataInfo.RepeatIntervalForNachaFileUpload)
        {
            Contract.Assert(achFileManager != null);
            Contract.Assert(remoteAccessManagerBuilder != null);
            this.achFileManager = achFileManager;
            this.remoteAccessManagerBuilder = remoteAccessManagerBuilder;
        }

        #endregion

        #region Methods

        protected override bool ProcessCore(RetryReferenceEntity reference, AchFileEntity achFile)
        {
            using (var stream = this.Repository.Load(new AchFileContentById { AchFileId = achFile.Id }))
            {
                this.Upload(achFile, stream);
            }

            this.achFileManager.UpdateStatus(achFile, AchFileStatus.Uploaded);

            return true;
        }

        private void Upload(AchFileEntity achFile, Stream stream)
        {
            Contract.Assert(achFile != null);
            Contract.Assert(stream != null);

            try
            {
                this.remoteAccessManagerBuilder.Upload(achFile, stream);
            }
            catch (Exception ex)
            {
                var rethrow = ExceptionPolicy.HandleException(ex, BusinessContainerExtension.OperationUploadPolicy);
                if (rethrow)
                {
                    throw;
                }
            }
        }

        #endregion
    }
}