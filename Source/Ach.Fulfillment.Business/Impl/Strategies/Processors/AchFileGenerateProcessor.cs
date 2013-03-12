namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;

    using Ach.Fulfillment.Business.Impl.Strategies.Enumerators;
    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Specifications.AchFiles;
    using Ach.Fulfillment.Persistence;

    public class AchFileGenerateProcessor : BaseEnumeratorProcessor
    {
        #region Fields

        private readonly IAchFileManager manager;

        #endregion

        #region Constructors and Destructors

        public AchFileGenerateProcessor(IQueue queue, IRepository repository, IAchFileManager manager)
            : base(queue, repository)
        {
            Contract.Assert(manager != null);
            this.manager = manager;
        }

        #endregion

        #region Methods

        protected override IEnumerator<AchFileEntity> CreateEnumerator()
        {
            var enumerator = new ReadyToGenerateAchFileEnumerator(this.Queue, this.Repository);
            return enumerator;
        }

        protected override void ExecuteCore(AchFileEntity achFile)
        {
            Contract.Assert(achFile != null);

            var actionData = new CreateAchFileContent
                                 {
                                     AchFileId = achFile.Id,
                                     WriteTo = stream => Serialize(achFile, stream)
                                 };
            this.Repository.Execute(actionData);
            this.manager.UpdateStatus(achFile, AchFileStatus.Generated);
        }

        private static void Serialize(AchFileEntity achFile, Stream stream)
        {
            // todo: do not use achFile.Transactions because of performance
            var achTransactionEntities = achFile.Transactions;
            achFile.ToStream(achTransactionEntities, stream);
        }

        #endregion
    }
}