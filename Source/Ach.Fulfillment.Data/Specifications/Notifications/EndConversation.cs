namespace Ach.Fulfillment.Data.Specifications.Notifications
{
    using System;

    using Ach.Fulfillment.Data.Common;

    public class EndConversation : IActionData
    {
        public Guid Handle { get; set; }
    }
}