namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data.Common;
    using Ach.Fulfillment.Data.Specifications.Notifications;

    using ServiceStack.Text;

    internal abstract class BaseEnqueueCommand<TActionData> : RelationalActionCommand<TActionData>
        where TActionData : class, IActionData, IEnqueueData
    {
        #region Constants

        private const string Sql = "exec [ach].[SendReference] :Content, :DestinationService";

        #endregion

        #region Constructors and Destructors

        protected BaseEnqueueCommand() : this(null)
        {
        }

        protected BaseEnqueueCommand(string destinationService)
        {
            this.DestinationService = destinationService;
        }

        #endregion

        #region Properties

        protected string DestinationService { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Execute(TActionData actionData)
        {
            this.Execute(actionData, this.DestinationService);
        }

        #endregion

        #region Methods

        protected void Execute(TActionData actionData, string destinationService)
        {
            var content = GetContent(actionData);
            var query = this.Session.CreateSQLQuery(Sql);
            query.SetString("Content", content);
            query.SetString("DestinationService", destinationService);
            query.ExecuteUpdate();
        }

        private static string GetContent(TActionData actionData)
        {
            Contract.Assert(actionData != null);
            Contract.Assert(actionData.Instance != null);
            var type = actionData.Instance.GetType();
            var content = JsonSerializer.SerializeToString(actionData.Instance, type);
            return content;
        }

        #endregion
    }
}