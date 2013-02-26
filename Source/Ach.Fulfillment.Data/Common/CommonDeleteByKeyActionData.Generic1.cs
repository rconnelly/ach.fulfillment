namespace Ach.Fulfillment.Data.Common
{
    public class CommonDeleteByKeyActionData<TEntity, TKey> : IActionData<TEntity>
    {
        #region Constructors and Destructors

        public CommonDeleteByKeyActionData(TKey key)
        {
            this.Key = key;
        }

        #endregion

        #region Public Properties

        public TKey Key { get; set; }

        #endregion
    }
}