namespace Ach.Fulfillment.Business.Impl.Strategies.Processors
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

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
            var actionData = new CreateAchFileContent { AchFileId = achFile.Id, WriteTo = achFile.ToStream };
            this.Repository.Execute(actionData);
            this.manager.UpdateStatus(achFile, AchFileStatus.Generated);
        }

        #endregion
    }
}