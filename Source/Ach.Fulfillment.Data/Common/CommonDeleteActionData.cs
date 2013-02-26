namespace Ach.Fulfillment.Data.Common
{
    public class CommonDeleteActionData<TEntity> : IInstanceActionData<TEntity>
    {
        public TEntity Instance { get; set; }
    }
}