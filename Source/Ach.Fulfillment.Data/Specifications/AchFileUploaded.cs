namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class AchFileUploaded : SpecificationBase<AchFileEntity>
    {
        public override Expression<Func<AchFileEntity, bool>> IsSatisfiedBy()
        {
            // todo: does it work?
            return m => m.FileStatus == AchFileStatus.Uploaded;
        }
    }
}