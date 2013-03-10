namespace Ach.Fulfillment.Data
{
    using System;

    public class RetryReferenceEntity : ReferenceEntity, IRetryReferenceEntity
    {
        public Guid Handle { get; set; }

        public int RetryCount { get; set; }
    }
}