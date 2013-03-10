namespace Ach.Fulfillment.Data.Specifications.AchFiles
{
    using System.IO;

    using Ach.Fulfillment.Data.Common;

    public class AchFileContentById : IQueryData<Stream>
    {
        public long AchFileId { get; set; }
    }
}