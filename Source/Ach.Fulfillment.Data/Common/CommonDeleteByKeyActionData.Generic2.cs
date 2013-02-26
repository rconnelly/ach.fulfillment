namespace Ach.Fulfillment.Data.Common
{
    public class CommonDeleteByKeyActionData<TEntity> : CommonDeleteByKeyActionData<TEntity, long>
    {
        #region Constructors and Destructors

        public CommonDeleteByKeyActionData(long key) : base(key)
        {
        }

        #endregion
    }
}