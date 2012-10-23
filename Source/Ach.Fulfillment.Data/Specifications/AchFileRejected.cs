namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class AchFileRejected : SpecificationBase<FileEntity>
    {
        public override Expression<Func<FileEntity, bool>> IsSatisfiedBy()
        {
            return m => m.FileStatus == AchFileStatus.Rejected;
        }
    }
}