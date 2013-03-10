namespace Ach.Fulfillment.Data.Specifications.Notifications
{
    using System;

    using Ach.Fulfillment.Data.Common;

    public class RescheduleConversation : IActionData
    {
        public Guid Handle { get; set; }

        public TimeSpan Timeout { get; set; }
    }
}