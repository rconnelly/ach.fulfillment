namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data.Specifications.Notifications;

    internal class EndConversationCommand : RelationalActionCommand<EndConversation>
    {
        #region Public Methods and Operators

        public override void Execute(EndConversation actionData)
        {
            Contract.Assert(actionData != null);
            this.Session.EndConversation(actionData.Handle);
        }

        #endregion
    }
}