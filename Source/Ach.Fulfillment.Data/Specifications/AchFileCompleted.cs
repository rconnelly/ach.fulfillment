namespace Ach.Fulfillment.Data.Specifications
{
    using System;
    using System.Linq.Expressions;

    using Ach.Fulfillment.Data.Common;

    public class AchFileCompleted : SpecificationBase<AchFileEntity>
    {
        public override Expression<Func<AchFileEntity, bool>> IsSatisfiedBy()
        {
            return m => m.FileStatus == AchFileStatus.Completed;
        }
    }
}
