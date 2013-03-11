namespace Ach.Fulfillment.Data
{
    using System;

    public enum AchFileStatus
    {
        None = 0,
        Created = 1,
        Generated = 2,
        Uploaded = 3,
        Rejected = 4,
        Accepted = 5,
        [Obsolete("not clear when should be set")]
        Finalized = 6
    }
}