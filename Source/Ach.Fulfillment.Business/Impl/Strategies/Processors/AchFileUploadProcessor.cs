namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchFiles;
    using Ach.Fulfillment.Data.Specifications.Notifications;
    using Ach.Fulfillment.Persistence;

    using Renci.SshNet;

    internal class AchFileUploadProcessor : BaseAchFileRetryingProcessor<ReadyToUploadAchFileReference>
    {
        #region Fields

        private const int DefaultFileUploadRepeatDelay = 60;

        private readonly IAchFileManager manager;

        private readonly PasswordConnectionInfo connectionInfo;

        #endregion

        #region Constructors and Destructors

        public AchFileUploadProcessor(IQueue queue, IRepository repository, IAchFileManager manager, PasswordConnectionInfo connectionInfo)
            : base(queue, repository, TimeSpan.FromSeconds(DefaultFileUploadRepeatDelay))
        {
            Contract.Assert(manager != null);
            Contract.Assert(connectionInfo != null);
            this.manager = manager;
            this.connectionInfo = connectionInfo;
        }

        #endregion

        #region Methods

        protected override void ProcessCore(RetryReferenceEntity reference, AchFileEntity achFile)
        {
            using (var stream = this.Repository.Load(new AchFileContentById { AchFileId = achFile.Id }))
            {
                this.Upload(achFile, stream);
            }

            this.manager.UpdateStatus(achFile, AchFileStatus.Uploaded);
        }

        private void Upload(AchFileEntity achFile, Stream stream)
        {
            Contract.Assert(achFile != null);
            Contract.Assert(stream != null);

            // todo: use ehab here to wrap necessary exceptions into BusinessException
            // todo: refactor SftpClient dependency
            this.Logger.Warn("------------------------- Upload is mock");

            return;

            using (var sftp = new SftpClient(connectionInfo))
            {
                try
                {
                    sftp.Connect();

                    var fileName = achFile.Name + ".ach";
                    sftp.UploadFile(stream, fileName);
                }
                finally
                {
                    sftp.Disconnect();
                }
            }
        }


        #endregion

        
    }
}