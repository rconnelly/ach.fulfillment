namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class AchFileUploaded : SpecificationBase<FileEntity>
    {
        public override Expression<Func<FileEntity, bool>> IsSatisfiedBy()
        {
            return m => m.FileStatus == AchFileStatus.Uploaded;
        }
    }
}