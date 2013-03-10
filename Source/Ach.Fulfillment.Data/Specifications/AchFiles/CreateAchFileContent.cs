namespace Ach.Fulfillment.Data.Specifications.AchFiles
{
    using System;
    using System.IO;

    using Ach.Fulfillment.Data.Common;

    public class CreateAchFileContent : IActionData
    {
        public long AchFileId { get; set; }

        public Action<Stream> WriteTo { get; set; }
    }
}