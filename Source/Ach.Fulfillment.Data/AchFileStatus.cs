namespace Ach.Fulfillment.Data
{
    using System;

    public enum AchFileStatus
    {
        Created = 0,
        Generated = 1,
        Uploaded = 2,
        Rejected = 3,
        Accepted = 4,
        [Obsolete("not clear when should be set")]
        Finalized = 5
    }
}