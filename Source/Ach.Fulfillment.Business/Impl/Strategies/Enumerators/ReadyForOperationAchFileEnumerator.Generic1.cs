namespace Ach.Fulfillment.Business.Impl.Strategies.Enumerators
{
    using System.Diagnostics.Contracts;
    using System.Globalization;

    using Ach.Fulfillment.Data;
    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Persistence;

    using global::Common.Logging;

    public class ReadyForOperationAchFileEnumerator<TQuery, TReference> : BaseEnumerator<AchFileEntity>
        where TQuery : IQueueQueryData<TReference>, new() 
        where TReference : class, IIdentified
    {
        #region Fields

        private readonly ILog logger = LogManager.GetCurrentClassLogger();

        private readonly string operation;

        private readonly IQueue queue;

        private readonly IRepository repository;

        #endregion

        #region Constructors and Destructors

        public ReadyForOperationAchFileEnumerator(IQueue queue, IRepository repository, string operation)
        {
            Contract.Assert(queue != null);
            Contract.Assert(repository != null);
            Contract.Assert(!string.IsNullOrEmpty(operation));
            this.queue = queue;
            this.repository = repository;
            this.operation = operation;
        }

        #endregion

        #region Public Methods and Operators

        public override bool MoveNext()
        {
            var fetched = false;

            AchFileEntity instance = null;
            var reference = this.queue.Dequeue(new TQuery());
            if (reference != null)
            {
                fetched = true;
                instance = this.repository.Get<AchFileEntity>(reference.Id);
                if (instance == null)
                {
                    this.logger.WarnFormat(
                        CultureInfo.InvariantCulture, 
                        "Cannot find ready for {0} ach file '{1}'", 
                        this.operation, 
                        reference.Id);
                }
            }

            this.Current = instance;

            return fetched;
        }

        #endregion
    }
}