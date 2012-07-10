namespace Ach.Fulfillment.Data.QueryData
{
    public interface IInstanceQueryData<T> : IQueryData<T>
    {
        T Instance { get; set; }
    }
}