namespace Ach.Fulfillment.Data
{
    using System;

    public interface IRetryReferenceEntity
    {
        long Id { get; set; }

        Guid Handle { get; set; }

        int RetryCount { get; set; }
    }
}