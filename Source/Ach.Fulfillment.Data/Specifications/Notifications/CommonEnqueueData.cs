namespace Ach.Fulfillment.Data.Specifications.Notifications
{
    using Ach.Fulfillment.Data.Common;

    public class CommonEnqueueData<TEntity> : IInstanceActionData<TEntity>, IEnqueueData<TEntity>
    {
        public TEntity Instance { get; set; }

        object IEnqueueData.Instance 
        { 
            get
            {
                return this.Instance;
            } 
        }
    }
}