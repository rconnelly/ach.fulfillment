namespace Ach.Fulfillment.Persistence.Impl.Commands.Notifications
{
    using System.Diagnostics.Contracts;

    using Ach.Fulfillment.Data.Specifications.Notifications;

    internal class RescheduleConversationCommand : RelationalActionCommand<RescheduleConversation>
    {
        #region Public Methods and Operators

        public override void Execute(RescheduleConversation actionData)
        {
            Contract.Assert(actionData != null);
            this.Session.BeginConversationTimer(actionData.Handle, actionData.Timeout);
        }

        #endregion
    }
}